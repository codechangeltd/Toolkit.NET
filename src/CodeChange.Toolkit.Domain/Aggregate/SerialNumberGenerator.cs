namespace CodeChange.Toolkit.Domain.Aggregate
{
    using System;

    /// <summary>
    /// Represents a serial number implementation of the globally unique key generator
    /// </summary>
    public class SerialNumberGenerator : IKeyGenerator
    {
        private Random _random;

        /// <summary>
        /// Initiates the generator with a new random generator
        /// </summary>
        public SerialNumberGenerator()
        {
            _random = new Random();
        }

        /// <summary>
        /// Generates a new key value using 4 random groups of numbers with 4 digits
        /// </summary>
        /// <returns>The key generated</returns>
        public string GenerateKey()
        {
            var number1 = GenerateRandomNumber();
            var number2 = GenerateRandomNumber();
            var number3 = GenerateRandomNumber();
            var number4 = GenerateRandomNumber();

            var part1 = PadNumber(number1);
            var part2 = PadNumber(number2);
            var part3 = PadNumber(number3);
            var part4 = PadNumber(number4);

            return part1 + "-" + part2 + "-" + part3 + "-" + part4;
        }

        /// <summary>
        /// Generates a new random number between 0 and 9999
        /// </summary>
        /// <returns>The number generated</returns>
        private int GenerateRandomNumber()
        {
            return _random.Next(0, 9999);
        }

        /// <summary>
        /// Pads the number specified with 0s for numbers with less than 4 digits
        /// </summary>
        /// <param name="number">The number to pad</param>
        /// <returns>A string representing the padded number</returns>
        private string PadNumber(int number)
        {
            return String.Format("{0:0000}", number);
        }
    }
}
