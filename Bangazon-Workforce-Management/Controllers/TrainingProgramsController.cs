﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bangazon_Workforce_Management.Controllers
{
    public class TrainingProgramsController : Controller
    {
        // GET: EmployeeTrainingPrograms
        public ActionResult Index()
        {
            return View();
        }

        // GET: EmployeeTrainingPrograms/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EmployeeTrainingPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeeTrainingPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmployeeTrainingPrograms/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EmployeeTrainingPrograms/Edit/5
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

        // GET: EmployeeTrainingPrograms/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmployeeTrainingPrograms/Delete/5
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
    }
}