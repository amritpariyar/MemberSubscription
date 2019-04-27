using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberService.Data.BLL
{
    public class Service
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public float Rate { get; set; }
        public char Status { get; set; }
        [Required]
        public string ServiceType { get; set; }
        [Required, Display(Name="Applied Date")]
        public DateTime AppliedDate { get; set; }

        public string StripePlanName { get; set; }
    }
}
