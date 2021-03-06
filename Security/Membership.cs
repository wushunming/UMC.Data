using System;
using System.Collections.Generic;
using System.Text;


namespace UMC.Security
{

    /// <summary>
    /// 用户安全标签
    /// </summary>
    public enum UserFlags
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 锁定
        /// </summary>
        Lock = 1,
        /// <summary>
        /// 要更新密码
        /// </summary>
        Changing = 2,
        /// <summary>
        /// 不能更新密码
        /// </summary>
        UnChangePasswork = 4,
        /// <summary>
        /// 没有通过验证
        /// </summary>
        UnVerification = 8,
        /// <summary>
        /// 禁用
        /// </summary>
        Disabled = 16

    }
    /// <summary> 
    /// 用户管理类
    /// </summary>
    public abstract class Membership : UMC.Configuration.DataProvider
    {
        public const string SessionCookieName = ".WDKTemporary";

        /// <summary>
        /// 管理员角色
        /// </summary>
        public const String AdminRole = "Administrators";
        /// <summary>
        /// 一般用户角色
        /// </summary>
        public const String UserRole = "Users";
        /// <summary>
        /// 来宾角色
        /// </summary>
        public const String GuestRole = "Guest";
        /// <summary>
        /// 实例
        /// </summary>
        /// <returns></returns>
        public static Membership Instance()
        {
            return UMC.Data.Reflection.CreateObject("Membership") as Membership ?? new UMC.Configuration.Membership();
        }
        /// <summary>
        /// 获取用户身份
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public abstract List<Identity> Identity(params string[] names);

        /// <summary>
        /// 获取用户身份
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public abstract Identity Identity(string username);
        /// <summary> 
        /// 检验用户密码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="maxtimes">失败最大次数</param>
        /// <returns>失败次数</returns>
        public abstract int Password(string username, string password, int maxtimes);

        /// <summary> 
        /// 获取用户的密码和角色
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>返回密码，如果用null，则表示无此用户</returns>
        public abstract string Password(string username);
        /// <summary>
        /// 获取用户身价
        /// </summary>
        /// <param name="SessionKey">用户标示</param>
        /// <param name="contentType">用户终端</param>
        /// <returns></returns>
        public virtual UMC.Security.Principal Authorization(string SessionKey, String contentType)
        {
            var sessionKey = Data.Utility.Guid(SessionKey, true).Value;
            if (sessionKey != Guid.Empty)
            {
                var session = new Configuration.Session<Security.AccessToken>(sessionKey.ToString());
                if (session.Value != null)
                {
                    var auth = session.Value;
                    auth.Id = sessionKey;
                    auth.ContentType = session.ContentType;

                    var passDate = Data.Utility.TimeSpan();
                    if (auth.Timeout == 0 || (auth.ActiveTime + auth.Timeout > passDate))
                    {
                        if (auth.ActiveTime < passDate - 600)
                        {
                            Data.Reflection.SetProperty(auth, "ActiveTime", passDate);

                            this.Activation(auth);
                        }

                        return UMC.Security.Principal.Create(auth.Identity(), auth);
                    }

                }
                var user = UMC.Security.Identity.Create(sessionKey, "?", String.Empty);
                return UMC.Security.Principal.Create(user, Security.AccessToken.Create(user, sessionKey, contentType, 0));

            }
            var id = UMC.Security.Identity.Create("?", String.Empty);
            return UMC.Security.Principal.Create(id, Security.AccessToken.Create(id, Guid.Empty, contentType, 0));

        }
        /// <summary>
        /// 通过ID获取用户身份
        /// </summary>
        /// <returns></returns>
        public abstract UMC.Security.Identity Identity(Guid id);
        /// <summary>
        /// 通过绑定的第三方账户获取用户身份
        /// </summary>
        /// <param name="name"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public abstract UMC.Security.Identity Identity(string name, int accountType);

        /// <summary>
        /// 变换密码
        /// </summary>
        /// <param name="username">用户密码</param>
        /// <param name="password"></param>
        public abstract bool Password(string username, string password);
        /// <summary>
        /// 增加用户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public abstract Identity CreateUser(Guid id, string username, string password, string alias);
        /// <summary>
        /// 增加用户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public abstract Guid CreateUser(string username, string password, string alias);
        /// <summary>
        /// 变更别名
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="alias">别名</param>
        /// <returns></returns>
        public abstract bool ChangeAlias(string username, string alias);
        /// <summary>
        /// 是否存在这个用户名
        /// </summary>
        /// <returns></returns>
        public abstract bool Exists(string username);
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public abstract bool DeleteUser(string username);
        /// <summary>
        /// 更改状态标志
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="flags">用户状态标示</param>
        /// <returns></returns>
        public abstract bool ChangeFlags(string username, UserFlags flags);

        /// <summary>
        /// 移除用户角色
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="role">角色</param>
        /// <returns></returns> 
        public abstract bool RemoveRole(string username, string role);
        /// <summary>
        /// 增加用户角色
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="role">用户名</param>
        /// <returns></returns> 
        public abstract bool AddRole(string username, params string[] roles);

        public virtual void Activation(Security.AccessToken token)
        {
            int cuttime = UMC.Data.Utility.TimeSpan();
            if (token.Timeout > 0 && ((token.ActiveTime ?? 0) + token.Timeout) < cuttime)
            {
                var at = AccessToken.Create(UMC.Security.Identity.Create(token.Id.Value, "?", "Guest"), token.Id.Value, token.ContentType, 0);
                var sesion = new Configuration.Session<UMC.Security.AccessToken>(at, token.Id.ToString());
                sesion.ContentType = at.ContentType;
                sesion.Commit(token.Id.Value);
                return;

            }


            switch (token.Username ?? "?")
            {
                case "#":
                case "?":
                    break;
                default:
                    if (String.IsNullOrEmpty(token.Username) == false)
                    {
                        UMC.Data.Database.Instance().ObjectEntity<UMC.Data.Entities.User>()
                            .Where.And(new UMC.Data.Entities.User { Username = token.Username }).Entities
                            .Update(new UMC.Data.Entities.User { ActiveTime = DateTime.Now, SessionKey = token.Id.Value });

                    }
                    break;
            }
            if (token.UId.HasValue)
            {
                var sesion = new Configuration.Session<UMC.Security.AccessToken>(token, token.Id.ToString());
                sesion.ContentType = token.ContentType;
                sesion.Commit(token.UId.Value);
            }
        }


    }

}
