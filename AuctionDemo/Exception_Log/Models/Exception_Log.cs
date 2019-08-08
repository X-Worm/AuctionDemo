namespace Exception_Log.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Exception_Log
    {
        [Key]
        public Guid Request_Id { get; set; }

        [StringLength(1000)]
        public string Route { get; set; }

        [StringLength(50)]
        public string Method { get; set; }

        public string Exception_Message { get; set; }

        [StringLength(1000)]
        public string RequestHeaders { get; set; }

        public DateTime? Exception_Date { get; set; }
    }
}
