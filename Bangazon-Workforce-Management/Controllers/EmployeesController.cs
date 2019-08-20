﻿using System;
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
            var departments = new List<Department>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor
                                        FROM Employee e
                                        LEFT JOIN Department d ON d.Id = e.DepartmentId;";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        employees.Add(new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            isSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                            departmentId = reader.GetInt32(reader.GetOrdinal("departmentId"))
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
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id AS 'Employee Id', e.FirstName, e.LastName, d.Id AS 'Department Id', d.Name, c.Id AS 'Computer Id', c.Make AS 'Computer Make', c.Manufacturer, tp.Id AS 'Training Program Id', tp.[Name] AS 'Training Program', tp.StartDate, tp.EndDate, tp.MaxAttendees
                        FROM Department d 
                        LEFT JOIN Employee e ON d.Id = e.DepartmentId
                        LEFT JOIN Computer c ON e.Id = c.Id
                        LEFT JOIN TrainingProgram tp ON c.Id = tp.Id
                        WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        {
                            employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Employee Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };


                            Computer computer = new Computer();
                            if (!reader.IsDBNull(reader.GetOrdinal("Computer Id")))
                            {
                                computer.Id = reader.GetInt32(reader.GetOrdinal("Computer Id"));
                                computer.Make = reader.GetString(reader.GetOrdinal("Computer Make"));
                                computer.Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"));
                                employee.Computer = computer;

                            }
                            else
                            {
                                employee.Computer = null;
                            };

                            employee.department = new Department()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Department Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };

                            var trainingProgram = new TrainingProgram();
                            if (!reader.IsDBNull(reader.GetOrdinal("StartDate")))
                            {
                                trainingProgram.Name = reader.GetString(reader.GetOrdinal("Training Program"));
                                trainingProgram.StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                                trainingProgram.EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));
                                trainingProgram.MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"));
                            }
                            employee.trainingPrograms.Add(trainingProgram);
                        };
                    };
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
                        cmd.Parameters.AddWithValue("@departmentId", employee.departmentId);

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
            // YOU HAVE TO WRITE ALL THIS LOGIC STILL!!
            Employee employee = GetSingleEmployee(id);
            List<Department> departments = GetAllDepartments();
            List<Computer> computers = GetAllComputers();
            var viewModel = new EmployeeEditViewModel(employee, departments, computers);
            return View(viewModel);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel model)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        if (model.Computers != null)
                        {
                            cmd.CommandText = @"UPDATE Employee 
                                            SET LastName = @lastName,
                                                DepartmentId = @departmentId
                                            WHERE Id = @id;

                                            UPDATE ComputerEmployee
                                            SET EmployeeId = @id,
                                                ComputerId = @computerId,
                                                AssignDate = GETDATE(),
                                                UnassignDate = null
                                            WHERE EmployeeId = @id";


                            cmd.Parameters.AddWithValue("@lastName", model.employee.LastName);
                            cmd.Parameters.AddWithValue("@departmentId", model.employee.departmentId);
                            cmd.Parameters.AddWithValue("@computerId", model.employee.Computer.Id);
                            cmd.Parameters.AddWithValue("@id", id);
                        }
                        else
                        {

                            cmd.CommandText = @"
                                            UPDATE Employee 
                                            SET LastName = @lastName,
                                                DepartmentId = @departmentId
                                            WHERE Id = @id
                                              ";
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@lastName", model.employee.LastName);
                            cmd.Parameters.AddWithValue("@departmentId", model.employee.departmentId);

                        }

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

        private Employee GetSingleEmployee(int id)
        {
            using (SqlConnection conn = Connection)
            {
                Employee employee = null;
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT Id, FirstName, LastName
                                        FROM Employee
                                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        };
                    }
                }
                return employee;
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

        private List<Computer> GetAllComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Make, Manufacturer FROM Computer";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> computers = new List<Computer>();
                    while (reader.Read())
                    {
                        computers.Add(new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        });
                    }

                    reader.Close();
                    return computers;
                }
            }
        }
    }
}