using System;
using System.Collections.Generic;
using System.Text;
using UMC.Data.Sql;
using UMC.Data.Entities;

namespace UMC.Configuration
{
    /// <summary>
    /// 会话对象
    /// </summary>
    /// <typeparam name="T">会员类型</typeparam>
    public class Session<T>
    {
        /// <summary>
        /// 会话值
        /// </summary>
        public T Value
        {
            get;
            private set;
        }
        /// <summary>
        /// 会话Key
        /// </summary>
        public string Key
        {
            get;
            private set;
        }
        public string ContentType
        {
            get;
            set;
        }
        private Guid _user_id;
        public Session(string sessionKey)
        {
            this.Key = sessionKey;

            var se = UMC.Data.Database.Instance().ObjectEntity<Session>()
                .Where.And(new Session { SessionKey = sessionKey })
                .Entities.Single();
            if (se != null)
            {
                this.ContentType = se.ContentType;

                if (typeof(T) == typeof(string))
                {
                    object obj = se.Content;
                    this.Value = (T)obj;
                }
                else
                {
                    this.Value = UMC.Data.JSON.Deserialize<T>(se.Content);
                }
                _user_id = se.user_id ?? Guid.Empty;
            }
        }

        public DateTime ModifiedTime
        {
            get;
            private set;
        }
        public Session(T value, string sessionKey)
        {
            this.Value = value;
            this.Key = sessionKey;
        }
        /// <summary>
        /// 提交更改
        /// </summary>
        public void Commit()
        {
            this.Commit(this._user_id);
        }
        /// <summary>
        /// 提交更改
        /// </summary>
        public void Commit(UMC.Security.Identity id)
        {
            this.Commit(id.Id ?? Guid.Empty);
        }
        /// <summary>
        /// 提交更改,且消除用户contentType类型的Sesion
        /// </summary>
        public void Commit(UMC.Security.Identity id, string contentType)
        {
            this.ContentType = contentType;
            this.Commit(Guid.Empty, id.Id ?? Guid.Empty);
        }
        public void Commit(T value, UMC.Security.Identity id)
        {
            this.Value = value;
            this.Commit(id, "app/json");
        }

        public void Commit(T value, params Guid[] ids)
        {
            this.Value = value;
            this.Commit(ids);
        }
        /// <summary>
        /// 提交更改
        /// </summary>
        public void Commit(params Guid[] ids)
        {
            var session = new Session
            {
                UpdateTime = DateTime.Now,
                user_id = ids[ids.Length - 1],
                ContentType = this.ContentType ?? "text/javascript",
                SessionKey = this.Key
            };
            if (this.Value is string)
            {
                session.Content = this.Value as string;
            }
            else
            {
                session.Content = UMC.Data.JSON.Serialize(this.Value, "ts");
            }
            this.ModifiedTime = DateTime.Now;
            if (ids.Length > 1)
            {
                UMC.Data.Database.Instance().ObjectEntity<Session>()
                .Where.And().Equal(new Session
                {
                    SessionKey = this.Key
                }).Or().Contains().And().Equal(new Session
                {
                    ContentType = ContentType,
                    user_id = session.user_id
                }).Entities
                .IFF(e => e.Delete() >= 0, e => e.Insert(session));
            }
            else
            {
                UMC.Data.Database.Instance().ObjectEntity<Session>()
                .Where.And().Equal(new Session
                {
                    SessionKey = this.Key
                }).Entities
                .IFF(e => e.Update(session) == 0, e => e.Insert(session));
            }

        }
    }

}
