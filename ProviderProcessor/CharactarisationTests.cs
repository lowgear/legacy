using System;
using NUnit.Framework;
using ApprovalTests;
using ApprovalTests.Reporters;
using ProviderProcessing.ProviderDatas;
using FakeItEasy;
using Newtonsoft.Json;
using ProviderProcessing.References;

namespace ProviderProcessing
{
    [TestFixture]
    [UseReporter(typeof(DiffReporter))]
    public class CharactarisationTests
    {
        private ProviderProcessor pp;
        private ProviderRepository fakeRepo;
        private ProviderData data = new ProviderData();
        private ProductsReference fakeReference = A.Fake<ProductsReference>();

        [SetUp]
        public void SetUp()
        {
            fakeRepo = A.Fake<ProviderRepository>();

//            pp = new ProviderProcessor(fakeRepo);
            pp = A.Fake<ProviderProcessor>(op => op.WithArgumentsForConstructor(new [] {fakeRepo}));
        }

        [Test]
        public void DoSomething_WhenSomething()
        {
            A.CallTo(() => fakeRepo.FindByProviderId(A<Guid>._)).Returns(data);
            A.CallTo(() => pp.GetInstance()).Returns(fakeReference);
            A.CallTo(() => fakeReference.FindCodeByName(A<string>._)).Returns(0);
            data.Products = new ProductData[0];

            var message = JsonConvert.SerializeObject(data);

            Approvals.Verify(pp.ProcessProviderData(message));
        }
    }
}
