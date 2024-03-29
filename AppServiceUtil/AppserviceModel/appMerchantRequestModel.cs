﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    /// <summary>
    /// RequestType 
    /// 1001 - cProduct
    /// 1004 - cNvod
    /// 1007 - cAccount
    /// 1010 - cOAccount
    /// 1013 - uProduct
    /// </summary>
    public class appMerchantRequestModel
    {
        public virtual string requestType { get; set; }
        public virtual chargeProductRequest cProduct { get; set; }
        public virtual orderNvodRequest cNvod { get; set; }
        public virtual chargeAccountRequest cAccount { get; set; }
        public virtual chargeOthersAccountRequest cOAccount { get; set; }
        public virtual chargeOthersAccountNoLoginRequest cOAccountNoLogin { get; set; }
        public virtual upgradeProductRequest uProduct { get; set; }


    }
    public class chargeProductRequest
    {
        public virtual string productId { get; set; }
        public virtual string month { get; set; }
        public virtual string amount { get; set; }
        public virtual string bankName { get; set; }
    }
    public class orderNvodRequest
    {
        public virtual string productId { get; set; }
        public virtual string smsCode { get; set; }
        public virtual string inDate { get; set; }
        public virtual string amount { get; set; }
        public virtual string bankName { get; set; }
    }
    public class chargeAccountRequest
    {
        public virtual string amount { get; set; }
        public virtual string bankName { get; set; }
    }
    public class chargeOthersAccountRequest
    {
        public virtual string amount { get; set; }
        public virtual string bankName { get; set; }
        public virtual string cardNo { get; set; }
    }
    public class chargeOthersAccountNoLoginRequest
    {
        public virtual string amount { get; set; }
        public virtual string bankName { get; set; }
        public virtual string cardNo { get; set; }
        public virtual bool isVat { get; set; }
        public virtual string email { get; set; }
        public virtual string deviceImei { get; set; }
    }
    public class upgradeProductRequest
    {
        public virtual string amount { get; set; }
        public virtual string bankName { get; set; }
        public virtual string toProductId { get; set; }
    }
}
