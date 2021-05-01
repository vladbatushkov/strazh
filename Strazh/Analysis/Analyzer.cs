using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis;
using Strazh.Domain;
using Buildalyzer;
using Buildalyzer.Workspaces;
using System.Collections.Generic;
using System;
using Strazh.Database;
using static Strazh.Analysis.AnalyzerConfig;

namespace Strazh.Analysis
{
    public static class Analyzer
    {
        public static async Task Analyze(AnalyzerConfig config)
        {
            Console.WriteLine($"Setup analyzer...");

            var manager = config.IsSolutionBased
                ? new AnalyzerManager(config.Solution)
                : new AnalyzerManager();

            var projectAnalyzers = config.IsSolutionBased
                ? manager.Projects.Values
                : config.Projects.Select(x => manager.GetProject(x));

            Console.WriteLine($"Analyzer ready to analyze {projectAnalyzers.Count()} project/s.");

            var workspace = new AdhocWorkspace();
            var isDelete = config.IsDelete;
            short index = 1;
            foreach (var projectAnalyzer in projectAnalyzers)
            {
                var triples = await AnalyzeProject(index, workspace, projectAnalyzer, config.Mode);
                if (triples.Count > 0)
                {
                    await DbManager.InsertData(triples, config.Credentials, isDelete);
                }
                index++;
                isDelete = false;
            }
            workspace.Dispose();
        }

        private static async Task<IList<Triple>> AnalyzeProject(short index, AdhocWorkspace workspace, IProjectAnalyzer projectAnalyzer, AnalyzeMode mode)
        {
            Console.WriteLine($"Project #{index}:");
            var project = projectAnalyzer.AddToWorkspace(workspace);
            var root = GetRoot(project.FilePath);
            var rootNode = new FolderNode(root, root);
            var projectName = GetProjectName(project.Name);
            Console.WriteLine($"Analyzing {projectName} project...");

            var triples = new List<Triple>();
            if (mode == AnalyzeMode.All || mode == AnalyzeMode.Structure)
            {
                Console.WriteLine($"Analyzing Structure level...");
                var projectBuild = projectAnalyzer.Build().FirstOrDefault();
                var projectNode = new ProjectNode(projectName);
                triples.Add(new TripleIncludedIn(projectNode, rootNode));
                projectBuild.ProjectReferences.ToList().ForEach(x =>
                {
                    var node = new ProjectNode(GetProjectName(x));
                    triples.Add(new TripleDependsOnProject(projectNode, node));
                });
                projectBuild.PackageReferences.ToList().ForEach(x =>
                {
                    var version = x.Value.Values.FirstOrDefault(x => x.Contains(".")) ?? "none";
                    var node = new PackageNode(x.Key, x.Key, version);
                    triples.Add(new TripleDependsOnPackage(projectNode, node));
                });
                Console.WriteLine($"Analyzing Structure level complete.");
            }

            if (project.SupportsCompilation
                && (mode == AnalyzeMode.All || mode == AnalyzeMode.Code))
            {
                Console.WriteLine($"Analyzing Code level...");
                var compilation = await project.GetCompilationAsync();
                var syntaxTreeRoot = compilation.SyntaxTrees;
                foreach (var st in syntaxTreeRoot)
                {
                    var sem = compilation.GetSemanticModel(st);
                    Extractor.AnalyzeTree<ClassDeclarationSyntax>(triples, st, sem, rootNode);
                    Extractor.AnalyzeTree<InterfaceDeclarationSyntax>(triples, st, sem, rootNode);
                }
                Console.WriteLine($"Analyzing Code level complete.");
                triples = triples.GroupBy(x => x.ToString()).Select(x => x.First()).ToList();
            }

            Console.WriteLine($"Analyzing {projectName} project complete.");
            return triples;
        }

        private static string GetProjectName(string fullName)
            => fullName.Split('\\').Last().Replace(".csproj", "");

        private static string GetRoot(string filePath)
            => filePath.Split("\\").Reverse().Skip(1).FirstOrDefault();
    }
}