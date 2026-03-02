using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

namespace Misokatsu.Framework
{
    public static class ContainerBuilderExtensions
    {
        public static RegistrationBuilder RegisterAsImplementedInterfacesAndSelf<T>(
            this IContainerBuilder builder,
            Lifetime lifetime)
        {
            return builder.Register<T>(lifetime)
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}
