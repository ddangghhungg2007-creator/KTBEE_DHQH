
using System;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using KTBEE_DHQH.Models;

namespace KTBEE_DHQH.Controllers
{
    public class HomeController : Controller
    {
        // Trang chủ và danh sách sản phẩm
        public ActionResult Index(string tuKhoa, int? categoryId)
        {
            using (var db = new KTBECKEntities())
            {
                // Lấy danh sách sản phẩm kèm danh mục
                var products = db.Products
                    .Include(p => p.Category)
                    .AsQueryable();

                // Tìm kiếm theo tên sản phẩm
                if (!String.IsNullOrEmpty(tuKhoa))
                {
                    products = products.Where(p =>
                        p.ProductName.Contains(tuKhoa));
                }

                // Lọc theo danh mục
                if (categoryId.HasValue)
                {
                    products = products.Where(p =>
                        p.CategoryID == categoryId.Value);
                }

                // Lấy danh mục từ database
                ViewBag.Categories = db.Categories
                    .OrderBy(c => c.CategoryID)
                    .ToList();

                ViewBag.TuKhoa = tuKhoa;
                ViewBag.CategoryId = categoryId;

                // Sản phẩm mới nhất hiển thị trước
                var danhSachSanPham = products
                    .OrderByDescending(p => p.ProductID)
                    .ToList();

                return View(danhSachSanPham);
            }
        }

        // Trang chi tiết sản phẩm
        public ActionResult Details(int id)
        {
            using (var db = new KTBECKEntities())
            {
                var product = db.Products
                    .Include(p => p.Category)
                    .FirstOrDefault(p => p.ProductID == id);

                if (product == null)
                {
                    return HttpNotFound();
                }

                return View(product);
            }
        }
    }
}
