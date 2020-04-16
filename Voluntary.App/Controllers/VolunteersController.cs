using System;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DataTables.AspNetCore.Mvc.Binder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Voluntary.App.Data;
using Voluntary.App.Data.Entities;
using Voluntary.App.Helpers;
using Voluntary.App.Models;

namespace Voluntary.App.Controllers
{
    [Authorize]
    public class VolunteersController:BaseController
    {
        public VolunteersController(ApplicationDbContext context, IHostingEnvironment env) : base(context, env) { }
       
        public IActionResult Index()
        {
            return View("Index");
        }
        [Authorize(Roles = "admin,user")]
        public IActionResult Create()
        {
            ViewData["Districts"] = new SelectList(Context.Districts.ToList(),"Id", "Name" , null,dataGroupField:"City");
            ViewData["FUnctions"] = new SelectList(Context.Tasks.ToList(), "Id", "Name");

            return View();
        }
        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Create(AddOrUpdateVolunteerViewModel model)
        {
            if (ModelState.IsValid)
            {
               var id = await SaveVolunteer(model).ConfigureAwait(false); 
               return View("Index");
            }
            //GetSelects(model);
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> CreateAndContinue(AddOrUpdateVolunteerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var id = await SaveVolunteer(model).ConfigureAwait(false);
                var mat = Context.Volunteers.Count() + 1;
                return View("CreateVolunteerJob", new AddMoreInfoVolunteerViewModel() {Id = id, RegistrationNumber = mat.ToString("D3") });
            }

            //GetSelects(model);
            return View("Create", model);
        }
        public IActionResult CreateVolunteerJob(Guid volunteerId)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateVolunteerJob(AddMoreInfoVolunteerViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var volunteer  = await Context.Volunteers.FindAsync(model.Id).ConfigureAwait(false);
            if (volunteer == null) return View(model);
            volunteer.Region = model.Region;
            volunteer.RegistrationNumber = model.RegistrationNumber;
            volunteer.Comment = model.Comment;
            volunteer.Neighborhood = model.Neighborhood;
            volunteer.Job = model.Job;
            volunteer.Sector = model.Sector;
            volunteer.DistrictName = model.DistrictName;
            volunteer.AffectedCity = model.AffectedCity;
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return View("Index");

        }

        public async  Task<IActionResult> UpdateAdditionalInfo(Guid id)
        {
            var model = await Context.Volunteers.FindAsync(id).ConfigureAwait(false);
            var volunteer = new AddMoreInfoVolunteerViewModel();
            volunteer.Region = model.Region;
            volunteer.RegistrationNumber = model.RegistrationNumber;
            volunteer.Comment = model.Comment;
            volunteer.Neighborhood = model.Neighborhood;
            volunteer.Job = model.Job;
            volunteer.Sector = model.Sector;
            volunteer.DistrictName = model.DistrictName;
            volunteer.AffectedCity = model.AffectedCity;
            return View(volunteer);
        }
        private async Task<Guid> SaveVolunteer(AddOrUpdateVolunteerViewModel model)
        {
            var entity = new Volunteer();
            entity.FirstName = model.FirstName;
            entity.LastName = model.LastName;
            entity.FirstNameAr = model.FirstNameAr;
            entity.LastNameAr = model.LastNameAr;
            entity.BirthPlace = model.BirthPlace;
            entity.BirthDate = model.BirthDate;
            entity.Email = model.Email;
            entity.Phone = model.Phone;
            entity.CardId = model.CardId;
            entity.Address = model.Address;
            await Context.Volunteers.AddAsync(entity).ConfigureAwait(false);
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return entity.Id;
        }

        //private void GetSelects(AddOrUpdateVolunteerViewModel model)
        //{
        //    ViewData["Districts"] =
        //        new SelectList(Context.Districts.ToList(), "Id", "Name", model.DistrictId, dataGroupField: "City");
        //    ViewData["FUnctions"] = new SelectList(Context.Tasks.ToList(), "Id", "Name", model.DistrictId);
        //}
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = new AddOrUpdateVolunteerViewModel(await Context.Volunteers.FindAsync(id).ConfigureAwait(false));
            //GetSelects(model);
            return View(model);
        }

        [Authorize(Roles = "admin,user")]
        [HttpPost]
        public async Task<IActionResult> Edit(AddOrUpdateVolunteerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = await Context.Volunteers.FindAsync(model.Id).ConfigureAwait(false);
                entity.FirstName = model.FirstName;
                entity.LastName = model.LastName;
                entity.FirstNameAr = model.FirstNameAr;
                entity.LastNameAr = model.LastNameAr;
                entity.BirthPlace = model.BirthPlace;
                entity.BirthDate = model.BirthDate;
                entity.Address = model.Address;
                entity.Email = model.Email;
                entity.Phone = model.Phone;
                entity.CardId = model.CardId;
                entity.Address = model.Address;
                await Context.SaveChangesAsync().ConfigureAwait(false);
                return View("Index");
            }
            //GetSelects(model);
            return View(model);
        }
        [HttpGet("getList")]
        public async Task<IActionResult> GetList([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var total = Context.Volunteers.Count();
            int recordsFilterd = total;

            var query = (from v in Context.Volunteers
                        
                    select v
                );
            if (!string.IsNullOrEmpty(dataRequest.Search?.Value))
            {
                var term = dataRequest.Search.Value;
                query = query
                    .Where(x => x.FirstNameAr.Contains(term)
                                || x.LastNameAr.Contains(term)
                                || x.Email.Contains(term)
                        || x.FirstName.Contains(term)
                        || x.LastName.Contains(term) 
                                || x.AffectedCity.Contains(term)
                                || x.Job.Contains(term)
                                || x.RegistrationNumber.Contains(term)
                                || x.Region.Contains(term)
                                || x.Phone.Contains(term)
                                || x.BirthPlace.Contains(term)
                                || x.DistrictName.Contains(term));
                 recordsFilterd = query.Count();

            }

            if (dataRequest.Orders != null)
                foreach (var order in dataRequest.Orders)
                    query = query.OrderBy(String.Concat(
                        LinqHelper.GetPropertyNameByIndex<VolunteerQueryViewModel>(order.Column),
                        order.Dir == "asc" ? "" : " descending"));
            else
                query = query
                    .OrderBy(x => x.FirstName);
            var data = await query.Skip(dataRequest.Start)
                // ReSharper disable once TooManyChainedReferences
                .Take(dataRequest.Length).Select(x => new VolunteerQueryViewModel
                    {
                        Id = x.Id.ToString(),
                        FullNameAr = x.FirstNameAr,
                        LastNameAr = x.LastNameAr,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        AffectedCity = x.AffectedCity,
                        Neighborhood = x.Neighborhood,
                        Region = x.Region,
                        RegistrationNumber = x.RegistrationNumber,
                        Job = x.Job,
                        Phone = x.Phone,
                        Email = x.Email,
                        District = x.DistrictName,
                        Sector = x.Sector,
                        CardId = x.CardId

                    }
                ).ToListAsync().ConfigureAwait(false);
            var result = new DatatablesQueryModel<VolunteerQueryViewModel>
            {
                Data = data,
                RecordsTotal = total,
                RecordsFilterd = recordsFilterd

            };
            return Json(result.Data.ToDataTablesResponse(dataRequest, result.RecordsTotal, result.RecordsFilterd));

        }
        [HttpGet]
        public FileResult ExportExcel()
        {
            var result = Context.Volunteers.Include(x=>x.District).Include(x=>x.Task).ToList();
            string webRootPath = Env.WebRootPath;
            FileStream fs = new FileStream(string.Concat(webRootPath, Path.DirectorySeparatorChar + "Report" + Path.DirectorySeparatorChar + "Volunteers.xlsx"), FileMode.Open);
            var stream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(stream,fs))
            {
                ExcelWorksheet sl = package.Workbook.Worksheets["Rapport"];
                int i = 0;
                foreach (var volunteer in result.OrderBy(x => x.FirstName))
                {
                    sl.Cells[5 + i, 1].Value= volunteer.FirstName;
                    sl.Cells[5 + i, 2].Value =  volunteer.LastName;
                    sl.Cells[5 + i, 3].Value= volunteer.BirthDate?.ToShortDateString();
                    sl.Cells[5 + i, 4].Value = volunteer.BirthPlace;
                    sl.Cells[5 + i, 5].Value= volunteer.Email;
                    sl.Cells[5 + i, 6].Value= volunteer.Phone;
                    sl.Cells[5 + i, 7].Value = volunteer.AffectedCity;
                    sl.Cells[5 + i, 8].Value = volunteer.Job;
                    i++;
                }
                // save our new workbook and we are done!
                 package.Save();
                 stream.Position = 0;
                 string excelName = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

                 //return File(stream, "application/octet-stream", excelName);  
                 return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var volunteer = await Context.Volunteers.FindAsync(id).ConfigureAwait(false);
            if (volunteer == null) return Json("Item not found");
            Context.Volunteers.Remove(volunteer);
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return Json("Item removed successfully");
        }
    }
}