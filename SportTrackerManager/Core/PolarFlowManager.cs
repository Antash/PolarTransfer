using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class PolarFlowManager : SportTrackerManagerBase
    {
        public override string LoginPostDataTemplate
        {
            get
            {
                return "email={0}&password={1}";
            }
        }

        public override string ServiceUrl
        {
            get
            {
                return "https://flow.polar.com/";
            }
        }

        public override string LoginUrl
        {
            get
            {
                return ServiceUrl + "login";
            }
        }

        public override string ExportTcxUrlTemplate
        {
            get
            {
                return ServiceUrl + "api/export/training/tcx/{0}";
            }
        }

        public override string AddTrainingUrl
        {
            get
            {
                return ServiceUrl + "exercises/add";
            }
        }
    }
}
