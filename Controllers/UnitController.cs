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
        private TMSEntities db = new TMSEntities();

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
            //check if data exits already
            var v = db.UnitTables.Where(a => a.UnitId.Equals(unitTable.UnitId)).FirstOrDefault();
            var vname = db.UnitTables.Where(a => a.UnitName == (unitTable.UnitName)).FirstOrDefault();


            if (ModelState.IsValid)
            {
                if(v != null )
                {
                    TempData["unitidexixts"] = "UNIT ID EXISTS";
                }else if(vname != null)
                {
                    TempData["unitnameexixts"] = "UNIT  EXISTS";
                }
                else
                {
                    try
                    {
                        db.UnitTables.Add(unitTable);
                        db.SaveChanges();

                    }catch(Exception e)
                    { 
                        TempData["savinguniterror"] = e.Message;    
                    }
                }
                   
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
