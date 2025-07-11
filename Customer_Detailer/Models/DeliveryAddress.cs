using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace N1285413.Models
{
        public class DeliveryAddress
        {
            public int DeliveryAddressId { get; set; }
            [Required, ForeignKey("Customer")]
            public int CustomerId { get; set; }
            [Required, StringLength(60)]
            public string ContactPerson { get; set; } = default!;
            [Required, StringLength(30)]
            public string Phone { get; set; } = default!;
            [Required, StringLength(100)]
            public string Address { get; set; } = default!;
            public virtual Customer? Customer { get; set; }
        }
    }