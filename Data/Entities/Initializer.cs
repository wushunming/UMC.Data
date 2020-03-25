using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UMC.Data.Sql;

namespace UMC.Data.Entities
{
    public class Initializer : UMC.Data.Sql.Initializer
    {
        public override string ProviderName => "defaultDbProvider";

        public override string Caption => "UMC基础";

        public override string Name => "UMC";

        public override string ResourceKey => "";
        public override string ResourceJS => "index.js";

        public Initializer()
        {
            this.Setup(new Account() { user_id = Guid.Empty, Type = 0 }, new Account { ConfigData = String.Empty });
            this.Setup(new Cache() { Id = Guid.Empty, CacheKey = String.Empty }, new Cache { CacheData = new System.Collections.Hashtable() });
            this.Setup(new Session { SessionKey = String.Empty }, new Session { Content = String.Empty });
            this.Setup(new Log(), new Log() { Context = String.Empty });
            this.Setup(new Number() { CodeKey = String.Empty });
            this.Setup(new User() { Username = String.Empty }, "Password");
            this.Setup(new Wildcard() { WildcardKey = String.Empty }, new Wildcard { Authorizes = String.Empty });
            this.Setup(new Picture() { group_id = Guid.Empty, Seq = 1 });
            this.Setup(new UserToRole() { role_id = Guid.Empty, user_id = Guid.Empty });
            this.Setup(new Role() { Id = Guid.Empty });
            this.Setup(new Link() { Id = Guid.Empty });
            this.Setup(new Click() { Code = 0 });
            this.Setup(new Menu() { Id = Guid.Empty });
            this.Setup(new LinkAccess() { link_id = Guid.Empty, Username = String.Empty });

            this.Setup(new Location() { Id = 0 });

            this.Setup(new Project() { Id = Guid.Empty });
            this.Setup(new ProjectItem() { Id = Guid.Empty });
            this.Setup(new ProjectDynamic() { Time = DateTime.MinValue, user_id = Guid.Empty });
            this.Setup(new ProjectMember() { project_id = Guid.Empty, user_id = Guid.Empty });
            this.Setup(new Portfolio() { Id = Guid.Empty });

            this.Setup(new Attention { Id = Guid.Empty, user_id = Guid.Empty });
            this.Setup(new Proposal { ref_id = Guid.Empty, user_id = Guid.Empty }, new Proposal { Content = String.Empty });
            this.Setup(new Subject { Id = Guid.Empty }, new Subject { ConfigXml = String.Empty, DataJSON = String.Empty, Content = String.Empty });
            this.Setup(new Comment { Id = Guid.Empty }, new Comment { Content = String.Empty });


        }
        protected override void Setup(IDictionary hash, Sql.DbFactory factory)
        {
            if (factory.ObjectEntity<Role>().Count() == 0)
            {
                var adminRole = new Role()
                {
                    Id = Guid.NewGuid(),
                    Rolename = UMC.Security.Membership.AdminRole,
                    Explain = "管理员"
                };
                factory.ObjectEntity<Role>().Insert(adminRole, new Role
                {
                    Id = Guid.NewGuid(),
                    Rolename = UMC.Security.Membership.UserRole,
                    Explain = "员工账户"
                }, new Role
                {
                    Id = Guid.NewGuid(),
                    Rolename = UMC.Security.Membership.GuestRole,
                    Explain = "来客"
                });

                var userEntiy = factory.ObjectEntity<User>();

                var sn = Guid.NewGuid();

                userEntiy.Insert(new User
                {
                    Alias = "管理员",
                    Flags = UMC.Security.UserFlags.Normal,
                    Id = sn,
                    RegistrTime = DateTime.Now,
                    IsMember = false,
                    Username = "admin"
                });
                userEntiy.Update(new Web.WebMeta().Put("Password", Convert.ToBase64String(UMC.Data.Utility.DES("admin", sn))).GetDictionary());


                factory.ObjectEntity<UserToRole>().Insert(new UserToRole { role_id = adminRole.Id, user_id = sn });


            }
        }

        public override void Menu(IDictionary hash, DbFactory factory)
        {
            factory.ObjectEntity<Data.Entities.Menu>()
                   .Insert(new Data.Entities.Menu()
                   {
                       Icon = "\uf188",
                       Caption = "开发调试",
                       IsDisable = false,
                       ParentId = Guid.Empty,
                       Seq = 90,
                       Id = Guid.NewGuid(),
                       Url = "#debug"
                       //}, new Data.Entities.Menu()
                       //{
                       //    Icon = "\uf0c1",
                       //    Caption = "连接管理",
                       //    IsDisable = false,
                       //    ParentId = Guid.Empty,
                       //    Seq = 91,
                       //    Id = Guid.NewGuid(),
                       //    Url = "#link/setting"
                       //}, new Data.Entities.Menu()
                       //{
                       //    Icon = "\uf02d",
                       //    Caption = "内容管理",
                       //    IsDisable = false,
                       //    ParentId = Guid.Empty,
                       //    Seq = 92,
                       //    Id = Guid.NewGuid(),
                       //    Url = "#subject/items"
                   }, new Data.Entities.Menu()
                   {
                       Icon = "\uf0ae",
                       Caption = "菜单管理",
                       IsDisable = false,
                       ParentId = Guid.Empty,
                       Seq = 93,
                       Id = Guid.NewGuid(),
                       Url = "#menu"
                   }, new Data.Entities.Menu()
                   {
                       Icon = "\uf0c0",
                       Caption = "用户管理",
                       IsDisable = false,
                       ParentId = Guid.Empty,
                       Seq = 94,
                       Id = Guid.NewGuid(),
                       Url = "#user"
                   });
        }
    }
}
