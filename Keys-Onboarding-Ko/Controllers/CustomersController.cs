using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Keys_Onboarding_Ko.Models;

namespace Keys_Onboarding_Ko.Controllers
{
    public class CustomersController : Controller
    {
        private Keys_Onboarding_KoContext db = new Keys_Onboarding_KoContext();

        // GET: Customers
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        // GET: Customer/GetCustomers
        public JsonResult GetCustomers()
        {
            var list = from c in db.Customers
                       select new
                       {
                           Id=c.Id,
                           Name = c.Name,
                           Address = c.Address
                       };
            return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Customer/GetCustomer/id
        public JsonResult GetCustomer(int? id)
        {
            if (id == null)
            {
                return Json(new HttpStatusCodeResult(HttpStatusCode.BadRequest), JsonRequestBehavior.AllowGet);
            }
            Customer c = db.Customers.Find(id);
            if (c == null)
            {
                return Json(HttpNotFound(), JsonRequestBehavior.AllowGet);
            }
            return Json(c, JsonRequestBehavior.AllowGet);
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return PartialView();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return PartialView(customer);
        }

        [HttpPost]
        public JsonResult CreateNew([Bind(Include = "Name,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                string name = customer.Name;
                string address = customer.Address;
                var duplicatedCustomer = (from c in db.Customers
                                          where c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                                c.Address.Equals(address, StringComparison.OrdinalIgnoreCase)
                                          select new { Id = c.Id }).Count();
                if (duplicatedCustomer > 0)
                {
                    return Json(new { error = true, message = "Duplicated Customer" }, JsonRequestBehavior.DenyGet);
                }
                db.Customers.Add(customer);
                db.SaveChanges();
                var newCustomer = db.Customers.ToList().Last();
                return Json(newCustomer, JsonRequestBehavior.DenyGet);
            }
            var errors = new List<string>();
            foreach (var modelState in ModelState.Values)
            {
                foreach (var modelError in modelState.Errors)
                {
                    errors.Add(modelError.ErrorMessage);
                }
            }
            return Json(new { error = true, message = errors[0] }, JsonRequestBehavior.DenyGet);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return PartialView(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
               
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        [HttpPost]
        public ActionResult SaveEdit([Bind(Include = "Id,Name,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                string name = customer.Name;
                string address = customer.Address;
                var duplicatedCustomer = (from c in db.Customers
                                         where c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                               c.Address.Equals(address, StringComparison.OrdinalIgnoreCase)
                                         select new { Id = c.Id }).Count();
                if (duplicatedCustomer > 0)
                {
                    return Json(new { error = true, message = "Duplicated Customer" }, JsonRequestBehavior.DenyGet);
                }

                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return Json(customer, JsonRequestBehavior.DenyGet);
            }
            var errors = new List<string>();
            foreach (var modelState in ModelState.Values)
            {
                foreach (var modelError in modelState.Errors)
                {
                    errors.Add(modelError.ErrorMessage);
                }
            }
            return Json(new { error = true, message = errors[0] }, JsonRequestBehavior.DenyGet);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return PartialView(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult DeleteConfirm(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return Json("Successful", JsonRequestBehavior.DenyGet);
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
