namespace AuctionDemo.DAL.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Bid = new HashSet<Bid>();
            Lot = new HashSet<Lot>();
        }

        [Key]
        public short User_Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(30)]
        public string Surname { get; set; }

        [Required]
        [StringLength(50)]
        public string Login { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string Mail_Address { get; set; }

        public int Balance { get; set; }

        public int Frozen_Balance { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bid> Bid { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lot> Lot { get; set; }

        [JsonIgnore]
        public virtual User_Configuration User_Configuration { get; set; }
    }
}
