using System;
using System.Collections.Generic;
using System.Text;

namespace UMC.Data.Entities
{
    /// <summary>
    /// 文集
    /// </summary>
    public class Portfolio
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id
        {
            get;
            set;
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Caption
        {
            get;
            set;
        }
        /// <summary>
        /// 版主
        /// </summary>
        public Guid? user_id
        {
            get;
            set;
        }
        public int? Count
        {
            get;
            set;
        }
        public DateTime? CreationTime { get; set; }
        public int? Sequence { get; set; }
        public Guid? project_item_id { get; set; }
        public Guid? project_id { get; set; }
    }


}
