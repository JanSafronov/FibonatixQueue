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
using Microsoft.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using StackExchange.Redis.Profiling;
using FibonatixQueue.Models;
using FibonatixQueue.Services;

namespace FibonatixQueue.Controllers
{
    public abstract class CustomerControllerBase : ControllerBase
    {
        private readonly QueueService _customerService;

        public CustomerControllerBase(QueueService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public ActionResult<string> Pop(string key)
        {
            string customerVal = _customerService.PopItem(new RedisKey(key));

            if (customerVal == null)
                return NotFound();

            return customerVal;
        }

        [HttpPost]
        public IActionResult Push(string key, Customer customer)
        {
            string json = customer.Jsonify();

            RedisValue[] redises = { new RedisValue(json) };

            _customerService.PushItem(new RedisKey(key), redises);

            return Created("redis", new RedisKey(key).ToString() + " " + redises[0]);
        }
    }

    [Route("api/redis/[controller]")]
    [ApiController]
    public class RedisCustomerController : CustomerControllerBase
    {
        public RedisCustomerController(RedisQueueService customerService) :
        base(customerService) { }
    }

    [Route("api/mongodb/[controller]")]
    [ApiController]
    public class MongoCustomerController : CustomerControllerBase
    {
        public MongoCustomerController(MongoQueueService customerService) :
        base(customerService) { }

    }
}
