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
    public class UsersRolesController : Controller
    {
        private TMS_db1Entities db = new TMS_db1Entities();

        // GET: UsersRoles
        public ActionResult Index()
        {
            var usersRolesTables = db.UsersRolesTables.Include(u => u.RolesTable).Include(u => u.UsersTable);
            return View(usersRolesTables.ToList());
        }

        // GET: UsersRoles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersRolesTable usersRolesTable = db.UsersRolesTables.Find(id);
            if (usersRolesTable == null)
            {
                return HttpNotFound();
            }
            return View(usersRolesTable);
        }

        // GET: UsersRoles/Create
        public ActionResult Create()
        {
            ViewBag.RolesId = new SelectList(db.RolesTables, "RolesId", "RoleName");
            ViewBag.Userid = new SelectList(db.UsersTables, "Userid", "Username");
            return View();
        }

      
        // POST: UsersRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UsersRolesID,Userid,RolesId")] UsersRolesTable usersRolesTable)
        {
            if (ModelState.IsValid)
            {
                //check if user has role
                var v = db.UsersRolesTables.Where(a => a.Userid.Equals(usersRolesTable.Userid)).FirstOrDefault();

                if (v != null)
                {
                    TempData["assignedrole"] = "USER HAS AN ASSIGNED ROLE!! ";
                    return RedirectToAction("Index");
                }
                else
                {
                    try
                    {
                        db.UsersRolesTables.Add(usersRolesTable);
                        db.SaveChanges();
                      
                    }catch(Exception e)
                    {
                        TempData["savingerrors1"] = e.Message;
                    }
                }

                return RedirectToAction("Index");
            }

            ViewBag.RolesId = new SelectList(db.RolesTables, "RolesId", "RoleName", usersRolesTable.RolesId);
            ViewBag.Userid = new SelectList(db.UsersTables, "Userid", "Username", usersRolesTable.Userid);
            return View(usersRolesTable);
        }

        // GET: UsersRoles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersRolesTable usersRolesTable = db.UsersRolesTables.Find(id);
            if (usersRolesTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.RolesId = new SelectList(db.RolesTables, "RolesId", "RoleName", usersRolesTable.RolesId);
            ViewBag.Userid = new SelectList(db.UsersTables, "Userid", "Username", usersRolesTable.Userid);
            return View(usersRolesTable);
        }

        // POST: UsersRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UsersRolesID,Userid,RolesId")] UsersRolesTable usersRolesTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usersRolesTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RolesId = new SelectList(db.RolesTables, "RolesId", "RoleName", usersRolesTable.RolesId);
            ViewBag.Userid = new SelectList(db.UsersTables, "Userid", "Username", usersRolesTable.Userid);
            return View(usersRolesTable);
        }

        // GET: UsersRoles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersRolesTable usersRolesTable = db.UsersRolesTables.Find(id);
            if (usersRolesTable == null)
            {
                return HttpNotFound();
            }
            return View(usersRolesTable);
        }

        // POST: UsersRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsersRolesTable usersRolesTable = db.UsersRolesTables.Find(id);
            db.UsersRolesTables.Remove(usersRolesTable);
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
