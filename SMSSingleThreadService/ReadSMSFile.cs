using System;
using System.IO;
using BackBone;
using SMSSingleThreadService.Properties;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SMSSingleThreadService
{
    class ReadSMSFile
    {

        static void Main(string[] args)
        {

            ReadSMSFile exe = new ReadSMSFile();

        }

        public string BeginMsgExecution()
        {
            Process p = new Process();

            //var fileName = file_info.Name;

            var ProgramName = new Settings().ProgramName;

            Utils.Log("calling program name " + ProgramName);

            p.StartInfo = new ProcessStartInfo("tRun.bat", ProgramName);

            p.StartInfo.WorkingDirectory = new Settings().TafjBinDirectory;

            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            p.Start();

            p.WaitForExit();

            Utils.Log("processing completed for " + ProgramName);

            return "completed";

        }

    }
}
