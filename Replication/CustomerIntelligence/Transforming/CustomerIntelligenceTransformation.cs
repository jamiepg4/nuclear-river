﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using LinqToDB.Expressions;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Model;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class CustomerIntelligenceTransformation
    {
        private static readonly Dictionary<Type, AggregateInfo> Aggregates = new []
        {
            AggregateInfo.Create<Firm>(Query.FirmsById, valueObjects:
                new[]
                {
                    new ValueObjectInfo(FirmChildren.FirmBalances),
                    new ValueObjectInfo(FirmChildren.FirmCategories),
                }),
            AggregateInfo.Create<Client>(Query.ClientsById, new[] { new EntityInfo(ClientChildren.Contacts) })
        }.ToDictionary(x => x.AggregateType);

        private readonly ICustomerIntelligenceContext _source;
        private readonly ICustomerIntelligenceContext _target;
        private readonly IDataMapper _mapper;

        public CustomerIntelligenceTransformation(ICustomerIntelligenceContext source, ICustomerIntelligenceContext target, IDataMapper mapper)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            _source = source;
            _target = target;
            _mapper = mapper;
        }

        public void Transform(IEnumerable<AggregateOperation> operations)
        {
            var slices = operations.GroupBy(x => new { Operation = x.GetType(), x.AggregateType })
                                   .OrderByDescending(x => x.Key.Operation, new AggregateOperationPriorityComparer());

            foreach (var slice in slices)
            {
                var operation = slice.Key.Operation;
                var aggregateType = slice.Key.AggregateType;
                var aggregateIds = slice.Select(x => x.AggregateId).ToArray();

                AggregateInfo aggregateInfo;
                if (!Aggregates.TryGetValue(aggregateType, out aggregateInfo))
                {
                    throw new NotSupportedException(string.Format("The '{0}' aggregate not supported.", aggregateType));
                }

                if (operation == typeof(InitializeAggregate))
                {
                    InitializeAggregate(aggregateInfo, aggregateIds);
                }
                
                if (operation == typeof(RecalculateAggregate))
                {
                    RecalculateAggregate(aggregateInfo, aggregateIds);
                }

                if (operation == typeof(DestroyAggregate))
                {
                    DestroyAggregate(aggregateInfo, aggregateIds);
                }
            }
        }

        private void InitializeAggregate(AggregateInfo aggregateInfo, long[] ids)
        {
            _mapper.InsertAll(aggregateInfo.Query(_source, ids));

            foreach (var valueObjectInfo in aggregateInfo.ValueObjects)
            {
                _mapper.InsertAll(valueObjectInfo.Query(_source, ids));
            }
        }

        private void RecalculateAggregate(AggregateInfo aggregateInfo, long[] ids)
        {
            foreach (var valueObjectInfo in aggregateInfo.ValueObjects)
            {
                var query = valueObjectInfo.Query;
                var result = MergeTool.Merge(query(_source, ids), query(_target, ids));

                var elementsToInsert = result.Difference;
                var elementsToUpdate = result.Intersection;
                var elementsToDelete = result.Complement;

                _mapper.InsertAll(elementsToInsert.AsQueryable());
                _mapper.UpdateAll(elementsToUpdate.AsQueryable());
                _mapper.DeleteAll(elementsToDelete.AsQueryable());
            }

            _mapper.UpdateAll(aggregateInfo.Query(_source, ids));

            foreach (var entityInfo in aggregateInfo.Entities)
            {
                var query = entityInfo.Query;
                var result = MergeTool.Merge(query(_source, ids), query(_target, ids));

                var elementsToInsert = result.Difference;
                var elementsToUpdate = result.Intersection;
                var elementsToDelete = result.Complement;

                _mapper.InsertAll(elementsToInsert.AsQueryable());
                _mapper.UpdateAll(elementsToUpdate.AsQueryable());
                _mapper.DeleteAll(elementsToDelete.AsQueryable());
            }
        }

        private void DestroyAggregate(AggregateInfo aggregateInfo, long[] ids)
        {
            foreach (var entityInfo in aggregateInfo.Entities)
            {
                _mapper.DeleteAll(entityInfo.Query(_target, ids));
            }

            foreach (var valueObjectInfo in aggregateInfo.ValueObjects)
            {
                _mapper.DeleteAll(valueObjectInfo.Query(_target, ids));
            }

            _mapper.DeleteAll(aggregateInfo.Query(_target, ids));
        }

        #region Query

        private static class Query
        {
            public static IQueryable<Client> ClientsById(ICustomerIntelligenceContext context, IEnumerable<long> ids)
            {
                return FilterById(context.Clients, ids);
            }

            public static IQueryable<Firm> FirmsById(ICustomerIntelligenceContext context, IEnumerable<long> ids)
            {
                return FilterById(context.Firms, ids);
            }

            private static IQueryable<TEntity> FilterById<TEntity>(IQueryable<TEntity> facts, IEnumerable<long> ids) where TEntity : IIdentifiableObject
            {
                return facts.Where(fact => ids.Contains(fact.Id));
            }
        }

        private static class FirmChildren
        {
            public static IQueryable<FirmBalance> FirmBalances(ICustomerIntelligenceContext context, IEnumerable<long> ids)
            {
                return context.FirmBalances.Where(x => ids.Contains(x.FirmId));
            }

            public static IQueryable<FirmCategory> FirmCategories(ICustomerIntelligenceContext context, IEnumerable<long> ids)
            {
                return context.FirmCategories.Where(x => ids.Contains(x.FirmId));
            }
        }

        private static class ClientChildren
        {
            public static IQueryable<Contact> Contacts(ICustomerIntelligenceContext context, IEnumerable<long> ids)
            {
                return context.Contacts.Where(x => ids.Contains(x.ClientId));
            }
        }

        #endregion

        #region MergeTool

        private static class MergeTool
        {
            private static readonly MethodInfo MergeMethodInfo = MemberHelper.MethodOf(() => Merge<IIdentifiableObject>(null, null)).GetGenericMethodDefinition();

            private static readonly ConcurrentDictionary<Type, MethodInfo> Methods = new ConcurrentDictionary<Type, MethodInfo>();

            public static MergeResult Merge(IQueryable newData, IQueryable oldData)
            {
                if (newData.ElementType != oldData.ElementType)
                {
                    throw new InvalidOperationException("The types are not matched.");
                }

                var type = newData.ElementType;
                var method = Methods.GetOrAdd(type, t => MergeMethodInfo.MakeGenericMethod(t));

                return (MergeResult)method.Invoke(null, new object[] { newData, oldData });
            }

            private static MergeResult Merge<T>(IEnumerable<T> data1, IEnumerable<T> data2)
            {
                var set1 = new HashSet<T>(data1);
                var set2 = new HashSet<T>(data2);

                // NOTE: avoiding enumerable extensions to reuse hashset performance
                var difference = set1.Where(x => !set2.Contains(x));
                var intersection = set1.Where(x => set2.Contains(x));
                var complement = set2.Where(x => !set1.Contains(x));

                return new MergeResult { Difference = difference, Intersection = intersection, Complement = complement };
            }

            public class MergeResult
            {
                public IEnumerable Difference { get; set; }
                public IEnumerable Intersection { get; set; }
                public IEnumerable Complement { get; set; }
            }
        }

        #endregion
    }
}
