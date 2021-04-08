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
        private static IList<Triple> Triples;

        public static async Task<IEnumerable<Triple>> Analyze(string path)
        {
            var manager = new AnalyzerManager();
            var analyzer = manager.GetProject(path);
            var workspace = new AdhocWorkspace();
            var roslynProject = analyzer.AddToWorkspace(workspace);
            var compilation = await roslynProject.GetCompilationAsync();

            Triples = new List<Triple>();
            foreach (var st in compilation.SyntaxTrees)
            {
                var sem = compilation.GetSemanticModel(st);
                Extractor.AnalyzeTree<ClassDeclarationSyntax>(Triples, st, sem);
                Extractor.AnalyzeTree<InterfaceDeclarationSyntax>(Triples, st, sem);
            }
            var result = Triples.GroupBy(x => x.ToString()).Select(x => x.First());
            Console.WriteLine($"Codebase of {path} analyzed with result of {result.Count()} triples.");
            return result;
        }
    }
}