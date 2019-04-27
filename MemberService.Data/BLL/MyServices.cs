using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberService.Data.BLL
{
    public class MyServices
    {
        public int Id { get; set; }
        [Required, Display(Name ="Member")]
        public int MemberId { get; set; }
        [Required, Display(Name ="Service")]
        public int ServiceId { get; set; }
        [Required,Display(Name ="Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime StartDate { get; set; }
        [Required,Display(Name ="Valid Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ValidDate { get; set; }
        public string Status { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CancelledDate { get; set; }

        public bool IsPaid { get; set; }
        public bool PaymentConfirmed { get; set; }

        public float Amount { get; set; }

        public string StripeEmail { get; set; }
        public string StripeToken { get; set; }
        public string StripeCustomerId { get; set; }
        public string StripeSubscriptionId { get; set; }

        //public virtual ICollection<Service> Service { get; set; }
        public virtual Service Service { get; set; }
    }
}
