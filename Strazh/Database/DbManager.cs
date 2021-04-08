using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Strazh.Domain;

namespace Strazh.Database
{
    public static class DbManager
    {
        private const string CONNECTION = "neo4j://localhost:7687";

        static DbManager()
        {
#if DEBUG
            // There is an issue with using Neo4j.Driver 4.2.0
            // System.IO.FileNotFoundException: Could not load file or assembly '4.2.37.0'. The system cannot find the file specified.
            // Workaround to load assembly and avoid issue 
            System.Reflection.Assembly.Load("Neo4j.Driver");
#endif
        }

        private static string[] ParseCredentials(string credentials)
        {
            if (!string.IsNullOrEmpty(credentials))
            {
                var args = credentials.Split(":");
                if (args.Length == 3)
                {
                    return args;
                }
            }
            return null;
        }

        public static async Task InsertData(IEnumerable<Triple> triples, string credentials, bool isOverride = true)
        {
            var cred = ParseCredentials(credentials);
            if (cred == null)
            {
                throw new ArgumentException($"Please, provide credentials.");
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
                    Console.WriteLine($"Executing command: {triple}");
                    await session.RunAsync(triple.ToString());
                }
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