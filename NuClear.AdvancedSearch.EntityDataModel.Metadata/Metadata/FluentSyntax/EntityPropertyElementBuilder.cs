using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityPropertyElementBuilder : MetadataElementBuilder<EntityPropertyElementBuilder, EntityPropertyElement>
    {
        private string _name;
        private string _enumName;
        private EntityPropertyType? _enumUnderlyingType;
        private readonly Dictionary<string, long> _enumMembers = new Dictionary<string, long>();

        public EntityPropertyElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public EntityPropertyElementBuilder NotNull()
        {
            AddFeatures(new EntityPropertyNullableFeature(false));
            return this;
        }

        public EntityPropertyElementBuilder OfType(EntityPropertyType propertyType)
        {
            AddFeatures(new EntityPropertyTypeFeature(propertyType));
            return this;
        }

        public EntityPropertyElementBuilder UsingEnum(string name, EntityPropertyType underlyingType = EntityPropertyType.Int32)
        {
            _enumName = name;
            _enumUnderlyingType = underlyingType;
            return this;
        }

        public EntityPropertyElementBuilder WithMember(string name, long value)
        {
            if (_enumUnderlyingType == null)
            {
                throw new InvalidOperationException("The enumeration was not declared.");
            }
            _enumMembers.Add(name, value);
            return this;
        }

        protected override EntityPropertyElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The property name was not specified.");
            }

            return new EntityPropertyElement(
                _name.AsRelativeUri().AsIdentity(),
                _enumUnderlyingType == null ? Features : Features.Concat(new[] {new EntityPropertyEnumTypeFeature(_enumName, _enumUnderlyingType.Value, _enumMembers) })
                );
        }
    }
}