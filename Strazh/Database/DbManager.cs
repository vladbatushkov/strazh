using Neo4j.Driver;
using System;
using System.Threading.Tasks;
using Strazh.Domain;
using static Strazh.Analysis.AnalyzerConfig;
using System.Collections.Generic;

namespace Strazh.Database
{
    public static class DbManager
    {
        private const string CONNECTION = "neo4j://localhost:7687";

        public static async Task InsertData(IList<Triple> triples, CredentialsConfig credentials, bool isOverride = true)
        {
            if (credentials == null)
            {
                throw new ArgumentException($"Please, provide credentials.");
            }
            Console.WriteLine($"Connecting to Neo4j database={credentials.Database}, user={credentials.User}, password={credentials.Password}");
            var driver = GraphDatabase.Driver(CONNECTION, AuthTokens.Basic(credentials.User, credentials.Password));
            var session = driver.AsyncSession(o => o.WithDatabase(credentials.Database));
            try
            {
                if (isOverride)
                {
                    await session.RunAsync("MATCH (n) DETACH DELETE n;");
                    Console.WriteLine($"Database \"{credentials.Database}\" is cleaned.");
                }
                foreach (var triple in triples)
                {
                    //Console.WriteLine($"Executing command: {triple}");
                    await session.RunAsync(triple.ToString());
                }
                Console.WriteLine($"Merged {triples.Count} triples.");
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