
using System.Collections;
using System.Linq.Dynamic.Core;
using System.Net;
using Appointment.SDK.Backend.Database;
using Appointment.SDK.Backend.Utilities;
using Appointment.SDK.Entities;
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
            using(var context = CreateContext())
            {
                context.Set<T>().Add(Item);
                context.SaveChanges();
                return Ok(Item);
            }
        }

        [HttpPost("addRange")]
        public virtual IActionResult AddRange([FromBody] T[] Items)
        {
            using(var context = CreateContext())
            {
                context.Set<T>().AddRange(Items);
                context.SaveChanges();
                return Ok(Items);
            }
        }

        [HttpPut]
        public virtual IActionResult Update([FromBody] T Item)
        {
            var Rowid = typeof(T).GetProperty("Rowid")!
                    .GetValue(Item);

            using(var context = CreateContext())
            {   
                var BdItem = context.Set<T>()
                    .Where("Rowid == @0", Rowid)
                    .First();

                var FieldsToUpdate = typeof(T).GetProperties()
                    .Select(x => x.Name);

                context.Entry(BdItem).CurrentValues.SetValues(Item);
                foreach (var relatedProperty in FieldsToUpdate)
                {
                    var Property = typeof(T).GetProperty(relatedProperty)!;
                    Property.SetValue(BdItem, Property.GetValue(Item));
                }

                context.SaveChanges();

                return Ok(Item);
            }
        }

        [HttpDelete("{Rowid}")]
        public virtual IActionResult Delete(int Rowid)
        {
            using(var context = CreateContext())
            {
                var Result = context.Set<T>()
                    .Where("Rowid == @0", Rowid)
                    .ExecuteDelete();
                return Ok(Result);
            }
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

                    if (typeof(T).GetProperty(Property)?.PropertyType == typeof(string))
                        Query = Query.Where($"({Property}).ToLower().Contains(\"{Value}\".ToLower())");
                    else
                        Query = Query.Where($"{Property} == @{i}", Value!);
                }

                return Ok(Query.ToList());
            }
        }
    }
}
