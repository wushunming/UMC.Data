using System;

namespace UMC.Data.Entities
{
    /// <summary>
    /// ��ע����
    /// </summary>
    public enum AttentionType
    {
        /// <summary>
        /// ������
        /// </summary>
        Follow = 2,
        /// <summary>
        /// ����
        /// </summary>
        Friend = 1,
        /// <summary>
        /// ������
        /// </summary>
        Black = -1,
        /// <summary>
        /// ����
        /// </summary>
        Atten = 0,
    }
    /// <summary>
    /// �ظ�����
    /// </summary>
    public enum ReplyType
    {
        /// <summary>
        /// �ظ�
        /// </summary>
        Reply = 0,
        /// <summary>
        ///����
        /// </summary>
        Request = 2,
        /// <summary>
        /// ˽��
        /// </summary>
        Mail = 1,
        /// <summary>
        /// ת��
        /// </summary>
        Farwork = 3,
        /// <summary>
        /// ����
        /// </summary>
        Blog = 4
    }

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
        /// �Ƿ���ʾ
        /// </summary>
        public int? Visible
        {
            get;
            set;
        }
        /// <summary>
        /// ��Ч������
        /// </summary>
        public bool? IsInvalidScore
        {
            get;
            set;
        }

    }

}
