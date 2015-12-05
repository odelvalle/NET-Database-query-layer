namespace ADO.Query.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Linq;

    public static class DataReaderToDynamic
    {
        public static IList<dynamic> ToDynamic(this IDataReader reader)
        {
            var dynamicDr = new List<dynamic>();
            while (reader.Read())
            {
                var dyn = ToExpando(reader);
                dynamicDr.Add(dyn);
            }

            return dynamicDr;
        }

        private static dynamic ToExpando(IDataRecord record)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (var i = 0; i < record.FieldCount; i++)
                expandoObject.Add(record.GetName(i), record[i]);

            return expandoObject;
        }
    }
}
