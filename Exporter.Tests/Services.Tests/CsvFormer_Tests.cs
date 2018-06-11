using Microsoft.VisualStudio.TestTools.UnitTesting;

using Exporter.Services;
using Exporter.Models.Interfaces;
using Exporter.Models.UnitOfWork;

namespace Exporter.Tests.Services.Tests
{
    [TestClass]
    public class CsvFormer_Tests
    {
        [TestMethod]
        public void CsvFormerClass_ShouldImplementIFormerInterface()
        {
            // Arrange
            UnitOfWork unit = new UnitOfWork();
            CsvFormer former = new CsvFormer(unit, "23", null);

            // Assert
            Assert.IsTrue(former is IFormer);
            Assert.IsTrue(former is FileFormer);
        }
    }
}
