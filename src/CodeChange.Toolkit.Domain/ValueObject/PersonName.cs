namespace CodeChange.Toolkit.Domain
{
    using CSharpFunctionalExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a persons full name
    /// </summary>
    public class PersonName : ValueObject
    {
        private PersonName() { }

        private PersonName
            (
                string firstName,
                string middleName,
                string lastName,
                string title = null,
                string suffix = null
            )
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Title = title;
            Suffix = suffix;
        }

        public static Result<PersonName> Create
            (
                string firstName,
                string middleName,
                string lastName,
                string title = null,
                string suffix = null
            )
        {
            var allNames = new string[]
            {
                firstName,
                middleName,
                lastName
            };

            var allEmpty = allNames.All(name => String.IsNullOrWhiteSpace(name));

            if (allEmpty)
            {
                return Result.Failure<PersonName>
                (
                    "At least one name (first, middle or last) is required."
                );
            }
            else
            {
                var personName = new PersonName
                (
                    firstName,
                    middleName,
                    lastName,
                    title,
                    suffix
                );

                return Result.Success(personName);
            }
        }

        /// <summary>
        /// Auto formats the person name to ensure it uses title case
        /// </summary>
        public virtual void AutoFormat()
        {
            FirstName = CorrectCase(FirstName);
            MiddleName = CorrectCase(MiddleName);
            LastName = CorrectCase(LastName);
            Title = CorrectCase(Title);
            Suffix = CorrectCase(Suffix);

            string CorrectCase(string name)
            {
                if (String.IsNullOrEmpty(name))
                {
                    return name;
                }
                else
                {
                    var isAllUpper = name.All(c => Char.IsUpper(c));
                    var isAllLower = name.All(c => Char.IsLower(c));

                    if (isAllUpper || isAllLower)
                    {
                        return name.ToTitleCase();
                    }
                    else
                    {
                        return name;
                    }
                }
            }
        }

        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string LastName { get; private set; }
        public string Title { get; private set; }
        public string Suffix { get; private set; }

        /// <summary>
        /// Gets a display name for presentation purposes (excluding title and suffix)
        /// </summary>
        public string DisplayName => Concat(FirstName, MiddleName, LastName);

        /// <summary>
        /// Gets the full name of the person (including title and suffix)
        /// </summary>
        public string FullName => Concat(Title, FirstName, MiddleName, LastName, Suffix);

        /// <summary>
        /// Concatenates an array of words into a single string separated by spaces
        /// </summary>
        /// <param name="words">The words to concatenate</param>
        /// <returns>The concatenated string</returns>
        private string Concat(params string[] words)
        {
            var builder = new StringBuilder();

            foreach (var word in words)
            {
                if (false == String.IsNullOrWhiteSpace(word))
                {
                    builder.Append($"{word} ");
                }
            }

            return builder.ToString().Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName.ToUpper();
            yield return MiddleName.ToUpper();
            yield return LastName.ToUpper();
            yield return Title.ToUpper();
            yield return Suffix.ToUpper();
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
