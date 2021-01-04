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
        private const string USER = "neo4j";
        private const string PASSWORD = "test";
        private const string CONNECTION = "neo4j://localhost:7687";

        public static async Task InsertData(IEnumerable<Triple> triples, string credentials = null, bool isOverride = true)
        {
            var cred = new[] { DBNAME, USER, PASSWORD };
            if (!string.IsNullOrEmpty(credentials))
            {
                var args = credentials.Split(":");
                if (args.Length == 3)
                {
                    cred = args;
                }
            }
            Console.WriteLine($"Connecting to Neo4j database={cred[0]}, user={cred[1]}, password={cred[2]}");
            var driver = GraphDatabase.Driver(CONNECTION, AuthTokens.Basic(cred[1], cred[2]));
            var session = driver.AsyncSession(o => o.WithDatabase(cred[0]));
            try
            {
                if (isOverride)
                {
                    await session.RunAsync("MATCH (n) DETACH DELETE n;");
                    Console.WriteLine($"Database `{cred[0]}` is cleaned.");
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