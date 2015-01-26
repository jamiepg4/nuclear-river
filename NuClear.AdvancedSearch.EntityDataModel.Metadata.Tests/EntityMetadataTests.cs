﻿using System;
using System.Collections;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;

using NUnit.Framework;

namespace NuClear.EntityDataModel.Tests
{
    [TestFixture]
    internal class EntityMetadataTests : BaseMetadataFixture<EntityElement, EntityElementBuilder>
    {
        public override IEnumerable TestCases
        {
            get
            {
                yield return Case(EntityElement.Config)
                    .Throws(typeof(InvalidOperationException))
                    .SetName("ShouldFailIfNoName");
                yield return Case(EntityElement.Config.Name("Entity"), MetadataKind.Identity)
                    .Returns("{'Identity':{'Id':'Entity'}}")
                    .SetName("ShouldDeclareEntity");
                yield return Case(EntityElement.Config.Name("Entity"), MetadataKind.Elements | MetadataKind.Features)
                    .Returns("{'Features':[],'Elements':[]}")
                    .SetName("ShouldDeclareEmptyEntity");
                yield return Case(EntityElement.Config.Name("Entity").EntitySetName("Entities"), MetadataKind.Features)
                    .Returns("{'Features':[{'EntitySetName':'Entities'}]}")
                    .SetName("ShouldDeclareEntitySetName");
                yield return Case(
                    EntityElement.Config.Name("Entity")
                        .Property(EntityPropertyElement.Config.Name("Property").OfType(EntityPropertyType.String)),
                    MetadataKind.Identity | MetadataKind.Elements)
                    .Returns("{'Identity':{'Id':'Entity'},'Elements':[{'Identity':{'Id':'Property'},'Elements':[]}]}")
                    .SetName("ShouldDeclareEntityWithProperty");
                yield return Case(
                    EntityElement.Config.Name("Entity")
                        .HasKey("Property")
                        .Property(EntityPropertyElement.Config.Name("Property").OfType(EntityPropertyType.Int64)), 
                    MetadataKind.Identity | MetadataKind.Features)
                    .Returns("{'Identity':{'Id':'Entity'},'Features':[{'IdentifyingProperties':[{'Identity':{'Id':'Property'},'Features':[{'PropertyType':'Int64'}]}]}]}")
                    .SetName("ShouldDeclareEntityWithKey");
                yield return Case(
                    EntityElement.Config.Name("Entity")
                        .Relation(EntityRelationElement.Config.Name("Relation").DirectTo(EntityElement.Config.Name("Target")).AsOne()),
                    MetadataKind.Identity | MetadataKind.Elements)
                    .Returns("{'Identity':{'Id':'Entity'},'Elements':[{'Identity':{'Id':'Relation'},'Elements':[]}]}")
                    .SetName("ShouldDeclareEntityWithRelation");
            }
        }
    }
}
