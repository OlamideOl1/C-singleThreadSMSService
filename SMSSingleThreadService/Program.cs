using BackBone;
using SMSSingleThreadService.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SMSSingleThreadService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new UploadService()
            };
            ServiceBase.Run(ServicesToRun);

            //MessageBirdClass value = new MessageBirdClass();

            //value.send_database("('SMS','CUSTOMER_ID','sms_msg','sender_id','Msisdn','fail_count','status','response_msg','date','time','fileName','Msisdn')}fileName}status", new SQLServerDBInterfac((new Settings().dbconstr)));

            //var upload = new UploadService();

            //upload.BeginProcess();
        }
    }
}