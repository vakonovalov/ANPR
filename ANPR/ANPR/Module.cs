using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ANPR
{
    abstract class Module
    {
        private String moduleName;
        private Dictionary<String, Parameter> parameters;

        public Module()
        {
            this.moduleName = null;
            this.parameters = null;
        }

        public Module(String name, Dictionary<String, Parameter> listOfparams)
        {
            this.moduleName = name;
            this.parameters = listOfparams;
        }

        public List<String> getListOfParams()
        {
            return parameters.Keys.ToList();   
        }

        public virtual void run();

        public dynamic getParameter(String name)
        { 
            return parameters.FirstOrDefault(p => p.Key == name);
        }

        public bool setParameter(String name, dynamic val)
        {
            if (parameters.ContainsKey(name))
            {
                parameters[name] = val;
                return true;
            }
            else 
            {
                return false;
            }
        }
        
    }
}
