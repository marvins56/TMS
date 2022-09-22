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
    public class PersonalToDoListController : Controller
    {
        private TMSEntities db = new TMSEntities();

        // GET: PersonalToDoList
        public ActionResult Index()
        {
            return View(db.PersonalToDoListTables.ToList());
        }

        // GET: PersonalToDoList/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonalToDoListTable personalToDoListTable = db.PersonalToDoListTables.Find(id);
            if (personalToDoListTable == null)
            {
                return HttpNotFound();
            }
            return View(personalToDoListTable);
        }

        // GET: PersonalToDoList/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PersonalToDoList/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PToDoListId,UserName,TaskName,TaskFeedback")] PersonalToDoListTable personalToDoListTable)
        {
            if (ModelState.IsValid)
            {
             
                    db.PersonalToDoListTables.Add(personalToDoListTable);
                    db.SaveChanges();
                
               
                return RedirectToAction("Index");
            }
            else
            {
                
                ViewBag.errormessage = "error creating todo list item";
            }

            return View(personalToDoListTable);
        }

        // GET: PersonalToDoList/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonalToDoListTable personalToDoListTable = db.PersonalToDoListTables.Find(id);
            if (personalToDoListTable == null)
            {
                return HttpNotFound();
            }
            return View(personalToDoListTable);
        }

        // POST: PersonalToDoList/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PToDoListId,UserName,TaskName,TaskFeedback")] PersonalToDoListTable personalToDoListTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personalToDoListTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(personalToDoListTable);
        }

        // GET: PersonalToDoList/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonalToDoListTable personalToDoListTable = db.PersonalToDoListTables.Find(id);
            if (personalToDoListTable == null)
            {
                return HttpNotFound();
            }
            return View(personalToDoListTable);
        }

        // POST: PersonalToDoList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PersonalToDoListTable personalToDoListTable = db.PersonalToDoListTables.Find(id);
            db.PersonalToDoListTables.Remove(personalToDoListTable);
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
