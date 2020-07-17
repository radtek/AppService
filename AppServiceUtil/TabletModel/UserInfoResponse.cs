using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.TabletModel
{
    public class UserInfoResponse
    {
        public virtual bool isSuccess { get; set; }
        public virtual string errorCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual string customerId { get; set; }
        public virtual string firstName { get; set; }
        public virtual string lastName { get; set; }
        public virtual string certificateNo { get; set; }
        public virtual string homeAddress { get; set; }
        public virtual string telephone { get; set; }
        public virtual string createdDate { get; set; }
        public virtual string createdUser { get; set; }
        public virtual string cardNo { get; set; }
        public virtual string stbNo { get; set; }
        public virtual string adminNo { get; set; }
        public virtual string password { get; set; }
        public virtual string customerType { get; set; }
        public virtual string ccType { get; set; }
        public virtual List<Product> activeProducts { get; set; }
        public virtual List<AccountInfo> activeAccounts { get; set; }
        public virtual List<UpointInfo> upoint { get; set; }
        public virtual List<Member> members { get; set; }

    }
    
    public class AccountInfo
    {
        public virtual string accountName { get; set; }
        public virtual double amount { get; set; }
        public virtual string expireDate { get; set; }
    }
    public class UpointInfo
    {
        public virtual string name { get; set; }
        public virtual double remains { get; set; }
    }
    public class Member
    {
        public virtual string memberNo { get; set; }
    }
}
