namespace Strazh.Domain
{
    public abstract class Triple
    {
        public Node NodeA { get; set; }

        public Node NodeB { get; set; }

        public Relationship Relationship { get; set; }

        protected Triple(Node nodeA, Node nodeB, Relationship relationship)
        {
            NodeA = nodeA;
            NodeB = nodeB;
            Relationship = relationship;
        }

        public override string ToString()
            => $"MERGE (a:{NodeA.Label} {{ {NodeA.Match()} }}) ON CREATE SET {NodeA.Set("a")} ON MATCH SET {NodeA.Set("a")} MERGE (b:{NodeB.Label} {{ {NodeB.Match()} }}) ON CREATE SET {NodeB.Set("b")} ON MATCH SET {NodeB.Set("b")} MERGE (a)-[:{Relationship.Type}]->(b);";
    }

    // Structure

    public class TripleDependsOnProject : Triple
    {
        public TripleDependsOnProject(
            ProjectNode projectA,
            ProjectNode projectB)
            : base(projectA, projectB, new DependsOnRelationship())
        { }
    }

    public class TripleDependsOnPackage : Triple
    {
        public TripleDependsOnPackage(
            ProjectNode projectA,
            PackageNode packageB)
            : base(projectA, packageB, new DependsOnRelationship())
        { }
    }

    public class TripleIncludedIn : Triple
    {
        public TripleIncludedIn(
            ProjectNode contentA,
            FolderNode contentB)
            : base(contentA, contentB, new IncludedInRelationship())
        { }

        public TripleIncludedIn(
            FolderNode contentA,
            FolderNode contentB)
            : base(contentA, contentB, new IncludedInRelationship())
        { }

        public TripleIncludedIn(
            FileNode contentA,
            FolderNode contentB)
            : base(contentA, contentB, new IncludedInRelationship())
        { }
    }

    public class TripleDeclaredAt : Triple
    {
        public TripleDeclaredAt(
            CodeNode codeA,
            FileNode fileB)
            : base(codeA, fileB, new DeclaredAtRelationship())
        { }
    }

    // Code

    public class TripleInvoke : Triple
    {
        public TripleInvoke(
            MethodNode methodA,
            MethodNode methodB)
            : base(methodA, methodB, new InvokeRelationship())
        { }
    }

    public class TripleInvokeAsClass : Triple
    {
        public TripleInvokeAsClass(
            MethodNode methodA,
            ClassNode classB)
            : base(methodA, classB, new InvokeRelationship())
        { }
    }

    public class TripleHave : Triple
    {
        public TripleHave(
            Node nodeA,
            MethodNode methodB)
            : base(nodeA, methodB, new HaveRelationship())
        { }
    }

    public class TripleClassHave : Triple
    {
        public TripleClassHave(
            ClassNode classA,
            MethodNode methodB)
            : base(classA, methodB, new HaveRelationship())
        { }
    }

    public class TripleInterfaceHave : Triple
    {
        public TripleInterfaceHave(
            InterfaceNode interfaceA,
            MethodNode methodB)
            : base(interfaceA, methodB, new HaveRelationship())
        { }
    }

    public class TripleConstruct : Triple
    {
        public TripleConstruct(
            ClassNode classA,
            ClassNode classB)
            : base(classA, classB, new ConstructRelationship())
        { }

        public TripleConstruct(
            MethodNode methodA,
            ClassNode classB)
            : base(methodA, classB, new ConstructRelationship())
        { }
    }

    public class TripleInherit : Triple
    {
        public TripleInherit(
            Node nodeA,
            Node nodeB)
            : base(nodeA, nodeB, new OfTypeRelationship())
        { }
    }
}