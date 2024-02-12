using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.ViewModel
{
    public class TerminateSessionsViewModel
    {
        public List<string> ?ActiveSessions { get; set; }
        public List<string> ?SessionsToTerminate { get; set; }
    }
}
