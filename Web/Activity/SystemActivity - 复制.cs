using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UMC.Data;

namespace UMC.Web.Activity
{
    class SystemActivity : WebActivity
    {
        public override void ProcessActivity(WebRequest request, WebResponse response)
        {
            switch (request.Command)
            {

                case "TimeSpan":
                    response.Redirect(new WebMeta().Put("time", Utility.TimeSpan()));
                    break;
                case "Mapping":

                    var Initializers = new List<Data.Sql.Initializer>(Data.Sql.Initializer.Initializers());
                    Initializers.Insert(0, new UMC.Data.Entities.Initializer());
                    var database = Reflection.Configuration("Database") ?? new Configuration.ProviderConfiguration();

                    var data = new System.Data.DataTable();
                    data.Columns.Add("name");
                    data.Columns.Add("text");
                    data.Columns.Add("setup", typeof(bool));
                    foreach (var n in Initializers)
                    {
                        data.Rows.Add(n.ProviderName, n.Caption, database.Providers.ContainsKey(n.ProviderName));
                    }
                    response.Redirect(new WebMeta().Put("component", data).Put("data", WebServlet.Mapping()));
                    break;
            }

        }

    }
}
