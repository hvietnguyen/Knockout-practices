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
    public class StoresController : Controller
    {
        private Keys_Onboarding_KoContext db = new Keys_Onboarding_KoContext();

        // GET: Stores
        public ActionResult Index()
        {
            return View(db.Stores.ToList());
        }

        public JsonResult GetStores()
        {
            var result = from s in db.Stores
                         select new
                         {
                             Id = s.Id,
                             Name = s.Name,
                             Address = s.Address
                         };
            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Stores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // GET: Stores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address")] Store store)
        {
            if (ModelState.IsValid)
            {
                db.Stores.Add(store);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(store);
        }

        [HttpPost]
        public ActionResult CreateNew([Bind(Include = "Name,Address")] Store store)
        {
            if (ModelState.IsValid)
            {
                string name = store.Name;
                string address = store.Address;
                var duplicatedCustomer = (from c in db.Stores
                                          where c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                                c.Address.Equals(address, StringComparison.OrdinalIgnoreCase)
                                          select new { Id = c.Id }).Count();
                if (duplicatedCustomer > 0)
                {
                    return Json(new { error = true, message = "Duplicated Store" }, JsonRequestBehavior.DenyGet);
                }
                db.Stores.Add(store);
                db.SaveChanges();
                return Json(store, JsonRequestBehavior.DenyGet);
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

        // GET: Stores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address")] Store store)
        {
            if (ModelState.IsValid)
            {
                db.Entry(store).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(store);
        }

        [HttpPost]
        public ActionResult SaveEdit([Bind(Include = "Id,Name,Address")] Store store)
        {
            if (ModelState.IsValid)
            {
                string name = store.Name;
                string address = store.Address;
                var duplicatedCustomer = (from c in db.Stores
                                          where c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                                c.Address.Equals(address, StringComparison.OrdinalIgnoreCase)
                                          select new { Id = c.Id }).Count();
                if (duplicatedCustomer > 0)
                {
                    return Json(new { error = true, message = "Duplicated Store" }, JsonRequestBehavior.DenyGet);
                }
                db.Entry(store).State = EntityState.Modified;
                db.SaveChanges();
                return Json(store, JsonRequestBehavior.DenyGet);
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

        // GET: Stores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Store store = db.Stores.Find(id);
            db.Stores.Remove(store);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteConfirm(int id)
        {
            Store store = db.Stores.Find(id);
            db.Stores.Remove(store);
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
