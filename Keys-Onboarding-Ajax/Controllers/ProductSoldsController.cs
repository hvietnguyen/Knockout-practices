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
    public class ProductSoldsController : Controller
    {
        private Keys_Onboarding_AjaxContext db = new Keys_Onboarding_AjaxContext();

        // GET: ProductSolds
        public ActionResult Index()
        {
            var productSolds = db.ProductSolds.Include(p => p.Customer).Include(p => p.Product).Include(p => p.Store);
            return View(productSolds.ToList());
        }

        public JsonResult GetProductSolds()
        {
            var productSolds = from s in db.ProductSolds
                               join p in db.Products on s.ProductId equals p.Id
                               join c in db.Customers on s.CustomerId equals c.Id
                               join st in db.Stores on s.StoreId equals st.Id
                               select new
                               {
                                   Id = s.Id,
                                   ProductId = p.Id,
                                   ProductName = p.Name,
                                   CustomerId = c.Id,
                                   CustomerName = c.Name,
                                   StoreId = st.Id,
                                   StoreName = st.Name,
                                   Date = s.DateSold
                               };
            var result = productSolds.ToList().Select(s => new
            {
                Id = s.Id,
                ProductId = s.ProductId,
                ProductName = s.ProductName,
                CustomerId = s.CustomerId,
                CustomerName = s.CustomerName,
                StoreId = s.StoreId,
                StoreName = s.StoreName,
                Date = s.Date.ToString("yyyy-MM-dd")
            });


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: ProductSolds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSold productSold = db.ProductSolds.Find(id);
            if (productSold == null)
            {
                return HttpNotFound();
            }
            return View(productSold);
        }

        // GET: ProductSolds/Create
        public ActionResult Create()
        {
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name");
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name");
            return PartialView();
        }

        // POST: ProductSolds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductId,CustomerId,StoreId,DateSold")] ProductSold productSold)
        {
            if (ModelState.IsValid)
            {
                db.ProductSolds.Add(productSold);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", productSold.CustomerId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", productSold.ProductId);
            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name", productSold.StoreId);
            return View(productSold);
        }

        [HttpPost]
        public JsonResult CreateNew([Bind(Include = "ProductId,CustomerId,StoreId, DateSold")] ProductSold productSold)
        {
            if (ModelState.IsValid)
            {
                db.ProductSolds.Add(productSold);
                db.SaveChanges();
                var productSolds = from s in db.ProductSolds
                                   join p in db.Products on s.ProductId equals p.Id
                                   join c in db.Customers on s.CustomerId equals c.Id
                                   join st in db.Stores on s.StoreId equals st.Id
                                   select new
                                   {
                                       Id = s.Id,
                                       ProductId = p.Id,
                                       ProductName = p.Name,
                                       CustomerId = c.Id,
                                       CustomerName = c.Name,
                                       StoreId = st.Id,
                                       StoreName = st.Name,
                                       Date = s.DateSold
                                   };
                var result = productSolds.ToList().Select(s => new
                {
                    Id = s.Id,
                    ProductId = s.ProductId,
                    ProductName = s.ProductName,
                    CustomerId = s.CustomerId,
                    CustomerName = s.CustomerName,
                    StoreId = s.StoreId,
                    StoreName = s.StoreName,
                    Date = s.Date.ToString("yyyy-MM-dd")
                }).Last();
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            return Json("Error", JsonRequestBehavior.DenyGet);
        }

        // GET: ProductSolds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSold productSold = db.ProductSolds.Find(id);
            if (productSold == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", productSold.CustomerId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", productSold.ProductId);
            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name", productSold.StoreId);
            return PartialView(productSold);
        }

        // POST: ProductSolds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductId,CustomerId,StoreId,DateSold")] ProductSold productSold)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productSold).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", productSold.CustomerId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", productSold.ProductId);
            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name", productSold.StoreId);
            return View(productSold);
        }

        [HttpPost]
        public JsonResult EditSale([Bind(Include = "Id,ProductId,CustomerId,StoreId,DateSold")] ProductSold productSold)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productSold).State = EntityState.Modified;
                db.SaveChanges();
                var productSolds = from s in db.ProductSolds
                                   join p in db.Products on s.ProductId equals p.Id
                                   join c in db.Customers on s.CustomerId equals c.Id
                                   join st in db.Stores on s.StoreId equals st.Id
                                   select new
                                   {
                                       Id = s.Id,
                                       ProductId = p.Id,
                                       ProductName = p.Name,
                                       CustomerId = c.Id,
                                       CustomerName = c.Name,
                                       StoreId = st.Id,
                                       StoreName = st.Name,
                                       Date = s.DateSold
                                   };
                var result = productSolds.ToList().Select(s => new
                {
                    Id = s.Id,
                    ProductId = s.ProductId,
                    ProductName = s.ProductName,
                    CustomerId = s.CustomerId,
                    CustomerName = s.CustomerName,
                    StoreId = s.StoreId,
                    StoreName = s.StoreName,
                    Date = s.Date.ToString("yyyy-MM-dd")
                }).ToList();
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            return Json("Error", JsonRequestBehavior.DenyGet);
        }

        // GET: ProductSolds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSold productSold = db.ProductSolds.Find(id);
            if (productSold == null)
            {
                return HttpNotFound();
            }
            return PartialView(productSold);
        }

        // POST: ProductSolds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductSold productSold = db.ProductSolds.Find(id);
            db.ProductSolds.Remove(productSold);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult DeleteSale(int id)
        {
            ProductSold productSold = db.ProductSolds.Find(id);
            db.ProductSolds.Remove(productSold);
            db.SaveChanges();
            var productSolds = from s in db.ProductSolds
                               join p in db.Products on s.ProductId equals p.Id
                               join c in db.Customers on s.CustomerId equals c.Id
                               join st in db.Stores on s.StoreId equals st.Id
                               select new
                               {
                                   Id = s.Id,
                                   ProductId = p.Id,
                                   ProductName = p.Name,
                                   CustomerId = c.Id,
                                   CustomerName = c.Name,
                                   StoreId = st.Id,
                                   StoreName = st.Name,
                                   Date = s.DateSold
                               };
            var result = productSolds.ToList().Select(s => new
            {
                Id = s.Id,
                ProductId = s.ProductId,
                ProductName = s.ProductName,
                CustomerId = s.CustomerId,
                CustomerName = s.CustomerName,
                StoreId = s.StoreId,
                StoreName = s.StoreName,
                Date = s.Date.ToString("yyyy-MM-dd")
            }).ToList();
            return Json(result, JsonRequestBehavior.DenyGet);
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
