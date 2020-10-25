using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UMC.Data;
using UMC.Data.Entities;
using UMC.Web;

namespace UMC.Web.Activity
{
    class SystemWebActivity : WebActivity
    {
        public override void ProcessActivity(WebRequest request, WebResponse response)
        {
            if (request.IsMaster == false)
            {
                this.Prompt("需要管理员权限");
            }

            var svalue = this.AsyncDialog("Key", d =>
            {
                return this.DialogValue("New");
            });
            switch (svalue)
            {
                case "RESETAPPSECRET":
                    this.Prompt("AppSecret", "AppSecret：" + Data.WebResource.Instance().AppSecret(true));
                    break;
                case "APPSECRET":
                    this.Prompt("AppSecret", "AppSecret：" + Data.WebResource.Instance().AppSecret());
                    break;
            }
            var file = Reflection.ConfigPath("UMC.xml");
            var cfg = Configuration.ProviderConfiguration.GetProvider(file) ?? new Configuration.ProviderConfiguration();
            var n = cfg[svalue] ?? Data.Provider.Create(String.Empty, String.Empty);
            var Settings = this.AsyncDialog("Settings", g =>
            {
                var fm = new UIFormDialog() { Title = "配置云模块" };

                if (String.IsNullOrEmpty(n.Name))
                    fm.AddText("云模块名", "name", n.Name);
                else
                {
                    fm.AddTextValue().Put("云模块名", n.Name);
                }
                fm.AddText("指令通配符", "type", n.Type);
                fm.AddText("描述", "desc", n["desc"]);
                fm.AddText("服务网址", "src", n["src"]);
                fm.AddText("AppSecret", "secret", n["secret"]).NotRequired();
                if (String.IsNullOrEmpty(n.Name) == false)
                {
                    fm.AddCheckBox("", "Status", "NO").Put("移除", "DEL");
                }
                fm.Submit("确认", request, "Config");
                return fm;
            });
            var status = Settings["Status"] ?? "";
            if (status.Contains("DEL"))
            {
                cfg.Providers.Remove(svalue);
            }
            else
            {
                var src = new Uri(Settings["src"]);
                var p = Data.Provider.Create(Settings["name"] ?? svalue, Settings["type"]);
                p.Attributes.Add("src", src.AbsoluteUri);
                p.Attributes.Add("desc", Settings["desc"]);
                if (String.IsNullOrEmpty(Settings["secret"]) == false)
                    p.Attributes.Add("secret", Settings["secret"]);
                cfg.Providers[p.Name] = p;//.Add(p.Name, p);
            }
            UMC.Configuration.ProviderConfiguration.Cache.Clear();

            cfg.WriteTo(file);
            var data2 = new System.Data.DataTable();
            data2.Columns.Add("model");
            data2.Columns.Add("cmd");
            data2.Columns.Add("text");
            data2.Columns.Add("src");


            for (var i = 0; i < cfg.Count; i++)
            {
                var p = cfg[i];
                var cmd = p.Type;// p["regex"]; // p.Type;

                if (String.IsNullOrEmpty(p.Type))
                {
                    cmd = "*";
                }

                data2.Rows.Add(p.Name, cmd, p["desc"], p["src"]);

            }

            this.Context.Send("Config", new WebMeta().Put("data", data2), false);

            this.Prompt("配置成功");

        }
    }

}
