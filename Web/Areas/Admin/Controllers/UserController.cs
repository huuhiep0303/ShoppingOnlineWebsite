using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/User")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;
        public UserController(DataContext dataContext, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = dataContext;
        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var usersWithRoles = await (from u in _dataContext.Users
                                        join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                        join r in _dataContext.Roles on ur.RoleId equals r.Id
                                        select new { User = u, RoleName = r.Name }).ToListAsync();
            return View(usersWithRoles);
        }
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (createUserResult.Succeeded)
                {
                    var createUser = await _userManager.FindByEmailAsync(user.Email);
                    var userID = createUser.Id;
                    var role = await _roleManager.FindByIdAsync(user.RoleID);
                    if (role != null)
                    {
                        var addRoleResult = await _userManager.AddToRoleAsync(createUser, role.Name);
                        if (!addRoleResult.Succeeded)
                        {
                            AddIdentityErrors(addRoleResult);
                            return View(user);
                        }
                        return RedirectToAction("Index", "User", new { area = "Admin" });
                    }
                    return RedirectToAction("Index", "User");
                } else
                {
                    AddIdentityErrors(createUserResult);
                    return View(user);
                }
            }
            TempData["error"] = "Model has error.";
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            string errorMsg = string.Join("\n", errors);
            return View(user);
        }
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string ID)
        {
            if (string.IsNullOrEmpty(ID))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(ID);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(AppUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);
            if (existingUser == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.RoleID = user.RoleID;

                var updateUserResult = await _userManager.UpdateAsync(existingUser);
                if (updateUserResult.Succeeded)
                {
                    return RedirectToAction("Index", "User", new { area = "Admin" });
                } else
                {
                    AddIdentityErrors(updateUserResult);
                    return View(existingUser);
                }
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            TempData["error"] = "Model validation failed";
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            string errorMsg = string.Join("\n", errors);
            return View(existingUser);
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string ID)
        {
            if (string.IsNullOrEmpty(ID))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(ID);
            if (user == null)
            {
                return NotFound();
            }
            var deleteUser = await _userManager.DeleteAsync(user);
            if (!deleteUser.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "Deleted user successfully";
            return RedirectToAction("Index");
        }
        public void AddIdentityErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
