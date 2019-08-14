using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuctionDemo.ViewModels.Mappers
{
    /// <summary>
    /// Represents view model of bid 
    /// </summary>
    public class BidViewModel
    {
        /// <summary>
		/// Identifier of bid 
		/// </summary>
		public short BidId { get; set; }

        /// <summary>
        /// Comment to bid
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Price of bid
        /// </summary>
        public int BidPrice { get; set; }

        /// <summary>
        /// Date of bid 
        /// </summary>
        public DateTime Date { get; set; }
    }
}