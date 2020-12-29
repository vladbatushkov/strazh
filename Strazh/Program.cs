using System;
using System.Linq;
using System.Threading.Tasks;
using Strazh.Analysis;

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
                var items = await Analyzer.Analyze(args[0]);
                foreach (var item in items)
                {
                    Console.WriteLine(item.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
