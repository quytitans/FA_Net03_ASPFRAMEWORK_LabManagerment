using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LabSem3.Data;
using LabSem3.Enum;
using LabSem3.Models;
using LabSem3.Models.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;

namespace LabSem3.Controllers
{
    public class ComplaintController : Controller
    {
        private UserManager<Account> userManager; //Bên database
        private RoleManager<IdentityRole> roleManager; //Bên database
        private LabSem3Context db;

        public ComplaintController()
        {
            db = new LabSem3Context();
            UserStore<Account> userStore = new UserStore<Account>(db); // create, update, delete giống UserModel
            userManager = new UserManager<Account>(userStore); // giống Service, xử lý các vấn đề liên quan đến logic
            RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(db); // create, update, delete giống UserModel
            roleManager = new RoleManager<IdentityRole>(roleStore); // giống Service, xử lý các vấn đề liên quan đến logic
        }
        // GET: Complaint
        public ActionResult Index(string keyWord, int? statusCheck,  int? page) {
            
            var result2 = db.Complaints.OrderBy(s => s.Id).AsQueryable();

            if (!string.IsNullOrEmpty(keyWord))
            {
                result2 = result2.Where(s => s.Title.Contains(keyWord) || s.Detail.Contains(keyWord));
            }

            if (statusCheck != -1 && statusCheck != null)
            {
                result2 = result2.Where(s => s.Status == statusCheck);
            }

            //if (statusCheck != -1)
            //{
            //    result2 = result2.Where(s => s.Status == statusCheck);
            //}

            ViewBag.statusCheck = statusCheck;

            ViewBag.keyWord = keyWord;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(result2.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ComplaintWaiting(int? page)
        {
            var listTechnicalStaff = new List<Account>();
            var listInstructor = new List<Account>();
            foreach (var user in db.Users.ToList())
            {
                var checkRole = userManager.GetRoles(user.Id).ToList();
                foreach (var role in checkRole)
                {
                    if (role == RoleEnum.TECHNICAL_STAFF.ToString())
                    {
                        listTechnicalStaff.Add(user);
                    }

                    if (role == RoleEnum.INSTRUCTOR.ToString())
                    {
                        listInstructor.Add(user);
                    }
                }
            }

            ViewBag.listTechnicalStaff = listTechnicalStaff;
            ViewBag.listInstructor = listInstructor;

            ViewBag.UserAll = db.Users.ToList();
            var result2 = db.Complaints.OrderBy(s => s.Id).AsQueryable().Where(s => s.Status == 4);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(result2.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AssignPeople(int comPlaintId, string StaffId)
        {
            var processComplaint = db.Complaints.Find(comPlaintId);
            processComplaint.Status = 2;
            processComplaint.SupportedId = StaffId;
            db.SaveChanges();
            return Redirect("/Complaint/ComplaintWaiting");
        }

        // GET: Complaint/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Complaint/Create
        public ActionResult Create()
        {
            ViewBag.TypeComplaints = db.TypeComplaints.ToList();
            return View();
        }

        // POST: Complaint/Create
        [HttpPost]
        public ActionResult Create(ComplaintViewModel complaintViewModel)
        {
            var newComPlaint = new Complaint(complaintViewModel)
            {
                AccountId = "985f35a0-32c3-4476-9e28-14ddb97c33fe"
            };

            db.Complaints.Add(newComPlaint);
            var result = db.SaveChanges();
            if (result == 1)
            {
                return Redirect("Index");
            }
            else
            {
                return View("Faile");
            }
        }

        // GET: Complaint/Edit/5
        public ActionResult Edit(int id)
        {
            return View(db.Complaints.Find(id));
        }

        // POST: Complaint/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Complaint complaintNew)
        {
            var findComplaint = db.Complaints.Find(id);
            findComplaint = complaintNew;
            db.Complaints.AddOrUpdate(findComplaint);
            var result = db.SaveChanges();
            if (result == 1)
            {
                return Redirect("/Complaint/Index");
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Complaint/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Complaint/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
