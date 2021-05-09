namespace CodeChange.Toolkit.Domain
{
    using CSharpFunctionalExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a telephone number
    /// </summary>
    public class PhoneNumber : ValueObject
    {
        private PhoneNumber() { }

        private PhoneNumber(string number)
        {
            Number = number.Trim();
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
                var pn = new PhoneNumber(number);

                return Result.Success(pn);
            }
            else
            {
                return Result.Failure<PhoneNumber>(failureMessage);
            }
        }

        public string Number { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number.ToUpper();
        }

        public override string ToString()
        {
            return Number;
        }
    }
}
