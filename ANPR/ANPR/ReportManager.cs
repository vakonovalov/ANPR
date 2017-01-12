using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANPR
{
    class ReportManager : Module
    {
        private Report reportCase;
        private List<Module> modules;
        
        public ReportManager()
        {
            this.reportCase = null;
            this.modules = null; 
        }

        public ReportManager(List<Module> modules)
        {
            this.reportCase = null;
            this.modules = modules;   
        }
        
        public Report generateReport()
        {
            Report curr = new Report();
            return curr;
        }

        public bool send(Report report)
        {
            return true;
        }
    }
}
