using Logit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Logit.Controllers
{
    public class ReminderController : RavenBaseController
    {
        //
        // GET: /Reminder/

        public ActionResult Index()
        {
            var data = RavenSession.Query<Reminder>();
            return View(data);
        }

        //
        // GET: /Reminder/Details/5

        public ActionResult Details(int id)
        {
            var data = RavenSession.Load<Reminder>(id);
            return View(data);
        }

        //
        // GET: /Reminder/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Reminder/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Reminder/Edit/5

        public ActionResult Edit(int id)
        {
            var data = RavenSession.Load<Reminder>(id);
            
            return View(data);
        }

        //
        // POST: /Reminder/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var data = RavenSession.Load<Reminder>(id);
                UpdateModel<Reminder>(data, collection);

                RavenSession.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Reminder/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Reminder/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
