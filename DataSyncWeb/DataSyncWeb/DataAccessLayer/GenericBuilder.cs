using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DataSyncWeb.DataAccessLayer
{
    public static class GenericBuilder
    {
        public static string Select<T>(T classType, string whereClause = "") where T : class
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ");
            query.Append(string.Format("{0}", string.Join(", ", getColumns(classType))));
            query.Append(string.Format(" FROM {0} {1}", classType.GetType().Name, whereClause));
            return query.ToString();
        }

        public static string Insert<T>(T classType) where T : class
        {
            StringBuilder query = new StringBuilder();
            query.Append("INSERT INTO ");
            List<string> columns = getColumns(classType);
            columns.Remove(string.Format("{0}Id", classType.GetType().Name));
            query.Append(string.Format("{0} ({1}) ", classType.GetType().Name, string.Join(", ", columns)));
            query.Append(string.Format("VALUES ({0}); SELECT SCOPE_IDENTITY();", InsertParameterHelper(columns)));
            return query.ToString();
        }

        private static string InsertParameterHelper(List<string> columns)
        {
            List<string> vlues = new List<string>();
            foreach (string column in columns)
            {
                vlues.Add(string.Format("@{0}", column));
            }
            return string.Join(", ", vlues);
        }

        private static string InsertValueHelper<T>(T classType, List<string> columns) where T : class
        {
            Dictionary<string, PropertyInfo> properties = getProperties(classType).ToDictionary(x => x.Name, x => x);
            List<string> vlues = new List<string>();
            foreach (string column in columns)
            {
                if (properties.ContainsKey(column))
                {
                    vlues.Add(string.Format("'{0}'", properties[column].GetValue(classType)));
                }
            }
            return string.Join(", ", vlues);
        }

        private static string UpdateValueHelper<T>(T classType, List<string> columns) where T : class
        {
            Dictionary<string, PropertyInfo> properties = getProperties(classType).ToDictionary(x => x.Name, x => x);
            List<string> vlues = new List<string>();
            string primary = string.Empty;
            foreach (string column in columns)
            {
                if (column == string.Format("{0}Id", classType.GetType().Name))
                {
                    primary = string.Format(" WHERE {0} = {1}", column, properties[column].GetValue(classType));
                }
                else
                {
                    if (properties.ContainsKey(column))
                    {
                        string format = "{0} = {1}";

                        switch (properties[column].PropertyType.Name.ToUpper())
                        {
                            case "DECIMAL":
                            case "INT32":
                            case "LONG":
                                format = "{0} = {1}";
                                break;
                            default:
                                format = "{0} = '{1}'";
                                break;
                        }
                        vlues.Add(string.Format(format, column, properties[column].GetValue(classType)));
                    }
                }
            }
            return string.Join(", ", vlues) + primary;
        }

        public static string Update<T>(T classType) where T : class
        {
            StringBuilder query = new StringBuilder();
            query.Append(string.Format("UPDATE {0} SET ", classType.GetType().Name));
            List<string> columns = getColumns(classType);
            
            query.Append(string.Format("{0}", UpdateValueHelper(classType, columns)));
            return query.ToString();
        }
        private static PropertyInfo[] getProperties<T>(T classType) where T : class
        {
            return classType.GetType().GetProperties();
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
