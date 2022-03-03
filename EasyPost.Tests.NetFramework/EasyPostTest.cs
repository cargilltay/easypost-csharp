using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyPost.Tests.NetFramework
{
    [TestClass]
    public class EasyPostTest
    {
        private const string FakeApikey = "fake_api_key";
        private const string FakeApiUrl = "https://fake.api.com";
        private const string HttpBinUrl = "https://httpbin.org/get";

        private static ClientConfiguration GetBasicClientConfiguration()
        {
            return new ClientConfiguration(FakeApikey);
        }

        [TestMethod]
        public void TestApiKeyConstructor()
        {
            ClientConfiguration config = GetBasicClientConfiguration();

            Assert.AreEqual(FakeApikey, config.ApiKey);
            Assert.AreEqual("https://api.easypost.com/v2", config.ApiBase);
        }

        [TestMethod]
        public void TestApiKeyPlusBaseUrlConstructor()
        {
            ClientConfiguration config = new ClientConfiguration(FakeApikey, FakeApiUrl);

            Assert.AreEqual(FakeApikey, config.ApiKey);
            Assert.AreEqual(FakeApiUrl, config.ApiBase);
        }

        [TestMethod]
        public void TestTimeout()
        {
            Client client = new Client(GetBasicClientConfiguration());
            client.ConnectTimeoutMilliseconds = 5000;
            client.RequestTimeoutMilliseconds = 5000;

            Assert.AreEqual(5000, client.ConnectTimeoutMilliseconds);
            Assert.AreEqual(5000, client.RequestTimeoutMilliseconds);
        }

        [TestMethod]
        public void TestNotConfigured()
        {
            ClientManager.Unconfigure();
            Assert.ThrowsException<ClientNotConfigured>(() => new Request("resource").Execute<dynamic>());
        }
    }
}