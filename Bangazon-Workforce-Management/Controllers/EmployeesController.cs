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
            var departments = new List<Department>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor
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
                            isSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
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
            var Computer = new List<Computer>();
            var department = new List<Department>();
            var trainingProgram = new List<TrainingProgram>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor, et.Id AS                               TrainingProgramId, ce.ComputerId FROM Employee e
                                        LEFT JOIN Department d ON d.Id = e.DepartmentId
                                        LEFT JOIN ComputerEmployee ce ON ce.ComputerId = e.Id
                                        LEFT JOIN EmployeeTraining et ON et.TrainingProgramId = e.Id
                                        WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            isSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"))
                            //ComputerId = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                            //TrainingProgramId = reader.GetInt32(reader.GetOrdinal("TrainingProgramId"))
                        };

                        employee.Computer = new Computer();
                        if (!reader.IsDBNull(reader.GetOrdinal("Make")))
                        {
                            employee.Computer.Make = reader.GetString(reader.GetOrdinal("Make"));
                            employee.Computer.Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"));
                            employee.Computer.PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"));
                        };

                        employee.department = new Department()
                        {
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        };

                        //employee.TrainingPrograms = new TrainingProgram();
                        //if (!reader.IsDBNull(reader.GetOrdinal("StartDate")))
                        //{
                        //    trainingProgram.Name = reader.GetString(reader.GetOrdinal("TrainingProgram"));
                        //    trainingProgram.StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                        //    trainingProgram.EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));
                        //    trainingProgram.MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"));
                        //}
                        //employee.TrainingPrograms.Add(trainingProgram);
                    };
                };
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
                    //cmd.Parameters.AddWithValue("@departmentId", employee.D);

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