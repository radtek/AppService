using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class vodListModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<vodDetial> vodChannels { get; set; }
    }
    public class vodDetial
    {
        public virtual string productId { get; set; }
        public virtual string productName { get; set; }
        public virtual string channelNo { get; set; }
        public virtual string channelLogo { get; set; }
    }
    public class vodProgramModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<programDetial> programList { get; set; }
    }
    public class programDetial
    {
        public virtual string productId { get; set; }
        public virtual string smsCode { get; set; }
        public virtual string contentId { get; set; }
        public virtual string contentNameMon { get; set; }
        public virtual string contentNameEng { get; set; }
        public virtual string contentGenres { get; set; }
        public virtual string contentPrice { get; set; }
        public virtual string inDate { get; set; }
        public virtual string beginDate { get; set; }
        public virtual string endDate { get; set; }
        public virtual string posterUrl { get; set; }
    }

    public class contentDetialModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual string contentId { get; set; }
        public virtual string contentNameMon { get; set; }
        public virtual string contentNameEng { get; set; }
        public virtual string contentGenres { get; set; }
        public virtual string contentPrice { get; set; }
        public virtual string contentDescr { get; set; }
        public virtual string posterUrl { get; set; }
        public virtual string contentYear { get; set; }
        public virtual string contentDuration { get; set; }
        public virtual string trailerUrl { get; set; }
        public virtual string directors { get; set; }
        public virtual string actors { get; set; }
        public virtual bool isOrdered { get; set; }

    }
    
}
