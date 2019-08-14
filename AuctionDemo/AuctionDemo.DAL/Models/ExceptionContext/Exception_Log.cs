namespace AuctionDemo.DAL.Models.ExceptionContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Exception_Log
    {
        [Key]
        public Guid RequestId { get; set; }

        [StringLength(1000)]
        public string Route { get; set; }

        [StringLength(50)]
        public string Method { get; set; }

        public string ExceptionMessage { get; set; }

        [StringLength(1000)]
        public string RequestHeaders { get; set; }

        public DateTime? ExceptionDate { get; set; }
    }
}
