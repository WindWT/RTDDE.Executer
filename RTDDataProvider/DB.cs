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
        private static readonly string DB_LOCAL_FILEPATH = "RTD.db";
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
        public void ImportDataSet(DataSet ds, bool isOverwrite)
        {
            foreach (DataTable dt in ds.Tables)
            {
                ImportDataTable(dt, isOverwrite);
            }
        }
        public void ImportDataTable(DataTable dt)
        {
            ImportDataTable(dt, "id", false);
        }
        public void ImportDataTable(DataTable dt, bool isOverwrite)
        {
            ImportDataTable(dt, "id", isOverwrite);
        }
        public void ImportDataTable(DataTable dt, string pk)
        {
            ImportDataTable(dt, pk, false);
        }
        public void ImportDataTable(DataTable dt, string pk, bool isOverwrite)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteTransaction trans = connection.BeginTransaction();
                try
                {
                    SQLiteCommand createTableCmd = new SQLiteCommand(connection);

                    //make a backup for old table, for data compare.
                    createTableCmd.CommandText = "DROP TABLE IF EXISTS " + dt.TableName + "_old";
                    createTableCmd.ExecuteNonQuery();

                    createTableCmd.CommandText = "CREATE TABLE " + dt.TableName + "_old" + " AS SELECT * FROM " + dt.TableName;
                    createTableCmd.ExecuteNonQuery();

                    if (isOverwrite)
                    {
                        //drop old table for clean insert
                        createTableCmd.CommandText = "DROP TABLE IF EXISTS " + dt.TableName;
                        createTableCmd.ExecuteNonQuery();
                    }
                    createTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS " + dt.TableName + "(";
                    foreach (DataColumn column in dt.Columns)
                    {
                        createTableCmd.CommandText += column.ColumnName;
                        if (String.Compare(column.ColumnName, pk, StringComparison.OrdinalIgnoreCase) == 0)
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
            return GetData(sql, null);
        }
        public DataTable GetData(string sql, List<SQLiteParameter> paras)
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

                object o = cmd.ExecuteScalar();
                string result = string.Empty;
                if (o != null)
                {
                    result = o.ToString();
                }

                connection.Close();
                return result;
            }
        }
    }
}