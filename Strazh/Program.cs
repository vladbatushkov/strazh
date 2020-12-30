using System;
using System.Threading.Tasks;
using Strazh.Analysis;
using Strazh.Database;

namespace Strazh
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var isNeo4jReady = await Healthcheck.IsNeo4jReady();
                if (!isNeo4jReady)
                {
                    Console.WriteLine("Strazh disappointed. There is no Neo4j instance ready to use.");
                    return;
                }
                Console.WriteLine("Brewing the Knowledge Graph...");
                var triples = await Analyzer.Analyze(args[0]);
                await DbManager.InsertData(triples);
                Console.WriteLine("Enjoy the Knowledge Graph of your codebase!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
