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
    public class QueryRepository_Tests
    {
        TestDbSet<Parameter> parametersDbSet;
        TestDbSet<SqlQuery> queriesDbSet;
        TestDbSet<SqlQueryParameter> queryParametersDbSet;

        Mock<IContext> mock;
        QueryRepository queryRepo;

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

            queryRepo = new QueryRepository(mock.Object);
        }

        [TestMethod]
        public void GetAllQueries()
        {
            // Act
            IEnumerable<SqlQuery> queries = queryRepo.GetAll();

            // Assert
            Assert.AreEqual(queriesDbSet.Count(), queries.Count());
            Assert.AreEqual(
                queriesDbSet.First(),
                queries.First()
            );
            Assert.AreEqual(
                queriesDbSet.Last(),
                queries.Last()
            );
            Assert.AreEqual(
                queriesDbSet.First().SqlQueryName,
                queries.First().SqlQueryName
            );
        }
        
        [TestMethod]
        public void GetQuery()
        {
            // Arrange
            int ElementsCountBeforeAct = queriesDbSet.Count();
            SqlQuery expectedQuery = queriesDbSet.First();

            // Act
            SqlQuery query = queryRepo.Get(0);

            // Assert
            Assert.AreEqual(expectedQuery, query);
            Assert.AreEqual(expectedQuery.SqlQueryName, query.SqlQueryName);
            Assert.AreEqual(expectedQuery.SqlQueryId, query.SqlQueryId);
            Assert.AreNotEqual(query, queriesDbSet.Last());
            Assert.IsTrue(queriesDbSet.Count() == ElementsCountBeforeAct);
        }

        [TestMethod]
        public void CreateQuery()
        {
            // Arrange
            int ElementsCountBeforeCreate = queriesDbSet.Count();
            SqlQuery query = new SqlQuery()
            {
                SqlQueryId = 2,
                SqlQueryName = "Added query",
                SqlQueryContent = "Select * from test",
                SqlQueryCreatedDate = DateTime.Now
            };

            // Act
            queryRepo.Create(query);

            // Assert
            Assert.AreNotEqual(ElementsCountBeforeCreate, queriesDbSet.Count());
            Assert.IsTrue(queriesDbSet.Count() > ElementsCountBeforeCreate);
            Assert.AreEqual("Added query", queriesDbSet.Last().SqlQueryName);
        }

        [TestMethod]
        public void UpdateQuery()
        {
            // Arrange
            int ElementsCountBeforeAct = queriesDbSet.Count();

            int queryIdBeforeAct = queriesDbSet.First().SqlQueryId;
            string queryNameBeforeAct = queriesDbSet.First().SqlQueryName;
            string queryContentBeforeAct = queriesDbSet.First().SqlQueryContent;

            string newQueryName = "Updated query name";
            string newQueryContent = "one word select";

            SqlQuery queryToUpdate = queriesDbSet.First();
            queryToUpdate.SqlQueryName = newQueryName;
            queryToUpdate.SqlQueryContent = newQueryContent;

            // Act
            queryRepo.Update(queryToUpdate);
            SqlQuery updatedQuery = queriesDbSet.First();

            // Assert
            Assert.AreEqual(newQueryName, updatedQuery.SqlQueryName);
            Assert.AreEqual(newQueryContent, updatedQuery.SqlQueryContent);

            Assert.AreNotEqual(queryNameBeforeAct, updatedQuery.SqlQueryName);
            Assert.AreNotEqual(queryContentBeforeAct, updatedQuery.SqlQueryContent);

            Assert.AreEqual(queryIdBeforeAct, updatedQuery.SqlQueryId);
            Assert.AreEqual(ElementsCountBeforeAct, queriesDbSet.Count());
        }

        [TestMethod]
        public void DeleteQuery()
        {
            // Arrange
            int ElementsCountBeforeAct = queriesDbSet.Count();

            string deletedQueryName = queriesDbSet.First().SqlQueryName;
            int queryIdToDelete = queriesDbSet.First().SqlQueryId;

            // Act
            queryRepo.Delete(queryIdToDelete);

            // Assert
            Assert.AreNotEqual(ElementsCountBeforeAct, queriesDbSet.Count());
            Assert.IsTrue(ElementsCountBeforeAct > queriesDbSet.Count());

            foreach (SqlQuery query in queriesDbSet)
                Assert.IsFalse(query.SqlQueryName == deletedQueryName);
        }

        [TestMethod]
        public void CreateQueryWithTwoParams_WhenSecondParameterIsNull()
        {
            // Arrange
            int queryElementsCountBeforeAct = queriesDbSet.Count();
            int queryParametersElementsCountBeforeAct = queryParametersDbSet.Count();

            string newQueryName = "New query";
            string newQueryСontent = "string type";

            SqlQuery queryToCreate = new SqlQuery()
            {
                SqlQueryId = queryElementsCountBeforeAct + 1,
                SqlQueryName = newQueryName,
                SqlQueryContent = newQueryСontent,
                SqlQueryCreatedDate = DateTime.Now
            };
            string[] parameterIds = null;

            // Act
            queryRepo.Create(queryToCreate, parameterIds);
            SqlQuery createdQuery = queriesDbSet.Last();

            // Assert
            Assert.AreNotEqual(queryElementsCountBeforeAct, queriesDbSet.Count());
            Assert.AreEqual(queryParametersElementsCountBeforeAct, queryParametersDbSet.Count());

            Assert.AreEqual(newQueryName, createdQuery.SqlQueryName);
            Assert.AreEqual(newQueryСontent, createdQuery.SqlQueryContent);
        }

        [TestMethod]
        public void CreateQueryWithTwoParams_WhenSecondParameterIsNotNull()
        {
            // Arrange
            int queryElementsCountBeforeAct = queriesDbSet.Count();
            int queryParametersElementsCountBeforeAct = queryParametersDbSet.Count();
            int parametersElementsCountBeforeAct = parametersDbSet.Count();

            string createQueryName = "New query";
            string createQueryContent = "string type";

            int firstQueryParameterId = parametersElementsCountBeforeAct + 1;
            string firstQueryParameterName = "CreatedQueryFirstParameterName";
            string firstQueryParameterRuName = "первый параметр запроса";
            string firstQueryParameterType = "text";

            int secondQueryParameterId = firstQueryParameterId + 1;
            string secondQueryParameterName = "CreateQuerySecondParameterName";
            string secondQueryParameterRuName = "второй параметр запроса";
            string secondQueryParameterType = "phone";

            SqlQuery queryToCreate = new SqlQuery()
            {
                SqlQueryId = queryElementsCountBeforeAct + 1,
                SqlQueryName = createQueryName,
                SqlQueryContent = createQueryContent,
                SqlQueryCreatedDate = DateTime.Now
            };

            Parameter firstParameter = new Parameter()
            {
                ParameterId = firstQueryParameterId,
                ParameterName = firstQueryParameterName,
                ParameterRuName = firstQueryParameterRuName,
                ParameterType = firstQueryParameterType
            };
            parametersDbSet.Add(firstParameter);
            Parameter secondParameter = new Parameter()
            {
                ParameterId = secondQueryParameterId,
                ParameterName = secondQueryParameterName,
                ParameterRuName = secondQueryParameterRuName,
                ParameterType = secondQueryParameterType
            };
            parametersDbSet.Add(secondParameter);

            string[] parameterIds = new string[]
            {
               firstParameter.ParameterId.ToString(),
               secondParameter.ParameterId.ToString()
            };

            // Act
            queryRepo.Create(queryToCreate, parameterIds);
            SqlQuery createdQuery = queriesDbSet.Last();
            int lastQueryParameterId = queryParametersDbSet.Last().SqlQueryParameterId;
            SqlQueryParameter[] twoLastQueryParameters = new SqlQueryParameter[]
            {
                queryParametersDbSet.Last(),
                queryParametersDbSet.ToList()[queryParametersDbSet.Count()-2]
            };

            // Assert
            Assert.IsTrue(queriesDbSet.Count() > queryElementsCountBeforeAct);
            Assert.IsTrue(parametersDbSet.Count() > parametersElementsCountBeforeAct);
            Assert.IsTrue(queryParametersDbSet.Count() > queryParametersElementsCountBeforeAct);

            foreach (SqlQueryParameter queryParameter in twoLastQueryParameters)
            {
                Assert.IsTrue(queryParameter.SqlQueryId == createdQuery.SqlQueryId);
                Assert.IsTrue(
                    queryParameter.ParameterId == firstQueryParameterId ||
                    queryParameter.ParameterId == secondQueryParameterId
                );
            }
        }

        [TestMethod]
        public void EditQueryWithTwoParams_WhenSecondParameterIsNull_ShouldRemoveParams()
        {
            // Arrange
            int queryElementsCountBeforeAct = queriesDbSet.Count();
            int parametersElementsCountBeforeAct = parametersDbSet.Count();
            int queryParametersElementsCountBeforeAct = queryParametersDbSet.Count();

            int queryIdBeforeAct = queriesDbSet.First().SqlQueryId;
            string queryNameBeforeAct = queriesDbSet.First().SqlQueryName;
            string queryContentBeforeAct = queriesDbSet.First().SqlQueryContent;
            DateTime queryCreatedDateBeforeAct = queriesDbSet.First().SqlQueryCreatedDate;

            string newQueryName = "new query name";
            string newQueryContent = "new query content";

            int queryParametersCount = queryParametersDbSet
                .Where(p => p.SqlQueryId == queriesDbSet.First().SqlQueryId)
                .Count();

            SqlQuery queryToUpdate = queriesDbSet.First();
            queryToUpdate.SqlQueryName = newQueryName;
            queryToUpdate.SqlQueryContent = newQueryContent;

            string[] parameterIds = null;

            // Act
            queryRepo.Edit(queryToUpdate, parameterIds);
            SqlQuery updatedQuery = queriesDbSet.ElementAt(queryToUpdate.SqlQueryId);

            // Assert
            Assert.AreEqual(queryElementsCountBeforeAct, queriesDbSet.Count());
            Assert.AreEqual(parametersElementsCountBeforeAct, parametersDbSet.Count());
            Assert.IsTrue(queryParametersElementsCountBeforeAct > queryParametersDbSet.Count());

            Assert.AreEqual(newQueryName, updatedQuery.SqlQueryName);
            Assert.AreEqual(newQueryContent, updatedQuery.SqlQueryContent);
            Assert.AreEqual(queryCreatedDateBeforeAct, updatedQuery.SqlQueryCreatedDate);

            Assert.AreEqual(
                queryParametersElementsCountBeforeAct,
                (queryParametersDbSet.Count() + queryParametersCount)
            );

            Assert.IsTrue(
                queryParametersDbSet
                .Where(p => p.SqlQueryId == queryToUpdate.SqlQueryId)
                .Count() <= 0    
            );
        }

        [TestMethod]
        public void DeleteQueryById_Test()
        {
            // Arrange
            int queryElementsCountBeforeAct = queriesDbSet.Count();
            int toDeleteQueryId = queriesDbSet.First().SqlQueryId;
            string toDeleteQueryName = queriesDbSet.First().SqlQueryName;

            SqlQuery queryToDelete = queriesDbSet.First();

            // Act
            queryRepo.Delete(queryToDelete.SqlQueryId);

            // Assert
            foreach (SqlQuery query in queriesDbSet)
                Assert.IsTrue(query.SqlQueryId != toDeleteQueryId && query.SqlQueryName != toDeleteQueryName);
            Assert.IsTrue(queryElementsCountBeforeAct > queriesDbSet.Count());
            Assert.IsTrue(queriesDbSet.Count() > 0);
        }

        [TestMethod]
        public void GetQueriesFromListById_Test()
        {
            // Arrange
            List<int> identifiers = new List<int>() { 0, 1 };

            // Act
            IEnumerable<SqlQuery> queries = queryRepo.GetQueriesFromListById(identifiers);

            // Assert
            foreach (SqlQuery query in queries)
                Assert.IsTrue(identifiers.Contains(query.SqlQueryId));
            Assert.AreEqual(queries.Count(), queriesDbSet.Count());
        }

        [TestMethod]
        public void GetQueriesFromListByName_Test()
        {
            // Arrange
            string select = "select";
            string person = "person";
            List<SqlQuery> allQueries = queriesDbSet.ToList();

            int QueriesWithWordSelectAmount = 0;
            int QueriesWithWordPersonAmount = 0;
            foreach (SqlQuery query in queriesDbSet)
            {
                if (query.SqlQueryName.Contains(select))
                    QueriesWithWordSelectAmount++;
                if (query.SqlQueryName.Contains(person))
                    QueriesWithWordPersonAmount++;
            }

            // Act
            IEnumerable<SqlQuery> queryNameContainsSelect = queryRepo
                .GetQueriesFromListByName(allQueries, select);
            IEnumerable<SqlQuery> queryNameContainsPerson = queryRepo
                .GetQueriesFromListByName(allQueries, person);

            // Assert
            Assert.AreEqual(
                QueriesWithWordSelectAmount,
                queryNameContainsSelect.Count()
            );
            Assert.AreEqual(
                QueriesWithWordPersonAmount,
                queryNameContainsPerson.Count()
            );

            foreach (SqlQuery query in queryNameContainsSelect)
                Assert.IsTrue(query.SqlQueryName.Contains(select));
            foreach (SqlQuery query in queryNameContainsPerson)
                Assert.IsTrue(query.SqlQueryName.Contains(person));
        }

        [TestMethod]
        public void OrderQueryByNameDesc_Test()
        {
            // Arrange
            List<SqlQuery> unorderedQueries = new List<SqlQuery>() {
                new SqlQuery()
                {
                    SqlQueryId = 1,
                    SqlQueryName = "QueryNameA",
                    SqlQueryContent = "QueryContentA",
                    SqlQueryCreatedDate = DateTime.Today.AddDays(-1)
                },
                new SqlQuery() {
                    SqlQueryId = 0,
                    SqlQueryName = "QueryNameB",
                    SqlQueryContent = "QueryContentB",
                    SqlQueryCreatedDate = DateTime.Today
                }
            };

            // Act
            IEnumerable<SqlQuery> orderedQueries = queryRepo.OrderQueryByNameDesc(unorderedQueries);

            // Assert
            Assert.AreNotEqual(
                unorderedQueries.First().SqlQueryName,
                orderedQueries.First().SqlQueryName
            );
            Assert.AreNotEqual(
                unorderedQueries.Last().SqlQueryName,
                orderedQueries.Last().SqlQueryName
            );

            Assert.AreEqual(
                unorderedQueries.Count(),
                orderedQueries.Count()
            );
        }
    }
}
