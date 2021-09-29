using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FibonatixQueue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {

        private readonly IConnectionMultiplexer _redis;

        public RedisController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }


        // GET: api/<RedisController>
        [HttpGet("foo")]
        public async Task<IActionResult> Foo()
        {
            var db = _redis.GetDatabase();
            var foo = await db.StringGetAsync("foo");
            return Ok(foo.ToString());
        }

        // GET api/<RedisController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RedisController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<RedisController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RedisController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
