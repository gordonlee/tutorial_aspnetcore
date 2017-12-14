using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace multiple_dbcontext_web.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private DbContextPool _pool { get; set; } = null;

        public ValuesController(DbContextPool pool)
        {
            _pool = pool;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            if (_pool.PoolDict.ContainsKey(id))
            {
                DbContext context = null;
                if (_pool.PoolDict.TryGetValue(id, out context))
                {
                    if (context is CustomDbContext)
                    {
                        int contextId = (context as CustomDbContext).ID;
                        return "Context ID is " + contextId.ToString();
                    }
                    else
                    {
                        return "This is not CustomDbContext";
                    }
                }
                else
                {
                    return "Failed to get value of dictionary.";
                }
            }
            else
            {
                return "Cannot find serviceId.";
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
