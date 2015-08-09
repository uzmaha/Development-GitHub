using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using P_CMS.Models;

namespace P_CMS.Controllers
{
    public class PriorityController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Priority
        public ActionResult Index()
        {
            return View(db.Priorities.ToList());
        }

        // GET: Priority/Details/5
     

        // GET: Priority/Create
        public ActionResult Create()
        {
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PriorityId,PriorityType")] Priority priority)
        {
            if (ModelState.IsValid)
            {
                db.Priorities.Add(priority);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(priority);
        }

      
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Priority priority = db.Priorities.Find(id);
            if (priority == null)
            {
                return HttpNotFound();
            }
            return View(priority);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PriorityId,PriorityType")] Priority priority)
        {
            if (ModelState.IsValid)
            {
                db.Entry(priority).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(priority);
        }

    
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Priority priority = db.Priorities.Find(id);
            if (priority == null)
            {
                return HttpNotFound();
            }
            return View(priority);
        }

        // POST: Priority/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Priority priority = db.Priorities.Find(id);
            db.Priorities.Remove(priority);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
