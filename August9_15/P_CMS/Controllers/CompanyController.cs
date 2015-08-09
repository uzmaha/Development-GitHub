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
    public class CompanyController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Company
        public ActionResult Index()
        {
            return View(db.ClientCompanies.ToList());
        }

        // GET: Company/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientCompany clientCompany = db.ClientCompanies.Find(id);
            if (clientCompany == null)
            {
                return HttpNotFound();
            }
            return View(clientCompany);
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientCompanyId,Name,CompanyType,ContactNo,EmailAddress,Fax,Location,Description,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn")] ClientCompany clientCompany)
        {
            if (ModelState.IsValid)
            {
                db.ClientCompanies.Add(clientCompany);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clientCompany);
        }

        // GET: Company/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientCompany clientCompany = db.ClientCompanies.Find(id);
            if (clientCompany == null)
            {
                return HttpNotFound();
            }
            return View(clientCompany);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientCompanyId,Name,CompanyType,ContactNo,EmailAddress,Fax,Location,Description,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn")] ClientCompany clientCompany)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientCompany).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clientCompany);
        }

        // GET: Company/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientCompany clientCompany = db.ClientCompanies.Find(id);
            if (clientCompany == null)
            {
                return HttpNotFound();
            }
            return View(clientCompany);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientCompany clientCompany = db.ClientCompanies.Find(id);
            db.ClientCompanies.Remove(clientCompany);
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
