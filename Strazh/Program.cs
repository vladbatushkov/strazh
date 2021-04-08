using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using Strazh.Analysis;
using Strazh.Database;

namespace Strazh
{
    public class Program
    {
        public static async Task Main(params string[] args)
        {
            var rootCommand = new RootCommand();

            var optionCred = new Option<string>("--credentials", "credentials as `dbname:user:password` to connect to Neo4j batabase");
            optionCred.AddAlias("-cs");
            optionCred.IsRequired = true;
            rootCommand.Add(optionCred);

            var optionPath = new Option<string[]>("--pathlist", "list of absolute path to .csproj files");
            optionPath.AddAlias("-pl");
            optionPath.IsRequired = true;
            rootCommand.Add(optionPath);

            rootCommand.Handler = CommandHandler.Create<string, string[]>(BuildKnowledgeGraph);
            await rootCommand.InvokeAsync(args);
        }

        private static async Task BuildKnowledgeGraph(string credentials, string[] pathlist)
        {
            try
            {
                var isNeo4jReady = await Healthcheck.IsNeo4jReady();
                if (!isNeo4jReady)
                {
                    Console.WriteLine("Strazh disappointed. There is no Neo4j instance ready to use.");
                    return;
                }
                Console.WriteLine("Brewing the Code Knowledge Graph.");
                var isOverride = true;
                foreach (var path in pathlist)
                {
                    var triples = await Analyzer.Analyze(path);
                    if (triples.Count() > 0)
                    {
                        await DbManager.InsertData(triples, credentials, isOverride);
                    }
                    isOverride = false;
                }
                Console.WriteLine("Code Knowledge Graph created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
