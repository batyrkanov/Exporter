using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Exporter.Services;
using System.IO;

namespace Exporter.Tests.Services.Tests
{
    [TestClass]
    public class FileFormer_Tests : FileFormer
    {
        [TestMethod]
        public void FileFormerInstanceShouldContainsFieldsFileNameAndFilePath()
        {
            // Arrange
            FileFormer former = new FileFormer();
            Type formerClass = former.GetType();

            //Assert
            Assert.IsTrue(formerClass.GetProperty("FileName") != null);
            Assert.IsTrue(formerClass.GetProperty("FilePath") != null);
        }

        [TestMethod]
        public void FileFormer_WrapItem_Test()
        {
            // Arrange
            string valueBeforeAct = "value,";

            // Act
            string wrapedValue = this.WrapItem(valueBeforeAct);

            // Assert
            Assert.AreNotEqual(valueBeforeAct, wrapedValue);
            Assert.IsTrue(wrapedValue.Contains(valueBeforeAct));
        }

        [TestMethod]
        public void FormQueryParametersDicitonary_Test()
        {
            // Arrange
            List<string> parameters = new List<string>()
            {
                "first-xyz-parameter",
                "second-xyz-value",
                "third-xyz-item"
            };
            Dictionary<string, string> expectedDict = new Dictionary<string, string>()
            {
                { "first", "parameter" },
                { "second", "value" },
                { "third", "item" }
            };

            // Act
            Dictionary<string, string> dict = this.FormQueryParametersDictionary(parameters);

            // Assert
            Assert.AreEqual(expectedDict.Count(), dict.Count());

            foreach (KeyValuePair<string, string> item in expectedDict)
                Assert.IsTrue(dict.ContainsKey(item.Key));
            foreach (KeyValuePair<string, string> item in expectedDict)
                Assert.IsTrue(dict.ContainsValue(item.Value));
        }

        [TestMethod]
        public void SplitQueryIntoPieces_Test()
        {
            // Arrange
            string query = "Select * from Table Union All Select * From CopyTable Union All Select * from TableDuplicate";

            // Act
            List<string> splitedQuery = this.SplitQueryIntoPieces(query);

            // Assert
            Assert.IsTrue(splitedQuery.Count() == 3);
            Assert.IsTrue(
                splitedQuery[0].Contains("Table") &&
                splitedQuery[0] == "Select * from Table"
            );
            Assert.IsTrue(
                splitedQuery[1].Contains("CopyTable") &&
                splitedQuery[1] == "Select * From CopyTable"
            );
            Assert.IsTrue(
                splitedQuery[2].Contains("TableDuplicate") &&
                splitedQuery[2] == "Select * from TableDuplicate"
            );
        }
    }
}
