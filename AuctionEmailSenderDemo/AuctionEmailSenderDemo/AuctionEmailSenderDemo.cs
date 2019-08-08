using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AuctionEmailSenderDemo
{
    public partial class AuctionEmailSender : ServiceBase
    {
        private System.Timers.Timer myTimer;


        public AuctionEmailSender()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            myTimer = new System.Timers.Timer();
                myTimer.Interval = 60000;
                myTimer.Enabled = true;
                myTimer.Start();
                myTimer.Elapsed += new System.Timers.ElapsedEventHandler(testTimer_Elapsed);
        }

        protected override void OnStop()
        {
            myTimer.Enabled = false;
        }

        private void testTimer_Elapsed(object sender,
          System.Timers.ElapsedEventArgs e)
        {
            //do some operations
            // 1. notify the lot owner that lot finished 
            // 2. notify user that his bid win the lot
            // We compare 2 parameter DateNow with EndDate , if DateNow > EndDate we perform this operations
            AuctionModel.AuctionContext db = new AuctionModel.AuctionContext();
            // Service will explain all lot in database
            var Lot = db.Lot.Select(item => item.Lot_Id).ToList();

            for (int i = 0; i < Lot.Count; i++)
            {
                int localLotId = Lot[i];
                // Check if this Lot finished
                var EndDate = db.Lot.Where(item => item.Lot_Id == localLotId).Select(item => item.End_Date).FirstOrDefault();
                if (DateTime.Now > EndDate)
                {
                    //Check if User_Id_Winner is set and != 0 , if User_Id_Winner is set its mean
                    // that service already check is lot
                    var IsComplete = db.Lot.Where(item => item.Lot_Id == localLotId).Select(item => item.User_Id_Winner).FirstOrDefault();

                    if (IsComplete == null | IsComplete == 0)
                    {
                        // Notify the lot owner that lot finished , Winner tha his lot win , and set User_Id_Winner
                        Service.LotFinishedService.NotifyOwnerAndWinner(Lot[i]);
                    }
                }
            }

            Console.WriteLine("Succsecful\n");
        }

    }
}
