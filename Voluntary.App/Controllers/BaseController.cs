using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Voluntary.App.Data;

namespace Voluntary.App.Controllers
{
    public class BaseController : Controller
    {
        public IHostingEnvironment Env { get; }
        public ApplicationDbContext Context { get; }
        public int TotalFiltred { get; set; }
        public BaseController(ApplicationDbContext context)
        {
            Context = context;
        }

        public BaseController( ApplicationDbContext context, IHostingEnvironment env)
        {
            Env = env;
            Context = context;
        }
      
    }
}