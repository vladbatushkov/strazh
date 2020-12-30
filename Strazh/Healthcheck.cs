
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Strazh
{
    public static class Healthcheck
    {
        public static async Task<bool> IsNeo4jReady(short retry = 3)
        {
            var statusCode = await GetStatusCode();
            if (statusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("Neo4j is ready to use.");
                return true;
            }
            if (retry > 0)
            {
                Console.WriteLine("Waiting for Neo4j...");
                await Task.Delay(10000);
                return await IsNeo4jReady(--retry);
            }
            return false;
        }

        private static async Task<HttpStatusCode> GetStatusCode()
        {
            try
            {
                using var client = new HttpClient();
                var result = await client.GetAsync("http://localhost:7474/");
                return result.StatusCode;
            }
            catch
            {
                return HttpStatusCode.NotFound;
            }
        }
    }
}