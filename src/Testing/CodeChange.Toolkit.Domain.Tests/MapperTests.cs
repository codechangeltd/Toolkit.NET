namespace CodeChange.Toolkit.Domain.Tests
{
    using CodeChange.Toolkit.Domain.Mapping;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MapperTests
    {
        [TestMethod]
        public void Ensure_Dto_Maps_To_Configuration()
        {
            var mapper = new SimpleDtoToConfigurationMapper();
            var dto = new TestDto();

            var configuration = mapper.Map<TestDto, TestConfiguration>(dto);

            Assert.AreEqual(dto.Name, configuration.Name);
        }
    }
}
