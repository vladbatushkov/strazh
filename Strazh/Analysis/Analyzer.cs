using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis;
using Strazh.Domain;
using Buildalyzer;
using Buildalyzer.Workspaces;
using System.Collections.Generic;
using System;

namespace Strazh.Analysis
{
    public static class Analyzer
    {
        private static string GetProjectName(string fullName)
        {
            return fullName.Split('\\').Last().Replace(".csproj", "");
        }

        public static async Task<Triple[]> Analyze(string path)
        {
            var triples = new List<Triple>();

            var manager = new AnalyzerManager();
            var analyzer = manager.GetProject(path);
            var workspace = new AdhocWorkspace();
            var project = analyzer.AddToWorkspace(workspace);

            var projectBuild = analyzer.Build().FirstOrDefault();
            var currentNode = new ProjectNode(GetProjectName(project.Name));
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

            var compilation = await project.GetCompilationAsync();
            var syntaxTreeRoot = compilation.SyntaxTrees;
            foreach (var st in syntaxTreeRoot)
            {
                var sem = compilation.GetSemanticModel(st);
                Extractor.AnalyzeTree<ClassDeclarationSyntax>(triples, st, sem);
                Extractor.AnalyzeTree<InterfaceDeclarationSyntax>(triples, st, sem);
            }
            var result = triples.GroupBy(x => x.ToString()).Select(x => x.First()).ToArray();
            Console.WriteLine($"Codebase of project \"{currentNode.Name}\" analyzed with result of {result.Length} triples.");
            return result;
        }
    }
}