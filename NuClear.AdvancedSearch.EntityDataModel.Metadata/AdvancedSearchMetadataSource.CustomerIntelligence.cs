﻿// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    partial class AdvancedSearchMetadataSource
    {
        private static readonly StructuralModelElement ConceptualModel =
                    StructuralModelElement.Config.Elements(
                        EntityElement.Config
                            .Name("Firm").EntitySetName("Firms")
                            .IdentifyBy("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
                            .Property(EntityPropertyElement.Config.Name("OrganizationUnitId").OfType(EntityPropertyType.Int64))
                            .Property(EntityPropertyElement.Config.Name("TerritoryId").OfType(EntityPropertyType.Int64))
                            .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(EntityPropertyType.DateTime))
                            .Property(EntityPropertyElement.Config.Name("LastQualifiedOn").OfType(EntityPropertyType.DateTime))
                            .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(EntityPropertyType.DateTime))
                            .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(EntityPropertyType.Boolean))
                            .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(EntityPropertyType.Boolean))
                            .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Byte))
                            .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(EntityPropertyType.Int32))
                            .Relation(EntityRelationElement.Config
                                .Name("Categories")
                                .DirectTo(
                                    EntityElement.Config
                                        .Name("Category")
                                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
                                        .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                                        .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Byte))
                                )
                                .AsMany())
                            .Relation(EntityRelationElement.Config
                                .Name("Client")
                                .DirectTo(
                                    EntityElement.Config
                                        .Name("Client")
                                        .IdentifyBy("Id")
                                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
                                        .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Byte))
                                        .Relation(
                                            EntityRelationElement.Config
                                                .Name("Accounts")
                                                .DirectTo(
                                                    EntityElement.Config
                                                        .Name("Account")
                                                        .Property(EntityPropertyElement.Config.Name("Balance").OfType(EntityPropertyType.Decimal))
                                                )
                                                .AsMany()
                                        )
                                        .Relation(
                                            EntityRelationElement.Config
                                                .Name("Contacts")
                                                .DirectTo(
                                                    EntityElement.Config.Name("Contact")
                                                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64))
                                                        .Property(EntityPropertyElement.Config.Name("Role")
                                                            .UsingEnum("ContactRole")
                                                            .WithMember("Employee", 200000)
                                                            .WithMember("InfluenceDecisions", 200001)
                                                            .WithMember("MakingDecisions", 200002)
                                                            )
                                                        .Property(EntityPropertyElement.Config.Name("IsFired").OfType(EntityPropertyType.Boolean))
                                                )
                                                .AsMany()
                                        )
                                )
                                .AsOneOptionally()));

        private static readonly StructuralModelElement StoreModel = StructuralModelElement
            .Config.Elements(
                         EntityElement.Config
                            .Name("CustomerIntelligence.Client")
                            .IdentifyBy("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
                            .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Byte)),
                         EntityElement.Config
                            .Name("CustomerIntelligence.Contact")
                            .IdentifyBy("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
                            .Property(EntityPropertyElement.Config.Name("Role").OfType(EntityPropertyType.Int32))
                            .Property(EntityPropertyElement.Config.Name("IsFired").OfType(EntityPropertyType.Boolean))
                            .Property(EntityPropertyElement.Config.Name("ClientId").OfType(EntityPropertyType.Int32)),
                         EntityElement.Config
                            .Name("CustomerIntelligence.Account")
                            .IdentifyBy("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
                            .Property(EntityPropertyElement.Config.Name("Balance").OfType(EntityPropertyType.Decimal))
                            .Property(EntityPropertyElement.Config.Name("ClientId").OfType(EntityPropertyType.Int64)),
                         EntityElement.Config
                            .Name("CustomerIntelligence.Firm")
                            .IdentifyBy("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
                            .Property(EntityPropertyElement.Config.Name("OrganizationUnitId").OfType(EntityPropertyType.Int64))
                            .Property(EntityPropertyElement.Config.Name("TerritoryId").OfType(EntityPropertyType.Int64))
                            .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(EntityPropertyType.DateTime))
                            .Property(EntityPropertyElement.Config.Name("LastQualifiedOn").OfType(EntityPropertyType.DateTime))
                            .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(EntityPropertyType.DateTime))
                            .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(EntityPropertyType.Boolean))
                            .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(EntityPropertyType.Boolean))
                            .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Byte))
                            .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(EntityPropertyType.Int32))
                            .Property(EntityPropertyElement.Config.Name("ClientId").OfType(EntityPropertyType.Int64)),
                         EntityElement.Config
                            .Name("CustomerIntelligence.Category")
                            .IdentifyBy("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
                            .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                            .Property(EntityPropertyElement.Config.Name("Level").OfType(EntityPropertyType.Int32).NotNull())
                            .Property(EntityPropertyElement.Config.Name("ParentId").OfType(EntityPropertyType.Int64)),
                         EntityElement.Config
                            .Name("CustomerIntelligence.FirmCategories")
                            .IdentifyBy("FirmId", "CategoryId")
                            .Property(EntityPropertyElement.Config.Name("FirmId").OfType(EntityPropertyType.Int64).NotNull())
                            .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(EntityPropertyType.Int64).NotNull())
                            .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Byte))
                   );

        private readonly BoundedContextElement _customerIntelligence =
            BoundedContextElement.Config.Name("CustomerIntelligence")
                                 .ConceptualModel(ConceptualModel)
                                 .StoreModel(StoreModel);
    }
}