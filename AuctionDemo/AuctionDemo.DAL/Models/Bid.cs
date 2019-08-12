namespace AuctionDemo.DAL.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bid")]
    public partial class Bid
    {
        [Key]
        public short Bid_Id { get; set; }

        public short Lot_Id { get; set; }

        [StringLength(50)]
        public string Comments { get; set; }

        public int bid_Price { get; set; }

        public DateTime Date { get; set; }

        public short User_Id { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual Lot Lot { get; set; }
    }
}
