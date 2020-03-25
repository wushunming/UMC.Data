using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using System.IO.Compression;
using System.Collections;
using UMC.Configuration;
using System.Collections.Generic;
using UMC.Data;

namespace UMC.Web
{
    /// <summary>
    /// POS¥¶¿Ì
    /// </summary>
    public class WebServlet : UMC.Net.INetHandler
    {
        class SysSession : WebSession
        {
            static string _Header;
            protected internal override void Check(WebContext context)
            {

                this.Header = _Header;// Security.AccessToken.Get("WebSession");
            }

            protected internal override bool IsAuthorization(string model, string command)
            {
                return true;
            }

            protected internal override IDictionary<string, object> Outer(WebClient client, WebContext context)
            {
                return null;

            }

            protected internal override void Storage(IDictionary header, WebContext context)
            {
                _Header = JSON.Serialize(header);
            }
        }
        public static List<MappingAttribute> Mappings(int category)
        {
            return WebRuntime.Categorys.FindAll(m => m.Category == category);
        }
        void Download(UMC.Net.NetContext context)
        {
            var file = context.Url.LocalPath.Substring(10);
            string filename = UMC.Data.Utility.MapPath(String.Format("App_Data\\Temp\\{0}", file));

            if (System.IO.File.Exists(filename))
            {
                context.ContentType = "application/octet-stream";
                using (System.IO.FileStream stream = System.IO.File.OpenRead(filename))
                {
                    Utility.Copy(stream, context.OutputStream);
                }

            }
        }
        void Process(UMC.Net.NetContext context)
        {
            if (String.IsNullOrEmpty(context.UserAgent))
            {
                return;
            }
            context.AddHeader("Access-Control-Allow-Origin", "*");
            context.AddHeader("Access-Control-Allow-Credentials", "true");
            context.AddHeader("Access-Control-Max-Age", "18000");
            context.AddHeader("Access-Control-Allow-Methods", "*");
            context.AddHeader("Access-Control-Allow-Headers", "x-requested-with,content-type");

            var QueryString = new NameValueCollection(context.QueryString);
            QueryString.Add(context.Form);

            var model = QueryString["_model"];
            var cmd = QueryString["_cmd"];
            var jsonp = context.QueryString.Get("jsonp");
            if ("Upload".Equals(model) && "Command".Equals(cmd))
            {
                try
                {

                    var url = new Uri(QueryString["src"]);
                    var wb = new Net.HttpClient();
                    var hash = UMC.Data.JSON.Deserialize<Hashtable>(wb.GetAsync(url).Result.Content.ReadAsStringAsync().Result).GetEnumerator();

                    QueryString.Clear();
                    while (hash.MoveNext())
                    {
                        QueryString.Add(hash.Key.ToString(), hash.Value.ToString());
                    }
                    model = QueryString["_model"];
                    cmd = QueryString["_cmd"];
                    if (String.IsNullOrEmpty(jsonp) == false)
                    {
                        QueryString.Add("jsonp", jsonp);
                    }
                }
                catch
                {
                    return;
                }
            }
            context.ContentType = "text/javascript;charset=utf-8";

            var Url = context.Url;
            var ip = context.Headers.Get("X-Real-IP");
            if (String.IsNullOrEmpty(ip))
            {
                ip = context.UserHostAddress; ;
            }
            var cahash = context.Headers.Get("CA-Host");
            if (String.IsNullOrEmpty(cahash) == false)
            {
                Url = new Uri(String.Format("https://{1}{0}", context.Url.PathAndQuery, cahash));
            }


            Process(QueryString, Url, context, model, cmd, url =>
            {
                context.Redirect(url.AbsoluteUri);
            });
        }


        void Process(NameValueCollection nvs,
           Uri Url, Net.NetContext context, string model, string cmd, Action<Uri> redirec)
        {
            NameValueCollection QueryString = new NameValueCollection();

            var SessionKey = String.Empty;
            var paths = new System.Collections.Generic.List<string>(Url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
            if (paths.Count > 0)
                paths.RemoveAt(0);
            if (paths.Count > 0)
            {
                SessionKey = paths[0];
                paths.RemoveAt(0);
            }

            if (String.IsNullOrEmpty(model))
            {
                if (Url.Segments.Length > 4)
                {
                    if (paths.Count > 0)
                    {
                        if (paths[paths.Count - 1].IndexOf('.') > -1)
                        {
                            paths.RemoveAt(paths.Count - 1);
                        }
                    }

                    if (paths.Count > 1)
                    {
                        model = paths[0];
                        cmd = paths[1];
                    }
                    if (paths.Count > 2)
                    {
                        QueryString.Add(null, paths[2]);

                    }

                }
            }
            string start = nvs.Get("_start");
            for (var i = 0; i < nvs.Count; i++)
            {
                var key = nvs.GetKey(i);
                if (String.IsNullOrEmpty(key))
                {
                    if (String.IsNullOrEmpty(nvs.Get(i)) == false)
                    {
                        QueryString.Add(null, nvs.Get(i));
                    }
                }
                else if (!key.StartsWith("_"))
                {
                    QueryString.Add(key, nvs.Get(i));
                }
            }
            var jsonp = QueryString.Get("jsonp");
            QueryString.Remove("jsonp");
            WebSession session = null;
            var zip = context.OutputStream;
            var writer = new System.IO.StreamWriter(zip);

            if (String.IsNullOrEmpty(SessionKey) == false)
            {
                context.Cookies[Membership.SessionCookieName] = SessionKey;
            }
            try
            {
                switch (model)
                {
                    case "System":
                        switch (cmd)
                        {
                            case "BackUp":
                            case "Upgrade":
                            case "Click":
                            case "Access":
                            case "Restore":
                            case "Menu":
                                Authorization(context);
                                break;
                            default:
                                session = new SysSession();
                                break;
                        }
                        break;
                    default:
                        Authorization(context);
                        break;
                }

            }
            catch (SystemException)
            {
                session = new SysSession();
                model = "System";
                cmd = "Start";

            }
            var client = new WebClient(Url, context.UrlReferrer, context.UserHostAddress, context.UserAgent, session);
            client.InputStream = context.InputStream;
            if (String.IsNullOrEmpty(jsonp) == false && jsonp.StartsWith("app"))
            {
                client.IsApp = true;
            }

            if (String.IsNullOrEmpty(start) == false)
            {
                client.Start(start);
            }
            else if (String.IsNullOrEmpty(model))
            {

                client.SendDialog(QueryString);
            }
            else
            {
                if (String.IsNullOrEmpty(cmd))
                {
                    if (model.StartsWith("[") == false)
                    {
                        throw new Exception("Command is empty");
                    }
                }
                else
                {
                    client.Command(model, cmd, QueryString);
                }
            }


            if (String.IsNullOrEmpty(model) == false && model.StartsWith("[") && String.IsNullOrEmpty(cmd))
            {
                client.JSONP(model, jsonp, writer);
            }
            else
            {
                if (String.IsNullOrEmpty(jsonp) == false)
                {
                    writer.Write(jsonp);
                    writer.Write('(');
                }
                client.WriteTo(writer, redirec);
                if (String.IsNullOrEmpty(jsonp) == false)
                {
                    writer.Write(")");
                }
            }
            writer.Flush();
            zip.Close();
        }

        protected virtual void Authorization(UMC.Net.NetContext context)
        {


            var cookie = context.Cookies[Membership.SessionCookieName];// : CookieKey;
            var sessionKey = String.Empty;
            string contentType = "Client/" + context.UserHostAddress;
            if (UMC.Data.Utility.IsApp(context.UserAgent))
            {
                contentType = "App/" + context.UserHostAddress;
            }
            if (String.IsNullOrEmpty(cookie) == false)
            {
                sessionKey = cookie;
            }
            if (String.IsNullOrEmpty(sessionKey))
            {
                var uid = Guid.NewGuid();
                sessionKey = Utility.Guid(uid);
                context.AppendCookie(Membership.SessionCookieName, sessionKey);
                var user = new UMC.Security.Guest(uid);

                UMC.Security.Principal.Create(user, UMC.Security.AccessToken.Create(user, uid, contentType, 0));
            }
            else
            {
                UMC.Security.Membership.Instance().Authorization(sessionKey, contentType);

            }
            var urf = context.UrlReferrer;
            if (urf != null)
            {
                if (String.IsNullOrEmpty(urf.Query) == false)
                {
                    var query = System.Web.HttpUtility.ParseQueryString(urf.Query.Substring(1));
                    var sp = UMC.Data.Utility.Guid(query["sp"]);
                    if (sp.HasValue)
                    {
                        if (String.Equals(UMC.Security.AccessToken.Get("Spread-Id"), sp.ToString()) == false)
                        {
                            UMC.Security.AccessToken.Set("Spread-Id", sp.ToString());
                        }
                    }
                }
            }


        }
        public static List<WebMeta> Auths()
        {
            List<WebMeta> metas = new List<WebMeta>();
            if (WebRuntime.flows.Count > 0)
            {
                var em = WebRuntime.flows.GetEnumerator();
                while (em.MoveNext())
                {

                    MappingAttribute mapping = (MappingAttribute)em.Current.Value[0].GetCustomAttributes(typeof(MappingAttribute), false)[0];



                    WebAuthType authType = WebRuntime.authKeys[em.Current.Key];
                    if (authType == WebAuthType.Check || authType == WebAuthType.UserCheck)
                    {
                        metas.Add(new WebMeta().Put("key", em.Current.Key + ".*").Put("desc", mapping.Desc));


                    }


                }
            }

            if (WebRuntime.activities.Count > 0)
            {
                var em = WebRuntime.activities.GetEnumerator();
                while (em.MoveNext())
                {
                    var em3 = em.Current.Value.GetEnumerator();
                    while (em3.MoveNext())
                    {
                        MappingAttribute mapping = (MappingAttribute)em3.Current.Value.GetCustomAttributes(typeof(MappingAttribute), false)[0];
                        if (WebRuntime.authKeys.ContainsKey(em.Current.Key))
                        {
                            WebAuthType authType = WebRuntime.authKeys[em.Current.Key];
                            if (authType == WebAuthType.Check || authType == WebAuthType.UserCheck)
                            {
                                metas.Add(new WebMeta().Put("key", mapping.Model + "." + mapping.Command).Put("desc", mapping.Desc));


                            }
                        }
                    }



                }
            }
            return metas;

        }

        class RKey
        {
            public string Name
            {
                get; set;
            }
            public string ResourceKey
            { get; set; }
            public string ResourceJS
            { get; set; }
            public Type Type { get; set; }
        }


        static Dictionary<String, RKey> ResourceKeys = new Dictionary<string, RKey>();
        static RKey GetResourceKey(string name)
        {
            if (ResourceKeys.ContainsKey(name) == false)
            {
                var Initializers = new List<Data.Sql.Initializer>(Data.Sql.Initializer.Initializers());
                foreach (var init in Data.Sql.Initializer.Initializers())
                {
                    ResourceKeys[init.Name] = new RKey
                    {
                        Name = init.Name,
                        ResourceKey = init.ResourceKey,
                        Type = init.GetType(),
                        ResourceJS = init.ResourceJS
                    };

                }
            }
            if (ResourceKeys.ContainsKey(name))
            {
                return ResourceKeys[name];
            }
            return null;
        }
        #region IMIMEHandler Members

        public virtual void ProcessRequest(UMC.Net.NetContext context)
        {
            var path = context.Url.LocalPath;
            var paths = new List<string>(path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
            String fileName = "";
            if (paths.Count > 0)
            {
                if (paths[paths.Count - 1].IndexOf('.') > -1)
                {
                    fileName = paths[paths.Count - 1];
                    paths.RemoveAt(paths.Count - 1);
                }

            }
            if (paths.Count > 0)
            {
                switch (paths[0])
                {
                    case "Admin":
                        IndexResource(context);
                        //Resource(context, "index");
                        return;
                    case "Click":
                    case "Page":
                    case "Sub":
                        Resource(context, paths[0].ToLower());
                        return;
                    case "Download":
                        Download(context);
                        return;

                }
                if (String.IsNullOrEmpty(fileName))
                {
                    switch (paths.Count)
                    {
                        case 1:
                            if (path.EndsWith("/"))
                            {
                                Process(context);
                            }
                            else
                            {
                                RKey key = GetResourceKey(paths[0]);
                                if (key != null)
                                {
                                    if (String.IsNullOrEmpty(key.ResourceKey) == false)
                                    {
                                        context.ContentType = "text/html";


                                        using (System.IO.Stream stream = key.Type.Assembly
                                                           .GetManifestResourceStream(key.ResourceKey))
                                        {
                                            if (stream != null)
                                            {
                                                //context.Output.WriteLine(";");
                                                //context.Output.Write("/****{0}***/", key.Name);
                                                //context.Output.WriteLine();
                                                ////ResourceKeys.m
                                                UMC.Data.Utility.Copy(stream, context.OutputStream);
                                                //}
                                                //else
                                                //{
                                                //    Resource(context, "index");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        IndexResource(context);
                                        //Resource(context, "index");

                                    }
                                }
                                else
                                {
                                    IndexResource(context);
                                    // Resource(context, "index");
                                }
                            }
                            break;
                        default:
                            Process(context);
                            break;
                    }
                }
                else
                {
                    var names = fileName.Split('.');
                    var configs = UMC.Data.Reflection.Configuration(names[0]);
                    if (configs != null)
                    {

                        UMC.Net.INetHandler handler = UMC.Data.Reflection.CreateObject(configs, names[1])
                                   as
                        UMC.Net.INetHandler;
                        if (handler != null)
                        {
                            handler.ProcessRequest(context);
                        }
                        else
                        {
                            Process(context);
                        }
                    }
                    else
                    {
                        Process(context);
                    }

                }
            }
            else
            {
                if (String.IsNullOrEmpty(fileName))
                {
                    IndexResource(context);
                    //Resource(context, "index");
                }
                else
                {
                    var names = fileName.Split('.');
                    RKey key = GetResourceKey(names[0]);
                    if (key != null)
                    {
                        if (fileName.EndsWith(".js"))
                        {
                            context.ContentType = "text/javascript";

                            if (String.Equals(key.Name, "UMC") == false)
                            {
                                using (System.IO.Stream stream = key.Type.Assembly
                                                     .GetManifestResourceStream(key.ResourceJS))
                                {
                                    if (stream != null)
                                    {
                                        context.Output.WriteLine(";");
                                        context.Output.Write("/****{0}***/", key.Name);
                                        context.Output.WriteLine();
                                        context.Output.WriteLine();
                                        UMC.Data.Utility.Copy(stream, context.OutputStream);
                                    }
                                }
                                return;

                            }

                            var me = ResourceKeys.GetEnumerator();
                            while (me.MoveNext())
                            {
                                key = me.Current.Value;
                                if (String.IsNullOrEmpty(key.ResourceJS) == false)
                                {

                                    using (System.IO.Stream stream = key.Type.Assembly
                                                   .GetManifestResourceStream(key.ResourceJS))
                                    {
                                        if (stream != null)
                                        {
                                            context.Output.WriteLine(";");
                                            context.Output.Write("/****{0}***/", key.Name);
                                            context.Output.WriteLine();
                                            context.Output.WriteLine();
                                            context.Output.WriteLine(new System.IO.StreamReader(stream).ReadToEnd());
                                            //UMC.Data.Utility.Copy(stream, context.OutputStream);
                                        }
                                    }
                                }
                            }



                        }
                        else
                        {
                            var rkeys = key.ResourceKey.Split('.');
                            rkeys[rkeys.Length - 2] = names[1];
                            context.ContentType = "text/html";
                            using (System.IO.Stream stream = key.Type.Assembly
                                                                        .GetManifestResourceStream(String.Join(".", rkeys)))
                            {
                                if (stream != null)
                                {
                                    UMC.Data.Utility.Copy(stream, context.OutputStream);
                                }
                                else
                                {
                                    //Resource(context, "index");
                                    IndexResource(context);
                                }
                            }
                        }
                    }
                    else
                    {
                        IndexResource(context);
                        //Resource(context, "index");
                    }
                }
            }

        }
        void Resource(Net.NetContext context, string key)
        {
            context.ContentType = "text/html";
            using (System.IO.Stream stream = typeof(WebServlet).Assembly
                               .GetManifestResourceStream(String.Format("UMC.Data.Resources.{0}.html", key)))
            {
                UMC.Data.Utility.Copy(stream, context.OutputStream);

            }
        }
        void IndexResource(Net.NetContext context)
        {
            context.ContentType = "text/html";
            using (System.IO.Stream stream = typeof(WebServlet).Assembly
                               .GetManifestResourceStream(String.Format("UMC.Data.Resources.header.html")))
            {
                context.Output.WriteLine(new System.IO.StreamReader(stream).ReadToEnd().Trim());

            }
            context.Output.Write("    <script>WDK.POS.Config({posurl: ['/");
            var ls = context.RawUrl.Split(new String[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (ls.Length == 0)
            {
                context.Output.Write("UMC");
            }
            else
            {

                context.Output.Write(ls[0]);
            }
            context.Output.WriteLine("/', WDK.cookie('device') || WDK.cookie('device', WDK.uuid())].join('')});</script>");
            context.Output.WriteLine();
            using (System.IO.Stream stream = typeof(WebServlet).Assembly
                               .GetManifestResourceStream(String.Format("UMC.Data.Resources.index.html")))
            {
                context.Output.WriteLine(new System.IO.StreamReader(stream).ReadToEnd());

            }
        }

        #endregion
        internal static List<WebMeta> Mapping()
        {

            List<WebMeta> metas = new List<WebMeta>();
            if (WebRuntime.webFactorys.Count > 0)
            {
                foreach (Type t in WebRuntime.webFactorys)
                {
                    WebMeta meta = new WebMeta();
                    meta.Put("type", t.FullName);
                    meta.Put("name", "." + t.Name);
                    metas.Add(meta);

                    MappingAttribute mapping = (MappingAttribute)t.GetCustomAttributes(typeof(MappingAttribute), false)[0];
                    if (String.IsNullOrEmpty(mapping.Desc) == false)
                    {
                        meta.Put("desc", mapping.Desc);

                    }

                }

            }
            if (WebRuntime.flows.Count > 0)
            {
                var em = WebRuntime.flows.GetEnumerator();
                while (em.MoveNext())
                {
                    var tls = em.Current.Value;
                    foreach (Type t in tls)
                    {
                        WebMeta meta = new WebMeta();
                        meta.Put("type", t.FullName);
                        meta.Put("name", em.Current.Key + ".");
                        meta.Put("auth", WebRuntime.authKeys[em.Current.Key].ToString().ToLower());
                        meta.Put("model", em.Current.Key);//.getKey())
                        metas.Add(meta);

                        var mappings = t.GetCustomAttributes(typeof(MappingAttribute), false);//[0];

                        MappingAttribute mapping = (MappingAttribute)mappings[0];
                        if (mappings.Length > 1)
                        {
                            foreach (var m in mappings)
                            {
                                var c = m as MappingAttribute;
                                if (String.Equals(c.Model, em.Current.Key))
                                {
                                    mapping = c;
                                    break;
                                }
                            }
                        }
                        if (String.IsNullOrEmpty(mapping.Desc) == false)
                        {
                            meta.Put("desc", mapping.Desc);

                        }

                    }


                }
            }
            if (WebRuntime.activities.Count > 0)
            {
                var em = WebRuntime.activities.GetEnumerator();
                while (em.MoveNext())
                {
                    var em3 = em.Current.Value.GetEnumerator();
                    while (em3.MoveNext())
                    {
                        var mappings = em3.Current.Value.GetCustomAttributes(typeof(MappingAttribute), false);
                        MappingAttribute mapping = (MappingAttribute)mappings[0];
                        if (mappings.Length > 1)
                        {
                            foreach (var m in mappings)
                            {
                                var c = m as MappingAttribute;
                                if (String.Equals(c.Model, em.Current.Key) && String.Equals(c.Command, em3.Current.Key))
                                {
                                    mapping = c;
                                    break;
                                }
                            }
                        }

                        WebAuthType authType = mapping.Auth;// WebRuntime.authKeys[em.Current.Key];

                        WebMeta meta = new WebMeta();
                        meta.Put("type", em3.Current.Value.FullName);
                        meta.Put("name", em.Current.Key + "." + em3.Current.Key);
                        meta.Put("auth", authType.ToString().ToLower());
                        meta.Put("model", mapping.Model);//.getKey())
                        meta.Put("cmd", mapping.Command);//.getKey())
                        metas.Add(meta);

                        if (String.IsNullOrEmpty(mapping.Desc) == false)
                        {
                            meta.Put("desc", mapping.Desc);

                        }


                    }



                }
            }
            return metas;
        }

    }
}