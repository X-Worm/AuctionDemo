namespace AuctionDemo.DAL.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Lot")]
    public partial class Lot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lot()
        {
            Bid = new HashSet<Bid>();
        }

        [Key]
        public short LotId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public int InitialPrice { get; set; }

        public int? FinalPrice { get; set; }

        public int? CurrentPrice { get; set; }

        public DateTime TimeOfLot { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public short UserId { get; set; }

        public short? UserIdWinner { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bid> Bid { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
