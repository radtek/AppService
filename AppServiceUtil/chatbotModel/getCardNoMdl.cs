using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.chatbotModel
{
    public class getCardNoMdl
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual string cardNo { get; set; }
        public virtual string phoneNo { get; set; }
        public virtual string confirmCode { get; set; }
    }
    public class activeProductsMdl
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual string cardNo { get; set; }
        public List<chatbot_productDetial> products { get; set; }
    }
    public class chatbot_productDetial
    {
        public virtual string productId { get; set; }
        public virtual string productName { get; set; }
        public virtual string endDate { get; set; }
    }
}
