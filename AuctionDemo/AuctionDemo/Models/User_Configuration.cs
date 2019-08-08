namespace AuctionDemo.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User_Configuration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short User_Id { get; set; }

        public bool? Auction_Finished { get; set; }

        public bool? Bid_Win_Lot { get; set; }

        public bool? Bid_Placed_Higher { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
