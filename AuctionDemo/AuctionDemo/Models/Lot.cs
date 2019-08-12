//namespace AuctionDemo.Models
//{
//    using Newtonsoft.Json;
//    using System;
//    using System.Collections.Generic;
//    using System.ComponentModel.DataAnnotations;
//    using System.ComponentModel.DataAnnotations.Schema;
//    using System.Data.Entity.Spatial;

//    [Table("Lot")]
//    public partial class Lot
//    {
//        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
//        public Lot()
//        {
//            Bid = new HashSet<Bid>();
//        }

//        [Key]
//        public short Lot_Id { get; set; }

//        [Required]
//        [StringLength(50)]
//        public string Name { get; set; }

//        [StringLength(200)]
//        public string Description { get; set; }

//        [Required]
//        public int Initial_Price { get; set; }

//        public int? Final_Price { get; set; }

//        public int? Current_Price { get; set; }

//        [Required]
//        public DateTime Time_Of_Lot { get; set; }

//        public DateTime Start_Date { get; set; }

//        public DateTime? End_Date { get; set; }

//        public short User_Id { get; set; }

//        public short? User_Id_Winner { get; set; }

//        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
//        [JsonIgnore]
//        public virtual ICollection<Bid> Bid { get; set; }

//        [JsonIgnore]
//        public virtual User User { get; set; }
//    }
//}
