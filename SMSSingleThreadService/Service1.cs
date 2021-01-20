using System;

using System.Data;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;
using System.Collections.Generic;
using SMSSingleThreadService.Properties;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Linq;
using System.Net;
//using WinSCP;
using BackBone;

namespace SMSSingleThreadService
{
    public partial class UploadService : ServiceBase
    {
        private static bool startnow;
        private static string interval = string.Empty;
        private static string idescr = string.Empty;
        public static string ClirecConstr = string.Empty;
        public static string CBAConstr = string.Empty;
        public static string CBAType = string.Empty;
        public static bool dosync = true;
        FileSystemWatcher watcher = new FileSystemWatcher();


        Int32 timerInterval = 0;
        Int32 count = 0;


        public UploadService()
        {
            try
            {
                try
                {
                    Settings.Default.Save();
                    interval = Settings.Default.Interval;
                    idescr = Settings.Default.IntervalDescription;
                    startnow = Settings.Default.StartImmediately;
                    CBAType = Settings.Default.ProgramName;




                }
                catch { }

                Utils.checkSettings(new object[] {
     new object[] { "Interval", interval },
     new object[] { "IntervalDescription", idescr },
     new object[] { "StartImmediately", startnow},


    });
                InitializeComponent();

                timerInterval = Int32.Parse(interval); if (timerInterval == 0) { throw new Exception("Input string"); }
            }
            catch (Exception e)
            {

                Utils.Log("Service Failed to Initialize because: " +
                    ((e.Message.Contains("Input string") ? "Interval must be an Integer greater than 0, found '" + interval + "'" : e.Message)));
                throw new Exception();
            }



        }

        protected override void OnStart(string[] args)
        {
            var timer = new System.Timers.Timer();

            switch (idescr.ToLower())
            {
                case "s":
                    //if (timerInterval < 30) { timerInterval = 30; };
                    //if (timerInterval > 60) { timerInterval = 60; };
                    timer.Interval = timerInterval * 1000;
                    break;

                case "m":
                    timer.Interval = timerInterval * 60000;
                    break;

                case "h":
                    timer.Interval = timerInterval * 60 * 60000;
                    break;
                case "d":
                    timer.Interval = (timerInterval * 60 * 60000) * 24;
                    break;

                default:
                    timer.Interval = 10 * 60000;
                    break;
            }

            ServiceStatus status = new ServiceStatus();
            status.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            status.dwWaitHint = 1000000;
            SetServiceStatus(this.ServiceHandle, ref status);


            timer.Elapsed += new ElapsedEventHandler(this.onTimer);
            timer.Start();
            Utils.Log("Service Started");



            status.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref status);

            if (startnow)
            {
                BeginProcess();
            }

        }

        protected override void OnStop()
        {

            Utils.Log("Service Stopped");

        }

        public void onTimer(object sender, ElapsedEventArgs args)
        {
            count++;
            if (count == 100)
            {
                Utils.Log("clearAll");
            }

            BeginProcess();

        }




        public enum ServiceState
        {
            SERVICE_STOPPED = 0X00000001,
            SERVICE_START_PENDING = 0X00000002,
            SERVICE_STOP_PENDING = 0X00000003,
            SERVICE_RUNNING = 0X00000004,
            SERVICE_CONTINUE_PENDING = 0X00000005,
            SERVICE_PAUSE_PENDING = 0X00000006,
            SERVICE_PAUSED = 0X00000007,
        }
        [StructLayout(LayoutKind.Sequential)]

        public struct ServiceStatus
        {
            public long dwServiceType;
            public ServiceState dwCurrentState;
            public long dwControlAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;


        };




        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus seviceStatus);



        public void BeginProcess()
        {

            if (dosync)
            {
                dosync = false;

                Checkmessages();

                Utils.Log("Initializing SMS file Listener...");

                dosync = true;
            }

        }




        private DataColumn createDataColumn(string columnName)
        {
            DataColumn cm = new DataColumn();
            cm.DataType = Type.GetType("System.String");
            cm.ColumnName = columnName;
            cm.Caption = columnName;
            return cm;

        }

        public void Checkmessages()
        {
            try
            {

                Utils.Log("Message processing started:");

                //foreach (var file in new DirectoryInfo(new Settings().ProcessDirectory).GetFiles())
                //{
                //    string resposne = new ReadSMSFile().BeginMsgExecution(file, file.Name);


                //}

                //string resposne = new PrvMsgBrdAsncEvnt().BeginMsgExecution(new Settings().in_dir, new Settings().fail_dir, new Settings().success_dir, new SQLServerDBInterfac((new Settings().dbconstr)));

                string resposne = new ReadSMSFile().BeginMsgExecution();

                Utils.Log("All processed sucessfully");

            }
            catch (Exception s)
            {
                Utils.Log("Message process failed because:" + s.Message);
            }
        }

    }
}
