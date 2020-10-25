using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using UMC.Data;
using UMC.Data.Sql;
using UMC.Web;

namespace UMC.Web.Activity
{
    class SystemConfigActivity : UMC.Web.WebActivity
    {
        public override void ProcessActivity(UMC.Web.WebRequest request, UMC.Web.WebResponse response)
        {
            if (request.IsMaster == false)
            {
                this.Prompt("只有管理员才能配置");
            }

            var configKey = this.AsyncDialog("Key", g =>
           {
               var form = request.SendValues ?? new UMC.Web.WebMeta();
               if (form.ContainsKey("limit") == false)
               {
                   this.Context.Send(new UISectionBuilder(request.Model, request.Command, new WebMeta(request.Arguments.GetDictionary()))
                       .RefreshEvent("ProviderConfiguration")
                           .Builder(), true);
               }
               var key = this.AsyncDialog("Type", "FILES");

               var ui = UISection.Create(new UITitle("配置文件"));
               if (key == "FILES")
               {
                   var files = System.IO.Directory.GetFiles(Data.Reflection.ConfigPath(""), "*.xml");
                   foreach (var f in files)
                   {
                       var name = f.Substring(f.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1);
                       name = name.Substring(0, name.IndexOf('.'));
                       var d = "";

                       ui.Title.Right(new UIEventText("新建").Click(new UIClick("Key", "NEW").Send(request.Model, request.Command)));

                       switch (name.ToLower())
                       {
                           case "assembly":
                               d = "处理类配置";
                               break;
                           case "database":
                               d = "数据库配置";
                               break;
                           case "umc":
                               d = "云模块配置";
                               break;
                           case "parser":
                               d = "转码配置";
                               break;
                           case "payment":
                               d = "支付配置";
                               break;
                       }
                       ui.AddCell(name, d, UIClick.Query(new WebMeta("Type", name)));
                   }
               }
               else
               {
                   var keys = key.Split('$');
                   var cfg = UMC.Configuration.ProviderConfiguration.GetProvider(Reflection.ConfigPath(keys[0] + ".xml"));//, "*.xml");

                   //ui.AddCell("")
                   if (keys.Length == 1)
                   {
                       ui.Title.Right(new UIEventText("新建").Click(new UIClick("Key", keys[0] + "$NEW").Send(request.Model, request.Command)));
                       ui.AddCell('\uf112', "上一层", keys[0], UIClick.Query(new WebMeta("Type", "FILES")));
                       var ui2 = ui.NewSection();
                       for (var i = 0; i < cfg.Count; i++)
                       {
                           var p = cfg[i];
                           ui2.AddCell(p.Name, "", UIClick.Query(new WebMeta("Type", String.Format("{0}${1}", keys[0], p.Name))));
                       }
                   }
                   else
                   {
                       var p = cfg[keys[1]];
                       ui.Title.Right(new UIEventText("新建").Click(new UIClick("Key", keys[0] + "$" + p.Name + "$NEW").Send(request.Model, request.Command)));
                       ui.AddCell('\uf112', "上一层", p.Name, UIClick.Query(new WebMeta("Type", keys[0])));
                       ui.AddCell("类型类型", p.Type);
                       var ui2 = ui.NewSection();
                       for (var i = 0; i < p.Attributes.Count; i++)
                       {
                           ui2.AddCell(p.Attributes.GetKey(i), new UIClick(g, String.Format("{0}${1}${2}", keys[0], p.Name, p.Attributes.GetKey(i))).Send(request.Model, request.Command));
                       }
                   }
               }

               response.Redirect(ui);
               return this.DialogValue("none");
           });
            switch (configKey)
            {
                case "NEW":
                    var fName = this.AsyncDialog("Setting", g =>
                    {
                        var fm = new UIFormDialog();
                        fm.Title = "新建文件配置";
                        fm.AddText("新建文件名", "Name", String.Empty);
                        fm.Submit("确认", request, "ProviderConfiguration");
                        return fm;
                    });
                    var pf = Reflection.ConfigPath(fName["Name"] + ".xml");
                    if (System.IO.File.Exists(pf) == false)
                    {
                        new UMC.Configuration.ProviderConfiguration().WriteTo(pf);
                        this.Context.Send("ProviderConfiguration", true);
                    }
                    else
                    {
                        this.Prompt("此文件已经存在");
                    }
                    break;
                default:
                    var ckeys = configKey.Split('$');
                    var cfg = UMC.Configuration.ProviderConfiguration.GetProvider(Reflection.ConfigPath(ckeys[0] + ".xml"));

                    if (ckeys.Length == 3)
                    {
                        var pro = cfg[ckeys[1]];
                        var ps = this.AsyncDialog("Setting", g =>
                         {
                             var fm = new UIFormDialog();
                             fm.Title = ckeys[1] + "配置";

                             if (ckeys[2] == "NEW")
                             {
                                 fm.AddText("新建属性名", "Name", String.Empty);
                                 fm.AddText("新建属性值", "Value", String.Empty);
                             }
                             else
                             {

                                 fm.AddTextValue().Add("属性名", ckeys[2]);
                                 fm.AddText("属性值", "Value", pro[ckeys[2]]);
                             }
                             fm.Submit("确认", request, "ProviderConfiguration");
                             return fm;
                         });
                        var value = ps["Value"];
                        if (value == "none")
                        {
                            pro.Attributes.Remove(ckeys[2]);
                        }
                        else
                        {
                            pro.Attributes[ps["Name"] ?? ckeys[2]] = ps["Value"];
                        }
                        cfg.WriteTo(Reflection.ConfigPath(ckeys[0] + ".xml"));
                        this.Context.Send("ProviderConfiguration", true);

                    }
                    else if (ckeys.Length == 2)
                    {
                        var pro = cfg[ckeys[1]] ?? Provider.Create("", "");
                        var ps = this.AsyncDialog("Setting", g =>
                        {
                            var fm = new UIFormDialog();
                            fm.Title = ckeys[0] + "节点";

                            if (ckeys[1] == "NEW")
                            {
                                fm.AddText("节点名", "Name", String.Empty);
                                fm.AddText("类型值", "Value", String.Empty);
                            }
                            else
                            {

                                fm.AddText("节点名", "Name", pro.Name);
                                fm.AddText("类型值", "Value", pro.Type);
                            }
                            fm.Submit("确认", request, "ProviderConfiguration");
                            return fm;
                        });

                        var pro2 = Provider.Create(ps["Name"], ps["Value"]);
                        pro2.Attributes.Add(pro.Attributes);
                        cfg.Providers[pro2.Name] = pro2;
                        cfg.WriteTo(Reflection.ConfigPath(ckeys[0] + ".xml"));
                        this.Context.Send("ProviderConfiguration", true);
                    }
                    break;
            }


        }
    }
}