using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OExpert
{
    public class XTrace
    {
        public List<InputFact> aInputFactList;

        public XTrace() 
        { 
        }

        public XTrace(List<InputFact> inputFactList)
        {
            aInputFactList = inputFactList;
        }

        public string toStr()
        {
            string rstr = "";
            foreach(InputFact inputFact in aInputFactList) { 
                rstr += "\r\n" + inputFact.toStr("  "); 
            }
            return rstr;
        }
    }
    public class XTrace2
    {
        public List<XInputFact> aXInputFactList;

        public XTrace2()
        {
        }

        public XTrace2(List<XInputFact> inputFactList)
        {
            aXInputFactList = inputFactList;
        }

        public string toStr()
        {
            string rstr = "";
            foreach (XInputFact inputFact in aXInputFactList)
            {
                rstr += "\r\n" + inputFact.toStr("  ");
            }
            return rstr;
        }
    }
}
