using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DataTables.AspNetCore.Mvc.Binder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voluntary.App.Data;
using Voluntary.App.Data.Entities;
using Voluntary.App.Helpers;
using Voluntary.App.Models;

namespace Voluntary.App.Controllers
{
    public class DistrictsController : BaseController
    {
        public DistrictsController(ApplicationDbContext context) : base(context) { }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetList([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var total = Context.Districts.Count();
            int recordsFilterd = total;

            IQueryable<District> query = Context.Districts;
            if (!string.IsNullOrEmpty(dataRequest.Search?.Value))
            {
                var term = dataRequest.Search.Value;
                query = query
                    .Where(x => x.Name.Contains(term)
                                || x.NameAr.Contains(term)
                                || x.Street.Contains(term)
                                || x.City.Contains(term)
                                || x.Department.Contains(term)
                                //|| x.BirthDate.ToString(CultureInfo.InvariantCulture).Contains(term)
                                || x.ZipCode.Contains(term));
                recordsFilterd = query.Count();
            }
            foreach (var order in dataRequest.Orders)
                query = query.OrderBy(String.Concat(
                    LinqHelper.GetPropertyNameByIndex<DistrictQueryViewModel>(order.Column),
                    order.Dir == "asc" ? "" : " descending"));

            var data = await query.Skip(dataRequest.Start)
                // ReSharper disable once TooManyChainedReferences
                .Take(dataRequest.Length).Select(x => new DistrictQueryViewModel
                    {
                        Id = x.Id.ToString(),
                        Name = x.Name,
                        NameAr = x.NameAr,
                        City = x.City,
                        Department = x.Department,
                        ZipCode = x.ZipCode
                    }
                ).ToListAsync().ConfigureAwait(false);
            var result = new DatatablesQueryModel<DistrictQueryViewModel>
            {
                Data = data,
                RecordsTotal = total,
                RecordsFilterd = recordsFilterd

            };
            return Json(result.Data.ToDataTablesResponse(dataRequest, result.RecordsTotal, result.RecordsFilterd));

        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Create(AddDistrictViewModel model)
        {
            if (ModelState.IsValid)
            {
                await Context.Districts.AddAsync(new District
                {
                    Name = model.Name,
                    NameAr = model.NameAr,
                    City = model.City,
                    Department = model.Department,
                    ZipCode = model.ZipCode,
                    Street = model.Street

                }).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("Index", "Volunteers");
            }

            return View(model);
        }
        public async Task<IActionResult> Delete(Guid id)

        {

            var item = await Context.Districts.FindAsync(id).ConfigureAwait(false);
            if (item != null)
            {
                Context.Districts.Remove(item);
                await Context.SaveChangesAsync().ConfigureAwait(false);
            }

            return Json("");
        }

    }
}