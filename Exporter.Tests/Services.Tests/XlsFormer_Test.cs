using Microsoft.VisualStudio.TestTools.UnitTesting;

using Exporter.Services;
using Exporter.Models.Interfaces;
using Exporter.Models.UnitOfWork;

namespace Exporter.Tests.Services.Tests
{
    [TestClass]
    public class XlsFormer_Test
    {
        [TestMethod]
        public void XlsFormerClass_ShouldImplementIFOrmerInterface()
        {
            // Ararnge
            UnitOfWork unit = new UnitOfWork();
            XlsFormer former = new XlsFormer(unit, "23", null, null);

            // Assert
            Assert.IsTrue(former is IFormer);
            Assert.IsTrue(former is FileFormer);
        }
    }
}
