using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class upointCheckModel
    {
        public virtual string ua_id { get; set; }
        public virtual int result { get; set; }
        public virtual string mobile { get; set; }
        public virtual string message { get; set; }
        public virtual double balance { get; set; }
        public virtual int card_status { get; set; }
        public virtual DateTime created_at { get; set; }
        public virtual string card_number { get; set; }
    }

    public class upointCheckRequest
    {
        public virtual string card_number { get; set; }
        public virtual string mobile { get; set; }
        public virtual string pin_code { get; set; } = "SuperDDish!@#$";
    }


    public class upointTransactionRequest
    {
        public virtual string card_number { get; set; }
        public virtual string mobile { get; set; }
        public virtual int spend_amount { get; set; }
        public virtual int bonus_amount { get; set; }
        public virtual string total_amount { get; set; }
        public virtual int cash_amount { get; set; }
        public virtual string regdate { get; set; }
        public virtual string trans_id { get; set; }
        public virtual List<Bank> bank { get; set; }
        public virtual List<Item> items { get; set; }
    }

    public class Bank
    {
        public virtual string bank_code { get; set; }
        public virtual int non_cash_amount { get; set; }
    }

    public class Item
    {
        public virtual string code { get; set; }
        public virtual string unit { get; set; }
        public virtual string name { get; set; }
        public virtual string quantity { get; set; }
        public virtual string price { get; set; }
        public virtual string total_price { get; set; }
    }

    public class upointTransResponse
    {
        public virtual string issuccess { get; set; }
        public virtual string statuscode { get; set; }
        public virtual string errormsg { get; set; }
        public virtual string receipt_id { get; set; }
        public virtual string point_balance { get; set; }
        public virtual string spend_point { get; set; }
        public virtual string total_point { get; set; }
    }


}
