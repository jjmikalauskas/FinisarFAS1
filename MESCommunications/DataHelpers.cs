using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MESCommunications.Utility
{
    public static class DataHelpers
    {
        public static DataTable MakeWaferListIntoDataTable(List<Wafer> wafers)
        {
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("Slot");
            dt.Columns.Add("WaferNo");
            dt.Columns.Add("WaferID");
            dt.Columns.Add("ScribeID");
            dt.Columns.Add("Product");
            dt.Columns.Add("Operation");
            dt.Columns.Add("ContainerID");
            dt.Columns.Add("Status");
            dt.Columns.Add("Recipe");

            foreach (var wafer in wafers)
            {
                DataRow row = dt.NewRow();
                row["Slot"] = wafer.Slot;
                row["WaferNo"] = wafer.WaferNo;
                row["WaferID"] = wafer.WaferID;
                row["ScribeID"] = wafer.ScribeID;
                row["Product"] = wafer.Product;
                row["Operation"] = wafer.Operation;
                row["ContainerID"] = wafer.ContainerID;
                row["Status"] = wafer.Status;
                row["Recipe"] = wafer.Recipe;
                dt.Rows.Add(row);
            }

            return dt;
        }

        public static List<Wafer> MakeDataTableIntoWaferList(DataTable dt)
        {
            List<Wafer> wafers = new List<Wafer>();
            wafers = (from DataRow row in dt.Rows
                      select new Wafer()
                      {
                          Slot = row["Slot"].ToString(),
                          WaferNo = row["WaferNo"].ToString(),
                          WaferID = row["WaferID"].ToString(),
                          ScribeID = row["ScribeID"].ToString(),
                          Product = row["Product"].ToString(),
                          Operation = row["Operation"].ToString(),
                          ContainerID = row["ContainerID"].ToString(),
                          Status = row["Status"].ToString(),
                          Recipe = row["Recipe"].ToString()
                      }).ToList();

            return wafers;
        }

        // LATER
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}
