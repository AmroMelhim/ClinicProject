using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicProject.Models
{
    public class Specialization
    {
        [Key]
        [Required]
        public long Id { get; set; }
        [StringLength (50,ErrorMessage ="Maximum Length{1}")]
        [Display(Name = "Specialization Name")]
        public string SpecializationName { get; set; }
    }
}
