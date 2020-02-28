using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class newPromotionModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<promotions> promotions { get; set; }
    }
    public class promotions
    {
        public virtual string promotionName { get; set; }
        public virtual string promotionText { get; set; }
        public virtual string promotionImg { get; set; }
        public virtual List<promoDetials> promoDetials { get; set; }
    }
    public class promoDetials
    {
        public virtual string promoId { get; set; }
        public virtual string detialPoster { get; set; }
    }

    public class req_newPromotionRegisterModel
    {
        public virtual string promoName { get; set; }
        public virtual string promoText { get; set; }
        public virtual string promoPosterImageUrl { get; set; }
        public virtual string expireDate { get; set; } // yyyy-MM-dd
        public virtual List<req_promoDetials> detialImageUrls { get; set; }
    }
    public class req_promoDetials
    {
        public virtual string detialImageUrls { get; set; }
    }
}
