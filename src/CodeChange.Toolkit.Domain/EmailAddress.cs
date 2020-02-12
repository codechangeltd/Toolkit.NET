namespace CodeChange.Toolkit.Domain
{
    using CSharpFunctionalExtensions;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a persons email address
    /// </summary>
    public class EmailAddress : ValueObject
    {
        private EmailAddress() { }

        private EmailAddress(string email)
        {
            this.Email = email;
        }

        /// <summary>
        /// Creates a new email address
        /// </summary>
        /// <param name="email">The email address string</param>
        /// <returns>The result with the email address</returns>
        public static Result<EmailAddress> Create
            (
                string email
            )
        {
            var isValid = Regex.IsMatch
            (
                email,
                @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"
            );

            if (isValid)
            {
                var address = new EmailAddress(email);

                return Result.Ok(address);
            }
            else
            {
                return Result.Failure<EmailAddress>
                (
                    $"The email address '{email}' is invalid."
                );
            }
        }

        /// <summary>
        /// Gets the email address raw value
        /// </summary>
        public string Email { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Email.ToUpper();
        }

        public override string ToString()
        {
            return this.Email;
        }
    }
}
