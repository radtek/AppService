using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.Utils
{
    public class upgradeProductCalculator
    {
        //pub string TAG = "upgradeProductCalculator";
        public static bool calcDate(string oldProductId, string oldEndDate, string newProductId, int addMonth, out int price, out string newEndDate)
        {
            bool result = false;
            price = 0;
            newEndDate = string.Empty;
            try
            {
                string newStart = DateTime.Now.ToString(appConstantValues.DATE_FORMAT_LONG) + "000000";
                int upgradeAmount = upPrice(oldProductId, newProductId);
                int dailyAmount = dailyPrice(newProductId);
                DateTime oldEnd = DateTime.ParseExact(oldEndDate, appConstantValues.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
                DateTime newBegin = DateTime.ParseExact(newStart, appConstantValues.FORMAT_DATE_TIME_LONG, CultureInfo.InvariantCulture);
                DateTime newEnd = newBegin.AddMonths(addMonth).AddSeconds(-1);
                newEndDate = newEnd.ToString(appConstantValues.DATE_TIME_FORMAT);
                if(upgradeAmount !=0)
                {
                    if(dailyAmount !=0)
                    {
                        if(oldEnd<DateTime.Now)
                        {
                            int totalDays = Convert.ToInt32((newEnd - newBegin).TotalDays);
                            price = totalDays * dailyAmount;
                        }
                        else
                        {
                            if (oldEnd >= newEnd)
                            {
                                int totalDays = Convert.ToInt32((newEnd - newBegin).TotalDays);
                                price = totalDays * upgradeAmount;
                            }
                            else
                            {
                                int upsDate = Convert.ToInt32((oldEnd - newBegin).TotalDays);
                                
                                int orgCalcDate = Convert.ToInt32((newEnd - newBegin).TotalDays) - upsDate;
                                int upgPrice = (upsDate * upgradeAmount);
                                int orgPrice = (orgCalcDate * dailyAmount);
                                price = upgPrice + orgPrice;
                                LogWriter._error(newProductId, string.Format(@"UpgradeDate: [{0} => {1}], OrgDate: [{2} => {3}], UpgradePrice: [{4}], DailyPrice: [{5}]", upsDate, upgPrice, orgCalcDate, orgPrice, upgradeAmount, dailyAmount));
                            }
                        }
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch(Exception ex)
            {
                exceptionManager.ManageException(ex, "upgradeProductCalculator");
                result = false;
            }
            return result;
        }
        public static int upPrice(string oldProd, string newProd)
        {
            int retPrice = 0;
            switch(oldProd)
            {
                case "28":
                    switch (newProd)
                    {
                        case "29":
                            retPrice = 200;
                            break;
                        case "27":
                            retPrice = 370;
                            break;
                        case "73":
                            retPrice = 1030;
                            break;
                        default:
                            retPrice = 0;
                            break;
                    }
                    break;
                case "29":
                    switch(newProd)
                    {
                        case "27":
                            retPrice = 170;
                            break;
                        case "73":
                            retPrice = 840;
                            break;
                        default:
                            retPrice = 0;
                            break;
                    }
                    break;
                case "27":
                    if(newProd == "73")
                    {
                        retPrice = 670;
                    }
                    else
                    {
                        retPrice = 0;
                    }
                    break;
                default:
                    retPrice = 0;
                    break;
            }
            return retPrice;
        }
        public static int dailyPrice(string prodId)
        {
            int retPrice = 0;
            switch(prodId)
            {
                case "28":
                    retPrice = 300;
                    break;
                case "29":
                    retPrice = 500;
                    break;
                case "27":
                    retPrice = 670;
                    break;
                case "73":
                    retPrice = 1330;
                    break;
                default:
                    retPrice = 0;
                    break;
            }
            return retPrice;
        }
    }
}
