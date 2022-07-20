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
    public class SuperAdminController : Controller
    {
        private TMSEntities1 db = new TMSEntities1();

        // GET: SuperAdmin
        public ActionResult Index()
        {
            return View(db.UsersTables.ToList());
        }

        // GET: SuperAdmin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersTable usersTable = db.UsersTables.Find(id);
            if (usersTable == null)
            {
                return HttpNotFound();
            }
            return View(usersTable);
        }

        // GET: SuperAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SuperAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Userid,Username,Email,DOB,Password,IsEmailVerified,ActivationCode,ResetPasswordCode")] UsersTable usersTable)
        {
            if (ModelState.IsValid)
            {
                db.UsersTables.Add(usersTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usersTable);
        }

        // GET: SuperAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersTable usersTable = db.UsersTables.Find(id);
            if (usersTable == null)
            {
                return HttpNotFound();
            }
            return View(usersTable);
        }

        // POST: SuperAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Userid,Username,Email,DOB,Password,IsEmailVerified,ActivationCode,ResetPasswordCode")] UsersTable usersTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usersTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usersTable);
        }

        // GET: SuperAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsersTable usersTable = db.UsersTables.Find(id);
            if (usersTable == null)
            {
                return HttpNotFound();
            }
            return View(usersTable);
        }

        // POST: SuperAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsersTable usersTable = db.UsersTables.Find(id);
            db.UsersTables.Remove(usersTable);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Users/Create
        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Exclude = "IsEmailVerified,ActivationCode")] UsersTable users)
        {

            string passoriginal = users.Password;

            var nwTm = DateTime.Now.ToShortTimeString();
            ViewData["nowDt"] = nwTm;

            bool status = false;
            string message = "";
            if (ModelState.IsValid)
            {

                //if user or email  exixts or not
                var emaillexists = IsMailExists(users.Email);
                var usernameexixts = IsuSernameExits(users.Username);
                if (emaillexists)
                {
                    ModelState.AddModelError("EmailExists", "EMAIL ALREADY EXISTS");

                }
                else if (usernameexixts)
                {
                    ModelState.AddModelError("usernameexixts", "USERNAME ALREADY EXISTS / TAKEN");
                }
                else
                {

                    if (users.Password == users.ConfirmPassword)
                    {
                        
                        #region activation code generation

                        users.ActivationCode = Guid.NewGuid();
                        SendEmailVerificationLink(users.Email, users.ActivationCode.ToString(), passoriginal);
                        #endregion
                        #region passowrd hashing 
                        users.Password = Crypto.Hash(users.Password);
                        users.ConfirmPassword = Crypto.Hash(users.ConfirmPassword);

                        //CHECK IF THEY MATCH

                            
                            #endregion
                            users.IsEmailVerified = false;
                            #region savr to db
                            using (db = new TMSEntities1())
                            {
                                db.UsersTables.Add(users);

                                db.SaveChanges();

                                //send email to user
                                //SendEmailVerificationLink(users.Email, users.ActivationCode.ToString(), passoriginal);
                                message = " ACTIVATION LINK HAS BEEN SENT TO THE USERS EMAIL ADDRESS --" + users.Email;
                                status = true;

                            }
                            #endregion

                    }
                    else
                    {
                        ModelState.AddModelError("passwordmissmatch", "PASSWORD MISSMATCH");
                    }


                }



            }

            else
            {
                message = "Invalid request";
            }


            ViewBag.message = message;

            ViewBag.Status = status;
            return View(users);
        }




        [NonAction]
        public void SendEmailVerificationLink(string emailId, string activationCode, string userpassowrd, string emailFor = "VerifyAccount")
        {
            //var scheme = Request.Url.Scheme;
            //var host = Request.Url.Host;
            //var port = Request.Url.Port;    
            var verifyUrl = "/Users/" + emailFor + "/" + activationCode;
            var link = verifyUrl;
            var fromEmail = new MailAddress("testmarvinug@gmail.com", "test asp");
            var toEmail = new MailAddress(emailId);
            //this password is generated by u in ur email account
            var fromEmailPassword = "kcywjucbmujbrycc";

            var subject = "";
            var body = "";
            if (emailFor == "VerifyAccount")
            {

                subject = "ACCOUNT CREATION SUCCESSFULL";
                body = "You're receiving this message because you signed up for an account on mysite. (If you didn't sign up, you can ignore this email.) please click below" +
                  "to verify your email." + "<br /> <a href='" + link + "'> click here</a>"   + "your current password will be  " + userpassowrd;

            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Password Reset";
                body = "ResetPassword CLICK ON LINK TO RESET UR PASSWORD--  " + "<a href=" + link + "   >Rest Now </a>";
            }

            var smtp = new SmtpClient("smtp.gmail.com")
            {

                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,

                Credentials = new System.Net.NetworkCredential(fromEmail.Address, fromEmailPassword),


            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {

                Subject = subject,
                Body = body,
                IsBodyHtml = true

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

        [NonAction]
        public bool IsMailExists(string emailId)
        {
            using (db = new TMSEntities1())
            {
                var v = db.UsersTables.Where(a => a.Email == emailId).FirstOrDefault();
                return v != null;
            }
        }
        [NonAction]
        public bool IsuSernameExits(string username)
        {
            using (db = new TMSEntities1())
            {
                var v = db.UsersTables.Where(a => a.Username == username).FirstOrDefault();
                return v != null;
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
