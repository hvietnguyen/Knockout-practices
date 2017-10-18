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
    public class ProductSoldsController : Controller
    {
        private Keys_Onboarding_KoContext db = new Keys_Onboarding_KoContext();

        // GET: ProductSolds
        public ActionResult Index()
        {
            var productSolds = db.ProductSolds.Include(p => p.Customer).Include(p => p.Product).Include(p => p.Store);
            return View(productSolds.ToList());
        }

        public JsonResult GetSales()
        {
            var sales = from s in db.ProductSolds
                        join c in db.Customers on s.CustomerId equals c.Id
                        join p in db.Products on s.ProductId equals p.Id
                        join st in db.Stores on s.StoreId equals st.Id
                        select new
                        {
                            Id = s.Id,
                            CustomerId = c.Id,
                            CustomerName = c.Name,
                            CustomerAddress = c.Address,
                            ProductId = p.Id,
                            ProductName = p.Name,
                            Price = p.Price,
                            StoreId = st.Id,
                            StoreName = st.Name,
                            StoreAddress = st.Address,
                            Date = s.DateSold
                        };

            var result = sales.ToList().Select(s => new
            {
                Id = s.Id,
                Customer = new {Id=s.CustomerId, Name=s.CustomerName, Address=s.CustomerAddress },
                Product = new {Id=s.ProductId, Name=s.ProductName, Price=s.Price},
                Store = new {Id=s.StoreId, Name=s.StoreName, Address = s.StoreAddress},
                Date = s.Date.ToString("yyyy-MM-dd")
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: ProductSolds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return Json(new HttpStatusCodeResult(HttpStatusCode.BadRequest), JsonRequestBehavior.AllowGet);
            }
            var sales = from s in db.ProductSolds
                        join c in db.Customers on s.CustomerId equals c.Id
                        join p in db.Products on s.ProductId equals p.Id
                        join st in db.Stores on s.StoreId equals st.Id
                        where s.Id == id
                        select new
                        {
                            Id = s.Id,
                            CustomerId = c.Id,
                            CustomerName = c.Name,
                            CustomerAddress = c.Address,
                            ProductId = p.Id,
                            ProductName = p.Name,
                            Price = p.Price,
                            StoreId = st.Id,
                            StoreName = st.Name,
                            StoreAddress = st.Address,
                            Date = s.DateSold
                        };

            var productSold = sales.ToList().Select(s => new
            {
                Id = s.Id,
                Customer = new { Id = s.CustomerId, Name = s.CustomerName, Address = s.CustomerAddress },
                Product = new { Id = s.ProductId, Name = s.ProductName, Price = s.Price },
                Store = new { Id = s.StoreId, Name = s.StoreName, Address = s.StoreAddress },
                Date = s.Date.ToString("yyyy-MM-dd")
            }).FirstOrDefault();
            if (productSold == null)
            {
                return Json(HttpNotFound(),JsonRequestBehavior.AllowGet);
            }
            return Json(productSold, JsonRequestBehavior.AllowGet);
        }

        // GET: ProductSolds/Create
        public ActionResult Create()
        {
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name");
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name");
            return View();
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
        public ActionResult CreateNew([Bind(Include = "ProductId,CustomerId,StoreId,DateSold")] ProductSold productSold)
        {
            if (ModelState.IsValid)
            {
                db.ProductSolds.Add(productSold);
                db.SaveChanges();
                var sales = from s in db.ProductSolds
                            join c in db.Customers on s.CustomerId equals c.Id
                            join p in db.Products on s.ProductId equals p.Id
                            join st in db.Stores on s.StoreId equals st.Id
                            select new
                            {
                                Id = s.Id,
                                CustomerId = c.Id,
                                CustomerName = c.Name,
                                CustomerAddress = c.Address,
                                ProductId = p.Id,
                                ProductName = p.Name,
                                Price = p.Price,
                                StoreId = st.Id,
                                StoreName = st.Name,
                                StoreAddress = st.Address,
                                Date = s.DateSold
                            };

                var newProductSold = sales.ToList().Select(s => new
                {
                    Id = s.Id,
                    Customer = new { Id = s.CustomerId, Name = s.CustomerName, Address = s.CustomerAddress },
                    Product = new { Id = s.ProductId, Name = s.ProductName, Price = s.Price },
                    Store = new { Id = s.StoreId, Name = s.StoreName, Address = s.StoreAddress },
                    Date = s.Date.ToString("yyyy-MM-dd")
                }).Last();

                return Json(newProductSold, JsonRequestBehavior.DenyGet);
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
            return View(productSold);
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
               
            }
            
            return Json(new { error=true, message="Can not save"}, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]

        public ActionResult SaveEdit([Bind(Include = "Id,ProductId,CustomerId,StoreId,DateSold")] ProductSold productSold)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productSold).State = EntityState.Modified;
                db.SaveChanges();
                return Json(productSold, JsonRequestBehavior.DenyGet);
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
            return View(productSold);
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
        public ActionResult DeleteConfirm(int id)
        {
            ProductSold productSold = db.ProductSolds.Find(id);
            db.ProductSolds.Remove(productSold);
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
