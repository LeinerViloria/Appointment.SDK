﻿
using NsDataAnnotations = System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using Appointment.SDK.Backend.Database;
using Appointment.SDK.Backend.Utilities;
using Appointment.SDK.Backend.Validations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Collections.Generic;
using Appointment.SDK.Entities;

namespace Appointment.SDK.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class StandardControllerBase(IServiceProvider serviceProvider) : ControllerBase
    {
        protected readonly IServiceProvider serviceProvider = serviceProvider;

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

    public abstract class StandardController<T, V>(IServiceProvider serviceProvider) : StandardControllerBase(serviceProvider)
        where T : class
        where V : BaseControllerValidator<T>
    {
        private void Validate(ref ValidationResult result, T Item)
        {
            var Errors = new List<NsDataAnnotations.ValidationResult>();

            var DataAnnotations = NsDataAnnotations.Validator.TryValidateObject(Item, new NsDataAnnotations.ValidationContext(Item), Errors);

            if(!DataAnnotations)
            {
                result = new ValidationResult(Errors.Select(x => new ValidationFailure(){
                    ErrorMessage = x.ErrorMessage,
                }));

                return;
            }

            V ValidatorInstance = ActivatorUtilities.CreateInstance<V>(serviceProvider);

            try
            {
                result = ValidatorInstance.Validate(Item);
            }
            catch (AsyncValidatorInvokedSynchronouslyException)
            {
                result = ValidatorInstance.ValidateAsync(Item).GetAwaiter().GetResult();
            }
        }

        [HttpPost]
        public virtual IActionResult Create([FromBody] T Item)
        {
            ValidationResult result = new();
            Validate(ref result, Item);

            if(!result.IsValid)
                return BadRequest(result.Errors.ToDictionary(x => x.PropertyName, x=>new List<string>() { x.ErrorMessage }));

            using(var context = CreateContext())
            {
                try
                {
                    context.Set<T>().Add(Item);
                    context.SaveChanges();
                    return Ok(Item);
                }
                catch(DbUpdateException Exception)
                {
                    dynamic Error = Exception.InnerException!;

                    var Property = ((string)Error.ConstraintName).Split("_").Last().Replace("Rowid", "");

                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Errors = new Dictionary<string, List<string>>()
                    {
                        { Property, [$"Invalid {Property}"] }
                    }
                    });
                }
            }
        }

        [HttpPost("addRange")]
        public virtual IActionResult AddRange([FromBody] T[] Items)
        {
            ValidationResult result = new();

            foreach (var Item in Items)
            {
                Validate(ref result, Item);

                if(!result.IsValid)
                    return BadRequest(result.Errors.ToDictionary(x => x.PropertyName, x => new List<string>() { x.ErrorMessage }));
            }

            using(var context = CreateContext())
            {
                try
                {
                    context.Set<T>().AddRange(Items);
                    context.SaveChanges();
                    return Ok(Items);
                }
                catch (DbUpdateException Exception)
                {
                    dynamic Error = Exception.InnerException!;

                    var Property = ((string)Error.ConstraintName).Split("_").Last().Replace("Rowid", "");

                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Errors = new Dictionary<string, List<string>>()
                    {
                        { Property, [$"Invalid {Property}"] }
                    }
                    });
                }
            }
        }

        [HttpPut]
        public virtual IActionResult Update([FromBody] T Item)
        {
            ValidationResult result = new();
            Validate(ref result, Item);

            if(!result.IsValid)
                return BadRequest(result.Errors.ToDictionary(x => x.PropertyName, x => new List<string>() { x.ErrorMessage }));

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

                try
                {
                    context.SaveChanges();

                    return Ok(Item);
                }
                catch (DbUpdateException Exception)
                {
                    dynamic Error = Exception.InnerException!;

                    var Property = ((string)Error.ConstraintName).Split("_").Last().Replace("Rowid", "");

                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Errors = new Dictionary<string, List<string>>()
                    {
                        { Property, [$"Invalid {Property}"] }
                    }
                    });
                }
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
                var CollectionType = typeof(ICollection<T>);
                var Query = context.Set<T>()
                    .AsNoTracking();

                bool HasCollection = false;

                for (int i = 0; i < Filters.Count; i++)
                {
                    var Property = Filters[i].Name;
                    var PropertyType = typeof(T).GetProperty(Property)?.PropertyType;
                    var Value = HttpContext.Request.Query[Property];

                    if (PropertyType == typeof(string))
                        Query = Query.Where($"({Property}).ToLower().Contains(\"{Value}\".ToLower())");
                    else if($"{Value}".EndsWith("_rs"))
                    {
                        Query = Query.Include(Property);

                        if (HasCollection) continue;

                        if(PropertyType.Name == typeof(ICollection<>).Name)
                        {
                            HasCollection = PropertyType.GenericTypeArguments[0]!
                                .GetProperties()
                                .Any(x => x.PropertyType == typeof(T));
                        }else
                        {
                            HasCollection = PropertyType!.GetProperties()
                                .Any(x => x.PropertyType == CollectionType);
                        }
                    }
                    else
                        Query = Query.Where($"{Property} == @0", Value!);
                }

                if(HasCollection)
                {
                    var Properties = typeof(T).GetProperties();

                    var SelectFields = string.Join(",", Properties.Select(x =>
                    {
                        return x.Name;
                    }));

                    Query = Query.Select<T>($"new({SelectFields})");
                }

                return Ok(Query.ToList());
            }
        }
    }
}
