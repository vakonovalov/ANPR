using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANPR
{
    class Report
    {
        private Dictionary<String, Parameter> reportData;

        public Report()
        {
            reportData = null;
        }

        public Report(Dictionary<String, Parameter> dataList)
        {
            reportData = dataList;
        }

        public Dictionary<String, Parameter> getDataList()
        {
            return this.reportData;
        }

        public dynamic getData(String name)
        {
            return this.reportData.FirstOrDefault(d => d.Key == name);
        }

        public bool setData(String name, dynamic value)
        {
            if (reportData.ContainsKey(name))
            {
                reportData[name] = value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
