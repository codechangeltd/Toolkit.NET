namespace CodeChange.Toolkit.Domain
{
    using CSharpFunctionalExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a persons name
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
            this.FirstName = firstName;
            this.MiddleName = middleName;
            this.LastName = lastName;
            this.Title = title;
            this.Suffix = suffix;
        }

        /// <summary>
        /// Creates a new person name
        /// </summary>
        /// <param name="firstName">The first name</param>
        /// <param name="middleName">The middle name</param>
        /// <param name="lastName">The last name</param>
        /// <param name="title">The title (optional)</param>
        /// <param name="suffix">The suffix (optional)</param>
        /// <returns>The result with the name created</returns>
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

            var allEmpty = allNames.All
            (
                name => String.IsNullOrWhiteSpace(name)
            );

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

                return Result.Ok(personName);
            }
        }

        /// <summary>
        /// Gets the persons first name
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// Gets the persons middle name
        /// </summary>
        public string MiddleName { get; private set; }

        /// <summary>
        /// Gets the persons last name
        /// </summary>
        public string LastName { get; private set; }
        
        /// <summary>
        /// Gets the persons title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the name suffix
        /// </summary>
        public string Suffix { get; private set; }

        /// <summary>
        /// Gets a display name for presentation purposes (excluding title and suffix)
        /// </summary>
        public string DisplayName
        {
            get
            {
                return Concat
                (
                    this.FirstName,
                    this.MiddleName,
                    this.LastName
                );
            }
        }

        /// <summary>
        /// Gets the full name of the person (including title and suffix)
        /// </summary>
        public string FullName
        {
            get
            {
                return Concat
                (
                    this.Title,
                    this.FirstName,
                    this.MiddleName,
                    this.LastName,
                    this.Suffix
                );
            }
        }

        /// <summary>
        /// Concatenates an array of words into a single string separated by spaces
        /// </summary>
        /// <param name="words">The words to concatenate</param>
        /// <returns>The concatenated string</returns>
        private string Concat
            (
                params string[] words
            )
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
            yield return this.FirstName;
            yield return this.MiddleName;
            yield return this.LastName;
            yield return this.Title;
            yield return this.Suffix;
        }

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}
