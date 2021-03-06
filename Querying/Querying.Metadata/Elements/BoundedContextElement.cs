﻿using System.Collections.Generic;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Querying.Metadata.Builders;

namespace NuClear.Querying.Metadata.Elements
{
    public sealed class BoundedContextElement : BaseMetadataElement<BoundedContextElement, BoundedContextElementBuilder>
    {
        internal BoundedContextElement(
            IMetadataElementIdentity contextIdentity,
            StructuralModelElement conceptualModel,
            StructuralModelElement storeModel,
            IEnumerable<IMetadataFeature> features)
            : base(contextIdentity, features)
        {
            ConceptualModel = conceptualModel;
            StoreModel = storeModel;
        }

        public StructuralModelElement ConceptualModel { get; }

        public StructuralModelElement StoreModel { get; }
    }
}