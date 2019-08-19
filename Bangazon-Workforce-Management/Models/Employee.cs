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
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public bool isSupervisor { get; set; }

        public Computer computer { get; set; }

        public Department department { get; set; }

        public List<TrainingProgram> trainingPrograms { get; set; } = new List<TrainingProgram>();
    }
}
