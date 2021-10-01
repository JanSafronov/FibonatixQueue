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
        private QueueClient client { get; set; }

        public AzureQueueService(CommonDBSettings settings)
        {
            client = new(settings.connectionString, settings.password);
        }

        public PeekedMessage PopItem() { return client.PeekMessage().Value; }

        public void PushItem(string message)
        {
            client.SendMessage(message);
        }
    }

    public class RedisQueueService
    {
        public IDatabase queryable { get; set; }

        public string algorithm { get; }

        public RedisQueueService(ISecureServiceSettings settings)
        {
            ConfigurationOptions options = new ConfigurationOptions();
            options.EndPoints.Add(settings.connectionString);
            options.Password = settings.password;

            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);
            queryable = redis.GetDatabase();

            algorithm = settings.algorithm;
        }

        public RedisQueueService(IServiceSettings settings)
        {
            ConfigurationOptions options = new ConfigurationOptions();
            options.EndPoints.Add(settings.connectionString);
            options.Password = settings.password;

            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);
            queryable = redis.GetDatabase();

            algorithm = null;
        }

        public RedisValue PopItem(RedisKey key) { 
            try {
                return queryable.ListRightPop(key);
            }
            catch {
                return new RedisValue();
            }
        }

        public void PushItem(RedisKey key, RedisValue[] values)
        {
            queryable.ListLeftPush(key, values);
        }
    }

    public class MongoQueueService<I> where I : BsonDocument
    {
        private IMongoCollection<I> queryable { get; set; }

        public MongoQueueService(MongoDBSettings settings)
        {
            MongoClient client = new(settings.connectionString);
            IMongoDatabase database = client.GetDatabase(settings.database);

            queryable = database.GetCollection<I>(settings.collection);
        }

        public BsonDocument PopItem() {
            FilterDefinitionBuilder<BsonDocument> builder = new FilterDefinitionBuilder<BsonDocument>();
            BsonObjectId _id = queryable.AsQueryable().FirstOrDefault()[0] as BsonObjectId;

            return queryable.FindOneAndDelete(item => item[0] == _id);
        }

        public void PushItem(I item)
        {
            queryable.InsertOne(item);
        }
    }
}
