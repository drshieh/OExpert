using OExpert.UILibraryWPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OExpert
{
    public class XWinExec
    {
        public int RunExternal(object hWnd,
                           string sProgramName,   //--Program Name {?FILE(*.Exe)}
                           string sArg,   //--Argument string {""}
                           string sTitle,   //--Display Title String {?PROP}
                           string sDisplay,   //--Display Flag {?RSS(WmShowCmd)}
                           int iWait,   //--Wait Flag {1:Wait|0:Don't wait}
                           bool fLog,
                           string sRtnVar  //--Variable for the Returned Task ID {?PROP}
                          )
        {
            fLog = false;
            LogFile.WriteLine("WinExec {0}", sProgramName);
            int iRtn = 0;
            bool fRtn2 = false;
            if (fLog) runMessages = new List<string>();
            else runMessages = null;
            try
            {
                Process procx = new Process();
                procx.StartInfo = new ProcessStartInfo(sProgramName, sArg);
                procx.StartInfo.RedirectStandardInput = true;
                procx.StartInfo.RedirectStandardOutput = true;
                procx.StartInfo.RedirectStandardError = true;
                procx.StartInfo.UseShellExecute = false;
                procx.StartInfo.CreateNoWindow = true;
                procx.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                procx.EnableRaisingEvents = true;
                procx.OutputDataReceived += new DataReceivedEventHandler(OnOutputDataReceived);
                procx.ErrorDataReceived += new DataReceivedEventHandler(OnErrorDataReceived);
                procx.Exited += new EventHandler(OnProcessExited);

                bool rtn = procx.Start();
                if (rtn)
                {
                    procx.BeginOutputReadLine();
                    procx.BeginErrorReadLine();
                    procx.WaitForExit();
                }
                procx.Close();
                fRtn2 = CheckRunMessagesForError();
                WriteRunMessages();
            }
            catch (Exception ex)
            {
                if (runMessages != null) runMessages.Add(ex.ToString());
                //  LogFile.WriteLine("ERROR: {0} {1}, {2}", sProgramName, sArg, ex.ToString());
            }
            finally
            {
                iRtn = fRtn2 == true ? 1 : 0;
                if (runMessages != null && !String.IsNullOrEmpty(sRtnVar))
                {
                }
            }
            return iRtn;
        }

        //========================== EXTERNAL PROGRAM CALL==================================

        public List<string> runMessages = new List<string>();
        //--------------------------------------------------
        public bool CheckRunMessagesForError()
        {
            if (runMessages == null || runMessages.Count <= 0) return true;
            for (int i = 0; i < runMessages.Count; i++)
            {
                if (runMessages[i].ToUpper().IndexOf("ERROR") >= 0) return false;
            }
            return true;
        }

        //--------------------------------------------------
        public void WriteRunMessages()
        {
            if (runMessages == null || runMessages.Count <= 0) return;
            for (int i = 0; i < runMessages.Count; i++)
            {
                //               LogFile.WriteLine(runMessages[i], true);
            }
        }

        //--------------------------------------------------
        public void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                if (runMessages != null && e.Data.IndexOf("rows sent to SQL") < 0) runMessages.Add(e.Data);
                //             LogFile.WriteLine(e.Data);
            }
        }

        //--------------------------------------------------
        public void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (runMessages != null && !String.IsNullOrEmpty(e.Data))
            {
                runMessages.Add("ERROR: " + e.Data);
            }
        }

        //--------------------------------------------------
        public void OnProcessExited(object sender, EventArgs e)
        {
            if (runMessages != null) runMessages.Add("Exit");
            //        LogFile.WriteLine("Exit");
        }
    }
}
