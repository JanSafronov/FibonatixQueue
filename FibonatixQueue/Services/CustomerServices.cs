using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Encryption;
using MongoDB.Driver.Linq;
using MongoDB.Libmongocrypt;
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
        private IDatabase Database { get; set; }

        private readonly SymAlgo _symAlgo;

        public RedisQueueService(ISecureServiceSettings settings)
        {
            ConfigurationOptions options = new();
            options.EndPoints.Add(settings.ConnectionString);
            options.Password = settings.Password;

            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);
            Database = redis.GetDatabase();

            _symAlgo = new SymAlgo(settings.Algorithm);
        }

        public RedisQueueService(IServiceSettings settings)
        {
            ConfigurationOptions options = new();
            options.EndPoints.Add(settings.ConnectionString);
            options.Password = settings.Password;

            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);
            Database = redis.GetDatabase();

            _symAlgo = null;
        }

        public RedisValue PopItem(RedisKey key)
        {
            string value = Database.ListRightPop(key);

            // Decrypts as json string with the algorithm property
            if (_symAlgo != null && value != null)
            {
                value = _symAlgo.Decrypt(value);
            }

            return value;
        }

        public void PushItem(RedisKey key, RedisValue[] values)
        {
            // Encrypts as json string with the algorithm property
            if (_symAlgo != null)
                values[0] = new RedisValue(Convert.ToBase64String(_symAlgo.Encrypt(values[0])));
            Database.ListLeftPush(key, values);
        }
    }

    public class MongoQueueService
    {
        private IMongoDatabase Database { get; set; }

        private readonly SymAlgo _symAlgo;

        public MongoQueueService(SecureDBSettings settings)
        {
            MongoClient client = new(settings.ConnectionString);
            Database = client.GetDatabase(settings.ConnectionString[(settings.ConnectionString.LastIndexOf("/") + 1)..]);

            _symAlgo = new SymAlgo(settings.Algorithm);
        }

        public RedisValue PopItem(RedisKey key)
        {
            string value = Database.GetCollection<BsonDocument>(key).FindOneAndDelete(item => item["id"] == 0).ToJson();

            // Decrypts as json string with the algorithm property
            if (_symAlgo != null && value != null)
            {
                value = _symAlgo.Decrypt(value);
            }

            return value;
        }

        public void PushItem(RedisKey key, RedisValue[] values)
        {
            // Encrypts as json string with the algorithm property
            if (_symAlgo != null)
                values[0] = new RedisValue(Convert.ToBase64String(_symAlgo.Encrypt(values[0])));

            IMongoCollection<BsonDocument> collection = Database.GetCollection<BsonDocument>(key.ToString());

            long length = collection.CountDocuments(doc => true);

            collection.InsertOne(new BsonDocument("id", new BsonInt64(length)).Add("value", values[0].ToString()));
        }
    }
}
