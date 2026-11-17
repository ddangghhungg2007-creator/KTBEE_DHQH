
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using KTBEE_DHQH.Models;

namespace KTBEE_DHQH.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private KTBECKEntities db = new KTBECKEntities();

        // Kiểm tra đã có Admin chưa
        private bool DaCoAdmin()
        {
            return db.Users.Any(x => x.UserRole.Trim() == "A");
        }

        // GET: Admin/Account/Register
        public ActionResult Register()
        {
            // Chỉ cho đăng ký Admin đầu tiên
            if (DaCoAdmin())
                return RedirectToAction("Login");

            return View();
        }

        // POST: Admin/Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(
            string username,
            string password,
            string confirmPassword)
        {
            if (DaCoAdmin())
                return RedirectToAction("Login");

            username = (username ?? "").Trim();

            if (username == "" || password == "" || confirmPassword == "")
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (username.Length > 255)
            {
                ViewBag.Error = "Tên tài khoản tối đa 255 ký tự.";
                return View();
            }

            if (password.Length < 8 || password.Length > 40)
            {
                ViewBag.Error = "Mật khẩu phải từ 8 đến 40 ký tự.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            if (db.Users.Any(x => x.Username == username))
            {
                ViewBag.Error = "Tên tài khoản đã tồn tại.";
                return View();
            }

            // Kiểm tra lại trong lúc đăng ký
            if (DaCoAdmin())
                return RedirectToAction("Login");

            User admin = new User();

            admin.Username = username;
            admin.Password = BamMatKhau(password);
            admin.UserRole = "A";

            db.Users.Add(admin);
            db.SaveChanges();

            return RedirectToAction("Login");
        }

        // GET: Admin/Account/Login
        public ActionResult Login()
        {
            if (Session["AdminUsername"] != null &&
                Session["AdminRole"] != null &&
                Session["AdminRole"].ToString() == "A")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.DaCoAdmin = DaCoAdmin();

            return View();
        }

        // POST: Admin/Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            username = (username ?? "").Trim();

            if (username == "" || password == null || password == "")
            {
                ViewBag.Error = "Vui lòng nhập tên tài khoản và mật khẩu.";
                ViewBag.DaCoAdmin = DaCoAdmin();
                return View();
            }

            User admin = db.Users.FirstOrDefault(x =>
                x.Username == username &&
                x.UserRole.Trim() == "A");

            if (admin == null || !KiemTraMatKhau(password, admin.Password))
            {
                ViewBag.Error = "Tên tài khoản hoặc mật khẩu không đúng.";
                ViewBag.DaCoAdmin = DaCoAdmin();
                return View();
            }

            Session.Clear();

            Session["AdminUsername"] = admin.Username;
            Session["AdminRole"] = "A";

            return RedirectToAction("Index", "Home");
        }

        // POST: Admin/Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Remove("AdminUsername");
            Session.Remove("AdminRole");

            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login");
        }

        // Băm mật khẩu PBKDF2
        private string BamMatKhau(string password)
        {
            byte[] salt = new byte[10];

            using (RNGCryptoServiceProvider rng =
                new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            using (Rfc2898DeriveBytes pbkdf2 =
                new Rfc2898DeriveBytes(password, salt, 200000))
            {
                byte[] hash = pbkdf2.GetBytes(24);

                // Tổng độ dài 49 ký tự, phù hợp Password NCHAR(50)
                return Convert.ToBase64String(salt)
                    + "."
                    + Convert.ToBase64String(hash);
            }
        }

        // Kiểm tra mật khẩu
        private bool KiemTraMatKhau(string password, string matKhauDaLuu)
        {
            try
            {
                matKhauDaLuu = (matKhauDaLuu ?? "").Trim();

                string[] parts = matKhauDaLuu.Split('.');

                if (parts.Length != 2)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] hashCu = Convert.FromBase64String(parts[1]);

                using (Rfc2898DeriveBytes pbkdf2 =
                    new Rfc2898DeriveBytes(password, salt, 200000))
                {
                    byte[] hashMoi = pbkdf2.GetBytes(24);

                    if (hashCu.Length != hashMoi.Length)
                        return false;

                    int saiKhac = 0;

                    for (int i = 0; i < hashCu.Length; i++)
                    {
                        saiKhac |= hashCu[i] ^ hashMoi[i];
                    }

                    return saiKhac == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
