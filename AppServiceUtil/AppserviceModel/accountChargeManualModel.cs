using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class accountChargeManualModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<manualDetial> manuals { get; set; }
    }
    public class manualDetial
    {
        public virtual string name { get; set; }
        public virtual string manualText { get; set; }
        public virtual string imgUrl { get; set; }
    }
}
