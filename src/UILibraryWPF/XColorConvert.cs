using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.DRS.UILibraryWPF
{
    public class XColorConvert
    {
        public static Dictionary<string, string> XColorConvertList = new Dictionary<string, string>
        {
            { "GRAY10", "Aquamarine" },
            { "LIGHTGRAY1", "LightGray" },
            { "CADETBLUE1","CadetBlue" },
            { "GRAY1", "DarkGray" },
            { "GRAY2", "DarkCyan" },
            { "INDIANRED8", "IndianRed" },
            { "GRAY7", "Gray" }
        };

        //------------------------------------------------------------------
        public static void ReadFile(string sFile, bool fClear)
        {
            if(!File.Exists(sFile))
            {
                Console.WriteLine("XColorConvert File does not exist: " + sFile);
                return;
            }
            if (fClear) XColorConvertList.Clear();
            StreamReader sr = new StreamReader(sFile);
            string str = "";
            while(!sr.EndOfStream)
            {
                str = sr.ReadLine().Trim();
                if (String.IsNullOrEmpty(str) || str.StartsWith("//") || str.StartsWith("/*")) continue;
                string[] subx = str.Split('=');
                if(subx.Length > 1)
                {
                    XColorConvertList.Add(subx[0].Trim().ToUpper(), subx[1].Trim());
                }
            }
            sr.Close();
        }

        //------------------------------------------------------------------
        public static string GetValue(string sKey)
        {
            if (String.IsNullOrEmpty(sKey)) return "";
            string sKeyU = sKey.ToUpper();
            string sValue = sKey;
            if (XColorConvertList.ContainsKey(sKeyU)) sValue = XColorConvertList[sKeyU];
            Console.WriteLine("XColorConvert: {0} => {1} ", sKey, sValue);
            return sValue;
        }
    }
}
