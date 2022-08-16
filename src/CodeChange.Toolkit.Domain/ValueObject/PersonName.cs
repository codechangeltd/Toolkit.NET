namespace CodeChange.Toolkit.Domain
{
    using BinaryFog.NameParser;
    
    /// <summary>
    /// Represents a persons full name
    /// </summary>
    public class PersonName : ValueObject
    {
        protected PersonName() { }

        private PersonName
            (
                string firstName,
                string? middleName,
                string lastName,
                string? title = null,
                string? suffix = null
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
                string? middleName,
                string lastName,
                string? title = null,
                string? suffix = null
            )
        {
            var allNames = new string?[] { firstName, middleName, lastName };
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
                return new PersonName(firstName, middleName, lastName, title, suffix);
            }
        }

        /// <summary>
        /// Parses a full name into its name parts then constructs a new PersonName
        /// </summary>
        /// <param name="fullName">The full name to parse</param>
        /// <returns>The PersonName constructed from the name parts</returns>
        public static Result<PersonName> Parse(string fullName)
        {
            if (String.IsNullOrEmpty(fullName))
            {
                return Result.Failure<PersonName>("The name must contain a value.");
            }
            else
            {
                var parsedName = FullNameParser.Parse(fullName);
                var firstName = parsedName.FirstName;
                var middleName = parsedName.MiddleName;
                var lastName = parsedName.LastName;
                var title = parsedName.Title;
                var suffix = parsedName.Suffix;

                // NOTE:
                // The name parser works for most names, but there are
                // some valid edge cases that do not work.
                //
                // For example, the name 'Mary Sarah-Jane Lucy' fails 
                // because the middle name contains a hyphen, which the 
                // parser can't handle.
                //
                // The following code is a failsafe for these cases.
                // It simply uses the first and last words as the first
                // and last names, then anything in between gets treated 
                // as the middle name.

                if (parsedName.Results.Count == 0)
                {
                    var names = fullName.Split(' ');

                    firstName = names.FirstOrDefault();

                    if (names.Length > 1)
                    {
                        lastName = names.LastOrDefault();
                    }

                    if (names.Length > 2)
                    {
                        middleName = String.Join(" ", names.Skip(1).Take(names.Length - 2));
                    }
                }

                return Create(firstName!, middleName, lastName!, title, suffix);
            }
        }

        /// <summary>
        /// Auto formats the person name to ensure it uses title case
        /// </summary>
        public virtual void AutoFormat()
        {
            FirstName = CorrectCase(FirstName)!;
            MiddleName = CorrectCase(MiddleName);
            LastName = CorrectCase(LastName)!;
            Title = CorrectCase(Title);
            Suffix = CorrectCase(Suffix);

            static string? CorrectCase(string? name)
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

        public string FirstName { get; private set; } = default!;
        public string? MiddleName { get; private set; }
        public string LastName { get; private set; } = default!;
        public string? Title { get; private set; }
        public string? Suffix { get; private set; }

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
        private static string Concat(params string?[] words)
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
            yield return MiddleName?.ToUpper() ?? String.Empty;
            yield return LastName.ToUpper();
            yield return Title?.ToUpper() ?? String.Empty;
            yield return Suffix?.ToUpper() ?? String.Empty;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
