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
    public class ClientController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Client
        public ActionResult Index()
        {
            var clients = db.Clients.Include(c => c.ApplicationUser).Include(c => c.City).Include(c => c.ClientCompany);
            return View(clients.ToList());
        }

        // GET: Client/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Client/Create
        public ActionResult Create()
        {
            ViewBag.ApplicationUserId = new SelectList(db.Users.ToList(), "Id", "Address");
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "CityName");
            ViewBag.ClientCompanyId = new SelectList(db.ClientCompanies, "ClientCompanyId", "Name");
            return View();
        }

        // POST: Client/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientId,ClientCompanyId,CityId,ApplicationUserId,Name,ContactNo,Description,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ApplicationUserId = new SelectList(db.Users.ToList(), "Id", "Address", client.ApplicationUserId);
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "CityName", client.CityId);
            ViewBag.ClientCompanyId = new SelectList(db.ClientCompanies, "ClientCompanyId", "Name", client.ClientCompanyId);
            return View(client);
        }

        // GET: Client/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users.ToList(), "Id", "Address", client.ApplicationUserId);
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "CityName", client.CityId);
            ViewBag.ClientCompanyId = new SelectList(db.ClientCompanies, "ClientCompanyId", "Name", client.ClientCompanyId);
            return View(client);
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientId,ClientCompanyId,CityId,ApplicationUserId,Name,ContactNo,Description,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users.ToList(), "Id", "Address", client.ApplicationUserId);
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "CityName", client.CityId);
            ViewBag.ClientCompanyId = new SelectList(db.ClientCompanies, "ClientCompanyId", "Name", client.ClientCompanyId);
            return View(client);
        }

        // GET: Client/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Client client = db.Clients.Find(id);
            db.Clients.Remove(client);
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
