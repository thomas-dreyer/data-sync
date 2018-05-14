using System.Collections.Generic;
using System.Data;

namespace DataSyncWeb.DataAccessLayer
{
    public class DataManager
    {
        private Connection connection = new Connection();
        public List<T> SelectData<T>(T item, string whereClause = "") where T : class, new()
        {
            string query = GenericBuilder.Select(item, whereClause);
            DataTable table = connection.Select(query);
            List<T> result = Mapper.mapFromTable<T>(table);
            return result;
        }

        public int InsertData<T>(T item) where T: class
        {
            string query = GenericBuilder.Insert(item);
            return connection.InsertParameterized(query, Mapper.getInsertParameters(item));
        }

        public bool UpdateData<T>(T item) where T : class
        {
            string query = GenericBuilder.Update(item);
            return connection.Update(query) > 0;
        }
    }
}
