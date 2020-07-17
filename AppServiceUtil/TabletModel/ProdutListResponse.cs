using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.TabletModel
{
    public class ProdutListResponse
    {
        public virtual bool isSuccess { get; set; }
        public virtual string errorCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual string firstName { get; set; }
        public virtual string cardNo { get; set; }
        public virtual string adminNo { get; set; }
        public virtual string ccType { get; set; }
        public virtual List<productDetial> productList { get; set; }
        public virtual List<productDetial> additionalProductList { get; set; }

    }
    public class productDetial
    {
        public virtual string productName { get; set; }
        public virtual string productId { get; set; }
        public virtual string smsCode { get; set; }
        public virtual string productImg { get; set; }
        public virtual string price { get; set; }
    }

    public class additionalProductListResponse
    {
        public virtual bool isSuccess { get; set; }
        public virtual string errorCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<productDetial> productList { get; set; }
    }
}
