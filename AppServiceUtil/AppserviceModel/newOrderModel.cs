﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class newOrderModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
    }
    public class defaultResponseModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
    }
    public class registerClientTokenModel
    {
        public virtual string clientToken { get; set; }
    }
    public class defaultResponseModelWidthVat
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual MTAResult mtaResult { get; set; }
    }
    public class MTAResult
    {
        public virtual string merchantId { get; set; }
        public virtual string date { get; set; }
        public virtual string billId { get; set; }
        public virtual string qrData { get; set; }
        public virtual string loterryNo { get; set; }
        public virtual string amount { get; set; }
        public virtual string vat { get; set; }
        public virtual string tax { get; set; }
    }

    public class _eBarimtRequest
    {
        public virtual string channelNo { get; set; }
        public virtual string transId { get; set; } // Account Transaction ID
        public virtual string employeeCode { get; set; }
        public virtual bool organization { get; set; }
        public virtual string customerNo { get; set; }
        public virtual string cardNo { get; set; } // ddish smart cardNo
        public virtual bool sendEmail { get; set; }
        public virtual string customerEmail { get; set; }
        public virtual List<_transactionDetial> transaction { get; set; }
    }
    public class _transactionDetial
    {
        public virtual string productId { get; set; }
        public virtual string productName { get; set; }
        public virtual string qty { get; set; }
        public virtual string price { get; set; }
        public virtual string unit { get; set; }
        public virtual string barCode { get; set; }
    }
    public class _eBarimtResponse
    {
        public virtual bool isSuccess { get; set; }
        public virtual string message { get; set; }
        public virtual string merchantId { get; set; }
        public virtual string billId { get; set; }
        public virtual string qrData { get; set; }
        public virtual string lotteryNo { get; set; }
        public virtual string cityTax { get; set; }
        public virtual string vat { get; set; }
        public virtual string amount { get; set; }
        public virtual string resultDate { get; set; }
        public virtual string billPosition { get; set; }
        public virtual string billUrl { get; set; }
        public virtual string billUrlCopy { get; set; }
    }
}
