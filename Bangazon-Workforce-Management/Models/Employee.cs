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

        public int departmentId { get; set; }

        public Computer Computer { get; set; }

        public Department department { get; set; }

        public List<TrainingProgram> trainingProgram { get; set; } = new List<TrainingProgram>();
    }
}
