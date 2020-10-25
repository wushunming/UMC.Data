﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UMC.Data;
using UMC.Data.Sql;

namespace UMC.Web.Activity
{

    class SystemSetupActivity : WebActivity
    {
        string MySQL(WebMeta meta)
        {
            var str = "Server={0};Port={4};Database={1};Uid={2};Pwd={3};Charset=utf8";
            if (meta["Port"] == "3306")
            {
                str = "Server={0};Database={1};Uid={2};Pwd={3};Charset=utf8";
            }

            return String.Format(str, meta["Server"], meta["Database"], meta["User"], meta["Password"], meta["Port"]);
            //Port
        }
        string MSSQL(WebMeta meta)
        {

            var str = "Data Source={0},{4};Initial Catalog={1};User ID={2};Password={3};";
            if (meta["Port"] == "1433")
            {
                str = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};";
            }

            return String.Format(str, meta["Server"], meta["Database"], meta["User"], meta["Password"], meta["Port"]);

        }
        string Oracle(WebMeta meta)
        {
            if (meta.ContainsKey("Port") == false)
            {
                meta["Port"] = "1521";
            }
            var str = "User Id={2};Password={3};Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={4})))(CONNECT_DATA=(SERVICE_NAME={1})))";
            return String.Format(str, meta["Server"], meta["Database"], meta["User"], meta["Password"], meta["Port"]);

        }
        public override void ProcessActivity(WebRequest request, WebResponse response)
        {

            var type = this.AsyncDialog("type", g =>
            {
                return this.DialogValue("SQLite");
                var fm = new UISheetDialog() { Title = "安装数据库" };

                //fm.Options.Add(new UIClick("Oracle") { Text = "Oracle数据库" }.Send(request.Model, request.Command));
                //fm.Options.Add(new UIClick("MySql") { Text = "MySql数据库" }.Send(request.Model, request.Command));
                //fm.Options.Add(new UIClick("MSSQL") { Text = "SQL Server数据库" }.Send(request.Model, request.Command));
                fm.Options.Add(new UIClick("SQLite") { Text = "SQLite文件数据库" }.Send(request.Model, request.Command));
                return fm;
            });
            var Settings = this.AsyncDialog("Settings", g =>
            {
                var fm = new UIFormDialog() { Title = "选择数据库" };

                fm.AddText("服务地址", "Server");
                fm.AddText("用户名", "User");
                fm.AddText("密码", "Password");
                fm.AddText("数据库名", "Database");
                switch (type)
                {
                    case "SQLite":
                        return this.DialogValue(new WebMeta().Put("File", "UMC"));
                    case "Oracle":
                        fm.AddText("端口", "Port", "1521");
                        fm.AddText("表前缀", "Prefix").Put("tip", "分表设置");
                        fm.Title = "Oracle连接配置";
                        break;
                    case "MySql":
                        fm.AddText("端口", "Port", "3306");
                        fm.AddText("表前缀", "Prefix").Put("tip", "分表设置");
                        fm.Title = "MySql连接配置";
                        break;
                    case "MSSQL":
                        fm.AddText("端口", "Port", "1433");
                        fm.AddText("表前缀", "Prefix").Put("tip", "分表设置");
                        fm.AddText("拆表符", "Delimiter", "_");
                        fm.Title = "SQL Server连接配置";
                        break;
                    default:
                        this.Prompt("数据类型错误");
                        break;
                }
                fm.Submit("确认安装", request, "Initializer");
                return fm;
            });
            UMC.Data.Provider provder = null;


            switch (type)
            {
                case "SQLite":
                    provder = UMC.Data.Provider.Create("Database", typeof(UMC.Data.Sql.SQLiteDbProvider).FullName);

                    var file = Settings["File"];
                    if (file.IndexOf('.') == -1)
                    {
                        file = file + ".sqlite";
                    }
                    var path = UMC.Data.Utility.MapPath(String.Format("~App_Data/{0}", file));

                    if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                    }
                    if (System.IO.File.Exists(path))
                    {
                        int i = 1;
                        var m = String.Format("{0}.{1}.bak", path, i);
                        while (System.IO.File.Exists(m))
                        {
                            i++;
                            String.Format("{0}.{1}.bak", path, i);
                        }
                        System.IO.File.Move(path, m);
                    }
                    provder.Attributes["db"] = file;
                    break;
                case "Oracle":
                    provder = UMC.Data.Provider.Create("Database", typeof(UMC.Data.Sql.OracleDbProvider).FullName);
                    provder.Attributes["conString"] = Oracle(Settings);
                    break;
                case "MySql":
                    provder = UMC.Data.Provider.Create("Database", typeof(UMC.Data.Sql.MySqlDbProvider).FullName);
                    provder.Attributes["conString"] = MySQL(Settings);
                    break;
                case "MSSQL":
                    provder = UMC.Data.Provider.Create("Database", typeof(UMC.Data.Sql.SqlDbProvider).FullName);
                    provder.Attributes["conString"] = MSSQL(Settings);
                    break;
                default:
                    this.Prompt("数据类型错误");
                    break;
            }
            if (String.IsNullOrEmpty(Settings["Prefix"]) == false)
            {
                provder.Attributes["delimiter"] = Settings["Delimiter"] ?? "_";
                provder.Attributes["prefix"] = Settings["Prefix"];
            }
            DbProvider provider = Reflection.CreateObject(provder) as DbProvider;

            DbFactory factory = new DbFactory(provider);
            try
            {
                factory.Open();
                factory.Close();

            }
            catch (Exception ex)
            {
                this.Prompt(ex.Message);
            }

            var Key = Utility.Guid(Guid.NewGuid());
            var log = new UMC.Data.CSV.Log(Utility.GetRoot(request.Url), Key, "开始安装");
            Data.Reflection.Start(() =>
               {
                   var Initializers = Data.Sql.Initializer.Initializers();
                   try
                   {
                       var now = DateTime.Now;
                       var Names = new Hashtable();
                       var database = Reflection.Configuration("database") ?? new UMC.Configuration.ProviderConfiguration();
                       var count = false;
                       foreach (var initer in Initializers)
                       {
                           var dataPro = database[initer.ProviderName] ?? UMC.Data.Provider.Create(initer.ProviderName, provder.Type);
                           var setupKey = dataPro["setup"] ?? "";
                           if (setupKey.Contains(initer.Name) == false)
                           {
                               count = true;
                               initer.Setup(new Hashtable(), log, provider);
                               initer.Menu(new Hashtable(), new DbFactory(provider));
                               var de = UMC.Data.Provider.Create(initer.ProviderName, provder.Type);
                               de.Attributes.Add(provder.Attributes);
                               de.Attributes["setup"] = String.Format("{0}{1},", setupKey, initer.Name);
                               database.Providers[initer.ProviderName] = de;

                           }
                           else
                           {
                               initer.Check(log, Reflection.CreateObject(dataPro) as DbProvider);
                           }
                       }

                       UMC.Configuration.ProviderConfiguration.Cache.Clear();
                       if (count == false)
                       {
                           log.End("对应组件已经安装");
                       }
                       else
                       {
                           var file = Reflection.AppDataPath("UMC\\database.xml");
                           if (System.IO.File.Exists(file))
                           {
                               int i = 1;
                               var m = Reflection.AppDataPath(String.Format("UMC\\database.xml.{0}.bak", i));
                               while (System.IO.File.Exists(m))
                               {
                                   i++;
                                   m = Reflection.AppDataPath(String.Format("UMC\\database.xml.{0}.bak", i));
                               }
                               System.IO.File.Move(file, m);
                           }
                           database.WriteTo(file);
                           log.End("安装完成", "默认账户:admin 密码:admin", "请刷新界面");
                           log.Info(String.Format("用时{0}", DateTime.Now - now));

                       }
                   }
                   catch (Exception ex)
                   {
                       log.End("安装失败");
                       log.Info(ex.Message);

                   }
                   finally
                   {
                       log.Close();
                   }
               });

            this.Context.Send("Initializer", false);

            response.Redirect("System", "Log", Key);
        }

    }
}
