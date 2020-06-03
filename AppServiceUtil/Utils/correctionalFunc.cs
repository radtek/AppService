using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.Utils
{
    public class correctionalFunc
    {
        public static bool cardFunc(string inCardNo, out string correctCardNo)
        {
            bool ret = false;
            correctCardNo = string.Empty;
            try
            {
                if (inCardNo.Length != 0)
                {
                    decimal check = decimal.Parse(inCardNo);
                    int cardLen = inCardNo.Length;
                    switch (cardLen)
                    {
                        case 9:
                            if (inCardNo.Substring(0, 1) == "0")
                            {
                                string tempCard = "213" + inCardNo;
                                correctCardNo = tempCard.Substring(0, 10);
                                ret = true;
                            }
                            else
                            {
                                string secPrefix = inCardNo.Substring(0, 3);
                                switch (secPrefix)
                                {
                                    case "997":
                                        correctCardNo = "8097602" + inCardNo;
                                        ret = true;
                                        break;
                                    case "994":
                                        correctCardNo = "8097602" + inCardNo;
                                        ret = true;
                                        break;
                                    case "622":
                                        correctCardNo = "8097602" + inCardNo;
                                        ret = true;
                                        break;
                                    case "726":
                                        correctCardNo = "8097602" + inCardNo;
                                        ret = true;
                                        break;
                                    case "536":
                                        correctCardNo = "8097603" + inCardNo;
                                        ret = true;
                                        break;
                                    case "818":
                                        correctCardNo = "8124003" + inCardNo;
                                        ret = true;
                                        break;
                                    default:
                                        ret = false;
                                        break;
                                }
                            }
                            break;
                        case 10:
                            if (inCardNo.Substring(0, 3) == "213")
                            {
                                correctCardNo = inCardNo;
                                ret = true;
                            }
                            else
                            {
                                ret = false;
                            }
                            break;
                        case 12:
                            if (inCardNo.Substring(0, 3) == "213")
                            {
                                correctCardNo = inCardNo.Substring(0, 10);
                                ret = true;
                            }
                            else
                            {
                                ret = false;
                            }
                            break;
                        case 16:
                            string prefixKonka = inCardNo.Substring(0, 6);
                            switch (prefixKonka)
                            {
                                case "809760":
                                    correctCardNo = inCardNo;
                                    ret = true;
                                    break;
                                case "812400":
                                    correctCardNo = inCardNo;
                                    ret = true;
                                    break;
                                default:
                                    ret = false;
                                    break;
                            }
                            break;
                        default:
                            ret = false;
                            break;
                    }
                }
                else
                {
                    ret = false;
                }
            }
            catch (Exception ex)
            {
                LogWriter._error("Card Correctional Function", string.Format("Input: [{0}], Exception: [{1}]", inCardNo, ex.Message));
                ret = false;
            }
            return ret;
        }
    }
}
