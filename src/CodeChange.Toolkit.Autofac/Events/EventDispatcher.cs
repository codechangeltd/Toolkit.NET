namespace CodeChange.Toolkit.Autofac.Events
{
    using global::Autofac;
    using CodeChange.Toolkit.Domain.Events;
    using Nito.AsyncEx.Synchronous;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an Autofac implementation of an event dispatcher
    /// </summary>
    public sealed class EventDispatcher : IEventDispatcher
    {
        private readonly IComponentContext _container;
        private readonly MethodInfo _genericDispatcher;

        public EventDispatcher(IComponentContext context)
        {
            Validate.IsNotNull(context);

            _container = context;

            _genericDispatcher = GetType()
                .GetMethods()
                .First(x => x.Name == "Dispatch" && x.IsGenericMethod);
        }

        [Obsolete]
        public void Dispatch<T>(T @event, bool preTransaction = false) where T : IDomainEvent
        {
            DispatchAsync<T>(@event, preTransaction).WaitAndUnwrapException();
        }

        [Obsolete]
        public void Dispatch(IDomainEvent @event, bool preTransaction = false)
        {
            Validate.IsNotNull(@event);

            var dispatcher = _genericDispatcher.MakeGenericMethod(@event.GetType());

            dispatcher.Invoke
            (
                this,
                new object[]
                {
                    @event,
                    preTransaction
                }
            );
        }

        public async Task DispatchAsync<T>(T @event, bool preTransaction = false) where T : IDomainEvent
        {
            Validate.IsNotNull(@event);

            var allHandlers = _container.Resolve<IEnumerable<IDomainEventHandler<T>>>();
            var handleTasks = new List<Task>();

            foreach (var handler in allHandlers)
            {
                bool isMatch;

                if (preTransaction)
                {
                    isMatch = handler.IsPreTransaction();
                }
                else
                {
                    isMatch = handler.IsPostTransaction();
                }

                if (isMatch)
                {
                    handleTasks.Add(handler.Handle(@event));
                }
            }

            await Task.WhenAll(handleTasks).ConfigureAwait(false);
        }

        public async Task DispatchAsync(IDomainEvent @event, bool preTransaction = false)
        {
            Validate.IsNotNull(@event);

            var dispatcher = _genericDispatcher.MakeGenericMethod(@event.GetType());
            var task = (Task)dispatcher.Invoke(this, new object[] { @event, preTransaction });

            await task.ConfigureAwait(false);
        }
    }
}
