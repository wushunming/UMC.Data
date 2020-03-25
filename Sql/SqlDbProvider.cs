using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;

namespace UMC.Data.Sql
{
    class SqlBuilder : DbBuilder
    {
        public override string AddColumn(string name, string field, string type)
        {
            return string.Format("ALTER TABLE {0} ADD {1} {2}", name, field, type);
        }

        public override string Boolean()
        {
            return "BIT";
        }

        public override string Date()
        {
            return "DATETIME";
        }

        public override string DropColumn(string name, string field)
        {
            return string.Format("ALTER TABLE {0} DROP COLUMN {1}", name, field);
        }

        public override string Float()
        {
            return "FLOAT";
        }

        public override string Guid()
        {
            return "UNIQUEIDENTIFIER";
        }

        public override string Integer()
        {
            return "INTEGER";
        }

        public override string Number()
        {
            return "DECIMAL(16,2)";
        }

        public override string PrimaryKey(string name, params string[] fields)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendFormat("ALTER TABLE {0} ADD PRIMARY KEY (", name);// id);")
            foreach (var s in fields)
            {
                sb.AppendFormat("{0}", s);
                sb.Append(',');

            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");
            return sb.ToString();
        }

        public override string String()
        {
            return "NVARCHAR(255)";
        }

        public override string Text()
        {
            return "NVARCHAR(MAX)";
        }
        public override string Schema(string Prefixion)
        {
            return string.Format("CREATE SCHEMA {0} AUTHORIZATION [dbo]", Prefixion);
        }
        public override bool? Check(string name, string field, ISqler sqler)
        {

            int m = Convert.ToInt32(sqler.ExecuteScalar("select  count(*)  from sys.columns where object_id=object_id({0}) AND name={1}", name, field));
            return m > 0;
        }
        public override bool? Check(string name, ISqler sqler)
        {

            int m = Convert.ToInt32(sqler.ExecuteScalar("select  count(*)  from sys.objects where object_id=object_id({0})", name));
            return m > 0;
        }
    }


    public class SqlDbProvider : UMC.Data.Sql.DbProvider
    {
        private static System.Data.Common.DbProviderFactory Instance;

        //public override string Year(string feild)
        //{
        //    return String.Format("DATEPART(yy,{0})", feild);
        //}
        //public override string Month(string feild)
        //{
        //    return String.Format("DATEPART(mm,{0})", feild);
        //}
        //public override string Day(string feild)
        //{
        //    return String.Format("DATEPART(dd,{0})", feild);
        //}
        //public override string Hour(string feild)
        //{
        //    return String.Format("DATEPART(hh,{0})", feild);
        //}
        //public override string Week(string feild)
        //{
        //    return String.Format("DATEPART(dw,{0})", feild);
        //}
        //public override string Minute(string feild)
        //{
        //    return String.Format("DATEPART(mi,{0})", feild);
        //}

        protected override string ParamsPrefix
        {
            get
            {
                return "@";
            }
        }

        public override DbBuilder Builder => new SqlBuilder();

        public override string GetIdentityText(string tableName)
        {
            return "SELECT @@IDENTITY";
        }
        public override string QuotePrefix
        {
            get
            {
                return "[";
            }
        }
        public override string QuoteSuffix
        {
            get
            {
                return "]";
            }
        }
        public override string GetPaginationText(int start, int limit, string SelectText)
        {
            StringBuilder sb = new StringBuilder(SelectText);

            if (start > 0)
            {
                sb.Insert(SelectText.IndexOf("select", StringComparison.OrdinalIgnoreCase) + 6, String.Format(" TOP {0} ", start + limit));
                sb.Insert(0, "SELECT IDENTITY(INT,0,1) AS __WDK_Page_ID , WebADNukePagge.* INTO #__WebADNukePagges FROM(");

                sb.Append(") AS WebADNukePagge");
                sb.AppendLine();
                sb.AppendFormat("SELECT *FROM  #__WebADNukePagges  WHERE __WDK_Page_ID >={0}", start);

                sb.AppendLine();
                sb.Append("DROP TABLE #__WebADNukePagges");

            }
            else
            {
                sb.Insert(SelectText.IndexOf("select", StringComparison.OrdinalIgnoreCase) + 6, String.Format(" TOP {0}", limit));
            }
            return sb.ToString();


        }
        public override string ConntionString
        {
            get
            {
                return this.Provider.Attributes["conString"];
            }
        }

        public override System.Data.Common.DbProviderFactory DbFactory
        {
            get
            {

                if (Instance == null)
                {
                    var als = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var a in als)//mscorlib, 
                    {
                        var type = a.GetType("System.Data.SqlClient.SqlClientFactory");
                        if (type != null)
                        {
                            Instance = type.GetField("Instance").GetValue(null) as System.Data.Common.DbProviderFactory;

                            break;
                        }
                    }
                    if (Instance == null)
                    {
                        throw new Exception("请引用SQL客户端");

                    }
                }
                return Instance;
            }
        }

    }
}
