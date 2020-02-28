using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class refreshListMdl
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual string refreshDesc { get; set; }
        public virtual string nvodPosterUrl { get; set; }
        public virtual string productPosterUrl { get; set; }
        public virtual string livePosterUrl { get; set; }
        public List<nvodActiveList> nvodList { get; set; }
        public List<productActiveList> productList { get; set; }
        public List<liveActiveList> liveList { get; set; }
    }
    public class nvodActiveList
    {
        public virtual string contentName { get; set; }
        public virtual string contentId { get; set; }
        public virtual string productId { get; set; }
        public virtual string endTime { get; set; }
    }
    public class productActiveList
    {
        public virtual string productName { get; set; }
        public virtual string productId { get; set; }
        public virtual string endTime { get; set; }
    }
    public class liveActiveList
    {
        public virtual string eventName { get; set; }
        public virtual string eventId { get; set; }
        public virtual string endTime { get; set; }
    }
}
