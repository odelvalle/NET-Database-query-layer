namespace ADO.Query.Test
{
    using System;
    using System.IO;

    using ADO.Query.Helper;
    using ADO.Query.Test.Query;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryIntegrationTest
    {
        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
        }

        [TestMethod]
        public void TestIntegrationDataTableQuery()
        {
            var queryRunner = QueryRunner.CreateHelper("SqlAdoHelper");
            var dt = queryRunner.ExecuteDataTable(new QueryUsers());

            Assert.IsNotNull(dt);
            Assert.IsNotNull(dt.Rows);

            Assert.AreEqual(2, dt.Rows.Count);
            Assert.AreEqual(1, Convert.ToInt32(dt.Rows[0]["Id"]));
            Assert.AreEqual("User 1", Convert.ToString(dt.Rows[0]["Name"]));
        }
    }
}
