using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Strazh.Domain;

namespace Strazh.Database
{
    public static class DbManager
    {
        private const string DBNAME = "neo4j";
        private const string USER = "strazh";
        private const string PASSWORD = "strazh";
        private const string CONNECTION = "neo4j://localhost:7687";

        public static async Task InsertData(IEnumerable<Triple> triples, string cred = null, bool isOverride = true)
        {
            var creds = new[] { DBNAME, USER, PASSWORD };
            if (!string.IsNullOrEmpty(cred))
            {
                var args = cred.Split(":");
                if (args.Length == 3)
                {
                    creds = args;
                }
            }
            var driver = GraphDatabase.Driver(CONNECTION, AuthTokens.Basic(creds[1], creds[2]));
            var session = driver.AsyncSession(o => o.WithDatabase(creds[0]));
            try
            {
                if (isOverride)
                {
                    await session.RunAsync("MATCH (n) DETACH DELETE n;");
                }
                foreach (var triple in triples)
                {
                    await session.RunAsync(triple.ToString());
                }
                Console.WriteLine("Knowledge Graph created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await session.CloseAsync();
                await driver.CloseAsync();
            }
        }
    }
}