using System;
using System.Collections.Generic;
using System.Text;

namespace UMC.Web
{
    /// <summary>
    /// UMC����ע��
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
    public class MappingAttribute : System.Attribute
    {
        /// <summary>
        /// ����ע��WebActivity
        /// </summary>
        public MappingAttribute(String model, String cmd)
        {
            this.Model = model;
            this.Command = cmd;
        }
        /// <summary>
        /// ����ע��IWebFactory  ��ע����
        /// </summary>
        public MappingAttribute()
        {

        }
        /// <summary>
        /// ����ע��WebFlow
        /// </summary>
        /// <param name="model"></param>
        public MappingAttribute(String model)
        {
            this.Model = model;

        }

        public String Model
        {
            get;
            private set;
        }
        public String Command
        {
            get;
            private set;
        }
        public String Desc
        {
            get; set;
        }
        public WebAuthType Auth
        {
            get; set;
        }
        /// <summary>
        /// Ȩ�أ�Ĭ��0����ͬ��ָ���£�ִ��Ȩ�ظߵ�ָ��
        /// </summary>
        public int Weight
        {
            get; set;
        }
        /// <summary>
        /// ���ܷ���
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// �Ƿ��¼��־
        /// </summary>
        public bool IsLog
        {
            get; set;
        }
    }
}
