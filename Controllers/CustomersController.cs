using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lashes.Database;
using Lashes.Models;

namespace test.Controllers
{
  [Route("api/[controller]")]
    public class CustomersController : Controller
    {        
        // https://docs.microsoft.com/en-us/ef/core/get-started/netcore/new-db-sqlite
        
        [HttpGet]
        public IEnumerable<Customer> GetAll()
        {
            using (var db = new DatabaseContext())
            {
                return (from c in db.Customers orderby c.name select c).ToList();
            }
        }

        [HttpGet("{id}", Name = "GetCustomer")]
        public IActionResult GetById(int id)
        {
            using (var db = new DatabaseContext())
            {
                Customer customer = db.Customers.Find(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return new ObjectResult(customer);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }
            using (var db = new DatabaseContext())
            {
                db.Customers.Add(customer);
                db.SaveChanges();
            }
        
            return CreatedAtRoute("GetCustomer", new { controller = "Customers", id = customer.id }, customer);
        }

        [HttpDelete("{id}", Name = "RemoveCustomer")]
        public IActionResult Delete(int id)
        {
            using (var db = new DatabaseContext()) {
                Customer customer = db.Customers.Find(id);
                db.Remove(customer);
                db.SaveChanges();
            }
   
            return new NoContentResult();
        }
    }
}
