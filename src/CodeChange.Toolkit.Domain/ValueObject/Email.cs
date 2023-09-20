namespace CodeChange.Toolkit.Domain
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a persons email address
    /// </summary>
    public class Email : ValueObject, ICloneable
    {
        protected Email() { }

        private Email(string email)
        {
            var parts = email.Split('@');

            Address = email;
            User = parts[0];
            Host = parts[1];
        }

        public static Result<Email> Create(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                return Result.Failure<Email>("The email address must contain a value.");
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
                return new Email(email);
            }
            else
            {
                return Result.Failure<Email>($"The email address '{email}' is invalid.");
            }
        }

        public string Address { get; private init; } = default!;
        public string Host { get; private init; } = default!;
        public string User { get; private init; } = default!;

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Address.ToUpper();
        }

        public object Clone() => MemberwiseClone();

        public override string ToString() => Address;
    }
}
