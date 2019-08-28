namespace CodeChange.Toolkit.Domain.Tests
{
    using System.Collections.Generic;
    
    public class TestConfiguration
    {
        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public IEnumerable<TestNestedConfiguration> Children { get; set; }
    }
}
