using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Akanksha.Api
{
    public class CategoryController : ApiController
    {
        private  AkankshaEntities db;
        public CategoryController()
        {
              db = new AkankshaEntities();
        }

        [HttpGet]
        public IHttpActionResult GetCategories()
        {
             var categories = db.Categories.ToList();
            return Ok(categories);
        }
    }
}
