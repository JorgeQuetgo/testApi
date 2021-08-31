using System;
using System.Text;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Net.Http;

using System.Security.Cryptography;
using RestSharp;
using RestSharp.Authenticators;
using testApi.Models;
using System.Net;
using AwsSig4Authentication;
using Aws4RequestSigner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace testApi.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class TestController : ControllerBase
    {
        private static IConfiguration configuration;

        public TestController(IConfiguration iconfiguration){
            configuration = iconfiguration;
        }
        

        static byte[] HmacSHA256(String data, byte[] key)
        {
            String algorithm = "HmacSHA256";
            KeyedHashAlgorithm kha = KeyedHashAlgorithm.Create(algorithm);
            kha.Key = key;

            return kha.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        static byte[] getSignatureKey(String key, String dateStamp, String regionName, String serviceName)
        {
            byte[] kSecret = Encoding.UTF8.GetBytes(("AWS4" + key).ToCharArray());
            byte[] kDate = HmacSHA256(dateStamp, kSecret);
            byte[] kRegion = HmacSHA256(regionName, kDate);
            byte[] kService = HmacSHA256(serviceName, kRegion);
            byte[] kSigning = HmacSHA256("aws4_request", kService);

            return kSigning;
        }

        private static string getSignatureKey(string key, string stringToSign)
        {
            byte[] kSecret = Encoding.UTF8.GetBytes(key.ToCharArray());
            byte[] kSigning = HmacSHA256(stringToSign, kSecret);
            return WebUtility.UrlEncode(Convert.ToBase64String(kSigning));
        }        

        
        [HttpGet]
        public String Get(){
            return "Hola2";
        }
        
        
        [HttpPost("/AddTigo")]         
        public String AddTigo([FromBody]TigoBody tigoBody)
        {
            var result = request5(tigoBody);            
            return result.Result;
        }

        private async static Task<String> request5(TigoBody tigoBody){
            //https://github.com/tsibelman/aws-signer-v4-dot-net    
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(tigoBody);
            Console.WriteLine(json);
            var accessKey = System.Environment.GetEnvironmentVariable("ACCESS_KEY");
            var secretKey = System.Environment.GetEnvironmentVariable("SECRET_KEY");
            var region = configuration.GetSection("Region").Value;
            var uri = System.Environment.GetEnvironmentVariable("URL_TIGO");
            var signer = new AWS4RequestSigner(accessKey, secretKey);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
                Content = content
            };

            request = await signer.Sign(request, "execute-api", region);

            var client = new HttpClient();
            var response = await client.SendAsync(request);

            var responseStr = await response.Content.ReadAsStringAsync(); 
            Console.WriteLine(responseStr);
            return responseStr;
        }

        private static byte[] ToBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str.ToCharArray());
        }

        private static string HexEncode(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
        }

        private static byte[] Hash(byte[] bytes)
        {
            return SHA256.Create().ComputeHash(bytes);
        }

        private static byte[] HmacSha256(string data, byte[] key)
        {
            return new HMACSHA256(key).ComputeHash(ToBytes(data));
        }
        
        internal class ServiceClient
        {
            public RestRequest GetRequest()
            {
                RestRequest request = new RestRequest();
                return request;
            }

            public RestClient GetClient(string baseUrl)
            {
                var credentials = new AwsApiKey()
                {
                    AccessKey = "AKIAVTPVHUVWDKJTZ7SL", // fill in your IAM API Token Access Key
                    SecretKey = "o9CQsglY021NpDfNUr3qz2DYAJzxEDfxce1DyPFN", // fill in your IAM API Token Secret Key
                    Region = "us-east-1" // fill in your service region
                };

                return new RestClient(baseUrl)
                {
                    Authenticator = new Sig4Authenticator(credentials)
                };
            }
        }                
    }
}
