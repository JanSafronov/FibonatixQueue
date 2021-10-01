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

        private readonly SymAlgo _symAlgo;

        public CustomerController(RedisQueueService customerService)
        {
            _customerService = customerService;

            if (_customerService.algorithm != null)
                _symAlgo = new SymAlgo(_customerService.algorithm);

        }

        [HttpGet(Name = "PopCustomer")]
        public ActionResult<string> Pop(string key)
        {
            string customerVal = _customerService.PopItem(new RedisKey(key));

            // Decrypts json string with the algorithm property
            if (_customerService.algorithm != null)
            {
                customerVal = _symAlgo.Decrypt(customerVal);
            }

            if (customerVal == null) 
                return NotFound();

            return customerVal;
        }

        [HttpPost(Name = "PushCustomer")]
        public ActionResult<string> Push(Customer customer)
        {
            string json = customer.Jsonify();

            // Encrypts json string with the algorithm property
            if (_customerService.algorithm != null)
            {
                json = _symAlgo.Encrypt(json);
            }

            var redises = new RedisValue[] {
                new RedisValue(json)
            };

            _customerService.PushItem(new RedisKey(customer.Name), redises);

            return (string)RedisResult.Create(new RedisValue[] { new RedisValue(json) });
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
