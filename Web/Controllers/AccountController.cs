using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Areas.Admin.Repository;
using Web.Models;
using Web.Models.ViewModels;
using Web.Repository;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly DataContext _dataContext;
        public AccountController(DataContext dataContext, IEmailSender emailSender, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager)
        {
            _dataContext = dataContext;
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login(string returnURL)
        {
            return View(new LoginViewModel { ReturnURL = returnURL });
        }
        
        public async Task<IActionResult> NewPass(AppUserModel user, string token)
        {
            var checkUser = await _userManager.Users
                .Where(u => u.Email == user.Email)
                .Where(u => u.Token == user.Token).FirstOrDefaultAsync();

            if (checkUser != null)
            {
                ViewBag.Email = checkUser.Email;
                ViewBag.Token = token;
            }
            else
            {
                TempData["error"] = "Email not found or token is not right";
                return RedirectToAction("ForgetPass", "Account");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateNewPassword(AppUserModel user, string token)
        {
            var checkUser = await _userManager.Users
                .Where(u => u.Email == user.Email)
                .Where(u => u.Token == token).FirstOrDefaultAsync();

            if (checkUser != null)
            {
                string newToken = Guid.NewGuid().ToString();

                var passwordHasher = new PasswordHasher<AppUserModel>();
                var passwordHash = passwordHasher.HashPassword(checkUser, user.PasswordHash);

                checkUser.PasswordHash = passwordHash;
                checkUser.Token = newToken;

                await _userManager.UpdateAsync(checkUser);
                TempData["success"] = "Password updated successfully";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["error"] = "Email not found or token is not right";
                return RedirectToAction("ForgetPass", "Account");
            }
            return View();
        }
        public async Task<IActionResult> ForgetPass(string returnURL)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendMailForgotPass(AppUserModel user)
        {
            var checkMail = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (checkMail == null)
            {
                TempData["error"] = "Email not found";
                return RedirectToAction("ForgetPass", "Account");
            }
            else
            {
                string token = Guid.NewGuid().ToString();
                checkMail.Token = token;
                _dataContext.Update(checkMail);
                await _dataContext.SaveChangesAsync();

                var receiver = checkMail.Email;
                var subject = "Reset Password";
                var body = "Click the link to reset your password " + "<a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass"
                    + $"?email=" + checkMail.Email + "&token=" + token + "'>";

                await _emailSender.SendEmailAsync(receiver, subject, body);
            }
            TempData["success"] = "An email has been sent to your email address";
            return RedirectToAction("ForgetPass", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(loginVM.ReturnURL ?? "/");
                }
                ModelState.AddModelError("", "Invalid username or password");
            }
            return View(loginVM);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        public async Task<IActionResult> History()
        {
            if ((bool)!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var orders = await _dataContext.Orders
                .Where(o => o.CustomerName == userEmail).OrderByDescending(o => o.Id)
                .ToListAsync();

            ViewBag.UserEmail = userEmail;
            return View(orders);
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserModel usermodel)
        {

            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(usermodel.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(usermodel);
                }
                AppUserModel newUser = new AppUserModel { UserName = usermodel.Username, Email = usermodel.Email, PhoneNumber = usermodel.PhoneNumber };
                IdentityResult result = await _userManager.CreateAsync(newUser, usermodel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, "Admin");
                    TempData["success"] = "Register successfully";
                    return Redirect("/account/login");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(usermodel);
        }
        public async Task<IActionResult> Logout(string returnURL = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnURL);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateAccount()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound();
            }

            return View(user); // truyền model cho form Razor
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAccount(AppUserModel model, string oldPassword, string newPassword, string confirmPassword)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin người dùng
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            IdentityResult updateResult = null;

            // Nếu người dùng muốn đổi mật khẩu
            if (!string.IsNullOrWhiteSpace(oldPassword) &&
                !string.IsNullOrWhiteSpace(newPassword) &&
                !string.IsNullOrWhiteSpace(confirmPassword))
            {
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                    return View(model);
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, oldPassword);
                if (!passwordCheck)
                {
                    ModelState.AddModelError("", "Mật khẩu cũ không đúng.");
                    return View(model);
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }

            // Cập nhật các thông tin khác
            updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                TempData["success"] = "Cập nhật tài khoản thành công.";
                return RedirectToAction("UpdateAccount");
            }

            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


    }

}

