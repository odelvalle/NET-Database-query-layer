
namespace ADO.Query.Test
{
    using ADO.Query.Helper;
    using ADO.Query.Test.AdoMocks;

    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FactoryTest
    {
        [TestMethod]
        public void TestCreateHelper()
        {
            var mockAdo = AdoHelper.CreateHelper("MockAdoHelper", null);
            Assert.IsInstanceOfType(mockAdo, typeof(AdoMockHelper));
        }

        [TestMethod]
        public void TestDependencyInjectionCreateHelper()
        {
            var container = new UnityContainer();
            container.RegisterType<IAdoHelper>(new InjectionFactory(c => AdoHelper.CreateHelper("MockAdoHelper", null)));

            var mockAdo = container.Resolve<IAdoHelper>();
            Assert.IsInstanceOfType(mockAdo, typeof(AdoMockHelper));
        }
    }
}
