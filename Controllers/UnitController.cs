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
    public class UnitController : Controller
    {
        private TMSEntities1 db = new TMSEntities1();

        // GET: Unit
        public ActionResult Index()
        {
            ViewBag.allunits = db.UnitTables.ToList();
            return View(db.UnitTables.ToList());
        }

        // GET: Unit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitTable unitTable = db.UnitTables.Find(id);
            if (unitTable == null)
            {
                return HttpNotFound();
            }
            return View(unitTable);
        }

        // GET: Unit/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Unit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UnitId,UnitName")] UnitTable unitTable)
        {
            if (ModelState.IsValid)
            {
                db.UnitTables.Add(unitTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(unitTable);
        }

        // GET: Unit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitTable unitTable = db.UnitTables.Find(id);
            if (unitTable == null)
            {
                return HttpNotFound();
            }
            return View(unitTable);
        }

        // POST: Unit/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UnitId,UnitName")] UnitTable unitTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unitTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(unitTable);
        }

        // GET: Unit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitTable unitTable = db.UnitTables.Find(id);
            if (unitTable == null)
            {
                return HttpNotFound();
            }
            return View(unitTable);
        }

        // POST: Unit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UnitTable unitTable = db.UnitTables.Find(id);
            db.UnitTables.Remove(unitTable);
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
