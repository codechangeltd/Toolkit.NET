namespace CodeChange.Toolkit.Domain
{
    using CSharpFunctionalExtensions;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a persons email address
    /// </summary>
    public class Email : ValueObject
    {
        private Email() { }

        private Email(string email)
        {
            var parts = email.Split('@');

            this.Address = email;
            this.User = parts[0];
            this.Host = parts[1];
        }

        /// <summary>
        /// Creates a new email address
        /// </summary>
        /// <param name="email">The email address string</param>
        /// <returns>The result with the email address</returns>
        public static Result<Email> Create
            (
                string email
            )
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                return Result.Failure<Email>
                (
                    "The email address must contain a value."
                );
            }

            var isValid = Regex.IsMatch
            (
                email,
                @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"
            );

            if (isValid)
            {
                var address = new Email(email);

                return Result.Ok(address);
            }
            else
            {
                return Result.Failure<Email>
                (
                    $"The email address '{email}' is invalid."
                );
            }
        }

        /// <summary>
        /// Gets the full email address
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Gets the host portion of the email address
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// Gets the user information from the email address
        /// </summary>
        public string User { get; private set; }

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
