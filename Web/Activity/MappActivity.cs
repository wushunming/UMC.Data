using System;
using System.Collections.Generic;
using System.Text;
using UMC.Data;

namespace UMC.Web.Activity
{
    public abstract class MappActivity : WebActivity
    {
        public abstract int Category { get; }
        protected virtual List<MappingAttribute> Mappings()
        {
            return WebRuntime.Categorys.FindAll(m => m.Category == this.Category);
        }

        public override void ProcessActivity(WebRequest request, WebResponse response)
        {


        

        }
    }
}
