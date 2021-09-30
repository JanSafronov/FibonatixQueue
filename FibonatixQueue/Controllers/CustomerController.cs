using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using StackExchange.Redis.Profiling;
using FibonatixQueue.Models;
using FibonatixQueue.Services;

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

        [HttpGet(Name = "PopCustomer")]
        public ActionResult<string[]> Pop(string key)
        {
            string[] customerVal = ((string[])_customerService.PopItem(new RedisKey(key)));

            if (customerVal == null) 
                return NotFound();

            return customerVal;
        }

        [HttpPost]
        public ActionResult<string[]> Post(Customer customer)
        {
            string json = customer.Jsonify();

            var redises = new RedisValue[] {
                new RedisValue(json)
            };

            _customerService.PushItem(new RedisKey(customer.Name), redises);

            return (string[])RedisResult.Create(new RedisValue[] { new RedisValue(json) });
        }

        // PUT api/<CustomerController>/5
        /*[HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}
