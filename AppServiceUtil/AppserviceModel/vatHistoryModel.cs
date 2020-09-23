using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class vatHistoryModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<vatList> vats { get; set; }
    }
    public class vatList
    {
        public virtual string cardNo { get; set; }
        public virtual string amount { get; set; }
        public virtual string billId { get; set; }
        public virtual string qrData { get; set; }
        public virtual string lotteryNo { get; set; }
        public virtual string billDate { get; set; }
        public virtual string createUser { get; set; }

    }
}
