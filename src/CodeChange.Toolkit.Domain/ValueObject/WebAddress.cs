namespace CodeChange.Toolkit.Domain
{
    /// <summary>
    /// Represents a website address
    /// </summary>
    public class WebAddress : ValueObject, ICloneable
    {
        protected WebAddress() { }

        private WebAddress(string url)
        {
            var uri = new Uri(url);

            Address = url;
            Scheme = uri.Scheme;
            Host = uri.Host;
        }

        public static Result<WebAddress> Create(string url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                return Result.Failure<WebAddress>("The web address must contain a value.");
            }

            var isValid = Uri.IsWellFormedUriString(url, UriKind.Absolute);

            if (isValid)
            {
                return new WebAddress(url);
            }
            else
            {
                return Result.Failure<WebAddress>($"The web address '{url}' is invalid.");
            }
        }

        public string Address { get; private init; } = default!;
        public string Scheme { get; private init; } = default!;
        public string Host { get; private init; } = default!;

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Address.ToUpper();
        }

        public object Clone() => MemberwiseClone();

        public override string ToString() => Address;
    }
}
