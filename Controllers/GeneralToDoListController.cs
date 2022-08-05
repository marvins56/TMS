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
    public class GeneralToDoListController : Controller
    {
        private TMS_db1Entities db = new TMS_db1Entities();

        // GET: GeneralToDoList
        public ActionResult Index()
        {
            var generalToDoListTables = db.GeneralToDoListTables.Include(g => g.TaskTable).Include(g => g.UnitTable).Include(g => g.UsersTable);
            return View(generalToDoListTables.ToList());
        }

        // GET: GeneralToDoList/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeneralToDoListTable generalToDoListTable = db.GeneralToDoListTables.Find(id);
            if (generalToDoListTable == null)
            {
                return HttpNotFound();
            }
            return View(generalToDoListTable);
        }

        // GET: GeneralToDoList/Create
        public ActionResult Create()
        {
            ViewBag.TaskId = new SelectList(db.TaskTables, "TaskId", "TaskName");
            ViewBag.UnitId = new SelectList(db.UnitTables, "UnitId", "UnitName");
            ViewBag.UserName = new SelectList(db.UsersTables, "Userid", "Username");
            return View();
        }

        // POST: GeneralToDoList/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ToDoId,UserName,TaskName,TaskFeedBack,TaskId,IsTaskComplete,UnitId,CompletionDate")] GeneralToDoListTable generalToDoListTable)
        {
            if (ModelState.IsValid)
            {
                db.GeneralToDoListTables.Add(generalToDoListTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TaskId = new SelectList(db.TaskTables, "TaskId", "TaskName", generalToDoListTable.TaskId);
            ViewBag.UnitId = new SelectList(db.UnitTables, "UnitId", "UnitName", generalToDoListTable.UnitId);
            ViewBag.UserName = new SelectList(db.UsersTables, "Userid", "Username", generalToDoListTable.UserName);
            return View(generalToDoListTable);
        }

        // GET: GeneralToDoList/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeneralToDoListTable generalToDoListTable = db.GeneralToDoListTables.Find(id);
            if (generalToDoListTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaskId = new SelectList(db.TaskTables, "TaskId", "TaskName", generalToDoListTable.TaskId);
            ViewBag.UnitId = new SelectList(db.UnitTables, "UnitId", "UnitName", generalToDoListTable.UnitId);
            ViewBag.UserName = new SelectList(db.UsersTables, "Userid", "Username", generalToDoListTable.UserName);
            return View(generalToDoListTable);
        }

        // POST: GeneralToDoList/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ToDoId,UserName,TaskName,TaskFeedBack,TaskId,IsTaskComplete,UnitId,CompletionDate")] GeneralToDoListTable generalToDoListTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(generalToDoListTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TaskId = new SelectList(db.TaskTables, "TaskId", "TaskName", generalToDoListTable.TaskId);
            ViewBag.UnitId = new SelectList(db.UnitTables, "UnitId", "UnitName", generalToDoListTable.UnitId);
            ViewBag.UserName = new SelectList(db.UsersTables, "Userid", "Username", generalToDoListTable.UserName);
            return View(generalToDoListTable);
        }

        // GET: GeneralToDoList/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeneralToDoListTable generalToDoListTable = db.GeneralToDoListTables.Find(id);
            if (generalToDoListTable == null)
            {
                return HttpNotFound();
            }
            return View(generalToDoListTable);
        }

        // POST: GeneralToDoList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GeneralToDoListTable generalToDoListTable = db.GeneralToDoListTables.Find(id);
            db.GeneralToDoListTables.Remove(generalToDoListTable);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult MyToDo()
        {
            int uid = Convert.ToInt32(Session["userId"]);
            var results = db.GeneralToDoListTables.Where(i => i.UserName == uid).ToList();

            return View(results);
        }

        public ActionResult unittodos()
        {
            int uid = Convert.ToInt32(Session["userunit"]);
            var results = db.GeneralToDoListTables.Where(i => i.UnitId == uid).ToList();

            
            return View(results);
        }
        public ActionResult AllToDos()
        {
            var generalToDoListTables = db.GeneralToDoListTables.Include(g => g.TaskTable).Include(g => g.UnitTable).Include(g => g.UsersTable);
            return View(generalToDoListTables.ToList());
        }

        //admin todo  for a unit
        public ActionResult loneUnittodos(int id)
        {
            var results = db.GeneralToDoListTables.Where(i => i.UnitId == id).ToList();
            Session["countcompleted"] = db.GeneralToDoListTables.Where(i => i.UnitId == id && i.IsTaskComplete == true).Count();
            Session["countincompleted"] = db.GeneralToDoListTables.Where(i => i.UnitId == id && i.IsTaskComplete == false).Count();

            return View(results);
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
