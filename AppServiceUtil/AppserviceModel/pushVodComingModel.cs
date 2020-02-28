using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class pushVodComingModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<comingContent> contents { get; set; }
    }
    public class comingContent
    {
        public virtual string contentName { get; set; }
        public virtual string contentImgUrl { get; set; }
    }
}
