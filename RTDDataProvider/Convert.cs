using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RTDDataProvider
{
    public static class ObjectConvert
    {
        public static DataTable ToDataTable(Object[] array)
        {
            FieldInfo[] fields = array.GetType().GetElementType().GetFields();
            PropertyInfo[] properties = array.GetType().GetElementType().GetProperties();

            bool isProperty = (properties.Length > 0);

            DataTable dt = null;
            if (isProperty)
            {
                dt = CreateDataTable(properties);
            }
            else
            {
                dt = CreateDataTable(fields);
            }

            if (array.Length != 0)
            {
                foreach (object o in array)
                {
                    if (isProperty)
                    {
                        FillData(properties, dt, o);
                    }
                    else
                    {
                        FillData(fields, dt, o);
                    }
                }
            }
            return dt;
        }

        private static DataTable CreateDataTable(FieldInfo[] fields)
        {
            DataTable dt = new DataTable();
            DataColumn dc = null;
            foreach (FieldInfo fi in fields)
            {
                dc = new DataColumn();
                dc.ColumnName = fi.Name;
                dc.DataType = fi.FieldType;
                dt.Columns.Add(dc);
            }
            return dt;
        }

        private static DataTable CreateDataTable(PropertyInfo[] properties)
        {
            DataTable dt = new DataTable();
            DataColumn dc = null;
            foreach (PropertyInfo pi in properties)
            {
                dc = new DataColumn();
                dc.ColumnName = pi.Name;
                dc.DataType = pi.PropertyType;
                dt.Columns.Add(dc);
            }
            return dt;
        }

        private static void FillData(FieldInfo[] fields, DataTable dt, Object o)
        {
            DataRow dr = dt.NewRow();
            foreach (FieldInfo fi in fields)
            {
                dr[fi.Name] = fi.GetValue(o);
            }
            dt.Rows.Add(dr);
        }

        private static void FillData(PropertyInfo[] properties, DataTable dt, Object o)
        {
            DataRow dr = dt.NewRow();
            foreach (PropertyInfo pi in properties)
            {
                dr[pi.Name] = pi.GetValue(o, null);
            }
            dt.Rows.Add(dr);
        }
    }
}
