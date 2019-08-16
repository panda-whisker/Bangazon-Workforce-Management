using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce_Management.Models
{
    public class TrainingProgram
    {
        [Display(Name = "Program Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Program Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Maximum Attendence")]
        public int MaxAttendees { get; set; }

        public List<Employee> attendingEmployees = new List<Employee>();
    }
}
