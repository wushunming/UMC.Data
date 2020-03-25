using System;
using System.Collections.Generic;
using System.Text;

namespace UMC.Data.Entities
{

    public class Click
    {
        public int? Code
        {
            get;
            set;
        }

        public string Query
        {
            get;
            set;
        }
        public Guid? user_id
        {
            get;
            set;

        }
        public int? Quality
        {
            get;
            set;

        }
        public string Description
        {
            get;
            set;

        }
    }
    public class Menu
    {
        public Guid? Id
        {
            get; set;
        }
        public String Icon
        {
            get; set;
        }
        public String Caption
        {
            get; set;
        }
        public string Url
        {
            get; set;
        }
        public int? Seq
        {
            get; set;
        }
        public Guid? ParentId { get; set; }

        public bool? IsDisable { get; set; }

    }

    /// <summary>
    /// 连接
    /// </summary>
    public class Link
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid? Id
        {
            get; set;
        }
        /// <summary>
        /// 赞数
        /// </summary>
        public int? Favs
        {
            get;
            set;
        }
        public string Url
        {
            get; set;
        }
        public string Caption
        {
            get; set;
        }
        public int? Times
        {
            get; set;
        }
        public bool? IsMenu
        {
            get;
            set;
        }

        public string GroupBy
        {
            get; set;
        }
        public DateTime? CreationTime
        {
            get; set;
        }

        public DateTime? ModifiedTime
        {
            get; set;
        }


    }
    /// <summary>
    /// 人员访问日志
    /// </summary>
    public class LinkAccess
    {
        public Guid? link_id
        {
            get; set;
        }
        public string Username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastAccessTime
        {
            get; set;
        }
        public int? Times
        {
            get; set;
        }

    }



}
