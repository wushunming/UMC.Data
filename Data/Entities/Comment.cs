using System;

namespace UMC.Data.Entities
{
    /// <summary>
    /// 关注类型
    /// </summary>
    public enum AttentionType
    {
        /// <summary>
        /// 收听的
        /// </summary>
        Follow = 2,
        /// <summary>
        /// 好友
        /// </summary>
        Friend = 1,
        /// <summary>
        /// 黑名单
        /// </summary>
        Black = -1,
        /// <summary>
        /// 听众
        /// </summary>
        Atten = 0,
    }
    /// <summary>
    /// 回复类型
    /// </summary>
    public enum ReplyType
    {
        /// <summary>
        /// 回复
        /// </summary>
        Reply = 0,
        /// <summary>
        ///请求
        /// </summary>
        Request = 2,
        /// <summary>
        /// 私信
        /// </summary>
        Mail = 1,
        /// <summary>
        /// 转发
        /// </summary>
        Farwork = 3,
        /// <summary>
        /// 博客
        /// </summary>
        Blog = 4
    }

    /// <summary>
    /// 评论
    /// </summary>
    public class Comment
    {
        public Guid? Id
        {
            get;
            set;
        }
        public Guid? ref_id
        {
            get;
            set;
        }
        public Guid? for_id
        {
            get;
            set;
        }
        public Guid? user_id
        {
            get;
            set;
        }
        public int? Score
        {
            get;
            set;
        }
        public int? Effective
        {
            get;
            set;
        }
        public int? Invalid
        {
            get;
            set;
        }
        public string Content
        {
            get;
            set;
        }
        /// <summary>
        ///评论时间
        /// </summary>
        public DateTime? CommentDate
        {
            get;
            set;
        }
        public int? Unhealthy
        {
            get;
            set;
        }
        public int? Reply
        {
            get;
            set;
        }
        public int? Farworks
        {
            get;
            set;
        }

        public string OuterId
        {
            get;
            set;
        }
        public string Poster
        {
            get;
            set;
        }

        public bool? IsPicture
        {
            get;
            set;
        }
        public Guid? project_id
        {
            get;
            set;
        }
        /// <summary>
        /// 是否显示
        /// </summary>
        public int? Visible
        {
            get;
            set;
        }
        /// <summary>
        /// 无效的评分
        /// </summary>
        public bool? IsInvalidScore
        {
            get;
            set;
        }

    }

}
