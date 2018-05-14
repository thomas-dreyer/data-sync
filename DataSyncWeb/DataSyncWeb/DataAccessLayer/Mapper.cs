using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace DataSyncWeb.DataAccessLayer
{
    public static class Mapper
    {

        private static T mapFromRow<T>(DataRow row, List<string> dataColumns) where T : class, new()
        {
            T type = new T();
            try
            {
                foreach (var property in getProperties(type))
                {
                    if (dataColumns.Contains(property.Name))
                    {
                        if (property.PropertyType.Name == "Char")
                        {
                            property.SetValue(type, row[property.Name].ToString()[0]);
                        }
                        else
                        {
                            property.SetValue(type, row[property.Name]);
                        }
                    }
                }
            }
            catch (Exception exception)
            {

                Console.WriteLine(exception);
            }
            return type;
        }

        private static PropertyInfo[] getProperties<T>(T classType) where T : class
        {
            return classType.GetType().GetProperties();
        }

        public static List<T> mapFromTable<T>(DataTable result) where T: class, new()
        {
            List<T> resultItems = new List<T>();
            List<string> columnMap = new List<string>();
            foreach (DataColumn dataColumn in result.Columns)
            {
                columnMap.Add(dataColumn.ColumnName);
            }
            foreach (DataRow dataRow in result.Rows)
            {
                resultItems.Add(mapFromRow<T>(dataRow, columnMap));
            }
            return resultItems;
        }

        public static SqlParameter[] getInsertParameters<T>(T classType) where T : class
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            List<string> columns = getColumns(classType);
            columns.Remove(string.Format("{0}Id", classType.GetType().Name));
            Dictionary<string, PropertyInfo> properties = getProperties(classType).ToDictionary(x => x.Name, x => x);
            foreach (string column in columns)
            {
                parameters.Add(new SqlParameter(string.Format("@{0}", column), properties[column].GetValue(classType)));
            }
            return parameters.ToArray();
        }

        private static List<string> getColumns<T>(T classType) where T : class
        {
            var properties = getProperties(classType);
            List<string> columns = new List<string>();
            foreach (PropertyInfo property in properties)
            {
                columns.Add(property.Name);
            }
            return columns;
        }
    }
}
