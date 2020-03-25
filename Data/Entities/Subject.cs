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
        /// <summary>
        /// 类别
        /// </summary>
        public Guid? category_id
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
        /// 主题项
        /// </summary>
        public string Items
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
        /// <summary>
        /// 提交审核的时间
        /// </summary>
        public DateTime? SubmitTime { get; set; }

        public int? Score
        {
            get; set;
        }
        /// <summary>
        /// 审核人
        /// </summary>
        public Guid? AppId
        {
            get;
            set;
        }
        /// <summary>
        /// 审核意见
        /// </summary>
        public string AppDesc
        {
            get; set;
        }
        /// <summary>
        /// 最后修改人ID
        /// </summary>
        public Guid? last_user_id
        {
            get; set;
        }
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
        public DateTime? Time { get; set; }
        public DynamicType? Type { get; set; }
        public Guid? refer_id { get; set; }
        public string Title { get; set; }
        public string Explain { get; set; }

    }
}
