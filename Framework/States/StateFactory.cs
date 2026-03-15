using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using VContainer;

namespace Misokatsu.Framework
{
    public sealed class StateFactory<T> : ObjectResolverFactory<T> where T : State<T>
    {
        public StateFactory(IObjectResolver resolver) : base(resolver)
        {
        }
    }
    
    public sealed class StateFactory<TParam, TResult> : ObjectResolverFactory<TParam, TResult> where TParam : State<TParam>
    {
        public StateFactory(IObjectResolver resolver) : base(resolver)
        {
        }
    }
    
    public sealed class SceneStateFactory<TParam, TResult> : ObjectResolverFactory<TParam, TResult> where TParam : State<TParam>
    {
        public SceneStateFactory(IObjectResolver resolver) : base(resolver)
        {
        }
    }
}
