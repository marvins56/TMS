using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TMS.Models;

namespace TMS.Controllers
{
    public class RolesController : Controller
    {
        private TMSEntities1 db = new TMSEntities1();

        // GET: Roles
        public ActionResult Index()
        {
            return View(db.RolesTables.ToList());
        }

        // GET: Roles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolesTable rolesTable = db.RolesTables.Find(id);
            if (rolesTable == null)
            {
                return HttpNotFound();
            }
            return View(rolesTable);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RolesId,RoleName")] RolesTable rolesTable)
        {
            if (ModelState.IsValid)
            {
                db.RolesTables.Add(rolesTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rolesTable);
        }

        // GET: Roles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolesTable rolesTable = db.RolesTables.Find(id);
            if (rolesTable == null)
            {
                return HttpNotFound();
            }
            return View(rolesTable);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RolesId,RoleName")] RolesTable rolesTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rolesTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rolesTable);
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolesTable rolesTable = db.RolesTables.Find(id);
            if (rolesTable == null)
            {
                return HttpNotFound();
            }
            return View(rolesTable);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RolesTable rolesTable = db.RolesTables.Find(id);
            db.RolesTables.Remove(rolesTable);
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
