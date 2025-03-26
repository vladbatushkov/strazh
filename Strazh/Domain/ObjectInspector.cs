namespace Strazh.Domain;

public static class ObjectInspector
{
    public static string Inspect(this object? instance)
    {
        const string nullResponse = "null";
        if (instance == null)
        {
            return nullResponse;
        }

        if (instance is IInspectable inspectable)
        {
            return inspectable.ToInspection();
        }

        var instanceAsString = instance.ToString();
        return instanceAsString != null ? $"\"{instanceAsString}\"" : nullResponse;
    }
}