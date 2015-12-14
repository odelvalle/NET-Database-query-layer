
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
            var mockAdo = QueryRunner.CreateHelper("MockAdoHelper");
            Assert.IsInstanceOfType(mockAdo, typeof(MockQueryRunner));
        }

        [TestMethod]
        public void TestDependencyInjectionCreateHelper()
        {
            var container = new UnityContainer();
            container.RegisterType<IQueryRunner>(new InjectionFactory(c => QueryRunner.CreateHelper("MockAdoHelper")));

            var mockAdo = container.Resolve<IQueryRunner>();
            Assert.IsInstanceOfType(mockAdo, typeof(MockQueryRunner));
        }
    }
}
