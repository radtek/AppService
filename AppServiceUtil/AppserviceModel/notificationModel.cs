using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class notificationModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<notificationDetial> notifications { get; set; }
    }
    public class notificationDetial
    {
        public virtual string notiId { get; set; }
        public virtual string notiDate { get; set; }
        public virtual string notiName { get; set; }
        public virtual string notiText { get; set; }
        public virtual string notiImgUrl { get; set; }
        public virtual bool isRead { get; set; }
    }
    public class ReadStat
    {
        public virtual string notifId { get; set; }
        public virtual string userId { get; set; }
    }
    public class unreadNotifCount
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual string countN { get; set; }
    }
}
