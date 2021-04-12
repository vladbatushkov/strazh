using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using Strazh.Domain;

namespace Strazh.Analysis
{
    public static class Extractor
    {
        public static Node CreateNode(this ISymbol symbol, TypeDeclarationSyntax declaration)
        {
            (string fullName, string name) raw = (symbol.ContainingNamespace.ToString() + '.' + symbol.Name, symbol.Name);
            switch (declaration)
            {
                case ClassDeclarationSyntax _:
                    return new ClassNode(raw.fullName, raw.name, symbol.IsStatic);
                case InterfaceDeclarationSyntax _:
                    return new InterfaceNode(raw.fullName, raw.name);
            }
            return null;
        }

        public static ClassNode CreateClassNode(this TypeInfo typeInfo)
            => new ClassNode(GetFullName(typeInfo), GetName(typeInfo), typeInfo.Type.IsStatic);

        public static InterfaceNode CreateInterfaceNode(this TypeInfo typeInfo)
            => new InterfaceNode(GetFullName(typeInfo), GetName(typeInfo));

        public static Node CreateNode(this TypeInfo typeInfo)
        {
            switch (typeInfo.ConvertedType.TypeKind)
            {
                case TypeKind.Interface:
                    return CreateClassNode(typeInfo);

                case TypeKind.Class:
                    return CreateInterfaceNode(typeInfo);

                default:
                    return null;
            }
        }

        public static string GetName(this TypeInfo typeInfo)
            => typeInfo.Type.Name;

        public static string GetFullName(this TypeInfo typeInfo)
            => typeInfo.Type.ContainingNamespace.ToString() + "." + GetName(typeInfo);

        public static MethodNode CreateMethodNode(this IMethodSymbol symbol)
        {
            var fullName = symbol.ContainingSymbol.ToString() + '.' + symbol.Name;
            var args = symbol.Parameters.Select(x => {
                var type = x.Type.ToString();
                return (name: x.Name, type);
            }).ToArray();
            return new MethodNode(fullName, symbol.Name, args, "");
        }

        public static string[] ChainKey(this IMethodSymbol symbol, string str)
            => new string[] { str }.Union(symbol.Parameters.Select(x => x.Type.ToString())).ToArray();


        /// <summary>
        /// Entry to analyze class or interface
        /// </summary>
        public static void AnalyzeTree<T>(IList<Triple> triples, SyntaxTree st, SemanticModel sem)
            where T : TypeDeclarationSyntax
        {
            var declarations = st.GetRoot().DescendantNodes().OfType<T>();
            foreach (var declaration in declarations)
            {
                var node = sem.GetDeclaredSymbol(declaration).CreateNode(declaration);
                if (node != null)
                {
                    GetInherits(triples, declaration, sem, node);
                    GetMethodsAll(triples, declaration, sem, node);
                }
            }
        }

        /// <summary>
        /// Member (field, property) initialization
        /// </summary>
        public static void GetConstructsWithinClass(IList<Triple> triples, ClassDeclarationSyntax classDeclaration, SemanticModel sem, ClassNode classNode)
        {
            var creates = classDeclaration.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            foreach (var creation in creates)
            {
                var node = sem.GetTypeInfo(creation).CreateClassNode();
                triples.Add(new TripleConstruct(classNode, node));
            }
        }

        /// <summary>
        /// Type inherited from BaseType
        /// </summary>
        public static void GetInherits(IList<Triple> triples, TypeDeclarationSyntax typeDeclaration, SemanticModel sem, Node node)
        {
            if (typeDeclaration.BaseList != null)
            {
                foreach (var baseTypeSyntax in typeDeclaration.BaseList.Types)
                {
                    var parentNode = sem.GetTypeInfo(baseTypeSyntax.Type).CreateNode();
                    if (parentNode != null)
                    {
                        triples.Add(new TripleInherit(node, parentNode));
                    }
                }
            }
        }

        /// <summary>
        /// Class or Interface have some method AND some method can call another method AND some method can creates an object of class
        /// </summary>
        public static void GetMethodsAll(IList<Triple> triples, TypeDeclarationSyntax declarationSyntax, SemanticModel sem, Node node)
        {
            var methods = declarationSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var method in methods)
            {
                var methodNode = sem.GetDeclaredSymbol(method).CreateMethodNode();
                triples.Add(new TripleHave(node, methodNode));

                foreach (var syntax in method.DescendantNodes().OfType<ExpressionSyntax>())
                {
                    switch (syntax)
                    {
                        case ObjectCreationExpressionSyntax creation:
                            var classNode = sem.GetTypeInfo(creation).CreateClassNode();
                            triples.Add(new TripleConstruct(methodNode, classNode));
                            break;

                        case InvocationExpressionSyntax invocation:
                            var invokedSymbol = sem.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                            if (invokedSymbol != null)
                            {
                                var invokedMethod = invokedSymbol.CreateMethodNode();
                                triples.Add(new TripleInvoke(methodNode, invokedMethod));
                            }
                            break;
                    }
                }
            }
        }
    }
}