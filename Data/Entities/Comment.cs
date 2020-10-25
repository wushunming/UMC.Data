using System;

namespace UMC.Data.Entities
{

    /// <summary>
    /// ����
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
        ///����ʱ��
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

        public string Poster
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
        /// �Ƿ���ʾ
        /// </summary>
        public int? Visible
        {
            get;
            set;
        }
    }

}
