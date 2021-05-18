using Neo4j.Driver;
using System;
using System.Threading.Tasks;
using Strazh.Domain;
using System.Collections.Generic;
using static Strazh.Analysis.AnalyzerConfig;

namespace Strazh.Database
{
    public static class DbManager
    {
        private const string CONNECTION = "neo4j://localhost:7687";

        public static async Task InsertData(IList<Triple> triples, CredentialsConfig credentials, bool isDelete)
        {
            if (credentials == null)
            {
                throw new ArgumentException($"Please, provide credentials.");
            }
            Console.WriteLine($"Code Knowledge Graph use \"{credentials.Database}\" Neo4j database.");
            var driver = GraphDatabase.Driver(CONNECTION, AuthTokens.Basic(credentials.User, credentials.Password));
            var session = driver.AsyncSession(o => o.WithDatabase(credentials.Database));
            try
            {
                if (isDelete)
                {
                    Console.WriteLine($"Deleting graph data of \"{credentials.Database}\" database...");
                    await session.RunAsync("MATCH (n) DETACH DELETE n;");
                    Console.WriteLine($"Deleting graph data of \"{credentials.Database}\" database complete.");
                }
                Console.WriteLine($"Processing {triples.Count} triples...");
                foreach (var triple in triples)
                {
                    await session.RunAsync(triple.ToString());
                }
                Console.WriteLine($"Processing {triples.Count} triples complete.");
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