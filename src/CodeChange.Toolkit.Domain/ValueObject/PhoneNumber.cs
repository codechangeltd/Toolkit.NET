namespace CodeChange.Toolkit.Domain
{
    /// <summary>
    /// Represents a telephone number
    /// </summary>
    public class PhoneNumber : ValueObject
    {
        protected PhoneNumber()
        {
            Number = String.Empty;
            HasValue = false;
        }

        private PhoneNumber(string number)
        {
            Number = number.Trim();
            HasValue = true;
        }

        public static Result<PhoneNumber> Create(string number)
        {
            var isValid = true;
            var failureMessage = String.Empty;

            if (String.IsNullOrWhiteSpace(number))
            {
                isValid = false;
                failureMessage = "The phone number must contain a value.";
            }

            if (false == number.Any(Char.IsDigit))
            {
                isValid = false;
                failureMessage = "The phone number must contain at least one digit.";
            }

            if (isValid)
            {
                return new PhoneNumber(number);
            }
            else
            {
                return Result.Failure<PhoneNumber>(failureMessage);
            }
        }

        public static PhoneNumber Empty()
        {
            return new PhoneNumber();
        }

        public string Number { get; private set; }
        public bool HasValue { get; private set; }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Number.ToUpper();
        }

        public override string ToString() => Number;
    }
}
