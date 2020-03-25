using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;

namespace UMC.Web.Activity
{
    class SystemClickActivity : WebActivity
    {
        class ScanningDialog : Web.UIGridDialog
        {

            public Guid? UserId
            {
                get; set;
            }
            protected override Hashtable GetData(IDictionary form)
            {
                int limit = UMC.Data.Utility.IntParse(form["limit"] as string, 25);
                int start = UMC.Data.Utility.IntParse(form["start"] as string, 0);


                string sort = form[("sort")] as string;
                string dir = form[("dir")] as string; ;

                var productEntity = UMC.Data.Database.Instance().ObjectEntity<UMC.Data.Entities.Click>();
                var where = productEntity.Where;
                if (this.UserId.HasValue)
                {
                    where = productEntity.Where.And().Equal(new UMC.Data.Entities.Click { user_id = this.UserId }).Contains();
                }


                if (!String.IsNullOrEmpty(sort))
                {
                    if (dir == "DESC")
                    {
                        productEntity.Order.Desc(sort);
                    }
                    else
                    {
                        productEntity.Order.Asc(sort);
                    }
                }
                else
                {
                    productEntity.Order.Desc(new UMC.Data.Entities.Click
                    {
                        Quality = 0
                    });
                }

                var cus = new List<IDictionary>();
                var webr = UMC.Data.WebResource.Instance();
                productEntity.Query(start, limit, dr =>
                {
                    var dic = UMC.Data.Reflection.PropertyToDictionary(dr);
                    //var src = webr.ResolveUrl(dr.Id.Value, 1, "0") + "!200?" + (dr.ModifiedTime.HasValue ? WebADNuke.Data.WebUtility.TimeSpan(dr.ModifiedTime.Value) : 0);
                    dic["Code"] = UMC.Data.Utility.Parse62Encode(dr.Code.Value);
                    dic.Remove("Query");
                    cus.Add(dic);
                });

                var total = productEntity.Count();
                var hash = new Hashtable();
                hash["data"] = cus;
                if (total == 0)
                {

                    hash["msg"] = "您未有宣传二维码";
                }
                else
                {
                    hash["total"] = total;
                }
                return hash;
            }

            protected override Hashtable GetHeader()
            {
                var header = new Header("Code", 25);
                header.AddField("Code", "短码"); ;
                header.AddField("Quality", "次数"); ;
                return header.GetHeader();
            }
        }
        public override void ProcessActivity(WebRequest request, WebResponse response)
        {
            var code = this.AsyncDialog("Code", g =>
            {
                var user = UMC.Security.Identity.Current;
                var t = new ScanningDialog() { Title = "我的宣传推码" };
                t.IsPage = true;
                t.RefreshEvent = "Scanning";
                t.UserId = user.Id;
                t.IsReturnValue = false;
                return t;
            });

            Hashtable hashtable;
            var url = UMC.Data.Utility.Scanning(code, request, out hashtable);
            if (url != null)
            {
                this.Context.Send(new UMC.Web.WebMeta().Put("value", url.AbsoluteUri).Put("type", "OpenUrl"), true);
            }
            if (hashtable != null)
            {
                var user = UMC.Security.Identity.Current;
                if (user.IsAuthenticated == false)
                {
                    response.Redirect("Account", "Login");
                }
                var model = hashtable["model"] as string;
                var cmd = hashtable["cmd"] as string;
                if (String.IsNullOrEmpty(model) == false && String.IsNullOrEmpty(cmd) == false)
                {
                    if (hashtable.ContainsValue("send"))
                    {
                        var send = hashtable["send"];
                        if (send is Hashtable)
                        {
                            var pos = new UMC.Web.WebMeta(send as Hashtable);

                            response.Redirect(model, cmd, pos, true);
                        }
                        else
                        {
                            response.Redirect(model, cmd, send.ToString());
                        }
                    }
                    else
                    {

                        response.Redirect(model, cmd);
                    }
                }
                else
                {

                    this.Prompt("不能识别，此指令");
                }

            }
            else
            {
                this.Prompt("未检测到此指令");
            }


        }


    }
}