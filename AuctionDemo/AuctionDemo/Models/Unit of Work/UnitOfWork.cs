//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using AuctionDemo.Models.Repository;

//namespace AuctionDemo.Models.Unit_of_Work
//{
//    public class UnitOfWork : IDisposable
//    {
//        private AuctionContext context = new AuctionContext();
//        private GenericRepository<Bid> bidrepository;
//        private GenericRepository<Lot> lotrepository;
//        private GenericRepository<User> userrepository;
//        private GenericRepository<User_Configuration> usersconfiguration;

//        public GenericRepository<Bid> Bid
//        {
//            get
//            {

//                if (this.bidrepository == null)
//                {
//                    this.bidrepository = new GenericRepository<Bid>(context);
//                }
//                return bidrepository;
//            }
//        }
//        public GenericRepository<Lot> Lot
//        {
//            get
//            {

//                if (this.lotrepository == null)
//                {
//                    this.lotrepository = new GenericRepository<Lot>(context);
//                }
//                return lotrepository;
//            }
//        }
//        public GenericRepository<User> User
//        {
//            get
//            {

//                if (this.userrepository == null)
//                {
//                    this.userrepository = new GenericRepository<User>(context);
//                }
//                return userrepository;
//            }
//        }
//        public GenericRepository<User_Configuration> User_Configuration
//        {
//            get
//            {

//                if (this.usersconfiguration == null)
//                {
//                    this.usersconfiguration = new GenericRepository<User_Configuration>(context);
//                }
//                return usersconfiguration;
//            }
//        }

//        public void Save()
//        {
//            context.SaveChanges();
//        }
//        private bool disposed = false;
//        protected virtual void Dispose(bool disposing)
//        {
//            if (!this.disposed)
//            {
//                if (disposing)
//                {
//                    context.Dispose();
//                }
//            }
//            this.disposed = true;
//        }
//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }


//    }
//}