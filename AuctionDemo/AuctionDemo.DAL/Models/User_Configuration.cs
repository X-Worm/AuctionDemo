namespace AuctionDemo.DAL.Models
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
        public short UserId { get; set; }

        public bool? AuctionFinished { get; set; }

        public bool? BidWinLot { get; set; }

        public bool? BidPlacedHigher { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
