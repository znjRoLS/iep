namespace iep_newer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        public enum OrderState { WAIT, FAIL, PASS}

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [ForeignKey("user")]
        public string user_id { get; set; }

        [Required]
        public virtual ApplicationUser user { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime created { get; set; }
        
        public int tokens { get; set; }
        
        public float price { get; set; }

        [Required]
        public OrderState state { get; set; }
    }
}
