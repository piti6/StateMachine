using VContainer;

namespace Misokatsu.Framework
{
    public sealed class ObjectResolverFactory<T> : IFactory<T>
    {
        private readonly IObjectResolver _resolver;

        public ObjectResolverFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public T Create()
        {
            return _resolver.CreateInstance<T>();
        }
    }

    public sealed class ObjectResolverFactory<TParam, TResult> : IFactory<TParam, TResult>
    {
        private readonly IObjectResolver _resolver;

        public ObjectResolverFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public TResult Create(TParam param)
        {
            return _resolver.CreateInstance<TParam, TResult>(param);
        }
    }
}
