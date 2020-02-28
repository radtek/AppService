using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class antennaInstallationManualMdl
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<AntennaManualDetial> manuals { get; set; }
    }
    public class AntennaManualDetial
    {
        public virtual string imgUrl { get; set; }
    }

    public class antennaInstallationVideoManualMdl
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<videoDetial> videoManuals { get; set; }
    }
    public class videoDetial
    {
        public virtual string videoId { get; set; }
        public virtual string videoName { get; set; }
    }
}
