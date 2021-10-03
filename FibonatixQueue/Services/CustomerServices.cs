using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using Azure.Storage;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Azure.Storage.Queues.Specialized;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using StackExchange.Redis;
using StackExchange.Redis.Profiling;
using StackExchange.Redis.KeyspaceIsolation;
using FibonatixQueue.Settings;
using FibonatixQueue.Services;

namespace FibonatixQueue.Services
{
    public enum ServiceStructure { Azure, Redis, MongoDB }

    public class AzureQueueService
    {
        private QueueClient Client { get; set; }

        public AzureQueueService(CommonDBSettings settings)
        {
            Client = new(settings.ConnectionString, settings.Password);
        }

        public PeekedMessage PopItem() { return Client.PeekMessage().Value; }

        public void PushItem(string message)
        {
            Client.SendMessage(message);
        }
    }

    public class RedisQueueService
    {
        private IDatabase Queryable { get; set; }

        private readonly SymAlgo _symAlgo;

        public RedisQueueService(ISecureServiceSettings settings)
        {
            ConfigurationOptions options = new();
            options.EndPoints.Add(settings.ConnectionString);
            options.Password = settings.Password;

            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);
            Queryable = redis.GetDatabase();

            _symAlgo = new SymAlgo(settings.Algorithm);
        }

        public RedisQueueService(IServiceSettings settings)
        {
            ConfigurationOptions options = new();
            options.EndPoints.Add(settings.ConnectionString);
            options.Password = settings.Password;

            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);
            Queryable = redis.GetDatabase();

            _symAlgo = null;
        }

        public RedisValue PopItem(RedisKey key)
        {
            string value = Queryable.ListRightPop(key);

            // Decrypts json string with the algorithm property
            if (_symAlgo != null && value != null)
            {
                value = _symAlgo.Decrypt(value);
            }

            return value;
        }

        public void PushItem(RedisKey key, RedisValue[] values)
        {
            // Encrypts json string with the algorithm property
            if (_symAlgo != null)
            {
                values[0] = new RedisValue(Convert.ToBase64String(_symAlgo.Encrypt(values[0])));
            }
            Queryable.ListLeftPush(key, values);
        }
    }

    public class MongoQueueService<I> where I : BsonDocument
    {
        private IMongoCollection<I> Queryable { get; set; }

        private readonly SymAlgo _symAlgo;

        public MongoQueueService(MongoDBSettings settings)
        {
            MongoClient client = new(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.Database);

            Queryable = database.GetCollection<I>(settings.Collection);
        }

        public BsonDocument PopItem()
        {
            FilterDefinitionBuilder<BsonDocument> builder = new();
            BsonObjectId _id = Queryable.AsQueryable().FirstOrDefault()[0] as BsonObjectId;

            return Queryable.FindOneAndDelete(item => item[0] == _id);
        }

        public void PushItem(I item)
        {
            Queryable.InsertOne(item);
        }
    }
}
