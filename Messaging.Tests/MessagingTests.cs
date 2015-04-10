﻿using System;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Messaging.Tests.DI;
using NuClear.AdvancedSearch.Messaging.Tests.Mocks.Receiver;
using NuClear.AdvancedSearch.Messaging.Tests.Properties;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Processing.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API;
using NuClear.OperationsProcessing.API.Metadata;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Messaging.Tests
{
    [TestFixture]
    public sealed class MessagingTests
    {
        [Test]
        public void PrimaryTest1()
        {
            var receiver = new MockMessageReceiver(new[]
            {
                Resources.UpdateFirm,
                Resources.ComplexUseCase
            },
            (succeeded, failed) =>
            {
                Assert.That(succeeded.Count(), Is.EqualTo(2));
            });

            var container = new UnityContainer().ConfigureUnity(receiver);
            var flowId = "Replicate2AdvancedSearchFlow".AsPrimaryProcessingFlowId();

            var processor = GetProcessor(container, flowId);
            processor.Process();
        }

        private static ISyncMessageFlowProcessor GetProcessor(IUnityContainer container, Uri id)
        {
            var metadataProvider = container.Resolve<IMetadataProvider>();

            MessageFlowMetadata messageFlowMetadata;
            if (!metadataProvider.TryGetMetadata(id, out messageFlowMetadata))
            {
                throw new ArgumentException();
            }

            var settings = container.Resolve<IPerformedOperationsFlowProcessorSettings>();
            var processorFactory = container.Resolve<IMessageFlowProcessorFactory>();
            var processor = processorFactory.CreateSync(messageFlowMetadata, settings);

            return processor;
        }
    }
}