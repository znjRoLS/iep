namespace iep_newer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bid")]
    public partial class Bid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [ForeignKey("user")]
        public string user_id { get; set; }

        [Required]
        public virtual ApplicationUser user { get; set; }

        [ForeignKey("auction")]
        public int auction_id { get; set; }

        [Required]
        public virtual Auction auction { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime created { get; set; }
    }
}
