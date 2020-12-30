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
        private const string PASSWORD = "strazh";
        private const string CONNECTION = "neo4j://localhost:7687";

        public static async Task InsertData(IEnumerable<Triple> triples, bool isOverride = true)
        {
            var driver = GraphDatabase.Driver(CONNECTION, AuthTokens.Basic(USER, PASSWORD));
            var session = driver.AsyncSession(o => o.WithDatabase(DBNAME));
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