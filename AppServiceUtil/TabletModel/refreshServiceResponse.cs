using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.TabletModel
{
    public class refreshServiceResponse
    {
        public virtual bool isSuccess { get; set; }
        public virtual string errorCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual string firstName { get; set; }
        public virtual string cardNo { get; set; }
        public virtual string adminNo { get; set; }
        public virtual List<Product> products { get; set; }
        public virtual List<Vod> vods { get; set; }
        public virtual List<Live> lives { get; set; }
    }
    public class Product
    {
        public virtual string productName { get; set; }
        public virtual string endDate { get; set; }
    }
    public class Vod
    {
        public virtual string contentName { get; set; }
        public virtual string endDate { get; set; }
    }
    public class Live
    {
        public virtual string concertName { get; set; }
        public virtual string endDate { get; set; }
    }
}
