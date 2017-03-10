using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using MetaDataV2;

namespace DbShow.Dal
{
    public class T4Base
    {
        protected readonly Configuration _configuration;
        public string NamespaceStr = "";
        public List<string> NoTable = new List<string>();
        public List<string> OnlyTable = new List<string>();
        public static Database MyDb;
        public static Dictionary<string, FieldObject> Decs = new Dictionary<string, FieldObject>();
        public static Dictionary<string, TableObject> TableDecs = new Dictionary<string, TableObject>();
        public string Indent = "";
        public SqlConnection DB;

        public void PushIndent(string str)
        {
            this.Indent += str;
        }

        public void PopIndent()
        {
            this.Indent = this.Indent.Remove(this.Indent.Length - 1);
        }

        public T4Base(string templateFilePath, string connectionName = "MetaDataDB")
        {
            string directoryName = Path.GetDirectoryName(templateFilePath);
            string exeConfigFilename = Directory.GetFiles(directoryName, "*.config").FirstOrDefault<string>() ?? Directory.GetParent(directoryName).GetFiles("*.config").FirstOrDefault<FileInfo>().FullName;
            this._configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
            {
                ExeConfigFilename = exeConfigFilename
            }, ConfigurationUserLevel.None);
            AppSettingsSection appSettingsSection = (AppSettingsSection)this._configuration.GetSection("appSettings");
            ConnectionStringsSection connectionStringsSection = (ConnectionStringsSection)this._configuration.GetSection("connectionStrings");
            MdFactory.SetConnectionStr(connectionStringsSection.ConnectionStrings[connectionName].ConnectionString);
            string strSql = appSettingsSection.Settings[connectionName].Value;
            T4Base.MyDb = MdFactory.SetCurrentDbName(strSql, true);
            foreach (TableObject current in T4Base.MyDb.GetTableView())
            {
                if (!T4Base.TableDecs.ContainsKey(current.name))
                {
                    T4Base.TableDecs.Add(current.name, current);
                }
                foreach (FieldObject current2 in current.Columns)
                {
                    if (!T4Base.Decs.ContainsKey(current2.ColumnName))
                    {
                        T4Base.Decs.Add(current2.ColumnName, current2);
                    }
                }
            }
        }

        public T4Base()
        {
        }

        public void SetDb(string dbName, bool refresh = false)
        {
            T4Base.MyDb = MdFactory.SetCurrentDbName(dbName, refresh);
        }

        public DataSet ExeSqlDataSet(string sqlStr)
        {
            if (this.DB == null || this.DB.Database != T4Base.MyDb.DbName)
            {
                this.DB = new SqlConnection(T4Base.MyDb.ConnectionString);
            }
            SqlCommand sqlCommand = new SqlCommand(sqlStr, this.DB);
            sqlCommand.CommandType = CommandType.Text;
            DataSet dataSet = new DataSet();
            new SqlDataAdapter(sqlCommand).Fill(dataSet);
            return dataSet;
        }

        public void TablesAndViews(StringBuilder str)
        {
            using (List<TableObject>.Enumerator enumerator = T4Base.MyDb.GetTableView().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    TableObject table = enumerator.Current;
                    if ((this.OnlyTable.Count == 0 || this.OnlyTable.Contains(table.name)) && !this.NoTable.Contains(table.name))
                    {
                        foreach (FieldObject arg_85_0 in table.Columns)
                        {
                        }
                        IEnumerable<ProObject> enumerable =
                            from z in T4Base.MyDb.Procedures
                            where z.text.IndexOf(table.name) >= 0
                            select z;
                        foreach (ProObject arg_D1_0 in enumerable)
                        {
                        }
                    }
                }
            }
        }

        public string GetTbDes(string tbName)
        {
            int num = tbName.LastIndexOf('.');
            if (num > -1)
            {
                tbName = tbName.Substring(num + 1);
            }
            if (T4Base.TableDecs.ContainsKey(tbName))
            {
                TableObject tableObject = T4Base.TableDecs[tbName];
                return tableObject.comments + tableObject.dates;
            }
            return "";
        }

        public string GetDes(string fieldName)
        {
            if (T4Base.Decs.ContainsKey(fieldName))
            {
                FieldObject fieldObject = T4Base.Decs[fieldName];
                return fieldObject.deText.Trim() + fieldObject.TypeNameCs + fieldObject.Length;
            }
            return "";
        }
    }
}