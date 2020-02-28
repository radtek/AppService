using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class pushNotiModel
    {
        public virtual string to { get; set; }
        public virtual string cardNo { get; set; }
        public virtual string expireDate { get; set; }
        public notificationBody notification { get; set; }
    }
    public class notificationBody
    {
        public virtual string title { get; set; }
        public virtual string body { get; set; }

    }

    public class googleAPIRequest
    {
        public virtual string to { get; set; }
        public Notification notification { get; set; }
    }

    public class Notification
    {
        public virtual string body { get; set; }
        public virtual bool content_available { get; set; } = true;
        public virtual string priority { get; set; } = "high";
        public virtual string title { get; set; }
    }


    public class googleAPIResponse
    {
        public virtual long multicast_id { get; set; }
        public virtual int success { get; set; }
        public virtual int failure { get; set; }
        public virtual int canonical_ids { get; set; }
        public Result[] results { get; set; }
    }

    public class Result
    {
        public string message_id { get; set; }
    }

}
