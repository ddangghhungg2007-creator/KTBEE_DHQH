using System;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web.Mvc;
using KTBEE_DHQH.Models;

namespace KTBEE_DHQH.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private KTBECKEntities db = new KTBECKEntities();

        // ==============================
        // DANH SÁCH SẢN PHẨM
        // ==============================
        public ActionResult Index(string tuKhoa)
        {
            var danhSach = db.Products
                .Include(x => x.Category)
                .AsQueryable();

            // Tìm kiếm sản phẩm
            if (!String.IsNullOrEmpty(tuKhoa))
            {
                danhSach = danhSach.Where(x =>
                    x.ProductName.Contains(tuKhoa));
            }

            ViewBag.TuKhoa = tuKhoa;

            return View(danhSach
                .OrderByDescending(x => x.ProductID)
                .ToList());
        }

        // ==============================
        // THÊM SẢN PHẨM - GET
        // ==============================
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(
                db.Categories.ToList(),
                "CategoryID",
                "CategoryName"
            );

            return View();
        }

        // ==============================
        // THÊM SẢN PHẨM - POST
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product sanPham)
        {
            // Kiểm tra tên sản phẩm
            if (String.IsNullOrWhiteSpace(sanPham.ProductName))
            {
                ModelState.AddModelError(
                    "ProductName",
                    "Vui lòng nhập tên sản phẩm."
                );
            }

            // Kiểm tra giá
            if (sanPham.ProductPrice <= 0)
            {
                ModelState.AddModelError(
                    "ProductPrice",
                    "Giá sản phẩm phải lớn hơn 0."
                );
            }

            // Kiểm tra danh mục
            bool danhMucTonTai = db.Categories.Any(
                x => x.CategoryID == sanPham.CategoryID
            );

            if (!danhMucTonTai)
            {
                ModelState.AddModelError(
                    "CategoryID",
                    "Vui lòng chọn danh mục hợp lệ."
                );
            }

            // Kiểm tra URL ảnh
            if (!KiemTraLinkAnh(sanPham.ProductImage))
            {
                ModelState.AddModelError(
                    "ProductImage",
                    "Link ảnh phải là URL bắt đầu bằng http:// hoặc https://."
                );
            }

            if (ModelState.IsValid)
            {
                db.Products.Add(sanPham);
                db.SaveChanges();

                TempData["ThongBao"] = "Thêm sản phẩm thành công!";

                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(
                db.Categories.ToList(),
                "CategoryID",
                "CategoryName",
                sanPham.CategoryID
            );

            return View(sanPham);
        }

        // ==============================
        // SỬA SẢN PHẨM - GET
        // ==============================
        public ActionResult Edit(int id)
        {
            var sanPham = db.Products.Find(id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryID = new SelectList(
                db.Categories.ToList(),
                "CategoryID",
                "CategoryName",
                sanPham.CategoryID
            );

            return View(sanPham);
        }

        // ==============================
        // SỬA SẢN PHẨM - POST
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product sanPham)
        {
            if (String.IsNullOrWhiteSpace(sanPham.ProductName))
            {
                ModelState.AddModelError(
                    "ProductName",
                    "Vui lòng nhập tên sản phẩm."
                );
            }

            if (sanPham.ProductPrice <= 0)
            {
                ModelState.AddModelError(
                    "ProductPrice",
                    "Giá sản phẩm phải lớn hơn 0."
                );
            }

            bool danhMucTonTai = db.Categories.Any(
                x => x.CategoryID == sanPham.CategoryID
            );

            if (!danhMucTonTai)
            {
                ModelState.AddModelError(
                    "CategoryID",
                    "Vui lòng chọn danh mục hợp lệ."
                );
            }

            if (!KiemTraLinkAnh(sanPham.ProductImage))
            {
                ModelState.AddModelError(
                    "ProductImage",
                    "Link ảnh không hợp lệ."
                );
            }

            if (ModelState.IsValid)
            {
                var duLieuCu = db.Products.Find(sanPham.ProductID);

                if (duLieuCu == null)
                {
                    return HttpNotFound();
                }

                duLieuCu.ProductName = sanPham.ProductName;
                duLieuCu.CategoryID = sanPham.CategoryID;
                duLieuCu.ProductDecription = sanPham.ProductDecription;
                duLieuCu.ProductPrice = sanPham.ProductPrice;
                duLieuCu.ProductImage = sanPham.ProductImage;

                db.SaveChanges();

                TempData["ThongBao"] = "Cập nhật sản phẩm thành công!";

                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(
                db.Categories.ToList(),
                "CategoryID",
                "CategoryName",
                sanPham.CategoryID
            );

            return View(sanPham);
        }

        // ==============================
        // XÓA SẢN PHẨM - GET
        // ==============================
        public ActionResult Delete(int id)
        {
            var sanPham = db.Products
                .Include(x => x.Category)
                .FirstOrDefault(x => x.ProductID == id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            return View(sanPham);
        }

        // ==============================
        // XÓA SẢN PHẨM - POST
        // ==============================
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var sanPham = db.Products.Find(id);

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            try
            {
                db.Products.Remove(sanPham);
                db.SaveChanges();

                TempData["ThongBao"] = "Xóa sản phẩm thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["Loi"] =
                    "Không thể xóa sản phẩm vì sản phẩm đã có trong đơn hàng.";
            }

            return RedirectToAction("Index");
        }

        // ==============================
        // KIỂM TRA LINK ẢNH
        // ==============================
        private bool KiemTraLinkAnh(string link)
        {
            Uri uri;

            if (String.IsNullOrWhiteSpace(link))
            {
                return true;
            }

            return Uri.TryCreate(
                link,
                UriKind.Absolute,
                out uri
            )
            && (uri.Scheme == Uri.UriSchemeHttp
                || uri.Scheme == Uri.UriSchemeHttps);
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