using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Bangazon_Workforce_Management.Models;
using Bangazon_Workforce_Management.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace Bangazon_Workforce_Management.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Employees
        public ActionResult Index()
        {
            var employees = new List<Employee>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, departmentId, IsSupervisor
                                        FROM Employee";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        employees.Add(new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            isSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"))
                        });
                    }

                    reader.Close();
                }
            }
            return View(employees);
        }

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {
            Employee employee = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor,
                                        ce.ComputerId, FROM Employee e
                                        JOIN Department d ON d.Id = e.DepartmentId
                                        JOIN ComputerEmployee ce ON ce.ComputerId = e.Id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirtName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            isSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"))
                        };
                    }
                }
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            var viewModel = new EmployeeCreateViewModel();
            var departments = GetAllDepartments();
            var selectItems = departments
                .Select(department => new SelectListItem
                {
                    Text = department.Name,
                    Value = department.Id.ToString()
                })
                .ToList();

                selectItems.Insert(0, new SelectListItem
                {
                Text = "Choose department...",
                Value = "0"
                });

            viewModel.Departments = selectItems;
            return View(viewModel);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT INTO Employee (FirstName, LastName, IsSupervisor, DepartmentId)
                            VALUES (@firstName, @lastName, @IsSupervisor, @departmentId)
                            ";

                        cmd.Parameters.AddWithValue("@firstName", employee.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", employee.LastName);
                        cmd.Parameters.AddWithValue("@IsSuperVisor", employee.isSupervisor);
                        cmd.Parameters.AddWithValue("@departmentId", employee.DepartmentId);

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Department";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }

                    reader.Close();
                    return departments;
                }
            }
        }
    }
}