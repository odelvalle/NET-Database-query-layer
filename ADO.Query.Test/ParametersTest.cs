
namespace ADO.Query.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using global::ADO.Query.Test.AdoMocks;
    using global::ADO.Query.Test.Query;

    [TestClass]
    public class ParametersTest
    {
        [TestMethod]
        public void TestParametersInQuery()
        {
            var adoHelper = new AdoMockHelper
            {
                ReturnValues = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Id", 1},
                        { "Name", "Test"}                   
                    }                
                }
            };

            adoHelper.ExecuteScalar<int>(new QueryWithParameters(1, "Test"));

            Assert.IsNotNull(adoHelper.Parameters.SingleOrDefault(p => p.ParameterName == "id"));
            Assert.IsNotNull(adoHelper.Parameters.SingleOrDefault(p => p.ParameterName == "name"));


            // ReSharper disable PossibleNullReferenceException
            var id = Convert.ToInt32(adoHelper.Parameters.SingleOrDefault(p => p.ParameterName == "id").Value);
            var name = Convert.ToString(adoHelper.Parameters.SingleOrDefault(p => p.ParameterName == "name").Value);
            // ReSharper restore PossibleNullReferenceException

            Assert.AreEqual(1, id);
            Assert.AreEqual("Test", name);
        }

        [TestMethod]
        public void TestNullParametersInQuery()
        {
            var adoHelper = new AdoMockHelper
            {
                ReturnValues = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Id", 1},
                        { "Name", "Test"}                   
                    }                
                }
            };

            adoHelper.ExecuteScalar<int>(new QueryWithParameters(1, null));

            Assert.IsNotNull(adoHelper.Parameters.SingleOrDefault(p => p.ParameterName == "name"));

            // ReSharper disable PossibleNullReferenceException
            Assert.AreEqual(DBNull.Value, adoHelper.Parameters.SingleOrDefault(p => p.ParameterName == "name").Value);
            // ReSharper restore PossibleNullReferenceException
        }
    }
}
