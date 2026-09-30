using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace OExpert.UILibraryWPF
{
    public class BackWorker
    {
        public BackgroundWorker worker = null;
        public ProgressBar aProgressBar = null;
        public ListBox aListener = null;
        public TextBox aListenerMessage = null;
        public WHelper.CBHandler pHandler = null;
        public object mainObject = null;


        //-----------------------------------------------------------
        public BackWorker()
        {
            pHandler = null;
            mainObject = null;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(mainObject);
        }

        //-----------------------------------------------------------
        public BackWorker(object aObj, WHelper.CBHandler aProc)
        {
            pHandler = aProc;
            mainObject = aObj;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(aObj);
        }

        //-----------------------------------------------------------
        public BackWorker(object aObj, WHelper.CBHandler aProc, ProgressBar aProgBar, ListBox aLB, TextBox aTB)
        {
            pHandler = aProc;
            mainObject = aObj;
            aProgressBar = aProgBar;
            aListener = aLB;
            aListenerMessage = aTB;

            if (aProgressBar != null) aProgressBar.Foreground = WBrush.CreateSolidBrush("Yellow");
            worker = new BackgroundWorker();

            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(aObj);
        }

        //-----------------------------------------------------------
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (pHandler != null) pHandler(mainObject);
        }

        //-----------------------------------------------------------
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (aProgressBar != null) aProgressBar.Value = e.ProgressPercentage;
            if (e.UserState != null)
            {
                aListener.Items.Add(e.UserState);
                aListenerMessage.Text = e.UserState.ToString();
            }
        }

        //-----------------------------------------------------------
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (aProgressBar != null) aProgressBar.Foreground = WBrush.CreateSolidBrush("Green");
            worker = null;
        }

    }
}
