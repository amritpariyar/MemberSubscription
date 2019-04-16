using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberService.Data.BLL
{
    public class MemberServices
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int ServiceId { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ValidDate { get; set; }
        public string Status { get; set; }
        public DateTime? CancelledDate { get; set; }
        public bool IsPaid { get; set; }
        public bool PaymentConfirmed { get; set; }
        public float Amount { get; set; }
        public string MemberName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public string ServiceRate { get; set; }
    }
}
