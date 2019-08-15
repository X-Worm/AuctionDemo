using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using AuctionDemo.DAL.Models;

namespace AuctionEmailSenderDemo
{
    public partial class AuctionEmailSender : ServiceBase
    {
        private System.Timers.Timer myTimer;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AuctionEmailSender()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(@"C:\Users\vasyl.kindii\Documents\GitHub\AuctionDemo\AuctionEmailSenderDemo\AuctionEmailSenderDemo\log4net.config"));
        }

        protected override void OnStart(string[] args)
        {
            log.Info("Service started");
            myTimer = new System.Timers.Timer();
                myTimer.Interval = 60000;
                myTimer.Enabled = true;
                myTimer.Start();
                myTimer.Elapsed += new System.Timers.ElapsedEventHandler(testTimer_Elapsed);
        }

        protected override void OnStop()
        {
            log.Info("Service stoped");
            myTimer.Enabled = false;
        }

        private void testTimer_Elapsed(object sender,
          System.Timers.ElapsedEventArgs e)
        {
            //do some operations
            // 1. notify the lot owner that lot finished 
            // 2. notify user that his bid win the lot
            // We compare 2 parameter DateNow with EndDate , if DateNow > EndDate we perform this operations
            AuctionContext db = new AuctionContext();

            var Lot = db.Lot.Where(item => item.EndDate < DateTime.UtcNow && item.UserIdWinner == 0).Select(item => item.LotId).ToList();
            for (int i = 0; i < Lot.Count; i++)
            {
                        Service.LotFinishedService.NotifyOwner(Lot[i] , log);
                        Service.LotFinishedService.NotifyWinner(Lot[i] , log);
            }

            log.Info(Lot.Count.ToString() + " lots has reviewed.");
        }

    }
}
