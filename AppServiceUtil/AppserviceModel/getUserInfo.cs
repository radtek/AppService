using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class getUserInfo
    {
        public virtual string cardNo { get; set; }
        public virtual string adminNumber { get; set; }
    }
    public class getUserInfoResponse
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual string userFirstName { get; set; }
        public virtual string userLastName { get; set; }
        public virtual string userRegNo { get; set; }
        public virtual string cardNo { get; set; }
        public virtual string adminNumber { get; set; }
        public virtual List<Products> activeProducts { get; set; }
        public virtual List<Products> additionalProducts { get; set; }
        public virtual List<Counters> activeCounters { get; set; }
    }
    public class Products
    {
        public virtual string productName { get; set; }
        public virtual string productId { get; set; }
        public virtual string endDate { get; set; }
        public virtual bool isMain { get; set; }
        public virtual string orderingNo { get; set; }
    }
    public class Counters
    {
        public virtual string counterName { get; set; }
        public virtual string countId { get; set; }
        public virtual string counterBalance { get; set; }
        public virtual string counterMeasureUnit { get; set; }
        public virtual string counterExpireDate { get; set; }
        public virtual bool isMain { get; set; }
    }
    public class mainAccount
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public Counters mainCounter { get; set; }
    }
}
