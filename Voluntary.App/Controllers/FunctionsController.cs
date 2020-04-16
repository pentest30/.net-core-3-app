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
    public class FunctionsController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetList([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var total = Context.Tasks.Count();
            int recordsFilterd = total;

            IQueryable<VoluntaryTask> query = Context.Tasks;
            if (!string.IsNullOrEmpty(dataRequest.Search?.Value))
            {
                var term = dataRequest.Search.Value;
                query = query
                    .Where(x => x.Name.Contains(term));
                recordsFilterd = query.Count();
            }
            foreach (var order in dataRequest.Orders)
                query = query.OrderBy(String.Concat(
                    LinqHelper.GetPropertyNameByIndex<TaskQueryViewModel>(order.Column),
                    order.Dir == "asc" ? "" : " descending"));

            var data = await query.Skip(dataRequest.Start)
                // ReSharper disable once TooManyChainedReferences
                .Take(dataRequest.Length).Select(x => new TaskQueryViewModel
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Description = x.Description
                }
                ).ToListAsync().ConfigureAwait(false);
            var result = new DatatablesQueryModel<TaskQueryViewModel>
            {
                Data = data,
                RecordsTotal = total,
                RecordsFilterd = recordsFilterd

            };
            return Json(result.Data.ToDataTablesResponse(dataRequest, result.RecordsTotal, result.RecordsFilterd));

        }

        public FunctionsController(ApplicationDbContext context) : base(context)
        {
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Create(AddorUpdateTaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                await Context.Tasks.AddAsync(new VoluntaryTask
                {
                    Name = model.Name,
                    Description = model.Description

                }).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("Index", "Volunteers");
            }

            return View(model);
        }
    }
}