
using System.Collections;
using System.Linq.Dynamic.Core;
using System.Net;
using Appointment.SDK.Backend.Database;
using Appointment.SDK.Backend.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Appointment.SDK.Backend.Controllers
{
    [ApiController]
    public abstract class StandardControllerBase(IServiceProvider serviceProvider) : ControllerBase
    {
        protected StoreContext CreateContext()
        {
            dynamic dbFactory = serviceProvider.GetService(typeof(IDbContextFactory<StoreContext>))!;

            var context = dbFactory.CreateDbContext();

            return context;
        }

        [HttpGet]
        public string HelloWorld() =>
            $"Hello world! Soy el controlador ({GetType().Name})";  
    }

    public abstract class StandardController(IServiceProvider serviceProvider) : StandardControllerBase(serviceProvider)
    {

        
    }

    public abstract class StandardController<T>(IServiceProvider serviceProvider) : StandardControllerBase(serviceProvider) where T : class
    {
        [HttpPost]
        public virtual IActionResult Create([FromBody] T Item)
        {
            return Ok();
        }

        [HttpPut]
        public virtual IActionResult Update([FromBody] T Item)
        {
            return Ok();
        }

        [HttpDelete("{Rowid}")]
        public virtual IActionResult Delete(int Rowid)
        {
            return Ok();
        }

        [HttpGet("getData")]
        public virtual IActionResult GetData()
        {
            var Filters = HttpContext.Request.Query.GetPropertiesByParams(typeof(T));

            using(var context = CreateContext())
            {
                var Query = context.Set<T>()
                    .AsNoTracking();

                for (int i = 0; i < Filters.Count; i++)
                {
                    var Property = Filters[i].Name;
                    var Value = HttpContext.Request.Query[Property];

                    Query = Query.Where($"{Property} == @{i}", Value!);
                }

                return Ok(Query.ToList());
            }
        }
    }
}
