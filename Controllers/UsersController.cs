using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TMS.Models;

namespace TMS.Controllers
{
    public class UsersController : Controller
    {
        private TMSEntities1 db = new TMSEntities1();
        TaskTable tasks = new TaskTable();
        GeneralToDoListTable Gtable = new GeneralToDoListTable();
        UnitTable unitsT = new UnitTable();
        RolesTable allroles = new RolesTable();
        PersonalToDoListTable Ptable = new PersonalToDoListTable();

        // GET: Users
        public ActionResult Index()
        {
            var usersTables = db.UsersTables.ToList();
            return View(usersTables);
        }

        // GET: Users/Details/5
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

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.RolesTables, "RolesId", "RoleName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Userid,Username,Email,DOB,Password,IsEmailVerified,ActivationCode,ResetPasswordCode,RoleId")] UsersTable usersTable)
        {
            if (ModelState.IsValid)
            {
                db.UsersTables.Add(usersTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usersTable);
        }

        // GET: Users/Edit/5
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

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Userid,Username,Email,DOB,Password,IsEmailVerified,ActivationCode,ResetPasswordCode,RoleId")] UsersTable usersTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usersTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usersTable);
        }

        // GET: Users/Delete/5
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

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsersTable usersTable = db.UsersTables.Find(id);
            db.UsersTables.Remove(usersTable);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Userperformance(int? sessionid)
        {

            if (sessionid == null)
            {
                ViewBag.sessioniderror = "USER HAS NO ROLE ASSIGNED TO THEM PLEASE ASSIGN ROLE TO VIEW STATISTICS";
            }
            else
            {
                ViewBag.mytotaltasks = db.TaskTables.Where(i => i.UserId == sessionid).Count();
                ViewBag.myincompletetasks = db.TaskTables.Where(i => i.UserId == sessionid && i.TaskStatus == false).Count();
                ViewBag.mycompletetasks = db.TaskTables.Where(i => i.UserId == sessionid && i.TaskStatus == true).Count();
                ViewBag.myToDotasks = db.GeneralToDoListTables.Where(i => i.UserName == sessionid).Count();
                ViewBag.myreports = db.UserReportsTables.Where(i => i.Userid == sessionid).Count();
            }

            return View();
        }


        // GET: Users/Create
        public ActionResult Registration()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] UsersTable users)
        {
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
                    #region activation code generation

                    users.ActivationCode = Guid.NewGuid();
                    #endregion
                    #region passowrd hashing 
                    users.Password = Crypto.Hash(users.Password);
                    users.ConfirmPassword = Crypto.Hash(users.ConfirmPassword);
                    #endregion
                    users.IsEmailVerified = false;
                    #region savr to db
                    using (db = new TMSEntities1())
                    {
                        db.UsersTables.Add(users);

                        db.SaveChanges();

                        //send email to user
                        SendEmailVerificationLink(users.Email, users.ActivationCode.ToString());
                        message = " ACTIVATION LINK HAS BEEN SENT TO YOUR EMAIL ADDESS --" + users.Email;
                        status = true;

                    }
                    #endregion
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
        //verify account

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool status = false;
            using (db = new TMSEntities1())
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var v = db.UsersTables.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();


                if (v != null)
                {
                    v.IsEmailVerified = true;
                    db.SaveChanges();
                    status = true;
                }
                else
                {
                    ViewBag.message = "INVALID REQUEST";
                }
            }
            ViewBag.Status = status;

            return View();
        }

        // any 10-12 char string for use as private key in google authenticator
        //login
        //[HttpGet]
        private const string key = "tms123!$@)(*";

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin loginuser)
        {

            if (ModelState.IsValid)
            {
                string message = "";
                bool status1 = true;
                bool status = false;
                using (db = new TMSEntities1())
                {
                    var v = db.UsersTables.Where(a => a.Email == loginuser.Email && a.IsEmailVerified == status1).FirstOrDefault();
                    var usernames = db.UsersTables.Where(a => a.Email == loginuser.Email && a.IsEmailVerified == status1).Select(a => a.Username).FirstOrDefault();
                    var userId = db.UsersTables.Where(a => a.Email == loginuser.Email && a.IsEmailVerified == status1).Select(a => a.Userid).FirstOrDefault();
                    var userrole = db.UsersRolesTables.Where(a => a.Userid == userId).Select(a => a.RolesId).FirstOrDefault();
                   
                    if (v != null)
                    {

                        if (string.Compare(Crypto.Hash(loginuser.Password), v.Password) == 0)
                        {

                            int timeout = loginuser.RememberMe ? 525600 : 20; // remember for  1 yr
                            var ticket = new FormsAuthenticationTicket(
                                loginuser.Email,
                                loginuser.RememberMe,
                                timeout);

                            string encrypted = FormsAuthentication.Encrypt(ticket);

                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
                            {
                                Expires = DateTime.Now.AddMinutes(timeout),
                                HttpOnly = true

                            };

                            Response.Cookies.Add(cookie);

                            //2fa hre
                            status = true;
                            message = "2FA Verification";
                            Session["Username"] = v.Username;
                            Session["Email"] = v.Email;

                            TwoFactorAuthenticator TFA = new TwoFactorAuthenticator();
                            string UserUniqueKey = (v.Username + key);

                            //as Its a demo, I have done this way. But you should use any encrypted value here which will be unique value per user.
                            Session["UserUniqueKey"] = UserUniqueKey;
                            var setupInfo = TFA.GenerateSetupCode("TMS", UserUniqueKey, key, false, 5);
                            ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                            ViewBag.SetupCode = setupInfo.ManualEntryKey;

                            //session area
                            Session["urserrole"] = userrole;

                            Session["usernames"] = usernames;
                            Session["userId"] = userId;
                            Session["allusers"] = db.UsersTables.Count();
                            Session["admincompleted"] = db.TaskTables.Where(a => a.TaskStatus == true).Count();
                            Session["adminincompleted"] = db.TaskTables.Where(a => a.TaskStatus == false).Count();
                            Session["admincadditional"] = db.GeneralToDoListTables.Count();
                            Session["alltasks"] = sum(Convert.ToInt32(Session["admincompleted"]), Convert.ToInt32(Session["adminincompleted"]));
                            Session["allroles"] = db.RolesTables.Count();
                            Session["mytotaltasks"] = db.TaskTables.Where(i => i.UserId == userId).Count();
                            Session["myincompletetasks"] = db.TaskTables.Where(i => i.UserId == userId && i.TaskStatus == false).Count();
                            Session["mycompletetasks"] = db.TaskTables.Where(i => i.UserId == userId && i.TaskStatus == true).Count();
                            Session["myToDotasks"] = db.GeneralToDoListTables.Where(i => i.UserName == userId).Count();
                            Session["allreports"] = db.UserReportsTables.Count();
                            Session["myreports"] = db.UserReportsTables.Where(i => i.Userid == userId).Count();

                            // SESSIONS FOR REPORTS SUBMITTED

                            //USER PERFORMANCE DETAILS
                        }

                        else
                        {
                            message = "INVALID CREDENTIALS";
                        }

                    }//close v != null
                    else
                    {
                        ModelState.AddModelError("emailNotVerified", "EMAIL NOT VERIFIED PLEASE TRY AGAIN / INVALID CREDENTIALS");
                        message = "EMAIL NOT VERIFIED PLEASE TRY AGAIN / INVALID CREDENTIALS ";
                    }

                }//closing db connection
                ViewBag.message = message;

                ViewBag.Status = status;
            }//close modle state

            return View();
        }

        // user functions
        public int sum(int a, int b)
        {
            return (a + b);
        }
        public ActionResult MyProfile()
        {
            if (Session["Username"] == null || Session["IsValid2FA"] == null || !(bool)Session["IsValid2FA"])
            {
                ViewBag.Message = "sessions invalid";
                return RedirectToAction("Login");
            }

            return RedirectToAction("Index", "Home");

        }

        //2FA VERIFY
        [HttpPost]
        public ActionResult Verify2FA()
        {
            //KEY AUTO GEN

            var token = Request["passcode"];
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            string UserUniqueKey = Session["UserUniqueKey"].ToString();
            bool isValid = tfa.ValidateTwoFactorPIN(key, token);
            if (isValid)
            {
                Session["IsValid2FA"] = true;
                return RedirectToAction("MyProfile", "Users");
            }
            return RedirectToAction("Login", "Users");

        }

        public ActionResult logout()
        {


            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Users");
        }

        // forgot passowrd

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            //Verify Email ID
            //Generate Reset password link 
            //Send Email 
            string message = "";
            bool status = false;

            using (db = new TMSEntities1())
            {
                var account = db.UsersTables.Where(a => a.Email == EmailID).FirstOrDefault();

                if (account != null)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendEmailVerificationLink(account.Email, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                    //in our model class 
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    //ModelState.AddModelError("modelrest", "Account not found");
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            ViewBag.status = status;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (db = new TMSEntities1())
            {
                var user = db.UsersTables.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel
                    {
                        ResetCode = id
                    };
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (db = new TMSEntities1())
                {
                    var user = db.UsersTables.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = Crypto.Hash(model.NewPassword);
                        user.ResetPasswordCode = "";
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        message = "New password updated successfully";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
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


        [NonAction]
        public void SendEmailVerificationLink(string emailId, string activationCode, string emailFor = "VerifyAccount")
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
                  "to verify your email." + "<br /> <a href='" + link + "'> click here</a>";

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
