using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.TabletModel
{
    public class NotificationResponse
    {
        public virtual bool isSuccess { get; set; }
        public virtual string errorCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<Notif> notifications { get; set; }
    }
    public class Notif
    {
        public virtual int notificationId { get; set; }
        public virtual string notificationName { get; set; }
        public virtual string notificationText { get; set; }
        public virtual string cDate { get; set; }
        public virtual bool readStatus { get; set; } // true is readed, false is no read
    }

    public class ReadStat
    {
        public virtual string notifId { get; set; }
        public virtual string userId { get; set; }
    }

    public class DefaultResponse
    {
        public virtual bool isSuccess { get; set; }
        public virtual string errorCode { get; set; }
        public virtual string resultMessage { get; set; }
    }
}
