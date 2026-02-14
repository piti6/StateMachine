using System;
using System.Collections.Generic;
using VContainer;
using VContainer.Internal;

public static class ContainerBuilderExtensions
{
    public static T CreateInstance<T>(this IObjectResolver container) =>
        (T)container.CreateInstance(typeof(T));

    public static T CreateInstance<T>(this IObjectResolver container, IReadOnlyList<IInjectParameter> parameters) =>
        (T)container.CreateInstance(typeof(T), parameters);
    
    public static T CreateInstance<Parameter1, T>(this IObjectResolver container, Parameter1 parameter) =>
        (T)container.CreateInstance(typeof(T), new []{ parameter.AsTypedParameter() });
    
    public static T CreateInstance<Parameter1, Parameter2, T>(this IObjectResolver container, Parameter1 parameter1, Parameter2 parameter2) =>
        (T)container.CreateInstance(typeof(T), new []{ parameter1.AsTypedParameter(), parameter2.AsTypedParameter() });
    
    public static T CreateInstance<Parameter1, Parameter2, Parameter3, T>(this IObjectResolver container, Parameter1 parameter1, Parameter2 parameter2, Parameter3 parameter3) =>
        (T)container.CreateInstance(typeof(T), new []{ parameter1.AsTypedParameter(), parameter2.AsTypedParameter(), parameter3.AsTypedParameter() });

    public static object CreateInstance(this IObjectResolver container, Type type) =>
        container.CreateInstance(type, null);

    public static object CreateInstance(this IObjectResolver container, Type type, IReadOnlyList<IInjectParameter> parameters)
    {
        var injector = InjectorCache.GetOrBuild(type);
        return injector.CreateInstance(container, parameters);
    }
    
    private static NamedParameter AsNamedParameter(this object value, string name)
    {
        return new NamedParameter(name, value);
    }

    private static TypedParameter AsTypedParameter<T>(this T value)
    {
        return new TypedParameter(typeof(T), value);
    }
}
