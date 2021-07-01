namespace CodeChange.Toolkit.Domain
{
    using CSharpFunctionalExtensions;
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents a persons birth record (i.e. the date and place they were born)
    /// </summary>
    public class PersonBirth : ValueObject
    {
        private PersonBirth(DateTime birthDate, string birthPlace)
        {
            BirthDate = birthDate;
            BirthPlace = birthPlace;
        }

        public static Result<PersonBirth> Create(DateTime birthDate, string birthPlace)
        {
            if (birthDate == default || birthDate > DateTime.Today)
            {
                return Result.Failure<PersonBirth>("The date of birth is not valid.");
            }
            else
            {
                return new PersonBirth(birthDate, birthPlace);
            }
        }

        /// <summary>
        /// Gets the persons birth date
        /// </summary>
        public DateTime BirthDate { get; }

        /// <summary>
        /// Calculates the persons age at the current date and time
        /// </summary>
        /// <returns>The age calculated</returns>
        public int CalculateAge()
        {
            return CalculateAge(DateTime.UtcNow);
        }

        /// <summary>
        /// Calculates the persons age for a given date and time
        /// </summary>
        /// <param name="onDate">The date to calculate their age on</param>
        /// <returns>The age calculated</returns>
        public int CalculateAge(DateTime onDate)
        {
            var age = onDate.Year - BirthDate.Year;

            // Go back to the year in which the person was born in case of a leap year
            if (BirthDate.Date > onDate.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        /// <summary>
        /// Gets the persons birth place (e.g. hospital or address)
        /// </summary>
        public string BirthPlace { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BirthDate;
            yield return BirthPlace;
        }

        public override string ToString()
        {
            return BirthDate.ToLongDateString();
        }
    }
}
