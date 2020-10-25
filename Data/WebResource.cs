using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Drawing;

namespace UMC.Data
{


    /// <summary>
    ///Web提供基础资源管理类
    /// </summary>
    public class WebResource : UMC.Configuration.DataProvider
    {

        /// <summary>
        /// 图片路径
        /// </summary>
        public const string ImageResource = "~/images/";
        /// <summary>
        /// 用户资源路径
        /// </summary>
        public const string UserResources = "~/UserResources/";
        static WebResource _Instance;

        public static WebResource Instance()
        {
            if (_Instance == null)
            {
                var pc = Reflection.Configuration("assembly");
                if (pc == null)
                {
                    pc = new Configuration.ProviderConfiguration();


                }
                var provider = pc["WebResource"];
                if (provider == null)
                {
                    provider = Data.Provider.Create("WebResource", "UMC.Data.WebResource");
                    provider.Attributes["authkey"] = Utility.Guid(Guid.NewGuid());
                    provider.Attributes["secret"] = Utility.Guid(Guid.NewGuid());
                    pc.Providers[provider.Name] = provider;
                    pc.WriteTo(Reflection.AppDataPath("UMC\\assembly.xml"));
                    Configuration.ProviderConfiguration.Cache.Clear();
                }
                _Instance = UMC.Data.Reflection.CreateObject(provider) as WebResource;
                if (_Instance == null)
                {
                    _Instance = new WebResource();
                    UMC.Data.Reflection.SetProperty(_Instance, "Provider", provider);

                }
            }
            return _Instance;
        }

        public virtual string WebDomain()
        {
            return this.Provider["domain"] ?? "/";
        }
        public virtual string APIHost()
        {
            return this.Provider["host"];
        }
        public virtual string AppSecret(bool isRefresh = false)
        {
            if (isRefresh)
            {
                var pc = Reflection.Configuration("assembly");
                if (pc == null)
                {
                    pc = new Configuration.ProviderConfiguration();

                }
                var provider = pc["WebResource"];
                if (provider == null)
                {
                    provider = Data.Provider.Create("WebResource", "UMC.Data.WebResource");
                }

                this.Provider.Attributes["secret"] = provider.Attributes["secret"] = Utility.Guid(Guid.NewGuid());
                pc.Providers[provider.Name] = provider;
                pc.WriteTo(Reflection.AppDataPath("UMC\\assembly.xml"));
                Configuration.ProviderConfiguration.Cache.Clear();
            }
            return this.Provider["secret"];

        }
        public virtual string TempDomain()
        {
            return "http://oss.365lu.cn/";

        }
        public virtual string AccessToken(bool isRefresh = false)
        {
            return String.Empty;
        }

        public virtual bool SubmitCheck(string appid)
        {
            return true;
        }
        public virtual System.Collections.Hashtable Submit(String method, string json, string appId = "")
        {
            return null;
        }


        public virtual void Cache<T>(String key, T value)
        {

        }
        public virtual T Cache<T>(String key)
        {
            return default(T);
        }

        public virtual string ResolveUrl(Guid id, object seq, object size)
        {
            var kdey = "";
            switch (size.ToString())
            {
                case "0":
                    break;
                case "1":
                    kdey = "!350";
                    break;
                case "2":
                    kdey = "!200";
                    break;
                case "3":
                    kdey = "!150";
                    break;
                case "4":
                    kdey = "!100";
                    break;
                case "5":
                    kdey = "!50";
                    break;
            }
            return ResolveUrl(String.Format("{2}{0}/{1}/0.jpg{3}", id, seq, UMC.Data.WebResource.ImageResource, kdey));

        }
        public virtual string ImageResolve(Guid id, string key, int size)
        {
            int seq = UMC.Data.Utility.Parse(key, -1);
            if (seq < 0)
            {
                seq = key.GetHashCode();
                if (seq > 0)
                {
                    seq = -seq;
                }
            }
            var kdey = "";
            switch (size)
            {
                case 0:
                    break;
                case 1:
                    kdey = "!350";
                    break;
                case 2:
                    kdey = "!200";
                    break;
                case 3:
                    kdey = "!150";
                    break;
                case 4:
                    kdey = "!100";
                    break;
                case 5:
                    kdey = "!50";
                    break;
            }
            return ResolveUrl(String.Format("{2}{0}/{1}/0.jpg{3}", id, seq, UMC.Data.WebResource.ImageResource, kdey));

        }

        public virtual void CopyResolveUrl(String source, String target)
        {
            var sourcePath = Data.Utility.MapPath(this.ResolveUrl(source));
            var targetPath = Data.Utility.MapPath(this.ResolveUrl(target));
            if (System.IO.File.Exists(sourcePath))
            {
                File.Copy(sourcePath, targetPath);
            }

        }

        /// <summary>
        /// 转移资源目录下文件
        /// </summary>
        /// <param name="fileName"></param>
        public virtual String Transfer(string fileName, string targetKey)
        {
            return String.Empty;
        }
        public virtual void Transfer(Stream stream, string targetKey)
        {
            if (targetKey.StartsWith("bin/", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            else if (targetKey.StartsWith("app_data/", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            else if (targetKey.StartsWith("UserResources/") || targetKey.StartsWith("images/"))
            {
                Utility.Copy(stream, Utility.MapPath("App_Data/Static/" + targetKey));
            }
            else if (targetKey.IndexOf('.') == -1)
            {
                Utility.Copy(stream, Utility.MapPath("App_Data/Static/" + targetKey + ".html"));

            }
        }
        public virtual string Download(string tempKey)
        {
            return String.Format("/TEMP/{0}", tempKey.Replace("\\", "/"));

        }
        public virtual void Transfer(Uri soureUrl, string targetKey)
        {
            if (targetKey.StartsWith("bin/", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            else if (targetKey.StartsWith("app_data/", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            else if (targetKey.StartsWith("UserResources/") || targetKey.StartsWith("images/"))
            {
                var httpClient = new UMC.Net.HttpClient();

                Utility.Copy(httpClient.GetStreamAsync(soureUrl).Result, Utility.MapPath(targetKey));
            }
            else if (targetKey.IndexOf('.') == -1)
            {
                var httpClient = new UMC.Net.HttpClient();

                Transfer(httpClient.GetStreamAsync(soureUrl).Result, targetKey);

            }
        }


        public virtual string ResolveUrl(string path)
        {
            String vUrl = path;
            if (path.StartsWith("~/"))
            {
                vUrl = path.Substring(1);
            }
            else if (path.StartsWith("~"))
            {
                vUrl = "/" + path.Substring(1);
            }
            String src = this.Provider["src"];
            if (String.IsNullOrEmpty(src))
            {

                String vpath = this.Provider["authkey"];

                if (String.IsNullOrEmpty(vpath) == false)
                {
                    String code = Utility.ParseEncode(Utility.Guid(vpath).GetHashCode(), 36);
                    vpath = code;// + "/";
                }

                return String.Format("https://image.365lu.cn/{0}{1}", vpath, vUrl);


            }
            return src + vUrl;
        }
        public virtual void Transfer(Uri uri, Guid guid, int seq, string type)
        {
            String vpath = this.Provider["authkey"];
            if (String.IsNullOrEmpty(vpath) == false)
            {
                String code = Utility.ParseEncode(Utility.Guid(vpath).GetHashCode(), 36);
                vpath = code + "/";


                String key = String.Format("{0}images/{1}/{2}/{3}.{4}", vpath, guid, seq, 0, type.ToLower());


                String sts = String.Format("https://ali.365lu.cn/OSS/Transfer/{0}", this.Provider["authkey"]);


                var httpClient = new UMC.Net.HttpClient();
                var res = httpClient.PostAsync(sts, new System.Net.Http.StringContent(JSON.Serialize(new Web.WebMeta().Put("src", uri.AbsoluteUri, "key", key)))).Result;

            }

        }
        public virtual void Transfer(Uri uri, Guid guid, int seq)
        {
            Transfer(uri, guid, seq, "jpg");
        }


        public virtual void Push(Guid tid, params object[] objs)
        {
            this.Push(new Guid[] { tid }, objs);
        }

        public virtual void Push(Guid[] devices, params object[] objs)
        {
            String vpath = this.Provider["authkey"];
            if (String.IsNullOrEmpty(vpath) == false && devices.Length > 0 && objs.Length > 0)
            {
                String sts = String.Format("https://ali.365lu.cn/OSS/Push/{0}", this.Provider["authkey"]);


                var httpClient = new UMC.Net.HttpClient();
                var res = httpClient.PostAsync(sts, new System.Net.Http.StringContent(JSON.Serialize(new Web.WebMeta().Put("device", devices).Put("data", objs)))).Result;

            }
        }





    }
}
