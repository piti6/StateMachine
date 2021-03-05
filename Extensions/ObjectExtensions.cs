using VContainer;

public static class ObjectExtensions
{
    public static NamedParameter AsNamedParameter(this object value, string name)
    {
        return new NamedParameter(name, value);
    }

    public static TypedParameter AsTypedParameter<T>(this T value)
    {
        return new TypedParameter(typeof(T), value);
    }
}
