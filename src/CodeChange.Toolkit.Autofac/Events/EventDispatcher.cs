namespace CodeChange.Toolkit.Autofac.Events
{
    using global::Autofac;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using System.Reflection;
    using CodeChange.Toolkit.Domain.Events;

    /// <summary>
    /// Represents an Autofac implementation of an event dispatcher
    /// </summary>
    public sealed class EventDispatcher : IEventDispatcher
    {
        private IComponentContext _container;
        private MethodInfo _genericDispatcher;

        /// <summary>
        /// Constructs the event dispatcher with required dependencies
        /// </summary>
        /// <param name="context">The container</param>
        public EventDispatcher
            (
                IComponentContext context
            )
        {
            Validate.IsNotNull(context);

            _container = context;

            _genericDispatcher = this.GetType().GetMethods().First
            (
                m => m.Name == "Dispatch" && m.IsGenericMethod
            );
        }

        /// <summary>
        /// Dispatches a domain event of the type specified
        /// </summary>
        /// <typeparam name="T">The domain event type</typeparam>
        /// <param name="event">The event to dispatch</param>
        /// <param name="preTransaction">True, if pre-transaction handlers required</param>
        public void Dispatch<T>
            (
                T @event,
                bool preTransaction = false
            )
            where T : IDomainEvent
        {
            Validate.IsNotNull(@event);

            var allHandlers = _container.Resolve<IEnumerable<IDomainEventHandler<T>>>();
            var matchingHandlers = new List<IDomainEventHandler<T>>();

            foreach (var handler in allHandlers)
            {
                var isMatch = false;

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
                    matchingHandlers.Add(handler);
                }
            }

            foreach (var handler in matchingHandlers)
            {
                handler.Handle(@event);
            }
        }

        /// <summary>
        /// Dispatches a domain event
        /// </summary>
        /// <param name="event">The event to dispatch</param>
        /// <param name="preTransaction">True, if pre-transaction handlers required</param>
        public void Dispatch
            (
                IDomainEvent @event,
                bool preTransaction = false
            )
        {
            Validate.IsNotNull(@event);

            var dispatcher = _genericDispatcher.MakeGenericMethod
            (
                @event.GetType()
            );

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
    }
}
