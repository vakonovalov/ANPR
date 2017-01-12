using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANPR
{
    class Parameter
    {
        private String type;
        private dynamic value;

        public Parameter()
        {
            this.type = null;
            this.value = null;
        }

        public Parameter(String t, dynamic val)
        {
            this.type = t;
            this.value = val;
        }

        public String getType()
        {
            return this.type;
        }

        public dynamic getValue()
        {
            return this.value;
        }

        public void setType(String t)
        {
            this.type = t;
        }

        public void setValue(dynamic val)
        {
            this.value = val;
        }

        public override String ToString()
        {
            return (String)this.value;
        }
    }
}
