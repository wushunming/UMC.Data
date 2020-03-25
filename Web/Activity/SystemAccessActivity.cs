using System;
using System.Collections.Generic;
using System.Text;
using UMC.Data;

namespace UMC.Web.Activity
{
    class SystemAccessActivity : WebActivity
    {
        public override void ProcessActivity(WebRequest request, WebResponse response)
        {
            var filename = Data.Utility.MapPath(String.Format("App_Data\\{1}\\{0}\\", "Log", "Access"));
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(filename)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filename));
            }

            var files = System.IO.Directory.GetFiles(filename);
            if (files.Length > 0)
            {
                var fName = files[0];
                var name = System.IO.Path.GetFileName(fName);
                var temp = Data.Utility.MapPath(String.Format("App_Data\\{1}\\{0}", name, "Access"));
                if (System.IO.File.Exists(temp) == false)
                {
                    System.IO.File.Move(fName, temp);
                    var logEntity = Database.Instance().ObjectEntity<Data.Entities.Log>();
                    new System.Threading.Tasks.Task(() =>
                    {
                        try
                        {
                            using (System.IO.StreamReader reader = new System.IO.StreamReader(temp, Encoding.UTF8))
                            {
                                CSV.EachRow(reader, data =>
                                {
                                    if (data.Length > 2)
                                    {
                                        var log = new Data.Entities.Log
                                        {
                                            Time = Utility.TimeSpan(Utility.IntParse(data[0], 0)),
                                            Username = data[1],
                                            LogKey = data[2]
                                        };
                                        if (data.Length > 3)
                                        {
                                            log.Caption = data[3];
                                        }
                                        if (data.Length > 4)
                                        {
                                            log.IPAddress = data[4];
                                        }
                                        if (data.Length > 5)
                                        {
                                            log.UserAgent = data[5];
                                        }
                                        if (data.Length > 6)
                                        {
                                            log.Context = data[6];
                                        }
                                        logEntity.Insert(log);
                                    }
                                });
                            }
                        }
                        catch (Exception ex)
                        {

                            Utility.Error("Log", ex.ToString());
                        }
                        finally
                        {
                            System.IO.File.Delete(temp);
                        }

                    }).Start();
                    response.Redirect(new WebMeta().Put("file", name));
                }
                else
                {
                    response.Redirect(new WebMeta().Put("wait", name));
                }
            }
            else
            {
                response.Redirect(new WebMeta().Put("file", "none"));
            }
        }
    }
}
