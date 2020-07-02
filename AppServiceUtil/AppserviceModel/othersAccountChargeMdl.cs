using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class othersAccountChargeMdl
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual string firstName { get; set; }
        public virtual string lastName { get; set; }
        public virtual string cardNo { get; set; }
        public virtual string adminNo { get; set; }
    }
}
