using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.Utils
{
    public class additionalProductSelector
    {
        public static string getAddProd(string productId)
        {
            string prods = string.Empty;
            switch(productId)
            {
                case "28":
                    prods = "3, 26, 67, 70, 79, 86";
                    break;
                case "27":
                    prods = "26, 67, 86";
                    break;
                case "29":
                    prods = "3, 26, 67, 86";
                    break;
                case "73":
                    prods = string.Empty;
                    break;
                default:
                    prods = string.Empty;
                    break;
            }
            return prods;
        }
    }
}
