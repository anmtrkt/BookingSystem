using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Core.Domain.Events
{
    
        public static class DomainEvents
        {
            private static readonly List<Action<object>> Handlers = new List<Action<object>>();

            public static void Register<T>(Action<T> handler) where T : class
            {
                Handlers.Add(e => handler(e as T));
            }

            public static void Raise<T>(T domainEvent) where T : class
            {
                foreach (var handler in Handlers)
                {
                    handler(domainEvent);
                }
            }
        }

    
}
