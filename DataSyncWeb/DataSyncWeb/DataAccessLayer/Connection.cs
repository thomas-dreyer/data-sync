using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace DataSyncWeb.DataAccessLayer
{
    public class Connection
    {
        private SqlConnection connection;


        private bool connect()
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            return connection.State == System.Data.ConnectionState.Open;
        }

        public DataTable Select(string query)
        {
            DataTable result = new DataTable("result");
            if (connect())
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(result);
            }
            return result;
        }

        public int Insert(string query)
        {
            int result = -1;
            if (connect())
            {
                SqlCommand insert = new SqlCommand(query, connection);
                var obj = insert.ExecuteScalar();
                result = Convert.ToInt32(obj);
            }
            return result;
        }

        public int Update(string query)
        {
            int result = -1;
            if (connect())
            {
                SqlCommand insert = new SqlCommand(query, connection);
                result = insert.ExecuteNonQuery();
            }
            return result;
        }

        public int InsertParameterized(string query, SqlParameter[] parameters)
        {
            int result = -1;
            if (connect())
            {
                SqlCommand insert = new SqlCommand(query, connection);
                insert.Parameters.AddRange(parameters);
                var obj = insert.ExecuteScalar();
                result = Convert.ToInt32(obj);
            }
            return result;
        }

        public Connection(string connectionName = "default")
        {
            this.connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionName].ConnectionString);
        }

    }
}
