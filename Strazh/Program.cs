using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Strazh.Analysis;

namespace Strazh
{
    public class Program
    {

        public static async Task Main(params string[] args)
        {
#if DEBUG
            // There is an issue with using Neo4j.Driver 4.2.0
            // System.IO.FileNotFoundException: Could not load file or assembly '4.2.37.0'. The system cannot find the file specified.
            // Workaround to load assembly and avoid issue 
            System.Reflection.Assembly.Load("Neo4j.Driver");
#endif
            var rootCommand = new RootCommand();

            var optionCredentials = new Option<string>("--credentials", "required flag of credentials as `dbname:user:password` to connect to Neo4j database");
            optionCredentials.AddAlias("-c");
            optionCredentials.IsRequired = true;
            rootCommand.Add(optionCredentials);

            var optionMode = new Option<string>("--mode", "optional flag of mode as `code` or `structure` or no flag (mean both `code` and `structure`) to explicitly limited scan of codebase");
            optionMode.AddAlias("-m");
            optionMode.IsRequired = false;
            rootCommand.Add(optionMode);

            var optionSolution = new Option<string>("--solution", "optional absolute path to only one .sln file (can't be used together with -p / --projects)");
            optionSolution.AddAlias("-s");
            optionSolution.IsRequired = false;
            rootCommand.Add(optionSolution);

            var optionProjects = new Option<string[]>("--projects", "optional list of absolute path to one or many .csproj files (can't be used together with -s / --solution)");
            optionProjects.AddAlias("-p");
            optionProjects.IsRequired = false;
            rootCommand.Add(optionProjects);

            rootCommand.Handler = CommandHandler.Create<string, string, string, string[]>(BuildKnowledgeGraph);
            await rootCommand.InvokeAsync(args);
        }

        private static async Task BuildKnowledgeGraph(string credentials, string mode, string solution, string[] projects)
        {
            try
            {
                var config = new AnalyzerConfig(
                       credentials,
                       mode,
                       solution,
                       projects
                   );
                if (!config.IsValid)
                {
                    Console.WriteLine("Please submit one thing: `--solution` (-s) or `--projects` (-p)");
                    return;
                }
                var isNeo4jReady = await Healthcheck.IsNeo4jReady();
                if (!isNeo4jReady)
                {
                    Console.WriteLine("Strazh disappointed. There is no Neo4j instance ready to use.");
                    return;
                }

                Console.WriteLine($"Brewing a Code Knowledge Graph in \"{config.Mode}\" mode.");                
                await Analyzer.Analyze(config);
                Console.WriteLine("Code Knowledge Graph created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
