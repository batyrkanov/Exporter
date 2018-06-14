using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Exporter.Models.Interfaces;
using Exporter.Models.Repositories;
using Exporter.Models.Entities;

using Exporter.Tests.Models.Tests.Entities.Tests;
using Moq;

namespace Exporter.Tests.Models.Tests.Repositories.Tests
{
    [TestClass]
    public class OutputTableRepository_Tests
    {
        TestDbSet<SqlQuery> queriesDbSet;
        TestDbSet<OutputTable> tablesDbSet;

        Mock<IContext> mock;
        OutputTableRepository outRepo;

        [TestInitialize]
        public void TestInitialize()
        {
            queriesDbSet = new TestDbSet<SqlQuery>()
            {
                new SqlQuery()
                {
                    SqlQueryId = 0,
                    SqlQueryName = "Select query",
                    SqlQueryContent = "select * from table",
                    SqlQueryCreatedDate = DateTime.Today.AddDays(-1)
                },
                new SqlQuery()
                {
                    SqlQueryId = 1,
                    SqlQueryName = "Update query",
                    SqlQueryContent = "update table table set name = 'new_name'",
                    SqlQueryCreatedDate = DateTime.Today
                },
                new SqlQuery()
                {
                    SqlQueryId = 2,
                    SqlQueryName = "Delete query",
                    SqlQueryContent = "delete from table where id = 1",
                    SqlQueryCreatedDate = DateTime.Today
                },
                new SqlQuery()
                {
                    SqlQueryId = 3,
                    SqlQueryName = "Drop table query",
                    SqlQueryContent = "drop table table",
                    SqlQueryCreatedDate = DateTime.Today.AddDays(-2)
                }
            };
            tablesDbSet = new TestDbSet<OutputTable>()
            {
                new OutputTable()
                {
                    OutputTableId = 0,
                    Name = "Drop table query",
                    FileName = "randomfilename_1.xls",
                    FileType = "xls",
                    CreatedAt = DateTime.Today,
                    UpdatedAt = DateTime.Now,
                    SqlQueryId = 3
                },
                new OutputTable()
                {
                    OutputTableId = 1,
                    Name = "Update query",
                    FileName = "randomfilename_update_query.xls",
                    FileType = "xls",
                    CreatedAt = DateTime.Today.AddDays(-1),
                    UpdatedAt = DateTime.Now,
                    SqlQueryId = 1
                },
                new OutputTable()
                {
                    OutputTableId = 2,
                    Name = "Drop table query",
                    FileName = "randomfilename_drop_table.csv",
                    FileType = "csv",
                    CreatedAt = DateTime.Today.AddDays(-1),
                    UpdatedAt = DateTime.Today,
                    SqlQueryId = 3
                },
                new OutputTable()
                {
                    OutputTableId = 3,
                    Name = "Select query",
                    FileName = "randomfilename_select_query.csv",
                    FileType = "csv",
                    CreatedAt = DateTime.Today.AddDays(-2),
                    UpdatedAt = DateTime.Today,
                    SqlQueryId = 0
                },
                new OutputTable()
                {
                    OutputTableId = 4,
                    Name = "Delete query",
                    FileName = "randomfilename_delete_query.xls",
                    FileType = "xls",
                    CreatedAt = DateTime.Today,
                    UpdatedAt = DateTime.Today,
                    SqlQueryId = 2
                },
                new OutputTable()
                {
                    OutputTableId = 5,
                    Name = "Delete query",
                    FileName = "randomfilename_delete_query.csv",
                    FileType = "csv",
                    CreatedAt = DateTime.Today.AddDays(-5),
                    UpdatedAt = DateTime.Today.AddDays(-5),
                    SqlQueryId = 2
                }
            };

            mock = new Mock<IContext>();
            mock
                .Setup(q => q.SqlQueries)
                .Returns(queriesDbSet);
            mock
                .Setup(t => t.OutputTables)
                .Returns(tablesDbSet);

            outRepo = new OutputTableRepository(mock.Object);
        }

        [TestMethod]
        public void GetAllOutputTables_Test()
        {
            // Arrange
            int tablesCount = tablesDbSet.Count();

            // Act
            IEnumerable<OutputTable> tables = outRepo.GetAll();

            // Assert
            Assert.AreEqual(tablesCount, tables.Count());
            Assert.AreEqual(tablesCount, tablesDbSet.Count());

            Assert.AreEqual(tables.First().OutputTableId, tablesDbSet.First().OutputTableId);
            Assert.AreEqual(tables.Last().FileName, tablesDbSet.Last().FileName);
        }

        [TestMethod]
        public void GetOutputTable_Test()
        {
            // Arrange
            int tablesCountBeforeAct = tablesDbSet.Count();
            int id = tablesDbSet.First().OutputTableId;

            // Act
            OutputTable outputTable = outRepo.Get(id);

            // Assert
            Assert.AreEqual(tablesCountBeforeAct, tablesDbSet.Count());
            Assert.AreEqual(id, outputTable.OutputTableId);
            Assert.AreEqual(outputTable.FileName, tablesDbSet.Find(id).FileName);
        }

        [TestMethod]
        public void CreateOutputTable_Test()
        {
            // Arrange
            int tablesCountBeforeAct = tablesDbSet.Count();
            int lastOutputTableId = tablesDbSet.Last().OutputTableId;
            OutputTable newOutputTable = new OutputTable()
            {
                OutputTableId = lastOutputTableId + 1,
                Name = "new output table",
                FileName = "newOutputTable.xls",
                FileType = "xls",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                SqlQueryId = 0
            };

            // Act
            outRepo.Create(newOutputTable);

            // Assert
            Assert.AreNotEqual(tablesDbSet.Last().OutputTableId, lastOutputTableId);
            Assert.AreEqual(tablesDbSet.Last().OutputTableId, newOutputTable.OutputTableId);
            Assert.AreEqual(tablesDbSet.Last().FileName, newOutputTable.FileName);

            Assert.IsTrue(tablesDbSet.Count() > tablesCountBeforeAct);
        }

        [TestMethod]
        public void UpdateOutputTable_Test()
        {
            // Arrange
            int tablesCountBeforeAct = tablesDbSet.Count();
            int updatedTableId = tablesDbSet.Last().OutputTableId;
            DateTime createdDateBeforeAct = tablesDbSet.Last().CreatedAt;
            string fileNameBeforeAct = tablesDbSet.Last().FileName;
            string nameBeforeAct = tablesDbSet.Last().Name;

            string newFileName = "updatedFileName.csv";
            string newName = "updatedName";

            // Act
            OutputTable tableToUpdate = tablesDbSet.Last();
            tableToUpdate.FileName = newFileName;
            tableToUpdate.Name = newName;
            outRepo.Update(tableToUpdate);

            OutputTable updatedTable = tablesDbSet.Last();

            // Assert
            Assert.AreEqual(tablesCountBeforeAct, tablesDbSet.Count());

            Assert.AreEqual(updatedTableId, updatedTable.OutputTableId);
            Assert.AreEqual(newFileName, updatedTable.FileName);
            Assert.AreEqual(newName, updatedTable.Name);
            Assert.AreEqual(createdDateBeforeAct, updatedTable.CreatedAt);

            Assert.AreNotEqual(fileNameBeforeAct, updatedTable.FileName);
            Assert.AreNotEqual(nameBeforeAct, updatedTable.Name);
        }

        [TestMethod]
        public void DeleteOutputTable_Test()
        {
            // Arrange
            int tablesCountBeforeAct = tablesDbSet.Count();
            int tableToDeleteId = tablesDbSet.First().OutputTableId;
            string tableToDeleteFileName = tablesDbSet.First().FileName;

            // Act
            outRepo.Delete(tableToDeleteId);

            // Assert
            foreach (OutputTable table in tablesDbSet)
            {
                Assert.IsTrue(table.FileName != tableToDeleteFileName);
                Assert.IsTrue(table.OutputTableId != tableToDeleteId);
            }

            Assert.IsTrue(tablesCountBeforeAct > tablesDbSet.Count());
        }

        //[TestMethod]
        //public void RemoveQueryOutputTableIfExists_Test()
        //{
        //    // Arrange
        //    int tablesCountBeforeAct = tablesDbSet.Count();
        //    int queryId = tablesDbSet.First().SqlQueryId;
        //    string outputTableType = tablesDbSet.First().FileType;

        //    // Act
        //    outRepo.RemoveQueryOutputTableIfExists(queryId, outputTableType);

        //    // Assert
        //    Assert.IsTrue(tablesCountBeforeAct > tablesDbSet.Count());
        //    foreach (OutputTable table in tablesDbSet)
        //        Assert.IsTrue(table.SqlQueryId != queryId || table.FileType != outputTableType);
        //}

        [TestMethod]
        public void BindOutputTableToQuery_IfOutputTableDoesNotExist_Test()
        {
            // Arrange
            int tablesCountBeforeAct = tablesDbSet.Count();
            string newFileName = "newFileName.pdf";

            int queryId = 0;
            string type = "xls";

            // Act
            outRepo.BindOutputTableToQuery(queryId, newFileName, type);

            // Assert
            Assert.IsTrue(tablesDbSet.Count() > tablesCountBeforeAct);
            Assert.AreEqual(tablesDbSet.Last().FileName, newFileName);
            Assert.AreEqual(tablesDbSet.Last().SqlQueryId, queryId);
            Assert.AreEqual(tablesDbSet.Last().FileType, type);
            Assert.IsTrue(
                tablesDbSet.Last().CreatedAt > DateTime.Today &&
                tablesDbSet.Last().UpdatedAt > DateTime.Today
            );
        }

        [TestMethod]
        public void GetQueryOutputTableByIdAndType_Test()
        {
            // Arrange
            int queryId = 0;
            int newId = tablesDbSet.Last().OutputTableId + 1;
            string type = "xls";
            string newName = "New table";
            string newFileName = "newTableRandomFileName.xls";

            OutputTable newOutputTable = new OutputTable()
            {
                OutputTableId = newId,
                Name = newName,
                FileName = newFileName,
                FileType = type,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                SqlQueryId = queryId
            };
            outRepo.Create(newOutputTable);

            // Act
            OutputTable outputTable = outRepo.GetQueryOutputTableByIdAndType(queryId, type);

            // Assert
            Assert.AreEqual(outputTable.OutputTableId, newId);
            Assert.AreEqual(outputTable.FileType, type);
            Assert.AreEqual(outputTable.Name, newName);
            Assert.AreEqual(outputTable.FileName, newFileName);
            Assert.IsTrue(
                DateTime.Today < outputTable.CreatedAt &&
                DateTime.Today < outputTable.UpdatedAt
            );
        }
    }
}
