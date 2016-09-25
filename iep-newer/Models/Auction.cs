using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace iep_newer.Models
{

    [Table("Auction")]
    public partial class Auction
    {
        public enum State { DRAFT, READY, OPEN, SOLD, EXPIRED, ALL };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        public int duration { get; set; }

        [Required]
        public int starting_price { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime created { get; set; }
        
        [Column(TypeName = "datetime2")]
        public DateTime? opened { get; set; }
        
        [Column(TypeName = "datetime2")]
        public DateTime? closed { get; set; }

        [Required]
        public State state { get; set; }
        public byte[] image { get; set; }

        [NotMapped]
        //[System.ComponentModel.DataAnnotations.Display(Name = "image")]
        public HttpPostedFileBase image_file { get; set; }
        public int current_price { get; set; }

        [ForeignKey("last_bid_user")]
        public string last_bid_user_id { get; set; }
        
        public virtual ApplicationUser last_bid_user { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }
    }
}