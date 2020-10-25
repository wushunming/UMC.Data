using System;
using System.Collections.Generic;
using System.Text;

namespace UMC.Data.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public enum LocationType
    {
        /// <summary>
        /// 国家
        /// </summary>
        Nation = 1,
        /// <summary>
        /// 省份
        /// </summary>
        Province = 2,
        /// <summary>
        /// 城市
        /// </summary>
        City = 3,
        /// <summary>
        /// 县区
        /// </summary>
        Region = 4
    }
    public class Location
    {
        public int? Id
        {
            get;
            set;
        }
        public LocationType? Type
        {
            get;
            set;
        }
        public string ZipCode
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public int? ParentId
        {
            get;
            set;
        }


    }
    public class Proposal
    {
        public Guid? ref_id
        {
            get;
            set;
        }
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid? user_id
        {
            get;
            set;
        }
        public DateTime? CreationDate
        {
            get;
            set;
        }
        public string Poster
        {
            get;
            set;
        }

        public int? Type
        {
            get; set;
        }
    }

}
