using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce_Management.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public List<SelectListItem> Departments { get; set; }

        public List<SelectListItem> Computers { get; set; }

        public Employee employee { get; set; }

        public EmployeeEditViewModel() { }

        public EmployeeEditViewModel(Employee employee, List<Department> departments, List<Computer> computers)
        {
            var Employee = employee;
            var selectItems = departments
                .Select(department => new SelectListItem
                {
                    Text = department.Name,
                    Value = department.Id.ToString()
                })
                .ToList();

            selectItems.Insert(0, new SelectListItem
            {
                Text = "Choose Department...",
                Value = "0"
            });

            Departments = selectItems;

            var selectComputers = computers
                .Select(computer => new SelectListItem
                {
                    Text = computer.Make,
                    Value = computer.Id.ToString()
                })
                .ToList();

            selectComputers.Insert(0, new SelectListItem
            {
                Text = "Choose Computer....",
                Value = "0"
            });

            Computers = selectComputers;
        }
    }
}
