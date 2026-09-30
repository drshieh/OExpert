using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace OExpert
{
    public class BackWorker
    {
        public BackgroundWorker worker = null;
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
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (pHandler != null) pHandler(mainObject);
        }

        //-----------------------------------------------------------
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                Console.WriteLine(e.UserState.ToString());
            }
        }

        //-----------------------------------------------------------
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Worker Completed");
            worker = null;
        }

    }
}
