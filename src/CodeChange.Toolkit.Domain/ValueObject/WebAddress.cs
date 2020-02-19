namespace CodeChange.Toolkit.Domain
{
    using CSharpFunctionalExtensions;
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents a website address
    /// </summary>
    public class WebAddress : ValueObject
    {
        private WebAddress() { }

        private WebAddress(string url)
        {
            var uri = new Uri(url);

            this.Address = url;
            this.Scheme = uri.Scheme;
            this.Host = uri.Host;
        }

        public static Result<WebAddress> Create
            (
                string url
            )
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                return Result.Failure<WebAddress>
                (
                    "The website address must contain a value."
                );
            }

            var isValid = Uri.IsWellFormedUriString
            (
                url,
                UriKind.Absolute
            );

            if (isValid)
            {
                var address = new WebAddress(url);

                return Result.Success(address);
            }
            else
            {
                return Result.Failure<WebAddress>
                (
                    $"The website address '{url}' is invalid."
                );
            }
        }

        public string Address { get; private set; }

        public string Scheme { get; private set; }

        public string Host { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Address.ToUpper();
        }

        public override string ToString()
        {
            return this.Address;
        }
    }
}
