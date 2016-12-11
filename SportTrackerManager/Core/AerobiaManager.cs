using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class AerobiaManager : SportTrackerManagerBase
    {
        public override string LoginPostDataTemplate
        {
            get
            {
                return "user%5Bemail%5D={0}&user%5Bpassword%5D={1}";
            }
        }

        public override string ServiceUrl
        {
            get
            {
                return "http://aerobia.ru/";
            }
        }

        public override string LoginUrl
        {
            get
            {
                return ServiceUrl + "users/sign_in";
            }
        }

        public override string ExportTcxUrlTemplate
        {
            get
            {
                return ServiceUrl + "export/workouts/{0}/tcx";
            }
        }

        public override string AddTrainingUrl
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
