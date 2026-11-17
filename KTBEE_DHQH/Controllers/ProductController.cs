using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data.Entity;
using System.Web.Mvc;
using KTBEE_DHQH.Models;

namespace KTBEE_DHQH.Controllers
{
    public class ProductController : Controller
    {
        private KTBECKEntities db = new KTBECKEntities();

        public ActionResult Details(int id)
        {
            var sanPham = db.Products
                .FirstOrDefault(x => x.ProductID == id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            return View(sanPham);
        }
    }
}