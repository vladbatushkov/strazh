using System;
using Microsoft.CodeAnalysis;

namespace Strazh.Domain
{
    public class Node
    {
        public virtual string Label { get; set; }

        public virtual string FullName { get; set; }

        public virtual string Name { get; set; }

        /// <summary>
        /// Primary Key
        /// </summary>
        public virtual string Pk { get; private set; }

        public Node(string[] str, string fullName, string name)
        {
            CreatePrimaryKey(string.Join(string.Empty, str));
            FullName = fullName;
            Name = name;
        }

        public Node(string fullName, string name)
        {
            CreatePrimaryKey(fullName);
            FullName = fullName;
            Name = name;
        }

        private void CreatePrimaryKey(string pk)
        {
            Pk = pk.GetHashCode().ToString();
        }

        public string Match()
            => $"pk: \"{Pk}\"";

        public string Set(string node)
            => $"{node}.pk = \"{Pk}\", {node}.fullName = \"{FullName}\", {node}.name = \"{Name}\"";
    }

    public class ClassNode : Node
    {
        public ClassNode(string fullName, string name) : base(fullName, name) { }

        public override string Label { get; set; } = "Class";
    }

    public class InterfaceNode : Node
    {
        public InterfaceNode(string fullName, string name) : base(fullName, name) { }

        public override string Label { get; set; } = "Interface";
    }

    public class MethodNode : Node
    {
        public MethodNode(string[] str, string fullName, string name) : base(str, fullName, name) { }

        public override string Label { get; set; } = "Method";
    }

    public class ModuleNode : Node
    {
        public ModuleNode(string fullName, string name) : base(fullName, name) { }

        public override string Label { get; set; } = "Module";
    }
}