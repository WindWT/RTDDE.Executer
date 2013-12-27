using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Data.SQLite;

namespace RTDDataProvider
{
    public class DB
    {
        private string connectionString = string.Empty;
        private static readonly string DB_LOCAL_FILEPATH = "/RTD.db";
        public DB()
            : this(DB_LOCAL_FILEPATH)
        {
        }
        public DB(string dbPath)
        {
            connectionString = "Data Source=" + dbPath;
            if (!System.IO.File.Exists(dbPath))
            {
                //创建表
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
                    connection.Close();
                }
            }
        }
        public void ImportDataSet(DataSet ds)
        {
            ImportDataSet(ds, false);
        }
        public void ImportDataSet(DataSet ds,bool isOverwrite)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteTransaction trans = connection.BeginTransaction();
                try
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        SQLiteCommand createTableCmd = new SQLiteCommand(connection);
                        if (isOverwrite)
                        {
                            createTableCmd.CommandText = "DROP TABLE IF EXISTS " + dt.TableName;
                            createTableCmd.ExecuteNonQuery();
                        }
                        createTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS " + dt.TableName + "(";
                        foreach (DataColumn column in dt.Columns)
                        {
                            createTableCmd.CommandText += column.ColumnName;
                            if (String.Compare(column.ColumnName, "id", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                createTableCmd.CommandText += " PRIMARY KEY,";
                            }
                            else if (String.Compare(dt.TableName, "LEVEL_DATA_MASTER", StringComparison.OrdinalIgnoreCase) == 0 &&
                                String.Compare(column.ColumnName, "level_data_id", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                createTableCmd.CommandText += " PRIMARY KEY,";
                            }
                            else
                            {
                                createTableCmd.CommandText += ",";
                            }
                        }
                        createTableCmd.CommandText = createTableCmd.CommandText.TrimEnd(',');
                        createTableCmd.CommandText += ");";
                        createTableCmd.ExecuteNonQuery();

                        foreach (DataRow dr in dt.Rows)
                        {
                            SQLiteCommand upsertRowCmd = new SQLiteCommand(connection);
                            upsertRowCmd.CommandText = "INSERT OR REPLACE INTO " + dt.TableName + "(";
                            StringBuilder sqlColumnName = new StringBuilder();
                            StringBuilder sqlColumnValue = new StringBuilder();
                            foreach (DataColumn column in dt.Columns)
                            {
                                string columnName = column.ColumnName;
                                object columnValue = dr[column];
                                sqlColumnName.Append(columnName);
                                sqlColumnName.Append(",");
                                sqlColumnValue.Append("@" + columnName);
                                sqlColumnValue.Append(",");
                                SQLiteParameter param = new SQLiteParameter(columnName, columnValue);
                                upsertRowCmd.Parameters.Add(param);
                            }
                            upsertRowCmd.CommandText += sqlColumnName.ToString();
                            upsertRowCmd.CommandText = upsertRowCmd.CommandText.TrimEnd(',');
                            upsertRowCmd.CommandText += ") VALUES (";
                            upsertRowCmd.CommandText += sqlColumnValue.ToString();
                            upsertRowCmd.CommandText = upsertRowCmd.CommandText.TrimEnd(',');
                            upsertRowCmd.CommandText += ");";
                            upsertRowCmd.ExecuteNonQuery();
                            System.Diagnostics.Debug.WriteLine(upsertRowCmd.CommandText);
                        }
                    }
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public DataTable GetData(string sql)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, connection);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                connection.Close();
                return dt;
            }
        }
        public string GetString(string sql)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, connection);

                string result = (string)cmd.ExecuteScalar();

                connection.Close();
                return result;
            }
        }
    }
}