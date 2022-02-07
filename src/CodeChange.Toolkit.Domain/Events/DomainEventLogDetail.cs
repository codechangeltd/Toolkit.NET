namespace CodeChange.Toolkit.Domain.Events
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a domain event log detail aggregate entity
    /// </summary>
    public class DomainEventLogDetail : IAggregateEntity
    {
        protected internal DomainEventLogDetail(DomainEventLog log, object model, PropertyInfo property)
        {
            Validate.IsNotNull(log);
            
            this.LookupKey = new EntityKeyGenerator().GenerateKey();
            this.NestedDetails = new Collection<DomainEventLogDetail>();
            this.Log = log;

            PopulateDetails(model, property);
        }

        protected internal DomainEventLogDetail(DomainEventLogDetail parent, object model, PropertyInfo property)
            : this(parent.Log, model, property)
        {
            this.Parent = parent;
        }

        protected internal DomainEventLogDetail(DomainEventLogDetail parent, string propertyName, string propertyValue)
        {
            Validate.IsNotNull(parent);
            Validate.IsNotEmpty(propertyName);

            this.LookupKey = new EntityKeyGenerator().GenerateKey();
            this.NestedDetails = new Collection<DomainEventLogDetail>();

            this.Parent = parent;
            this.Log = parent.Log;

            this.PropertyName = propertyName;
            this.PropertyStringValue = propertyValue;
            this.PropertyTypeName = nameof(String);
        }

        /// <summary>
        /// A database auto generated ID value, used internally for persistence
        /// </summary>
        public long ID { get; protected set; }

        /// <summary>
        /// A lookup key value for the entity, this must be unique for each entity of the same type
        /// </summary>
        public string LookupKey { get; protected set; }

        /// <summary>
        /// Gets the aggregate entities unique key value
        /// </summary>
        /// <returns>The key value</returns>
        public virtual string GetKeyValue() => this.LookupKey;

        /// <summary>
        /// Gets the associated event log
        /// </summary>
        public virtual DomainEventLog Log { get; protected set; }

        /// <summary>
        /// Gets the ID of the event log
        /// </summary>
        public long LogId { get; protected set; }

        /// <summary>
        /// Gets the parent detail
        /// </summary>
        public virtual DomainEventLogDetail Parent { get; protected set; }

        /// <summary>
        /// Gets the ID of the parent detail
        /// </summary>
        public long? ParentId { get; protected set; }

        /// <summary>
        /// Populates the property details using a model and property
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="property">The property info</param>
        protected virtual void PopulateDetails(object model, PropertyInfo property)
        {
            Validate.IsNotNull(model);
            Validate.IsNotNull(property);

            var propertyType = property.PropertyType;

            this.PropertyName = property.Name;
            this.PropertyTypeName = propertyType.Name;

            var propertyValue = property.GetValue(model, null);

            if (propertyValue != null)
            {
                this.PropertyStringValue = propertyValue.ToString();

                var isSimple = propertyType.IsSimple();

                if (false == (isSimple || propertyType.IsDateTime()))
                {
                    var interfaces = propertyType.GetInterfaces();
                    var isAggregate = interfaces.Contains(typeof(IAggregateEntity));

                    if (false == isAggregate)
                    {
                        var isCollection = interfaces.Contains(typeof(IEnumerable));

                        if (isCollection && propertyType != typeof(string))
                        {
                            var isStringCollection = false;

                            if (propertyType.IsArray)
                            {
                                if (propertyType == typeof(string[]))
                                {
                                    isStringCollection = true;
                                }
                            }
                            else
                            {
                                var genericArguments = propertyType.GetGenericArguments();

                                if (genericArguments.Length == 1 && genericArguments[0] == typeof(string))
                                {
                                    isStringCollection = true;
                                }
                            }
                            
                            if (isStringCollection)
                            {
                                GenerateNestedDetails(property.Name, (IEnumerable<string>)propertyValue);
                            }
                            else
                            {
                                foreach (var item in propertyValue as IEnumerable)
                                {
                                    GenerateNestedDetails(item);
                                }
                            }
                        }
                        else if (false == isAggregate)
                        {
                            GenerateNestedDetails(propertyValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates a collection of nested details for a string collection
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="collection">The string collection</param>
        protected virtual void GenerateNestedDetails(string propertyName, IEnumerable<string> collection)
        {
            var index = 0;

            foreach (var value in collection)
            {
                var itemName = $"{propertyName}[{index}]";
                var detail = new DomainEventLogDetail(this, itemName, value);

                this.NestedDetails.Add(detail);
                this.HasNestedDetails = true;

                index++;
            }
        }

        /// <summary>
        /// Generates a collection of nested details for a model
        /// </summary>
        /// <param name="model">The model</param>
        protected virtual void GenerateNestedDetails(object model)
        {
            var properties = model.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(_ => _.CanRead)
                .ToArray();

            foreach (var property in properties)
            {
                var detail = new DomainEventLogDetail(this, model, property);

                this.NestedDetails.Add(detail);
                this.HasNestedDetails = true;
            }
        }
        
        /// <summary>
        /// Gets the property name
        /// </summary>
        public string PropertyName { get; protected set; }

        /// <summary>
        /// Gets the name of the property type
        /// </summary>
        public string PropertyTypeName { get; protected set; }

        /// <summary>
        /// Gets a string representation of the property value
        /// </summary>
        public string PropertyStringValue { get; protected set; }
    
        /// <summary>
        /// Gets a flag indicating if there are any nested details
        /// </summary>
        public bool HasNestedDetails { get; protected set; }

        /// <summary>
        /// Gets a collection of nested details
        /// </summary>
        public virtual ICollection<DomainEventLogDetail> NestedDetails { get; protected set; }
    }
}
