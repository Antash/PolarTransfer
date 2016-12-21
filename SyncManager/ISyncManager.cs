using SportTrackerManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncManager
{
    public interface ISyncManager
    {
        void AddSource(ISportTrackerManager source);
    }
}
