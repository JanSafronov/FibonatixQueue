using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using FibonatixQueue.Models;
using FibonatixQueue.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FibonatixQueue.Controllers
{


    [Route("api/[controller]", Name = "Customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly RedisQueueService _customerService;

        public CustomerController(RedisQueueService customerService)
        {
            _customerService = customerService;
        }

        // GET api/<CustomerController>/5
        [HttpGet("{queueName:length(24)}", Name = "PopCustomer")]
        public RedisValue Pop(string queueName)
        {
            return _customerService.PopItem(queueName);
        }

        // POST api/<CustomerController>
        [HttpPost]
        public void Post([FromBody] Customer customer)
        {
            _customerService.PushItem(customer.Name, new RedisValue[] { new RedisValue(customer.ToList().ToString()) });
        }

        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
