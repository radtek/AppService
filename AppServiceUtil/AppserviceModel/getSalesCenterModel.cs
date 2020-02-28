using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.AppserviceModel
{
    public class getSalesCenterModel
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<salesCenterDetial> salesCenters { get; set; }
    }
    public class salesCenterDetial
    {
        public virtual string branchId { get; set; }
        public virtual string branchName { get; set; }
        public virtual string address { get; set; }
        public virtual string type { get; set; }
        public virtual string longtitue { get; set; }
        public virtual string latitude { get; set; }
        public virtual string img { get; set; }
        public virtual string timeTable { get; set; }
        public virtual string door { get; set; }
        public virtual string zoom { get; set; }
    }
    public class salesCenterType
    {
        public virtual bool isSuccess { get; set; }
        public virtual string resultCode { get; set; }
        public virtual string resultMessage { get; set; }
        public virtual List<Area> branchAreas { get; set; }
        public virtual List<BranchType> branchTypes { get; set; }
        public virtual List<Service> branchServices { get; set; }
    }
    public class Area
    {
        public virtual string areaName { get; set; }
        public virtual string areaCode { get; set; }
    }
    public class BranchType
    {
        public virtual string typeName { get; set; }
        public virtual string typeCode { get; set; }
    }
    public class Service
    {
        public virtual string serviceName { get; set; }
        public virtual string serviceCode { get; set; }
    }
}
