namespace CodeChange.Toolkit.Domain.Events.Logging;

using CodeChange.Toolkit.Domain.Aggregate;

/// <summary>
/// Represents a domain event log detail aggregate entity
/// </summary>
public class DomainEventLogDetail
{
    protected internal DomainEventLogDetail(object model, PropertyInfo property)
    {
        NestedDetails = new Collection<DomainEventLogDetail>();
        
        PopulateDetails(model, property);
    }

    protected DomainEventLogDetail(DomainEventLogDetail parent, object model, PropertyInfo property)
        : this(model, property)
    {
        Parent = parent;
    }

    protected internal DomainEventLogDetail(DomainEventLogDetail parent, string propertyName, string propertyValue)
    {
        Validate.IsNotNull(parent);
        Validate.IsNotEmpty(propertyName);

        NestedDetails = new Collection<DomainEventLogDetail>();
        Parent = parent;

        PropertyName = propertyName;
        PropertyStringValue = propertyValue;
        PropertyTypeName = nameof(String);
    }

    /// <summary>
    /// The parent detail
    /// </summary>
    public virtual DomainEventLogDetail? Parent { get; protected set; }

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

        PropertyName = property.Name;
        PropertyTypeName = propertyType.Name;

        var propertyValue = property.GetValue(model, null);

        if (propertyValue != null)
        {
            PropertyStringValue = propertyValue?.ToString();

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
                            GenerateNestedDetails(property.Name, (IEnumerable<string>)propertyValue!);
                        }
                        else
                        {
                            foreach (var item in (IEnumerable)propertyValue!)
                            {
                                GenerateNestedDetails(item);
                            }
                        }
                    }
                    else if (false == isAggregate)
                    {
                        GenerateNestedDetails(propertyValue!);
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
        var details = new List<DomainEventLogDetail>();

        foreach (var value in collection)
        {
            var itemName = $"{propertyName}[{index}]";
            var detail = new DomainEventLogDetail(this, itemName, value);

            details.Add(detail);
            HasNestedDetails = true;

            index++;
        }

        NestedDetails = details.ToArray();
    }

    /// <summary>
    /// Generates a collection of nested details for a model
    /// </summary>
    /// <param name="model">The model</param>
    protected virtual void GenerateNestedDetails(object model)
    {
        var properties = model.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.CanRead)
            .ToArray();

        var details = new List<DomainEventLogDetail>();

        foreach (var property in properties)
        {
            details.Add(new DomainEventLogDetail(this, model, property));

            HasNestedDetails = true;
        }

        NestedDetails = details.ToArray();
    }

    /// <summary>
    /// The property name
    /// </summary>
    public string PropertyName { get; protected set; } = default!;

    /// <summary>
    /// The name of the property type
    /// </summary>
    public string PropertyTypeName { get; protected set; } = default!;

    /// <summary>
    /// Gets a string representation of the property value
    /// </summary>
    public string? PropertyStringValue { get; protected set; }

    /// <summary>
    /// Gets a flag indicating if there are any nested details
    /// </summary>
    public bool HasNestedDetails { get; protected set; }

    /// <summary>
    /// Gets a collection of nested details
    /// </summary>
    public IEnumerable<DomainEventLogDetail> NestedDetails { get; protected set; }
}
