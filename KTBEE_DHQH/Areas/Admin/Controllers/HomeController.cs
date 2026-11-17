using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace KTBEE_DHQH.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private KTBEE_DHQH.Models.KTBECKEntities db =new KTBEE_DHQH.Models.KTBECKEntities();

        public ActionResult Index()
        {
            ViewBag.TongDonHang = db.Orders.Count();

            ViewBag.TongKhachHang = db.Customers.Count();

            ViewBag.TongSanPham = db.Products.Count();

            ViewBag.DonDaGiao = db.Orders.Count(
                x => x.OrderStatus == "Đã giao"
            );

            ViewBag.DoanhThu = db.Orders
                .Where(x => x.OrderStatus == "Đã giao")
                .Select(x => (decimal?)x.TotalAmount)
                .DefaultIfEmpty(0)
                .Sum();

            return View();
        }
    }
}