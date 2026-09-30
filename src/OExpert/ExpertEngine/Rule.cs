//---------------------------------------------
//-- Expert System Rule 
//---------------------------------------------
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace OExpert
{
    public class Rule
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public List<RCondition> Conditions { get; set; }
        public string Conclusion { get; set; }
        public double Probability { get; set; }

        public int Value { get; set; }  //--RULE_TRUE, RULE_FALSE, RULE_UNKNOWN
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        //-------------------------------------------------------
        public Rule()
        {
            ID = 0;
            Name = "";
            Message = "";
            Conditions = null;
            Conclusion = null;
            Probability = 0;
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
            Value = -1;
        }

        //---------------------------------------------------------
        public Rule(int id, List<RCondition> conditions, string conclusion, double probability, double _x, double _y)
        {
            ID = id;
            Name = "Rule " + id;
            Message = "";
            //   if (conditions != null) Conditions = conditions.ToDictionary(x => x.Condition, x => x.Value);
            if (conditions != null) Conditions = conditions;
            else conditions = null;
            Conclusion = conclusion;
            Probability = probability;
            X = _x;
            Y = _y;
            Value = -1;
        }

        //---------------------------------------------------------
        public Rule(int id, string _name, List<RCondition> conditions, string conclusion, double probability, double _x, double _y)
        {
            ID = id;
            Name = _name;
            Message = "";
            //   if (conditions != null) Conditions = conditions.ToDictionary(x => x.Condition, x => x.Value);
            if (conditions != null) Conditions = conditions;
            else conditions = null;
            Conclusion = conclusion;
            Probability = probability;
            X = _x;
            Y = _y;
            Value = -1;
        }


        //---------------------------------------------------------
        public string toStr()
        {
            string rstr = "";
            string sRtn = "\r\n";
            rstr += "ID : " + ID;
            rstr += "Name : " + Name;
            rstr += "Message : " + Message;
            rstr += sRtn + "Conditions : ";
            if (Conditions != null)
            {
                foreach (RCondition c in Conditions)
                {
                    rstr += sRtn + String.Format("  {0} {1} {2}", c.Operator, c.Condition, c.WantValue);
                }
            }
            rstr += sRtn + "Conclusion: " + Conclusion;
            rstr += sRtn + "Probability: " + Probability;

            return rstr;
        }

        //---------------------------------------------------------
        public static int Find(List<Rule> aList, int id)
        {
            if (aList == null) return -1;
            for (int i = 0; i < aList.Count; i++)
            {
                if (aList[i].ID == id) return i;
            }
            return -1;
        }

        //---------------------------------------------------------
        public static int FindByName(List<Rule> aList, string name)
        {
            if (aList == null) return -1;
            for (int i = 0; i < aList.Count; i++)
            {
                if (aList[i].Name == name) return i;
            }
            return -1;
        }

        //---------------------------------------------------------
        public static ObservableCollection<XRule> ReadFile(string sFile)
        {
            if (String.IsNullOrEmpty(sFile)) return null;
            if (!File.Exists(sFile)) return null;
            string sJson = File.ReadAllText(sFile);
            if (String.IsNullOrEmpty(sJson)) return null;
            List<Rule> rList = JsonConvert.DeserializeObject<List<Rule>>(sJson);
            if (rList == null) return null;
            ObservableCollection<XRule> rxList = XRule.FromRuleList(rList);
            XRule.ResetRuleValues(rxList);
            return rxList;
        }

        //------------------------------------------------------------------------------
        public static void WriteFile(string sFile, ObservableCollection<XRule> axList)
        {
            if (String.IsNullOrEmpty(sFile) || axList == null) return;
            List<Rule> aList = XRule.ToRuleList(axList);
            StreamWriter sw = new StreamWriter(sFile);
            string sJson = JsonConvert.SerializeObject(aList, Newtonsoft.Json.Formatting.Indented);
            sw.WriteLine(sJson);
            sw.Close();
        }
    }
}
