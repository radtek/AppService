using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class productListModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<productDetial> productList { get; set; }
    }
    public class productDetial
    {
        public virtual string productName { get; set; }
        public virtual string productId { get; set; }
        public virtual string smsCode { get; set; }
        public virtual string productImg { get; set; }
        public virtual string price { get; set; }
        //public virtual List<addChennelDetial> upProducts { get; set; }
        //public virtual List<addChennelDetial> additionalProducts { get; set; }
        //public virtual List<priceList> priceList { get; set; }
    }
    public class productAdditionDetial
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual string productId { get; set; }
        public virtual List<productDetial> upProducts { get; set; }
        public virtual List<productDetial> additionalProducts { get; set; }
    }
    public class addChennelDetial
    {
        public virtual string productName { get; set; }
        public virtual string productId { get; set; }
        public virtual string smsCode { get; set; }
        public virtual string productImg { get; set; }
    }
    
    

    // example object
    
}
