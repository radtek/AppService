using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class upgradeProductListModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<upgradeproductDetial> upProducts { get; set; }
    }
    public class upgradeproductDetial
    {
        public virtual string productName { get; set; }
        public virtual string productId { get; set; }
        public virtual string smsCode { get; set; }
        public virtual string productImg { get; set; }
        public virtual List<priceList> priceList { get; set; }
    }
    public class priceList
    {
        public virtual string productId { get; set; }
        public virtual string month { get; set; }
        public virtual string price { get; set; }
        public virtual string endDate { get; set; }
    }

    public class upgradeoneproductDetial
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public priceList priceInfo { get; set; }

    }
    // - Upgrade Product New Model

    public class upNewProd
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<upProdDetial> upProducts { get; set; }
    }
    public class upProdDetial
    {
        public virtual string productName { get; set; }
        public virtual string productId { get; set; }
        public virtual string smsCode { get; set; }
        public virtual string productImg { get; set; }
        public virtual string convertDay { get; set; }
        public virtual string amount { get; set; }
        public virtual string endDate { get; set; }
    }

    // - localAPI Model
    public class localCheckProductResponse
    {
        public virtual bool issuccess { get; set; }
        public virtual string errorMsg { get; set; }
        public virtual string statusCode { get; set; }
        public virtual string pday { get; set; }
        public virtual string pamount { get; set; }
        public virtual string penddate { get; set; }
    }

    public class localCheckProduct
    {
        public virtual string cardNo { get; set; }
        public virtual string ConvertProduct { get; set; }
        public virtual string Pay_type { get; set; }
    }

    public class localConvertProdcutMdl
    {
        public virtual string cardNo { get; set; }
        public virtual string ConvertProduct { get; set; }
        public virtual string Pay_type { get; set; }
        public virtual string Username { get; set; }
        public virtual string Channel { get; set; }
        public virtual string BranchId { get; set; }
        public virtual string TransgroupId { get; set; }
    }


}
