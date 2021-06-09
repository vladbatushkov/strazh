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
using System.IO;

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
            var projects = new List<(Project, IProjectAnalyzer)>();

            var sortedProjectAnalyzers = projectAnalyzers; // TODO sort

            foreach (var projectAnalyzer in sortedProjectAnalyzers)
            {
                var project = projectAnalyzer.AddToWorkspace(workspace);
                projects.Add((project, projectAnalyzer));
            }
            for (var index = 0; index < projects.Count; index++)
            {
                var triples = await AnalyzeProject(index + 1, projects[index], config.Tier);
                triples = triples.GroupBy(x => x.ToString()).Select(x => x.First()).OrderBy(x => x.NodeA.Label).ToList();
                await DbManager.InsertData(triples, config.Credentials, config.IsDelete && index == 0);
            }
            workspace.Dispose();
        }

        private static async Task<IList<Triple>> AnalyzeProject(int index, (Project project, IProjectAnalyzer projectAnalyzer) item, Tiers mode)
        {
            Console.WriteLine($"Project #{index}:");
            var root = GetRoot(item.project.FilePath);
            var rootNode = new FolderNode(root, root);
            var projectName = GetProjectName(item.project.Name);
            Console.WriteLine($"Analyzing {projectName} project...");

            var triples = new List<Triple>();
            if (mode == Tiers.All || mode == Tiers.Project)
            {
                Console.WriteLine($"Analyzing Project tier...");
                var projectBuild = item.projectAnalyzer.Build().FirstOrDefault();
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
                Console.WriteLine($"Analyzing Project tier complete.");
            }

            if (item.project.SupportsCompilation
                && (mode == Tiers.All || mode == Tiers.Code))
            {
                Console.WriteLine($"Analyzing Code tier...");
                var compilation = await item.project.GetCompilationAsync();
                var syntaxTreeRoot = compilation.SyntaxTrees.Where(x => !x.FilePath.Contains("obj"));
                foreach (var st in syntaxTreeRoot)
                {
                    var sem = compilation.GetSemanticModel(st);
                    Extractor.AnalyzeTree<InterfaceDeclarationSyntax>(triples, st, sem, rootNode);
                    Extractor.AnalyzeTree<ClassDeclarationSyntax>(triples, st, sem, rootNode);
                }
                Console.WriteLine($"Analyzing Code tier complete.");
            }

            Console.WriteLine($"Analyzing {projectName} project complete.");
            return triples;
        }

        private static string GetProjectName(string fullName)
            => fullName.Split(Path.DirectorySeparatorChar).Last().Replace(".csproj", "");

        private static string GetRoot(string filePath)
            => filePath.Split(Path.DirectorySeparatorChar).Reverse().Skip(1).FirstOrDefault();
    }
}