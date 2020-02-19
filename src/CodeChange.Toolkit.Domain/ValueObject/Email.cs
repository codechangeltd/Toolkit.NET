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

            bool isValid;

            try
            {
                isValid = Regex.IsMatch
                (
                    email,
                    @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",
                    RegexOptions.IgnoreCase
                );
            }
            catch (RegexMatchTimeoutException)
            {
                isValid = false;
            }

            if (isValid)
            {
                var address = new Email(email);

                return Result.Success(address);
            }
            else
            {
                return Result.Failure<Email>
                (
                    $"The email address '{email}' is invalid."
                );
            }
        }

        public string Address { get; private set; }

        public string Host { get; private set; }

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
