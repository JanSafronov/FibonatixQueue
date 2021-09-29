using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Azure.Storage.Queues.Specialized;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using StackExchange.Redis;
using StackExchange.Redis.Profiling;
using FibonatixQueue.Settings;

namespace FibonatixQueue.Services
{
    public enum ServiceStructure { Azure, Redis, MongoDB }

    public abstract class ServiceBase
    {
        private IServiceSettings settings { get; set; }

        public ServiceBase(IServiceSettings settings)
        {
            this.settings = settings;
        }
    }

    public class AzureQueueService
    {
        private QueueClient client { get; set; }

        public AzureQueueService(AzureDBSettings settings)
        {
            this.client = new(settings.connectionString, settings.storage);
        }

        public PeekedMessage GetItem() { return client.PeekMessage().Value; }

        public void CreateItem(string message)
        {
            client.SendMessage(message);
        }

        public void DeleteItem(string messageId)
        {
            client.DeleteMessage(messageId, "");
        }
    }

    public class RedisQueueService
    {
        private IDatabase queryable { get; set; }

        public RedisQueueService(RedisDBSettings settings)
        {
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(settings.connectionString + "," + settings.storage);
            this.queryable = redis.GetDatabase();
        }

        public RedisValue PopItem(string queueName) { return queryable.ListLeftPop(queueName); }

        public void PushItem(RedisKey key, RedisValue[] values)
        {
            queryable.ListRightPush(key, values);
        }

        public void ReplaceItem(string messageId)
        {
            queryable.list
        }
    }

    public class MongoQueueService
    {
        private IMongoCollection<BsonDocument> queryable { get; set; }

        public MongoQueueService(MongoDBSettings settings)
        {
            MongoClient client = new(settings.connectionString);
            IMongoDatabase database = client.GetDatabase(settings.storage);

            this.queryable = database.GetCollection<BsonDocument>(settings.collection);
        }
    }
}
