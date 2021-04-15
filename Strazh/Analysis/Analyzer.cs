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
        private static string GetProjectName(string fullName)
        {
            return fullName.Split('\\').Last().Replace(".csproj", "");
        }

        public static async Task Analyze(AnalyzerConfig config)
        {
            var manager = config.IsSolutionBased
                ? new AnalyzerManager(config.Solution)
                : new AnalyzerManager();

            var projectAnalyzers = config.IsSolutionBased
                ? manager.Projects.Values
                : config.Projects.Select(x => manager.GetProject(x));

            var isOverride = true;
            Console.WriteLine($"Analyzing {projectAnalyzers.Count()} projects.");
            foreach (var projectAnalyzer in projectAnalyzers)
            {
                var triples = await AnalyzeProject(projectAnalyzer, config.Mode);
                if (triples.Count > 0)
                {
                    await DbManager.InsertData(triples, config.Credentials, isOverride);
                }
                isOverride = false;
            }
        }

        private static async Task<IList<Triple>> AnalyzeProject(IProjectAnalyzer projectAnalyzer, AnalyzeMode mode)
        {
            var triples = new List<Triple>();
            
            var workspace = new AdhocWorkspace();
            var project = projectAnalyzer.AddToWorkspace(workspace);
            var projectName = GetProjectName(project.Name);
            if (mode == AnalyzeMode.All || mode == AnalyzeMode.Structure)
            {
                var projectBuild = projectAnalyzer.Build().FirstOrDefault();
                var currentNode = new ProjectNode(projectName);
                projectBuild.ProjectReferences.ToList().ForEach(x =>
                {
                    var node = new ProjectNode(GetProjectName(x));
                    triples.Add(new TripleDependsOnProject(currentNode, node));
                });
                projectBuild.PackageReferences.ToList().ForEach(x =>
                {
                    var version = x.Value.Values.FirstOrDefault(x => x.Contains(".")) ?? "none";
                    var node = new PackageNode(x.Key, x.Key, version);
                    triples.Add(new TripleDependsOnPackage(currentNode, node));
                });
            }

            if (mode == AnalyzeMode.All || mode == AnalyzeMode.Code)
            {
                var compilation = await project.GetCompilationAsync();
                var syntaxTreeRoot = compilation.SyntaxTrees;
                foreach (var st in syntaxTreeRoot)
                {
                    var sem = compilation.GetSemanticModel(st);
                    Extractor.AnalyzeTree<ClassDeclarationSyntax>(triples, st, sem);
                    Extractor.AnalyzeTree<InterfaceDeclarationSyntax>(triples, st, sem);
                }
                triples = triples.GroupBy(x => x.ToString()).Select(x => x.First()).ToList();
            }

            Console.WriteLine($"Codebase of project \"{projectName}\" analyzed with result of {triples.Count} triples.");
            return triples;
        }
    }
}