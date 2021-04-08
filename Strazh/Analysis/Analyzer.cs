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
        public static async Task<IEnumerable<Triple>> Analyze(string path)
        {
            var manager = new AnalyzerManager();
            var analyzer = manager.GetProject(path);
            var workspace = new AdhocWorkspace();
            var project = analyzer.AddToWorkspace(workspace);
            var compilation = await project.GetCompilationAsync();

            var triples = new List<Triple>();
            foreach (var st in compilation.SyntaxTrees)
            {
                var sem = compilation.GetSemanticModel(st);
                Extractor.AnalyzeTree<ClassDeclarationSyntax>(triples, st, sem);
                Extractor.AnalyzeTree<InterfaceDeclarationSyntax>(triples, st, sem);
            }
            var result = triples.GroupBy(x => x.ToString()).Select(x => x.First());
            Console.WriteLine($"Codebase of {path} analyzed with result of {result.Count()} triples.");
            return result;
        }
    }
}