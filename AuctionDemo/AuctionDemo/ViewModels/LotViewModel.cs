using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuctionDemo.ViewModels
{
    /// <summary>
    /// Represents view model of lot 
    /// </summary>
    public class LotViewModel
    {
        /// <summary>
		/// Identifier of lot
		/// </summary>
		public short LotId { get; set; }

        /// <summary>
        /// Name of Lot
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of Lot
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// InitialPrice of Lot
        /// </summary>
        public int InitialPrice { get; set; }

        /// <summary>
        /// FinalPrice of Lot
        /// </summary>
        public int? FinalPrice { get; set; }

        /// <summary>
        /// Time of Lot
        /// </summary>
        public DateTime TimeOfLot { get; set; }


    }
}