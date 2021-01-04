using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Strazh.Analysis;
using Strazh.Database;

namespace Strazh
{
    class Program
    {
        static async Task Main(params string[] args)
        {
            var rootCommand = new RootCommand();

            var optionPath = new Option<string>("--path", "absolute path to .csproj file");
            optionPath.AddAlias("-p");
            optionPath.IsRequired = true;
            rootCommand.Add(optionPath);

            var optionCred = new Option<string>("--credentials", "credentials of `dbname:user:password` to connect to Neo4j batabase");
            optionCred.AddAlias("-c");
            rootCommand.Add(optionCred);

            rootCommand.Handler = CommandHandler.Create<string, string>(BuildKnowledgeGraph);

            await rootCommand.InvokeAsync(args);
        }

        static async Task BuildKnowledgeGraph(string path, string credentials)
        {
            try
            {
                var isNeo4jReady = await Healthcheck.IsNeo4jReady();
                if (!isNeo4jReady)
                {
                    Console.WriteLine("Strazh disappointed. There is no Neo4j instance ready to use.");
                    return;
                }
                Console.WriteLine("Brewing the Knowledge Graph.");
                var triples = await Analyzer.Analyze(path);
                await DbManager.InsertData(triples, credentials);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
