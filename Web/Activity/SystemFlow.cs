using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UMC.Data;

namespace UMC.Web.Activity
{

    [Mapping("System", Desc = "UMC基础组件")]
    class SystemFlow : WebFlow
    {
        public override WebActivity GetFirstActivity()
        {
            switch (this.Context.Request.Command)
            {
                case "Icon":
                    return new SystemIconActivity();
                case "TimeSpan":
                    this.Context.Response.Redirect(new WebMeta().Put("time", Utility.TimeSpan()));
                    break;
                case "Start":
                    this.Context.Send("Setup", true);
                    break;
                case "Mapping":

                    var Initializers = Data.Sql.Initializer.Initializers();
                    var database = Reflection.Configuration("Database") ?? new Configuration.ProviderConfiguration();

                    var data = new System.Data.DataTable();
                    data.Columns.Add("name");
                    data.Columns.Add("text");
                    data.Columns.Add("setup", typeof(bool));
                    foreach (var n in Initializers)
                    {
                        data.Rows.Add(n.Name, n.Caption, database.Providers.ContainsKey(n.ProviderName));
                    }
                    this.Context.Response.Redirect(new WebMeta().Put("component", data).Put("data", WebServlet.Mapping()));
                    break;
                case "Menu":
                    return new SystemMenuActivity();
                case "Log":
                    return new SystemLogActivity();
                case "Setup":
                    return new SystemSetupActivity();
                case "Upgrade":
                    return new SystemUpgradeActivity();
                case "BackUp":
                    return new SystemBackUpActivity();
                case "Access":
                    return new SystemAccessActivity();
                case "Click":
                    return new SystemClickActivity();
                case "Restore":
                    return new SystemRestoreActivity();
            }
            return WebActivity.Empty;
        }

    }
}
