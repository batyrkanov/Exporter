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
    public class SlqQueryParameterRepository_Tests
    {
        TestDbSet<Parameter> parametersDbSet;
        TestDbSet<SqlQuery> queriesDbSet;
        TestDbSet<SqlQueryParameter> queryParametersDbSet;

        Mock<IContext> mock;
        SqlQueryParameterRepository queryParamRepo;

        [TestInitialize]
        public void TestInitialize()
        {
            parametersDbSet = new TestDbSet<Parameter>()
            {
                new Parameter()
                {
                    ParameterId = 0,
                    ParameterName = "@Name",
                    ParameterRuName = "Имя",
                    ParameterType = "text",
                    ParameterCreatedDate = DateTime.Today.AddDays(-1)
                },
                new Parameter()
                {
                    ParameterId = 1,
                    ParameterName = "@Age",
                    ParameterRuName = "Возраст",
                    ParameterType = "number",
                    ParameterCreatedDate = DateTime.Now
                },
                new Parameter()
                {
                    ParameterId = 2,
                    ParameterName = "@Position",
                    ParameterRuName = "Должность",
                    ParameterType = "text",
                    ParameterCreatedDate = DateTime.Today.AddDays(-2)
                },
                new Parameter()
                {
                    ParameterId = 3,
                    ParameterName = "@Skills",
                    ParameterRuName = "Навыки",
                    ParameterType = "text",
                    ParameterCreatedDate = DateTime.Today.AddDays(-4)
                },
                new Parameter()
                {
                    ParameterId = 4,
                    ParameterName = "@Address",
                    ParameterRuName = "Адрес",
                    ParameterType = "text",
                    ParameterCreatedDate = DateTime.Now
                },
                new Parameter()
                {
                    ParameterId = 5,
                    ParameterName = "@Phone",
                    ParameterRuName = "Телефон",
                    ParameterType = "phone",
                    ParameterCreatedDate = DateTime.Today.AddDays(-1)
                },
                new Parameter()
                {
                    ParameterId = 6,
                    ParameterName = "@Mail",
                    ParameterRuName = "Почта",
                    ParameterType = "email",
                    ParameterCreatedDate = DateTime.Today.AddDays(-2)
                }
            };
            queriesDbSet = new TestDbSet<SqlQuery>()
            {
                new SqlQuery()
                {
                    SqlQueryId = 0,
                    SqlQueryName = "SelectEmployeesByAgeAndPosition",
                    SqlQueryContent = "Select age, position from Employees where age = @Age and position = @Postion",
                    SqlQueryCreatedDate = DateTime.Today
                },
                new SqlQuery()
                {
                    SqlQueryId = 1,
                    SqlQueryName = "SelectEmployeesByContacts",
                    SqlQueryContent = "Select phone, mail from Employees where phone = @Phone and mail = @Mail",
                    SqlQueryCreatedDate = DateTime.Today.AddDays(-1)
                }
            };
            queryParametersDbSet = new TestDbSet<SqlQueryParameter>()
            {
                new SqlQueryParameter()
                {
                    SqlQueryParameterId = 0,
                    SqlQueryId = 0,
                    ParameterId = 1
                },
                new SqlQueryParameter()
                {
                    SqlQueryParameterId = 1,
                    SqlQueryId = 0,
                    ParameterId = 2
                },
                new SqlQueryParameter()
                {
                    SqlQueryParameterId = 2,
                    SqlQueryId = 1,
                    ParameterId = 5
                },
                new SqlQueryParameter()
                {
                    SqlQueryParameterId = 3,
                    SqlQueryId = 1,
                    ParameterId = 6
                }
            };

            mock = new Mock<IContext>();
            mock
                .Setup(p => p.Parameters)
                .Returns(parametersDbSet);
            mock
                .Setup(s => s.SqlQueries)
                .Returns(queriesDbSet);
            mock
                .Setup(q => q.SqlQueryParameters)
                .Returns(queryParametersDbSet);

            queryParamRepo = new SqlQueryParameterRepository(mock.Object);
        }

        [TestMethod]
        public void GetAllQueryParameters_Test()
        {
            // Arrange
            int queryParametersCountBeforeAct = queryParametersDbSet.Count();

            // Act
            IEnumerable<SqlQueryParameter> queryParameters = queryParamRepo.GetAll();

            // Assert
            Assert.AreEqual(
                queryParametersCountBeforeAct,
                queryParameters.Count()
            );
            Assert.AreEqual(
                queryParametersCountBeforeAct,
                queryParametersDbSet.Count()
            );
            Assert.AreEqual(
                queryParametersDbSet.First().SqlQueryParameterId,
                queryParameters.First().SqlQueryParameterId
            );
        }

        [TestMethod]
        public void GetQueryParameterById_Test()
        {
            // Arrange
            int queryParameterId = queryParametersDbSet.First().SqlQueryParameterId;
            int queryParametersCountBeforeAct = queryParametersDbSet.Count();

            // Act
            SqlQueryParameter queryParam = queryParamRepo.Get(queryParameterId);

            // Assert
            Assert.AreEqual(
                queryParameterId,
                queryParam.SqlQueryParameterId
            );
            Assert.AreEqual(
                queryParametersCountBeforeAct,
                queryParametersDbSet.Count()
            );
            Assert.AreEqual(
                queryParametersDbSet.First().SqlQueryParameterId,
                queryParam.SqlQueryParameterId
            );
        }

        [TestMethod]
        public void CreateQueryParameter_Test()
        {
            // Arrange
            int queryParametersCountBeforeAct = queryParametersDbSet.Count();
            int lastQueryParameterIdInDbSetBeforeAct = queryParametersDbSet
                .Last()
                .SqlQueryParameterId;

            int queryParameterId = 12;
            int parameterId = 12;
            int queryId = 12;
            SqlQueryParameter queryParameterToCreate = new SqlQueryParameter()
            {
                SqlQueryParameterId = queryParameterId,
                ParameterId = parameterId,
                SqlQueryId = queryId
            };

            // Act
            queryParamRepo.Create(queryParameterToCreate);
            SqlQueryParameter createdQueryParam = queryParametersDbSet.Last();

            // Assert
            Assert.IsTrue(queryParametersDbSet.Count() > queryParametersCountBeforeAct);
            Assert.AreNotEqual(
                lastQueryParameterIdInDbSetBeforeAct,
                createdQueryParam.SqlQueryParameterId
            );
            Assert.AreEqual(queryParameterId, createdQueryParam.SqlQueryParameterId);
            Assert.AreEqual(parameterId, createdQueryParam.ParameterId);
            Assert.AreEqual(queryId, createdQueryParam.SqlQueryId);
        }

        [TestMethod]
        public void UpdateParameter_Test()
        {
            // Arrange
            int queryParametersCountBeforeAct = queryParametersDbSet.Count();
            int queryParameterIdBeforeAct = queryParametersDbSet.First().SqlQueryParameterId;
            int queryParameterParamIdBeforeAct = queryParametersDbSet.First().ParameterId;
            int queryParameterQueryIdBeforeAct = queryParametersDbSet.First().SqlQueryId;

            SqlQueryParameter queryParamToUpdate = queryParametersDbSet.First();
            int newQueryParameterParamId = 999;
            int newQueryParameterQueryId = 999;
            queryParamToUpdate.ParameterId = newQueryParameterParamId;
            queryParamToUpdate.SqlQueryId = newQueryParameterQueryId;

            // Act
            queryParamRepo.Update(queryParamToUpdate);
            SqlQueryParameter updatedParam = queryParametersDbSet.First();

            // Assert
            Assert.AreEqual(newQueryParameterParamId, updatedParam.ParameterId);
            Assert.AreEqual(newQueryParameterQueryId, updatedParam.SqlQueryId);
            Assert.AreEqual(queryParameterIdBeforeAct, updatedParam.SqlQueryParameterId);
            Assert.AreEqual(queryParametersCountBeforeAct, queryParametersDbSet.Count());

            Assert.AreNotEqual(queryParameterParamIdBeforeAct, updatedParam.ParameterId);
            Assert.AreNotEqual(queryParameterQueryIdBeforeAct, updatedParam.SqlQueryId);
        }

        [TestMethod]
        public void DeleteParaemter_Test()
        {
            // Arrange
            int queryParametersCountBeforeAct = queryParametersDbSet.Count();
            int queryParameterIdToDelete = queryParametersDbSet.Last().SqlQueryParameterId;

            // Act
            queryParamRepo.Delete(queryParametersDbSet.Last().SqlQueryParameterId);

            // Assert
            Assert.IsTrue(queryParametersCountBeforeAct > queryParametersDbSet.Count());
            Assert.AreEqual(queryParametersCountBeforeAct, (queryParametersDbSet.Count()+1));
            foreach (SqlQueryParameter queryParam in queryParametersDbSet)
                Assert.IsTrue(queryParam.SqlQueryParameterId != queryParameterIdToDelete);
        }

        [TestMethod]
        public void GetSqlQueryIdByParameterId_Test()
        {
            // Arrange
            TestDbSet<Parameter> parameters = new TestDbSet<Parameter>()
            {
                new Parameter()
                {
                    ParameterId = 0,
                    ParameterName = "@Name",
                    ParameterRuName = "RuName",
                    ParameterType = "text",
                    ParameterCreatedDate = DateTime.Today
                },
                new Parameter()
                {
                    ParameterId = 1,
                    ParameterName = "@Age",
                    ParameterRuName = "RuAge",
                    ParameterType = "number",
                    ParameterCreatedDate = DateTime.Today.AddDays(-1)
                }
            };
            TestDbSet<SqlQuery> queries = new TestDbSet<SqlQuery>()
            {
                new SqlQuery()
                {
                    SqlQueryId = 0,
                    SqlQueryName = "SelectAllFromTable",
                    SqlQueryContent = "Select * from Table",
                    SqlQueryCreatedDate = DateTime.Now
                },
                new SqlQuery()
                {
                    SqlQueryId = 1,
                    SqlQueryName = "DropTable",
                    SqlQueryContent = "Drop table Table",
                    SqlQueryCreatedDate = DateTime.Today
                }
            };
            TestDbSet<SqlQueryParameter> queryParams = new TestDbSet<SqlQueryParameter>()
            {
                new SqlQueryParameter()
                {
                    SqlQueryParameterId = 0,
                    ParameterId = 0,
                    SqlQueryId = 1
                },
                new SqlQueryParameter()
                {
                    SqlQueryParameterId = 1,
                    ParameterId = 1,
                    SqlQueryId = 0
                }
            };

            Mock<IContext> mock = new Mock<IContext>();
            mock
                .Setup(p => p.Parameters)
                .Returns(parameters);
            mock
                .Setup(q => q.SqlQueries)
                .Returns(queries);
            mock
                .Setup(s => s.SqlQueryParameters)
                .Returns(queryParams);

            SqlQueryParameterRepository queryParamRepo = new SqlQueryParameterRepository(mock.Object);

            int parameterId = parameters.First().ParameterId;
            List<int> queryIds = queryParams.
                Where(p => p.ParameterId == parameterId).
                Select(q => q.SqlQueryId).
                ToList();

            // Act
            IEnumerable<int> funcResult = queryParamRepo.GetSqlQueryIdByParameterId(parameters.First().ParameterId);

            // Assert
            Assert.AreEqual(queryIds.Count(), funcResult.Count());
            Assert.AreEqual(queryIds.First(), funcResult.First());
        }
    }
}
