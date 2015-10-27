using Microsoft.WindowsAzure.Storage.Queue;
using NSubstitute;
using NUnit.Framework;
using QueueWebjob;
using QueueWebjob.QueueAndRedis;
using StackExchange.Redis;

namespace WebJob.Test.QueueAndRedis
{
    [TestFixture]
    public class QueueAndRedisTest
    {
        [Test]
        public void IQueueAndredis_Test()
        {

            IQueueAndredis iqueueandredis = Substitute.For<IQueueAndredis>();

            Functions f = new Functions(iqueueandredis);
            f.ProcessQueueMessage("haha");
            f.ProcessQueueMessage("hasdaha");
            f.ProcessQueueMessage("hasdha");
            //CloudQueue queue = iqueueandredis.GetQueue();

            //iqueueandredis.Received(4).GetQueue();            
            iqueueandredis.Received().InsertData("haha");
            iqueueandredis.Received(3).InsertData(Arg.Any<string>());
            iqueueandredis.Received(3).ProcessMessageAsync();

        }
        [Test]
        public void Redis_Test()
        {
            IDatabase cache = Substitute.For<IDatabase>();
            var key = "ds";
            var message = "hello";
            var message2 = "java";
            cache.ListLeftPush(key, "hello");
            cache.ListLeftPush(key, "java");
   
            cache.Received().ListLeftPush(key,message);
            cache.Received().ListLeftPush(key, message2);
            cache.ListLeftPop(key).Returns((RedisValue)"hello");           
            Assert.AreEqual((RedisValue)message, cache.ListLeftPop(key));
            cache.ListLeftPop(key).Returns((RedisValue)"java");
            Assert.AreEqual((RedisValue)message2, cache.ListLeftPop(key));
        }
    }
}

