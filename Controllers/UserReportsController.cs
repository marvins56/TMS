
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TMS.Models;

namespace TMS.Controllers
{
    public class UserReportsController : Controller
    {
        private TMSEntities1 db = new TMSEntities1();

        // GET: UserReports
        public ActionResult Index()
        {
            var userReportsTables = db.UserReportsTables.Include(u => u.TaskTable).Include(u => u.UsersTable);
            return View(userReportsTables.ToList());
        }

        // GET: UserReports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserReportsTable userReportsTable = db.UserReportsTables.Find(id);
            if (userReportsTable == null)
            {
                return HttpNotFound();
            }
            return View(userReportsTable);
        }

        // GET: UserReports/Create
        public ActionResult Create()
        {
            ViewBag.TaskId = new SelectList(db.TaskTables, "TaskId", "TaskName");
            ViewBag.Userid = new SelectList(db.UsersTables, "Userid", "Username");
            return View();
        }

        // POST: UserReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Reportid,Userid,TaskId,Report,Title")] UserReportsTable userReportsTable, HttpPostedFileBase reportfile)
        {    
            //saving changes           
            if (ModelState.IsValid)
            {
                try
                {
                    //FILE PROCESSING 
                    if (reportfile.ContentLength > 0)
                    {
                        string filename = Path.GetFileName(reportfile.FileName);
                        string filepath = Path.Combine(Server.MapPath("~/App_Start/images/report_documents/"), filename);
                        userReportsTable.Report = reportfile.FileName;
                        //userReportsTable.Title = filepath;
                        reportfile.SaveAs(filepath);

                    }

                    db.UserReportsTables.Add(userReportsTable);
                    db.SaveChanges();
                    ViewBag.upload = "report uploadded successfully";

                    // SENDING EMAILS TO SUPERVISOR
                    int id_user = Convert.ToInt32(userReportsTable.Userid);

                    UsersTable userdetails = db.UsersTables.Find(id_user);
                    //sending email for task 
                    if (userdetails == null)
                    {
                                TempData["sessiondataaccess"] = "error fetching data";
                                if (Session["urserrole"].Equals(2))
                                {
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    return RedirectToAction("Myreports");
                                }                      
                    }
                    else
                    {
                        SendEmail(userdetails.Email, userdetails.Username);

                        TempData["emailstatus"] = "REPORT SUBMITTED TO " + userdetails.Username + " with " + userdetails.Email;
                                if (Session["urserrole"].Equals(2))
                                {
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    return RedirectToAction("Myreports");
                                }
                    }

                }
                catch(Exception e)
                {
                    TempData["reporterror"] = e.Message;
                }
            
                //return RedirectToAction("Index");
            }
 
            ViewBag.TaskId = new SelectList(db.TaskTables, "TaskId", "TaskName", userReportsTable.TaskId);
            ViewBag.Userid = new SelectList(db.UsersTables, "Userid", "Username", userReportsTable.Userid);
            return View();
        }

        // GET: UserReports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserReportsTable userReportsTable = db.UserReportsTables.Find(id);
            if (userReportsTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaskId = new SelectList(db.TaskTables, "TaskId", "TaskName", userReportsTable.TaskId);
            ViewBag.Userid = new SelectList(db.UsersTables, "Userid", "Username", userReportsTable.Userid);
            return View(userReportsTable);
        }

        // POST: UserReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Reportid,Userid,TaskId,Report,Title")] UserReportsTable userReportsTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userReportsTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TaskId = new SelectList(db.TaskTables, "TaskId", "TaskName", userReportsTable.TaskId);
            ViewBag.Userid = new SelectList(db.UsersTables, "Userid", "Username", userReportsTable.Userid);
            return View(userReportsTable);
        }

        // GET: UserReports/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserReportsTable userReportsTable = db.UserReportsTables.Find(id);
            if (userReportsTable == null)
            {
                return HttpNotFound();
            }
            return View(userReportsTable);
        }

        // POST: UserReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserReportsTable userReportsTable = db.UserReportsTables.Find(id);
            db.UserReportsTables.Remove(userReportsTable);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DownloadFile(string fileName)
        {
            string fullName = Server.MapPath("~/App_Start/images/report_documents/" + fileName);

            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

        public ActionResult Myreports()
        {
            int uid = Convert.ToInt32(Session["userId"]);
            var results = db.UserReportsTables.Where(i => i.Userid == uid).ToList();

            return View(results);
        }
        public void SendEmail(string emailId, string usernames)
        {

            var fromEmail = new MailAddress("testmarvinug@gmail.com", "UPF TASK MANAGEMENT");
            var toEmail = new MailAddress(emailId);
            //this password is generated by u in ur email account
            var fromEmailPassword = "kcywjucbmujbrycc";

            var subject = "NEW REPORT ADDED TO YOUR SPACE";
            //var body = " HELLO" + usernames + ", A new task has been added to your space please check for deadlines," + "<br/>"

            //    + "TASK DETAILS." + "<br/>"
            //    + task + "   with a deadline set from" + start + " to " + setdeadline + " KIND REGARDS...SUPERVISOR";


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
                StrContent = StrContent.Replace("[username]", usernames);
                //StrContent = StrContent.Replace("[Code]", callbackUrl);
                //StrContent = StrContent.Replace("[Year]", DateTime.Now.Year.ToString());
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
