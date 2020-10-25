using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UMC.Data;
using UMC.Data.Sql;

namespace UMC.Web.Activity
{

    class SystemUpgradeActivity : WebActivity
    {
        public override void ProcessActivity(WebRequest request, WebResponse response)
        {

            if (request.IsMaster == false)
            {
                this.Prompt("只有管理员才能升级");
            }
            var Key = Utility.Guid(Guid.NewGuid());
            var log = new UMC.Data.CSV.Log(Utility.GetRoot(request.Url), Key, "开始安装");

            var Initializers = Data.Sql.Initializer.Initializers();
            var Hask = new Dictionary<String, DbProvider>();
            foreach (var initer in Initializers)
            {
                Hask[initer.ProviderName] = UMC.Data.Database.Instance(initer.ProviderName).DbProvider;


            }
            Data.Reflection.Start(() =>
            {
                try
                {
                    var now = DateTime.Now;

                    var database = Reflection.Configuration("database") ?? new UMC.Configuration.ProviderConfiguration();
                    foreach (var initer in Initializers)
                    {
                        if (database.Providers.ContainsKey(initer.ProviderName))
                        {

                            log.Info("正在升级", initer.Caption);
                            initer.Check(log, Hask[initer.ProviderName]);
                        }

                        else
                        {
                            log.Info("未安装", initer.Caption);
                        }


                    }


                    log.End("检测升级已完成");

                }
                catch (Exception ex)
                {
                    log.End("安装失败");
                    log.Info(ex.Message);

                }

            });

            this.Context.Send("Initializer", false);

            response.Redirect("System", "Log", Key);
        }

    }
}
