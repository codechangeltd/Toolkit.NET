namespace CodeChange.Toolkit.Domain.Tests
{
    using System.Collections.Generic;
    
    public class TestDto
    {
        public TestDto()
        {
            this.Name = "Test";
            this.IsDefault = true;

            this.Children = new List<TestNestedDto>()
            {
                new TestNestedDto()
                {
                    Description = "This is the first.",
                    Number = 1
                },
                new TestNestedDto()
                {
                    Description = "This is the second.",
                    Number = 2
                }
            };
        }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public List<TestNestedDto> Children { get; set; }
    }
}
