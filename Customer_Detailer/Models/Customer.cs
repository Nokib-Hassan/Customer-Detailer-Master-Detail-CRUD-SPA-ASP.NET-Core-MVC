using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace N1285413.Models
{
        public class Customer
        {
            public int CustomerId { get; set; }
            public string? CustomerCode { get; set; } = default!;
            [Required, StringLength(50)]
            public string Name { get; set; } = default!;
            public string Address { get; set; } = default!;
            [Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime BusinessStart { get; set; }
            [StringLength(50)]
            public string CustomerType { get; set; } = default!;

            [Required, StringLength(30)]
            public string Phone { get; set; } = default!;
            public string Email { get; set; } = default!;
            [Column(TypeName = "money"), Display(Name = "Credit Details")]
            public decimal CreditDetails { get; set; }
            public string? Photo { get; set; } = default!;

            public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; } = new List<DeliveryAddress>();

        }
    }