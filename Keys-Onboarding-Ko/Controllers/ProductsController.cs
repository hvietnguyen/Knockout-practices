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
    public class ProductsController : Controller
    {
        private Keys_Onboarding_KoContext db = new Keys_Onboarding_KoContext();

        // GET: Products
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        [HttpGet]
        public JsonResult GetProducts()
        {
            var result = from p in db.Products
                         select new
                         {
                             Id = p.Id,
                             Name = p.Name,
                             Price = p.Price
                         };
            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        [HttpPost]
        public ActionResult CreateNew([Bind(Include = "Name,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                string name = product.Name;
                var duplicatedProduct = (from p in db.Products
                                         where p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                                         select new { Id = p.Id }).Count();
                if(duplicatedProduct > 0)
                {
                    return Json(new { error = true, message = "Duplicated Product" }, JsonRequestBehavior.DenyGet);
                }

                db.Products.Add(product);
                db.SaveChanges();
                return Json(product, JsonRequestBehavior.DenyGet);
            }
            var errors = new List<string>();
            foreach(var modelState in ModelState.Values)
            {
                foreach (var modelError in modelState.Errors)
                {
                    errors.Add(modelError.ErrorMessage);
                }
            }
            return Json(new { error = true, message = errors[0] }, JsonRequestBehavior.DenyGet);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult SaveEdit([Bind(Include = "Id,Name,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                string name = product.Name;
                var price = product.Price;
                var duplicatedProduct = (from p in db.Products
                                         where p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                               p.Price.Equals(price)
                                         select new { Id = p.Id }).Count();
                if (duplicatedProduct > 0)
                {
                    return Json(new { error = true, message = "Duplicated Product" }, JsonRequestBehavior.DenyGet);
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return Json(product, JsonRequestBehavior.DenyGet);
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

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteConfirm(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
