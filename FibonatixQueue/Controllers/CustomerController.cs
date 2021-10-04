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
        private readonly MongoQueueService _customerService;

        public CustomerController(MongoQueueService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet(Name = "PopCustomer")]
        public ActionResult<string> Pop(string key)
        {
            string customerVal = _customerService.PopItem(new RedisKey(key));

            if (customerVal == null)
                return NotFound();

            return customerVal;
        }

        [HttpPost(Name = "PushCustomer")]
        public IActionResult Push(string key, Customer customer)
        {
            string json = customer.Jsonify();

            RedisValue[] redises = { new RedisValue(json) };

            _customerService.PushItem(new RedisKey(key), redises);

            return Created("redis", new RedisKey(key).ToString() + " " + redises[0]);
        }
    }
}
