using System;
using System.Collections.Generic;
using System.Text;

namespace UMC.Data.Entities
{

    /// <summary>
    /// 主题
    /// </summary>
    public class Subject
    {
        public string Code
        {
            get; set;
        }
        public Guid? Id
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }
        /// <summary>
        /// 查看数量
        /// </summary>
        public int? Look
        {
            get;
            set;
        }
        /// <summary>
        /// 回复数据
        /// </summary>
        public int? Reply
        {
            get;
            set;
        }
        public int? Status
        {
            get;
            set;
        }
        public int? Visible
        {
            get;
            set;
        }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastDate
        {
            get;
            set;
        }
        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime? ReleaseDate
        {
            get;
            set;
        }
        /// <summary>
        /// 举报次数
        /// </summary>
        public int? TipOffs
        {
            get; set;
        }
        /// <summary>
        /// 发布日期
        /// </summary>
        public int? PublishTime
        {
            get;
            set;
        }
        /// <summary>
        /// 来源URL
        /// </summary>
        public string Url
        {
            get;
            set;
        }
        public string DataJSON
        {
            get;
            set;
        }
        /// <summary>
        /// 赞数
        /// </summary>
        public int? Favs
        {
            get;
            set;
        }
        /// <summary>
        /// 所属文集
        /// </summary>
        public Guid? portfolio_id
        {
            get; set;
        }
        /// <summary>
        /// 所属项目 
        /// </summary>
        public Guid? project_item_id { get; set; }
        /// <summary>
        /// 所属团队 
        /// </summary>
        public Guid? project_id { get; set; }

        public string ContentType
        {
            get;
            set;
        }

        public string ConfigXml
        {
            get;
            set;
        }
        public Guid? user_id
        {
            get;
            set;
        }
        public string Poster
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }
        public bool? IsPicture
        {
            get;
            set;
        }
        public int? Seq
        {
            get;
            set;
        }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Description
        {
            get;
            set;
        }
        /// <summary>
        /// 是否可以评论
        /// </summary>
        public bool? IsComment
        {
            get; set;
        }
        /// <summary>
        /// 复制的源主题
        /// </summary>
        public Guid? soure_id { get; set; }

        public int? Score
        {
            get; set;
        }
        public DateTime? CreationTime { get; set; }
        /// <summary>
        /// 最后修改人ID
        /// </summary>
        public Guid? last_user_id
        {
            get; set;
        }
        /// <summary>
        /// 草稿
        /// </summary>
        public bool? IsDraught
        {
            get; set;
        }

    }
    /// <summary>
    /// 举报
    /// </summary>
    public class SubjectTipOff
    {
        public Guid? sub_id
        {
            get;
            set;
        }

        public DateTime? CreationTime { get; set; }

        public Guid? user_id { get; set; }
        /// <summary>
        /// 举报内容
        /// </summary>
        public string Description
        {
            get;
            set;
        }
        /// <summary>
        /// 举报类型
        /// </summary>
        public string Type
        {
            get;
            set;
        }
        public string Url
        {
            get; set;
        }
    }
    /// <summary>
    /// 黑名单
    /// </summary>
    public class ProjectBlock
    {
        public Guid? ref_id { get; set; }
        public int? Type { get; set; }
        public Guid? user_id { get; set; }
    }

    public class ProjectItem
    {

        public Guid? Id
        {
            get;
            set;
        }
        public string Caption
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public Guid? user_id
        {
            get;
            set;
        }
        public int? Sequence { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid? project_id { get; set; }
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? Hide { get; set; }
        public int? PublishTime
        {
            get;
            set;
        }
    }
    public class Project
    {

        public Guid? Id
        {
            get;
            set;
        }
        public string Caption
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public string Code
        {
            get;
            set;
        }
        public Guid? user_id
        {
            get;
            set;
        }
        public int? Sequence { get; set; }
        public DateTime? CreationTime { get; set; }
        public int? PublishTime
        {
            get;
            set;
        }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifiedTime
        {
            get; set;
        }
    }

    public class ProjectMember
    {
        public Guid? user_id
        {
            get;
            set;
        }
        public Guid? project_id { get; set; }
        public DateTime? CreationTime { get; set; }
        public Web.WebAuthType? AuthType { get; set; }
        public string Alias { get; set; }
        public int? AccountType { get; set; }


    }
    /// <summary>
    /// 用户设置
    /// </summary>
    public class ProjectUserSetting
    {
        public Guid? Id { get; set; }
        public int? Type { get; set; }
        public Guid? user_id { get; set; }

        public string CorpId { get; set; }
        public string AppId { get; set; }
        public string AgentId { get; set; }
        public string AppSecret { get; set; }
        public string AccessToken { get; set; }
        public int? ExpiresTime { get; set; }

        public string APITicket { get; set; }
        public int? APIExpiresTime { get; set; }

    }
    /// <summary>
    /// 项目设置
    /// </summary>
    public class ProjectSetting
    {
        public Guid? project_id { get; set; }
        public Guid? user_setting_id { get; set; }
        public int? Type { get; set; }
    }
    public enum DynamicType
    {
        Subject = 0,
        Member = 1,
        Project = 2,
        ProjectItem = 3,
        Portfolio = 4
    }
    public class ProjectDynamic
    {
        public Guid? user_id
        {
            get;
            set;
        }
        public Guid? project_id { get; set; }
        public int? Time { get; set; }
        public DynamicType? Type { get; set; }
        public Guid? refer_id { get; set; }
        public string Title { get; set; }
        public string Explain { get; set; }

    }
    /// <summary>
    /// 搜索关键字
    /// </summary>
    public class SearchKeyword
    {
        public string Keyword
        {
            get; set;
        }
        public Guid? user_id
        {
            get; set;
        }
        public int? Time
        {
            get; set;
        }
    }
}
