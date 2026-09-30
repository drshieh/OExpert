using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OExpert.UILibraryWPF
{
    public class LogFile
    {
        public static string LogFileName = null;
        public static StreamWriter swLogFile = null;
        public delegate void CBHandler(object o);  //--Caller Provided Callback function
        public static CBHandler pWriteListener = null;
        public static ListBox aListener = null;
        public static TextBox aListenerMessage = null;
        public static ListBox aStepLB = null;
        public static int iStep = 0;

        public static bool fDebug = false;  //--Debug mode, more display

        //-------------------------------------------------------
        public static bool OpenLogFile(string sLogFile, bool fAppend, bool _fdebug)
        {
            LogFileName = sLogFile;
            fDebug = _fdebug;
            try
            {
                if (swLogFile != null) swLogFile.Close();
                swLogFile = new StreamWriter(sLogFile, fAppend);
                iStep = 0;
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
                if (swLogFile != null)
                {
                    swLogFile.WriteLine(sMsg);
                    swLogFile.Flush();
                }
                if (fDebug)
                {
                    if (pWriteListener != null) pWriteListener(sMsg);
                    if (aListener != null) AddMsg(sMsg);
                    if (aStepLB != null) AddStep(sMsg);
                    Console.WriteLine(sMsg);
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
                if (swLogFile != null)
                {
                    swLogFile.WriteLine(sFmt, argList);
                    swLogFile.Flush();
                }
                if (fDebug)
                {
                    if (pWriteListener != null) pWriteListener(String.Format(sFmt, argList));
                    if (aListener != null) AddMsg(sFmt, argList);
                    if (aStepLB != null) AddStep(sFmt, argList);
                    Console.WriteLine(sFmt, argList);
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
        public static void WriteLine(string sMsg, bool fShow)
        {
            if (swLogFile != null)
            {
                swLogFile.WriteLine(sMsg);
                swLogFile.Flush();
            }
            if (fShow)
            {
                if (pWriteListener != null) pWriteListener(String.Format(sMsg));
                if (aListener != null) AddMsg("{0}", sMsg);
                else Console.WriteLine(sMsg);
                if (aStepLB != null) AddStep("{0}", sMsg);
            }
        }

        //------------------------------------------------------------
        public static void AddMsg(string sFmt, params object[] args)
        {
            string sMsg = String.Format(sFmt, args);
            int isel = aListener.Items.Add(sMsg);
            //            aListener.Refresh();
            aListenerMessage.Text = sMsg;
            //            aListenerMessage.Refresh();
            //         aListener.SelectedIndex = isel;
            //         aListener.ScrollIntoView(aListener.SelectedItem);
        }

        //------------------------------------------------------------
        public static void AddStep(string sFmt, params object[] args)
        {
            string sMsg = String.Format(sFmt, args);
            int isel = aStepLB.Items.Add(iStep.ToString("D6") + "> " + sMsg);
            iStep++;
            //            aListener.Refresh();
            //            aListenerMessage.Refresh();
            aStepLB.SelectedIndex = isel;
            aStepLB.ScrollIntoView(aStepLB.SelectedItem);
        }

        //-------------------------------------------------------
        public static void CloseLogFile()
        {
            if (swLogFile != null) swLogFile.Close();
            swLogFile = null;
        }
    }
}

