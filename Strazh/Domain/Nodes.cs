using System.Linq;

namespace Strazh.Domain
{
    public class Node
    {
        public virtual string Label { get; }

        public virtual string FullName { get; }

        public virtual string Name { get; }

        /// <summary>
        /// Primary Key used to compare Matching of nodes on MERGE operation
        /// </summary>
        public virtual string Pk { get; protected set; }

        public Node(string fullName, string name)
        {
            FullName = fullName;
            Name = name;
            SetPrimaryKey();
        }

        protected virtual void SetPrimaryKey()
        {
            Pk = FullName.GetHashCode().ToString();
        }

        public string Match()
            => $"pk: \"{Pk}\"";

        public virtual string Set(string node)
            => $"{node}.pk = \"{Pk}\", {node}.fullName = \"{FullName}\", {node}.name = \"{Name}\"";
    }

    // Code

    public abstract class CodeNode : Node
    {
        public CodeNode(string fullName, string name, string[] modifiers = null)
            : base(fullName, name)
        {

            Modifiers = modifiers == null ? "" : string.Join(", ", modifiers);
        }

        public string Modifiers { get; }

        public override string Set(string node)
            => $"{base.Set(node)}{(string.IsNullOrEmpty(Modifiers) ? "" : $", {node}.modifiers = \"{Modifiers}\"")}";
    }

    public class ClassNode : CodeNode
    {
        public ClassNode(string fullName, string name, string[] modifiers = null)
            : base(fullName, name, modifiers)
        {
        }

        public override string Label { get; } = "Class";
    }

    public class InterfaceNode : CodeNode
    {
        public InterfaceNode(string fullName, string name, string[] modifiers = null)
            : base(fullName, name, modifiers)
        {
        }

        public override string Label { get; } = "Interface";
    }

    public class MethodNode : CodeNode
    {
        public MethodNode(string fullName, string name, (string name, string type)[] args, string returnType, string[] modifiers = null)
            : base(fullName, name, modifiers)
        {
            Arguments = string.Join(", ", args.Select(x => $"{x.type} {x.name}"));
            ReturnType = returnType;
        }

        public override string Label { get; } = "Method";

        public string Arguments { get; }

        public string ReturnType { get; }

        public override string Set(string node)
            => $"{base.Set(node)}, {node}.returnType = \"{ReturnType}\", {node}.arguments = \"{Arguments}\"";

        protected override void SetPrimaryKey()
        {
            Pk = $"{FullName}{Arguments}{ReturnType}".GetHashCode().ToString();
        }
    }

    // Structure

    public abstract class StructureNode : Node
    {
        public StructureNode(string fullName, string name)
            : base(fullName, name)
        {
        }
    }

    public class FileNode : StructureNode
    {
        public FileNode(string fullName, string name)
            : base(fullName, name) { }

        public override string Label { get; } = "File";
    }

    public class FolderNode : StructureNode
    {
        public FolderNode(string fullName, string name)
            : base(fullName, name) { }

        public override string Label { get; } = "Folder";
    }

    public class ProjectNode : StructureNode
    {
        public ProjectNode(string name)
            : this(name, name) { }

        public ProjectNode(string fullName, string name)
            : base(fullName, name) { }

        public override string Label { get; } = "Project";
    }

    public class PackageNode : StructureNode
    {
        public PackageNode(string fullName, string name, string version)
            : base(fullName, name)
        {
            Version = version;
        }

        public override string Label { get; } = "Package";

        public string Version { get; }

        public override string Set(string node)
            => $"{base.Set(node)}, {node}.version = \"{Version}\"";

        protected override void SetPrimaryKey()
        {
            Pk = $"{FullName}{Version}".GetHashCode().ToString();
        }
    }
}