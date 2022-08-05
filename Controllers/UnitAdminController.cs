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
    public class UnitAdminController : Controller
    {
        private TMS_db1Entities db = new TMS_db1Entities();

        // GET: UnitAdmin
        public ActionResult Index()
        {
            var unitAdminTables = db.UnitAdminTables.Include(u => u.UnitTable).Include(u => u.UsersTable);
            return View(unitAdminTables.ToList());
        }

        //unit performance
        public ActionResult unitperformance( int id)
        {
            Session["unitcompleted"] = db.GeneralToDoListTables.Where(i => i.UnitId == id && i.IsTaskComplete == true).Count();
            Session["Unitincompleted"] = db.GeneralToDoListTables.Where(i => i.UnitId == id && i.IsTaskComplete == false).Count();
            Session["UnitTaskcompleted"] = db.TaskTables.Where(i => i.UnitId == id && i.TaskStatus == true).Count();
            Session["UnitTaskincompleted"] = db.TaskTables.Where(i => i.UnitId == id && i.TaskStatus == false).Count();

            return View();
        }

        // GET: UnitAdmin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitAdminTable unitAdminTable = db.UnitAdminTables.Find(id);
            if (unitAdminTable == null)
            {
                return HttpNotFound();
            }
            return View(unitAdminTable);
        }

        // GET: UnitAdmin/Create
        public ActionResult Create()
        {
            ViewBag.UnitId = new SelectList(db.UnitTables, "UnitId", "UnitName");
            ViewBag.Userid = new SelectList(db.UsersTables, "Userid", "Username");
            return View();
        }

        // POST: UnitAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UnitAdminid,Userid,UnitId")] UnitAdminTable unitAdminTable)
        {
            var vname = db.UnitAdminTables.Where(a => a.Userid == (unitAdminTable.Userid)).FirstOrDefault();


            if (ModelState.IsValid)
            {
                if (vname != null)
                {
                    TempData["unitsexixts"] = "USER ALREADY HAS AN ASSIGNED  UNIT ";
                }
               
                else
                {
                    try
                    {
                        db.UnitAdminTables.Add(unitAdminTable);
                        db.SaveChanges();

                    }
                    catch (Exception e)
                    {
                        TempData["savinguniterrorsunit"] = e.Message;
                    }
                }

                return RedirectToAction("Index");

            }

            ViewBag.UnitId = new SelectList(db.UnitTables, "UnitId", "UnitName", unitAdminTable.UnitId);
            ViewBag.Userid = new SelectList(db.UsersTables, "Userid", "Username", unitAdminTable.Userid);
            return View(unitAdminTable);
        }

        // GET: UnitAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitAdminTable unitAdminTable = db.UnitAdminTables.Find(id);
            if (unitAdminTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnitId = new SelectList(db.UnitTables, "UnitId", "UnitName", unitAdminTable.UnitId);
            ViewBag.Userid = new SelectList(db.UsersTables, "Userid", "Username", unitAdminTable.Userid);
            return View(unitAdminTable);
        }

        // POST: UnitAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UnitAdminid,Userid,UnitId")] UnitAdminTable unitAdminTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unitAdminTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UnitId = new SelectList(db.UnitTables, "UnitId", "UnitName", unitAdminTable.UnitId);
            ViewBag.Userid = new SelectList(db.UsersTables, "Userid", "Username", unitAdminTable.Userid);
            return View(unitAdminTable);
        }

        // GET: UnitAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitAdminTable unitAdminTable = db.UnitAdminTables.Find(id);
            if (unitAdminTable == null)
            {
                return HttpNotFound();
            }
            return View(unitAdminTable);
        }

        

        // POST: UnitAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UnitAdminTable unitAdminTable = db.UnitAdminTables.Find(id);
            db.UnitAdminTables.Remove(unitAdminTable);
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
