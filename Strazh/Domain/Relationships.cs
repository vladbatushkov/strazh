namespace Strazh.Domain
{
    public abstract class Relationship
    {
        public virtual string Type { get; }
    }

    public class HaveRelationship : Relationship
    {
        public override string Type => "HAVE";
    }

    public class InvokeRelationship : Relationship
    {
        public override string Type => "INVOKE";
    }

    public class ConstructRelationship : Relationship
    {
        public override string Type => "CONSTRUCT";
    }

    public class OfTypeRelationship : Relationship
    {
        public override string Type => "OF_TYPE";
    }

    public class DependsOnRelationship : Relationship
    {
        public override string Type => "DEPENDS_ON";
    }
}