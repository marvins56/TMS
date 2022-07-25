using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TMS.Models;

namespace TMS.Controllers
{
    [Authorize]
    [HandleError]
    public class TasksController : Controller
    {
        private TMSEntities1 db = new TMSEntities1();

        // GET: Tasks
        public ActionResult Index()
        {
            var taskTables = db.TaskTables.Include(t => t.UnitTable).Include(t => t.UsersTable);
            return View(taskTables.ToList());
        }

        // GET: Tasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskTable taskTable = db.TaskTables.Find(id);
            if (taskTable == null)
            {
                return HttpNotFound();
            }
            return View(taskTable);
        }

        // GET: Tasks/Create

        public ActionResult Create()
        {
            ViewBag.UnitId = new SelectList(db.UnitTables, "UnitId", "UnitName");
            ViewBag.UserId = new SelectList(db.UsersTables, "Userid", "Username");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TaskId,UserId,TaskName,FeedBack,BodyContent,TaskStatus,TastStartTime,TaskEndTime,UnitId")] TaskTable taskTable)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.TaskTables.Add(taskTable);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    TempData["savingerror"] = e.Message;
                }


                // admin sending notification to email
                int id_user = Convert.ToInt32(taskTable.UserId);

                
                UsersTable userdetails = db.UsersTables.Find(id_user);
                //sending email for task 
                if (userdetails == null)
                {
                    TempData["sessiondataaccess"] = "error fetching data";
                    return RedirectToAction("Index");
                }
                else
                {
                    SendTaskEmail(userdetails.Email, userdetails.Username, taskTable.TastStartTime, taskTable.TaskEndTime, taskTable.BodyContent);
                    TempData["emailstatus"] = "USER " + userdetails.Username + " with " + userdetails.Email + " has been notified";
                    return RedirectToAction("Index");
                }
  
            }

            ViewBag.UnitId = new SelectList(db.UnitTables, "UnitId", "UnitName", taskTable.UnitId);
            ViewBag.UserId = new SelectList(db.UsersTables, "Userid", "Username", taskTable.UserId);
            return View(taskTable);
        }

        // GET: Tasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskTable taskTable = db.TaskTables.Find(id);
            if (taskTable == null)
            {
                return HttpNotFound();
            }

            ViewBag.UnitId = new SelectList(db.UnitTables, "UnitId", "UnitName", taskTable.UnitId);
            ViewBag.UserId = new SelectList(db.UsersTables, "Userid", "Username", taskTable.UserId);
            return View(taskTable);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaskId,UserId,TaskName,FeedBack,BodyContent,TaskStatus,TastStartTime,TaskEndTime,UnitId")] TaskTable taskTable)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(taskTable).State = EntityState.Modified;
                    db.SaveChanges();


                    //rediection according to users
                    switch (Session["urserrole"])
                    {
                        case 2: {

                                try
                                {
                                    // admin sending notification to email
                                    int id_user = Convert.ToInt32(taskTable.UserId);

                                    UsersTable userdetails = db.UsersTables.Find(id_user);
                                    SendTaskEmail(userdetails.Email, userdetails.Username, taskTable.TastStartTime, taskTable.TaskEndTime, taskTable.BodyContent);
                                    TempData["emailstatus"] = "USER " + userdetails.Username + " with " + userdetails.Email + " has been notified";
                                    return RedirectToAction("Index");
                                }
                                catch (Exception e)
                                {
                                    TempData["emailerror"] = e.Message;
                                   
                                }
                            }
                            break;
                        default: {
                                try
                                {
                                    //send self email for notification
                                    UsersTable userdetails2 = db.UsersTables.Find(Session["userId"]);
                                    SendTaskEmail(userdetails2.Email, userdetails2.Username, taskTable.TastStartTime, taskTable.TaskEndTime, taskTable.BodyContent);
                                    TempData["emailstatus"] = "Hello " + userdetails2.Username + ",you have made Changes have been made to your task space";
                                    return RedirectToAction("Mytask");
                                }
                                catch (Exception e)
                                {
                                    TempData["emailerror"] = e.Message;
                                   
                                }

                            }
                            break;    
                    }
                }
                catch (Exception e)
                {
                    TempData["savingerror"] = e.Message;
                }

            }

            ViewBag.UnitId = new SelectList(db.UnitTables, "UnitId", "UnitName", taskTable.UnitId);
            ViewBag.UserId = new SelectList(db.UsersTables, "Userid", "Username", taskTable.UserId);
            return View(taskTable);
        }

        // GET: Tasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskTable taskTable = db.TaskTables.Find(id);
            if (taskTable == null)
            {
                return HttpNotFound();
            }
            return View(taskTable);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TaskTable taskTable = db.TaskTables.Find(id);
            db.TaskTables.Remove(taskTable);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Mytask()
        {
            int uid = Convert.ToInt32(Session["userId"]);
            var results = db.TaskTables.Where(i => i.UserId == uid).ToList();

            return View(results);
        }
        public ActionResult AllTasks()
        {
            var taskTables = db.TaskTables.Include(t => t.UnitTable).Include(t => t.UsersTable);
            return View(taskTables.ToList());
        }

        public ActionResult unittasks()
        {
            int uid = Convert.ToInt32(Session["userunit"]);
            var results = db.TaskTables.Where(i => i.UnitId == uid).ToList();
            return View(results);
        }
      
        //admin tasks for a unit
        public ActionResult loneUnittask(int id)
        {
            var results = db.TaskTables.Where(i => i.UnitId == id).ToList();

            Session["countTaskcompleted"] = db.TaskTables.Where(i => i.UnitId == id && i.TaskStatus == true).Count();
            Session["countTaskincompleted"] = db.TaskTables.Where(i => i.UnitId == id && i.TaskStatus == false).Count();
            return View(results);
        }
        //function to send email
        [NonAction]
        public void SendTaskEmail(string emailId, string usernames, DateTime start, DateTime setdeadline, string task)
        {

            var fromEmail = new MailAddress("testmarvinug@gmail.com", "UPF TASK MANAGEMENT");
            var toEmail = new MailAddress(emailId);
            //this password is generated by u in ur email account
            var fromEmailPassword = "kcywjucbmujbrycc";

           var subject = "NEW TASK ADDED TO YOUR SPACE";
           var body1 = usernames + ", A new task has been added to your space please check for deadlines," + "<br/>"

                + "TASK DETAILS." + "<br/>"
                + task + "   with a deadline set from" + start + " to " + setdeadline + " KIND REGARDS...SUPERVISOR";


            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,

                Credentials = new System.Net.NetworkCredential(fromEmail.Address, fromEmailPassword),
            };

            string body = string.Empty;
            var root = AppDomain.CurrentDomain.BaseDirectory;

            using (var reader = new System.IO.StreamReader(root + @"/App_Start/Email/index.html"))
            {
                string readFile = reader.ReadToEnd();
                string StrContent = string.Empty;
                StrContent = readFile;
                //Assing the field values in the template
                StrContent = StrContent.Replace("[body]", body1);
               
                body = StrContent.ToString();
            }

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
                //Fetching Email Body Text from EmailTemplate File.  
            })
                try
                {
                    smtp.Send(message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
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
