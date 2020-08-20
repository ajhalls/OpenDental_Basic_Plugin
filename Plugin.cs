using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using OpenDental;
using OpenDentBusiness;
using MySql.Data.MySqlClient;
using MessageBox = System.Windows.Forms.MessageBox;

///<summary>The namespace for this class must match the dll filename, including capitalization.
///All other classes will typically belong to the same namespace too, but that's not a requirement.</summary>
namespace PluginExample
{
    ///<summary>Required class.  Don't change the name.</summary>
    public class Plugin : PluginBase
    {
        public override void LaunchToolbarButton(long patNum)
        {
            var patient = FormOpenDental.CurPatNum;
            var results = Query("SELECT * FROM patient");
            MessageBox.Show("Total Patients: " + results.Rows.Count);
            OpenDental.GotoModule.GotoChart(patient);
            // OnModuleSelected(new ModuleEventArgs(DateTime.MinValue, new List<long>(), 0, EnumModuleType.Chart, 0, patNum, 0));
        }

        public static DataTable Query(string Query)
        {
            DataTable rawData = null;
            var ConnectionString = DataConnectionBase.DataConnection.GetConnectionString();
            using (var mySqlConnection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    mySqlConnection.Open();
                }
                catch (MySqlException ex)
                {
                }
                catch (Exception ex)
                {
                }

                // Setting timeout on mysqlServer
                var timeoutAdjust = new MySqlCommand("set net_write_timeout=400; set net_read_timeout=400;use opendental;", mySqlConnection);
                timeoutAdjust.ExecuteNonQuery();

                using (DbCommand com = new MySqlCommand(Query, mySqlConnection))
                {
                    com.CommandTimeout = 400;
                    IDataReader reader = null;

                    using (reader = com.ExecuteReader())
                    {
                        var dtSchema = reader.GetSchemaTable();
                        rawData = QueryToDataTable(reader, dtSchema);
                    }
                }
                mySqlConnection.Close();
            }

            return rawData;
        }

        public static DataTable QueryToDataTable(IDataReader reader, DataTable dtSchema)
        {
            var listCols = new List<DataColumn>();
            var rawData = new DataTable();
            if (dtSchema != null)
                foreach (DataRow drow in dtSchema.Rows)
                {
                    var columnName = Convert.ToString(drow["ColumnName"]);
                    DataColumn column;
                    if ((Type)drow["DataType"] == typeof(System.Byte[]))
                    {
                        column = new DataColumn(columnName, typeof(String));
                    }
                    else
                    {
                        column = new DataColumn(columnName, (Type)drow["DataType"]);
                    }

                    column.Unique = false;
                    column.AllowDBNull = true;
                    column.AutoIncrement = false;

                    listCols.Add(column);
                    rawData.Columns.Add(column);
                }

            while (reader.Read())
            {
                var dataRow = rawData.NewRow();
                var columnCount = listCols.Count;
                for (var i = 0; i < columnCount; i++)
                {
                    if (reader[i].GetType() == typeof(System.Byte[]))
                    {
                        dataRow[listCols[i]] = BitConverter.ToString((byte[])reader[i]).Replace("-", "");
                    }
                    else
                    {
                        dataRow[listCols[i]] = reader[i];
                    }
                }

                rawData.Rows.Add(dataRow);
            }

            return rawData;
        }

        public static void WriteLine(string str = "", bool PrintAlways = false, int MessageLevel = 1, string FileName = @"Plugin.log", bool AccentItem = false)
        {
            if (AccentItem)
            {
                Console.WriteLine("\n");
            }

            Console.WriteLine(DateTime.Now + " [Info] " + str);

            if (AccentItem)
            {
                Console.WriteLine("\n");
            }

            try
            {
                using (var fs = File.Open(@"C:\BitBucket\OpenDental\head\OpenDental\bin\Debug\" + FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                    FileShare.Delete | FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(fs))
                    using (var writer = new StreamWriter(fs))
                    {
                        //read
                        fs.Position = fs.Length;
                        writer.Write(DateTime.Now + " [Info] " + str + Environment.NewLine);
                        writer.Flush();
                        //write
                    }

                    fs.Close();
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("WriteLine - Error: " + e.Message);
            }
        }
    }
}