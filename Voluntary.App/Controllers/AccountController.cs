using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DataTables.AspNetCore.Mvc.Binder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Voluntary.App.Data;
using Voluntary.App.Helpers;
using Voluntary.App.Models;

namespace Voluntary.App.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;
        // private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            /*IEmailSender emailSender,*/
            ILogger<AccountController> logger,
            UserManager<IdentityUser> userManager
            , ApplicationDbContext context): base(context)
        {
            _signInManager = signInManager;
            //_emailSender = emailSender;
            _logger = logger;
            _userManager = userManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync().ConfigureAwait(false);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync().ConfigureAwait(false);
            return RedirectToAction("Login");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe,
                    lockoutOnFailure: false).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new
                    {
                        ReturnUrl = returnUrl,
                        model.RememberMe
                    });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }

            }
            return View(model);
        }
        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetList([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var total = Context.Users.Count();
            int recordsFilterd = total;

            var query = (from user in Context.Users join userRole in Context.UserRoles on user.Id equals userRole.UserId
                join role in Context.Roles on userRole.RoleId equals role.Id 
                select new {user.Email, user.UserName, user.Id, user.PhoneNumber,RolaName= role.Name});
            if (!string.IsNullOrEmpty(dataRequest.Search?.Value))
            {
                var term = dataRequest.Search.Value;
                query = query
                    .Where(x => x.UserName.Contains(term)
                                || x.RolaName.Contains(term)
                                || x.Email.Contains(term)
                                || x.PhoneNumber.Contains(term)
                               );
                recordsFilterd = query.Count();
            }
            foreach (var order in dataRequest.Orders)
                query = query.OrderBy(String.Concat(
                    LinqHelper.GetPropertyNameByIndex<AccountQueryViewModel>(order.Column),
                    order.Dir == "asc" ? "" : " descending"));

            var data = await query.Skip(dataRequest.Start)
                // ReSharper disable once TooManyChainedReferences
                .Take(dataRequest.Length).Select(x => new AccountQueryViewModel
                {
                    Id = x.Id.ToString(),
                    UserName = x.UserName,
                    Email = x.Email,
                    Role = x.RolaName,
                    Phone = x.PhoneNumber
                }
                ).ToListAsync().ConfigureAwait(false);
            var result = new DatatablesQueryModel<AccountQueryViewModel>
            {
                Data = data,
                RecordsTotal = total,
                RecordsFilterd = recordsFilterd

            };
            return Json(result.Data.ToDataTablesResponse(dataRequest, result.RecordsTotal, result.RecordsFilterd));

        }
        [Authorize(Roles = "admin")]
        public async Task <IActionResult> Delete(Guid id)
        {
            var currentUser = await _userManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            if (currentUser == null)
                Json("User not found");
            await _userManager.DeleteAsync(currentUser).ConfigureAwait(false);
            return Json("Item removed  successfully");
        }
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            var dicRoles = GetDicRoles();
            ViewData["roles"] = new SelectList(dicRoles, "Key", "Value",null);

            return View();
        }

        private static Dictionary<string, string> GetDicRoles()
        {
            var dicRoles = new Dictionary<string, string>();
            dicRoles.Add("admin", "admin");
            dicRoles.Add("user", "Agent de saisi");
            dicRoles.Add("guest", "guest");
            return dicRoles;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Create(AddOrUpdateAccount model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    var currentUser = await _userManager.FindByNameAsync(user.UserName).ConfigureAwait(false);
                    await _userManager.AddToRoleAsync(currentUser, model.RoleName).ConfigureAwait(false);
                }

                return View("Index");
            }
            ViewData["roles"] = new SelectList(GetDicRoles(), "Key", "Value", model.RoleName);

            return View(model);
        }

    }
}