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
    class SystemIconActivity : UMC.Web.WebActivity
    {
        public override void ProcessActivity(UMC.Web.WebRequest request, UMC.Web.WebResponse response)
        {
            var key = this.AsyncDialog("Key", g => this.DialogValue("Promotion"));



            var UserId = Utility.Parse(this.AsyncDialog("Icon", g =>
            {
                var form = request.SendValues ?? new UMC.Web.WebMeta();
                if (form.ContainsKey("limit") == false)
                {
                    this.Context.Send(new UISectionBuilder(request.Model, request.Command, new WebMeta(request.Arguments.GetDictionary()))
                        .CloseEvent("UI.Event")
                            .Builder(), true);
                }
                var uTitle = new UITitle("选择图标");
                var ui = UISection.Create(new UIHeader().Search("搜索"), uTitle);
                var Keyword = (form["Keyword"] as string ?? String.Empty);
                var icons = new List<UIEventText>();

                Array es = System.Enum.GetValues(typeof(UIIcon));

                foreach (var v in es)
                {
                    var name = v.ToString().Substring(3);
                    if (String.IsNullOrEmpty(Keyword) == false)
                    {
                        if (name.IndexOf(Keyword, StringComparison.CurrentCultureIgnoreCase) == -1)
                        {
                            continue;
                        }
                    }
                    icons.Add(new UIEventText(name).Icon(UIFormDialog.icons[Convert.ToInt32(v)][0], 0x111).Click(new UIClick(new WebMeta(request.Arguments).Put(g, v)).Send(request.Model, request.Command)));

                    if (icons.Count % 4 == 0)
                    {
                        ui.AddIcon(icons.ToArray());
                        icons.Clear();
                    }

                }
                if (icons.Count > 0)
                    ui.AddIcon(icons.ToArray());
                if (ui.Length == 0)
                {
                    ui.Add("Desc", new UMC.Web.WebMeta().Put("desc", "未搜索到图标").Put("icon", "\uEA05")
                , new UMC.Web.WebMeta().Put("desc", "{icon}\n{desc}"), new UIStyle().Align(1).Color(0xaaa).Padding(20, 20).BgColor(0xfff).Size(12).Name("icon", new UIStyle().Font("wdk").Size(60)));//.Name 

                }
                response.Redirect(ui);
                return this.DialogValue("none");
            }), UIIcon.fa_glass);

            var item = new ListItem(UserId.ToString(), UIFormDialog.icons[Convert.ToInt32(UserId)]);
            this.Context.Send(new UMC.Web.WebMeta().UIEvent(key, item), true);
        }



    }
}