using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OExpert
{
    public class RunHistoryFile
    {
        public static string ResultFileName = null;
        public static StreamWriter swResultFile = null;

        //-------------------------------------------------------
        public static bool OpenResultFile(string resultFile, bool fAppend)
        {
            ResultFileName = resultFile;
            try
            {
                if (swResultFile != null) swResultFile.Close();
                swResultFile = new StreamWriter(resultFile, fAppend);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
                return false;
            }
        }

        //----------U1---------------------------------------------
        public static void WriteLine(string sMsg)
        {
            try
            {
                if (swResultFile != null)
                {
                    swResultFile.WriteLine(sMsg);
                    swResultFile.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}\r\n{1}", sMsg, ex.ToString());
            }
        }

        //-----------U2--------------------------------------------
        public static void WriteLine(string sFmt, params object[] argList)
        {
            try
            {
                if (swResultFile != null)
                {
                    swResultFile.WriteLine(sFmt, argList);
                    swResultFile.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //-------------------------------------------------------
        public static void WriteLine(params object[] argList)
        {
            string sMsg = "";
            foreach (object o in argList)
            {
                if (o != null) sMsg += o.ToString() + " ";
                else sMsg += "<null> ";
            }
            WriteLine("{0}", sMsg);
        }

        //-------------------------------------------------------
        public static void CloseResultFile()
        {
            if (swResultFile != null) swResultFile.Close();
            swResultFile = null;
        }
    }
}

