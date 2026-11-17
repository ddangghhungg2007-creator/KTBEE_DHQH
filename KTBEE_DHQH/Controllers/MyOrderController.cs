using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data.Entity;
using System.Web.Mvc;
using KTBEE_DHQH.Models;

namespace KTBEE_DHQH.Controllers
{
    public class MyOrderController : Controller
    {
        private KTBECKEntities db = new KTBECKEntities();

        // Lấy Username từ Session
        private string LayUsername()
        {
            object user = Session["User"];

            if (user == null)
            {
                return null;
            }

            // Trường hợp Session lưu chuỗi Username
            if (user is string)
            {
                return user.ToString();
            }

            // Trường hợp Session lưu đối tượng User
            User taiKhoan = user as User;

            if (taiKhoan != null)
            {
                return taiKhoan.Username;
            }

            // Trường hợp Session lưu một đối tượng có thuộc tính Username
            PropertyInfo property = user.GetType().GetProperty("Username");

            if (property != null)
            {
                object value = property.GetValue(user, null);

                if (value != null)
                {
                    return value.ToString();
                }
            }

            return null;
        }

        // GET: MyOrder

        public ActionResult Index(string tuKhoa, string trangThai)
        {
            Customer khachHang = null;

            // 1. Ưu tiên lấy khách hàng mua không cần đăng nhập
            if (Session["GuestCustomerID"] != null)
            {
                int customerID = Convert.ToInt32(
                    Session["GuestCustomerID"]
                );

                khachHang = db.Customers.FirstOrDefault(
                    x => x.CustomerID == customerID
                );
            }

            // 2. Nếu không phải khách vãng lai thì kiểm tra tài khoản đăng nhập
            if (khachHang == null)
            {
                string username = LayUsername();

                if (!String.IsNullOrEmpty(username))
                {
                    khachHang = db.Customers.FirstOrDefault(
                        x => x.Username == username
                    );
                }
            }

            // 3. Không tìm thấy khách hàng
            //if (khachHang == null)
            //{
            //    ViewBag.CoDonHang = false;

            //    return View("MyOrder", new List<Order>());
            //}

            // 4. Lấy đơn hàng của khách hàng
            ViewBag.CoDonHang = db.Orders.Any();

            var danhSach = db.Orders
            .Include(x => x.Customer)
            .Include(x => x.OrderDetails.Select(ct => ct.Product))
            .OrderByDescending(x => x.OrderDate)
            .AsQueryable();

            // 5. Tìm theo mã đơn hàng hoặc tên sản phẩm
            if (!String.IsNullOrEmpty(tuKhoa))
            {
                int maDon;
                bool laMaDon = int.TryParse(tuKhoa, out maDon);

                danhSach = danhSach.Where(x =>
                    (laMaDon && x.OrderID == maDon)
                    ||
                    x.OrderDetails.Any(ct =>
                        ct.Product.ProductName.Contains(tuKhoa)
                    )
                );
            }

            // 6. Lọc theo trạng thái
            if (!String.IsNullOrEmpty(trangThai)
                && trangThai != "Tất cả")
            {
                danhSach = danhSach.Where(
                    x => x.OrderStatus == trangThai
                );
            }

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TrangThai = trangThai ?? "Tất cả";

            return View("MyOrder", danhSach.ToList());
        }


        // Mua lại sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuyAgain(int productID)
        {
            var sanPham = db.Products.Find(productID);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            // Lấy giỏ hàng từ Session
            List<CartItem> gioHang =
                Session["Cart"] as List<CartItem>;

            if (gioHang == null)
            {
                gioHang = new List<CartItem>();
            }

            // Mua lại với số lượng 1
            var item = gioHang.FirstOrDefault(
                x => x.ProductID == productID
            );

            if (item != null)
            {
                item.Quantity += 1;
            }
            else
            {
                gioHang.Add(new CartItem
                {
                    ProductID = sanPham.ProductID,
                    ProductName = sanPham.ProductName,
                    ProductImage = sanPham.ProductImage,
                    ProductPrice = sanPham.ProductPrice,
                    Quantity = 1
                });
            }

            Session["Cart"] = gioHang;

            TempData["ThongBao"] =
                "Đã thêm sản phẩm vào giỏ hàng!";

            return RedirectToAction("Index");

    
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