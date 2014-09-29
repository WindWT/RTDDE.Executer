using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Data.SQLite;
using System.Reflection;

namespace RTDDataProvider
{
    public static class DAL
    {        
        private static readonly string DB_LOCAL_FILEPATH = "RTD.db";
        private static readonly string connectionString = "Data Source=" + DB_LOCAL_FILEPATH;

        static DAL()
        {
            if (!System.IO.File.Exists(DB_LOCAL_FILEPATH))
            {
                //如果文件不存在，则创建数据库
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "CREATE TABLE Demo(id integer NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE)";
                        command.ExecuteNonQuery();

                        command.CommandText = "DROP TABLE Demo";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// 执行SQL获取值类型数据
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public static T Get<T>(string sql) where T : struct
        {
            return Get<T>(sql, null);
        }

        /// <summary>
        /// 执行SQL获取值类型数据
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="paras">SQL参数List</param>
        /// <returns></returns>
        public static T Get<T>(string sql, List<SQLiteParameter> paras) where T : struct
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                if (paras != null)
                {
                    foreach (var para in paras)
                    {
                        cmd.Parameters.Add(para);
                    }
                }

                object o = cmd.ExecuteScalar();
                return (T)Convert.ChangeType(o, typeof(T));
            }
        }

        /// <summary>
        /// 执行SQL获取引用类型数据
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T ToSingle<T>(string sql) where T : class,new()
        {
            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();

            bool isFieldOnly = (properties.Length == 0);
            T result = default(T);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string[] names = GetColumnNames(reader);
                        for (int i = 0; i < names.Length; i++)
                        {
                            string name = names[i];
                            object value = reader.GetValue(i);
                            if (isFieldOnly)
                            {
                                FieldInfo field = fields.First(o => o.Name == name);
                                field.SetValue(result, Convert.ChangeType(value, field.FieldType));
                            }
                            else
                            {
                                PropertyInfo property = properties.First(o => o.Name == name);
                                property.SetValue(result, Convert.ChangeType(value, property.PropertyType), null);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static List<T> ToList<T>(string sql) where T : class,new()
        {
            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();

            bool isFieldOnly = (properties.Length == 0);
            List<T> list = new List<T>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    string[] names = GetColumnNames(reader);
                    while (reader.Read())
                    {
                        T obj = new T();
                        for (int i = 0; i < names.Length; i++)
                        {
                            string name = names[i];
                            object value = reader.GetValue(i);
                            if (isFieldOnly)
                            {
                                FieldInfo field = fields.First(o => o.Name == name);
                                field.SetValue(obj, Convert.ChangeType(value, field.FieldType));
                            }
                            else
                            {
                                PropertyInfo property = properties.First(o => o.Name == name);
                                property.SetValue(obj, Convert.ChangeType(value, property.PropertyType), null);
                            }
                        }
                        list.Add(obj);
                    }
                }
            }
            return list;
        }

        public static void FromSingle<T>(T obj) where T : class,new()
        {
            throw new NotImplementedException();
        }

        public static void FromList<T>(List<T> obj) where T : class,new()
        {
            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();

            bool isFieldOnly = (properties.Length == 0);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteTransaction trans = connection.BeginTransaction())
                {
                }                
            }
        }

        public static void DropTable(string tableName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(String.Format("DROP TABLE IF EXISTS {0}",tableName), connection);
                command.ExecuteNonQuery();
            }
        }

        private static string[] GetColumnNames(SQLiteDataReader reader)
        {
            int count = reader.FieldCount;
            string[] names = new string[count];
            for (int i = 0; i < count; i++)
            {
                names[i] = reader.GetName(i);
            }
            return names;
        }
    }
    public class DALAttribute : Attribute
    {
        public bool PrimaryKey { get; set; }
    }
}
