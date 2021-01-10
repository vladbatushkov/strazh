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
            var roslynProject = analyzer.AddToWorkspace(workspace);
            var compilation = await roslynProject.GetCompilationAsync();

            var triples = new List<Triple>();
            foreach (var st in compilation.SyntaxTrees)
            {
                var sem = compilation.GetSemanticModel(st);
                triples.AddRange(Extractor.AnalyzeTree<ClassDeclarationSyntax>(st, sem).SelectMany(x => x));
                triples.AddRange(Extractor.AnalyzeTree<InterfaceDeclarationSyntax>(st, sem).SelectMany(x => x));
            }
            Console.WriteLine($"Codebase of {path} analyzed.");
            return triples.GroupBy(x => x.ToString()).Select(x => x.First());
        }
    }
}