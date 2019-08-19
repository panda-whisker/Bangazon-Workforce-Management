using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce_Management.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Supervisor")]
        public bool isSupervisor { get; set; }

        [Display(Name = "Department Id")]
        public int departmentId { get; set; }

        public Computer Computer { get; set; }

        public Department department { get; set; }

        public List<TrainingProgram> trainingPrograms { get; set; } = new List<TrainingProgram>();

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
