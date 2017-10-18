using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Keys_Onboarding_Ajax.Models;

namespace Keys_Onboarding_Ajax.Controllers
{
    public class StoresController : Controller
    {
        private Keys_Onboarding_AjaxContext db = new Keys_Onboarding_AjaxContext();

        // GET: Stores
        public ActionResult Index()
        {
            return View(db.Stores.ToList());
        }

        public JsonResult GetStores()
        {
            var list = from st in db.Stores
                       select new
                       {
                           Id=st.Id,
                           Name=st.Name,
                           Address=st.Address,
                       };
            return Json(list.ToList(), JsonRequestBehavior.AllowGet);
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
            return PartialView();
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

            return PartialView(store);
        }

        [HttpPost]
        public JsonResult CreateNew([Bind(Include = "Name,Address")] Store store)
        {
            if (ModelState.IsValid)
            {
                db.Stores.Add(store);
                db.SaveChanges();
                var newStore = db.Stores.ToList().Last();
                return Json(newStore, JsonRequestBehavior.DenyGet);
            }

            return Json("Error", JsonRequestBehavior.DenyGet);
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
            return PartialView(store);
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
            return PartialView(store);
        }

        [HttpPost]
        public JsonResult EditStore([Bind(Include = "Id,Name,Address")] Store Store)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Store).State = EntityState.Modified;
                db.SaveChanges();
                var list = db.Stores.ToList();
                return Json(list, JsonRequestBehavior.DenyGet);
            }
            return Json("Error", JsonRequestBehavior.DenyGet);
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
            return PartialView(store);
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
        public JsonResult DeleteStore(int id)
        {
            Store store = db.Stores.Find(id);
            db.Stores.Remove(store);
            db.SaveChanges();
            return Json(db.Stores.ToList(), JsonRequestBehavior.DenyGet);
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
