using System;
using System.Linq;
using System.Threading.Tasks;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Strazh
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var manager = new AnalyzerManager();
            var analyzer = manager.GetProject(@"/Users/vladbatushkov/Documents/src/github/strazh/Fakelib/Fakelib.csproj");
            var workspace = new AdhocWorkspace();
            var roslynProject = analyzer.AddToWorkspace(workspace);
            var compilation = await roslynProject.GetCompilationAsync();
            foreach (var st in compilation.SyntaxTrees)
            {
                var sem = compilation.GetSemanticModel(st);
                AnalyzeTree<ClassDeclarationSyntax>(st, sem, (n) => Console.WriteLine(n));
            }
        }

        public class Node
        {
            public string FullName { get; set; }
            public string Name { get; set; }

            public override string ToString()
                => $"FullName={FullName}, Name={Name}";
        }

        private static void AnalyzeTree<T>(SyntaxTree st, SemanticModel sem, Action<Node> callback)
            where T : TypeDeclarationSyntax
        {
            (string code, string name) NameClass(ISymbol symbol)
                => (symbol.ContainingNamespace.ToString() + '.' + symbol.Name, symbol.Name);

            var declarations = st.GetRoot().DescendantNodes().OfType<T>();
            foreach (var declaration in declarations)
            {
                var classSymbol = sem.GetDeclaredSymbol(declaration);
                (string fullName, string name) = NameClass(classSymbol);
                callback(new Node { FullName = fullName, Name = name });
            }
        }
    }
}
