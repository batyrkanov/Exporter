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
    public class ParameterRepository_Tests
    {
        TestDbSet<Parameter> parametersDbSet;

        [TestInitialize]
        public void TestInitailize()
        {
            parametersDbSet = new TestDbSet<Parameter>()
            {
                new Parameter()
                {
                    ParameterId = 0,
                    ParameterName = "@test",
                    ParameterRuName = "test",
                    ParameterType = "text",
                    ParameterCreatedDate = DateTime.Now
                },
                new Parameter()
                {
                    ParameterId = 1,
                    ParameterName = "@another_one",
                    ParameterRuName = "another_one",
                    ParameterType = "number",
                    ParameterCreatedDate = DateTime.Now
                }
            };
        }

        [TestMethod]
        public void CreateParameter()
        {
            // Arrange
            var ParametersDbSet = new TestDbSet<Parameter>();
            var mock = new Mock<IContext>();
            mock.Setup(x => x.Parameters).Returns(ParametersDbSet);

            ParameterRepository repository = new ParameterRepository(mock.Object);

            // Act
            Parameter parameter = new Parameter()
            {
                ParameterName = "@added",
                ParameterRuName = "added",
                ParameterCreatedDate = DateTime.Now
            };
            repository.Create(parameter);

            // Assert
            Assert.AreEqual(1, ParametersDbSet.Count());
            Assert.AreEqual("@added", ParametersDbSet.ToList()[0].ParameterName);
        }

        [TestMethod]
        public void GetAllParameters()
        {
            // Arrange
            var mock = new Mock<IContext>();
            mock
                .Setup(x => x.Parameters)
                .Returns(parametersDbSet);

            ParameterRepository repository = new ParameterRepository(mock.Object);

            // Act
            IEnumerable<Parameter> allParameters = repository.GetAll();

            // Assert
            Assert.AreEqual(2, allParameters.Count());
            Assert.AreEqual("@test", allParameters.ToList()[0].ParameterName);
            Assert.AreNotEqual("@new", allParameters.ToList()[1].ParameterName);
        }

        [TestMethod]
        public void GetParameterById()
        {
            // Arrange
            var mock = new Mock<IContext>();
            mock
                .Setup(x => x.Parameters)
                .Returns(parametersDbSet);

            ParameterRepository repository = new ParameterRepository(mock.Object);

            // Act
            Parameter parameter_1 = repository.Get(0);
            Parameter parameter_2 = repository.Get(1);

            // Assert
            Assert.AreEqual("@test", parameter_1.ParameterName);
            Assert.AreEqual(parameter_1, parametersDbSet.ToList()[0]);

            Assert.AreEqual("@another_one", parameter_2.ParameterName);
            Assert.AreEqual(parameter_2, parametersDbSet.ToList()[1]);

            Assert.AreEqual(2, parametersDbSet.Count());
        }

        [TestMethod]
        public void UpdateParameter()
        {
            // Arrange
            var mock = new Mock<IContext>();
            mock
                .Setup(x => x.Parameters)
                .Returns(parametersDbSet);

            ParameterRepository repository = new ParameterRepository(mock.Object);

            // Act
            Parameter parameter = parametersDbSet.ToList()[0];
            parameter.ParameterName = "UpdatedName";
            parameter.ParameterRuName = "название было изменено";
            parameter.ParameterType = "number";
            repository.Update(parameter);

            // Assert
            Parameter updatedParameter = parametersDbSet.ToList()[0];
            Assert.AreEqual("UpdatedName", updatedParameter.ParameterName);
            Assert.AreEqual("название было изменено", updatedParameter.ParameterRuName);
            Assert.AreEqual("number", updatedParameter.ParameterType);

            Assert.AreEqual("@another_one", parametersDbSet.ToList()[1].ParameterName);
            Assert.AreNotEqual("UpdatedName", parametersDbSet.ToList()[1].ParameterName);

            Assert.AreEqual(2, parametersDbSet.Count());
        }

        [TestMethod]
        public void DeleteParameterById()
        {
            // Arrange
            var mock = new Mock<IContext>();
            mock
                .Setup(x => x.Parameters)
                .Returns(parametersDbSet);

            ParameterRepository repository = new ParameterRepository(mock.Object);

            // Act
            repository.Delete(0);
            repository.Delete(35);

            // Assert
            Assert.AreEqual(1, parametersDbSet.Count());
            Assert.AreEqual("@another_one", parametersDbSet.Last().ParameterName);
        }

        [TestMethod]
        public void SaveParameterChanges()
        {
            // Arrange
            var mock = new Mock<IContext>();
            mock
                .Setup(x => x.Parameters)
                .Returns(parametersDbSet);

            ParameterRepository repository = new ParameterRepository(mock.Object);

            // Act
            Parameter parameter = parametersDbSet.First();
            int parameterId = repository.SaveChanges(parameter.ParameterId, "newName", "новое имя", "phone");

            Parameter modifiedParameter = repository.Get(parameterId);

            // Assert
            Assert.AreEqual(parameter, modifiedParameter);

            Assert.AreEqual("newName", modifiedParameter.ParameterName);
            Assert.AreEqual("новое имя", modifiedParameter.ParameterRuName);
            Assert.AreEqual("phone", modifiedParameter.ParameterType);

            Assert.AreNotEqual("wrongNewName", modifiedParameter.ParameterName);
        }

        [TestMethod]
        public void CreateMethodWithTwoParams()
        {
            // Arrange
            var mock = new Mock<IContext>();
            mock
                .Setup(x => x.Parameters)
                .Returns(parametersDbSet);

            ParameterRepository repository = new ParameterRepository(mock.Object);

            // Act
            Parameter parameter = new Parameter()
            {
                ParameterName = "@newName",
                ParameterRuName = "имя"
            };

            repository.Create(parameter, "text");

            Parameter createdParameter = parametersDbSet.Last();

            // Assert
            Assert.AreEqual(3, parametersDbSet.Count());

            Assert.AreEqual("@newName", createdParameter.ParameterName);
            Assert.AreEqual("имя", createdParameter.ParameterRuName);
            Assert.AreEqual("text", createdParameter.ParameterType);

            Assert.IsNotNull(createdParameter.ParameterCreatedDate);
            Assert.AreEqual(typeof(DateTime), createdParameter.ParameterCreatedDate.GetType());
            Assert.AreNotEqual(DateTime.Now, createdParameter.ParameterCreatedDate);
        }

        [TestMethod]
        public void CreateMethodWithThreeParams()
        {
            // Arrange
            var mock = new Mock<IContext>();
            mock
                .Setup(x => x.Parameters)
                .Returns(parametersDbSet);

            ParameterRepository repository = new ParameterRepository(mock.Object);

            // Act
            int parameterId = repository.Create("new parameter", "новый параметр", "phone");
            Parameter lastParameter = parametersDbSet.Last();

            // Assert
            Assert.AreEqual(3, parametersDbSet.Count());
            Assert.IsNotNull(parameterId);

            Assert.AreEqual("new parameter", lastParameter.ParameterName);
            Assert.AreEqual("новый параметр", lastParameter.ParameterRuName);
            Assert.AreEqual("phone", lastParameter.ParameterType);
        }

        [TestMethod]
        public void UpdateMethodWithTwoParams()
        {
            // Arrange
            var mock = new Mock<IContext>();
            mock
                .Setup(x => x.Parameters)
                .Returns(parametersDbSet);

            ParameterRepository repository = new ParameterRepository(mock.Object);
            Parameter parameter = parametersDbSet.Last();
            parameter.ParameterName = "@newName";
            parameter.ParameterRuName = "новое имя";

            // Act
            repository.Update(parameter, "phone");
            Parameter modifiedParameter = parametersDbSet.Last();

            // Assert
            Assert.AreEqual(2, parametersDbSet.Count());

            Assert.AreEqual("@newName", modifiedParameter.ParameterName);
            Assert.AreEqual("новое имя", modifiedParameter.ParameterRuName);
            Assert.AreEqual(1, modifiedParameter.ParameterId);
            Assert.AreEqual("phone", modifiedParameter.ParameterType);
        }

        [TestMethod]
        public void GetQueryParametersByQueryIdMethodTest()
        {
            // Arrange
            TestDbSet<Parameter> parametersDbSet = new TestDbSet<Parameter>()
            {
                new Parameter()
                {
                    ParameterId = 0,
                    ParameterName = "@test",
                    ParameterRuName = "test",
                    ParameterType = "text",
                    ParameterCreatedDate = DateTime.Now
                },
                new Parameter()
                {
                    ParameterId = 1,
                    ParameterName = "@another_one",
                    ParameterRuName = "another_one",
                    ParameterType = "number",
                    ParameterCreatedDate = DateTime.Now
                },
                new Parameter()
                {
                    ParameterId = 2,
                    ParameterName = "@param",
                    ParameterRuName = "param",
                    ParameterType = "phone",
                    ParameterCreatedDate = DateTime.Now
                }
            };
            TestDbSet<SqlQuery> queriesDbSet = new TestDbSet<SqlQuery>()
            {
                new SqlQuery()
                {
                    SqlQueryId = 0,
                    SqlQueryName = "First query",
                    SqlQueryContent = "Select * from test",
                    SqlQueryCreatedDate = DateTime.Now
                }
            };
            TestDbSet<SqlQueryParameter> queryParametersDbSet = new TestDbSet<SqlQueryParameter>()
            {
                new SqlQueryParameter()
                {
                    SqlQueryParameterId = 0,
                    SqlQueryId = 0,
                    ParameterId = 0
                },
                new SqlQueryParameter()
                {
                    SqlQueryParameterId = 1,
                    SqlQueryId = 0,
                    ParameterId = 1
                }
            };

            var mock = new Mock<IContext>();
            mock
                .Setup(x => x.Parameters)
                .Returns(parametersDbSet);
            mock
                .Setup(x => x.SqlQueries)
                .Returns(queriesDbSet);
            mock
                .Setup(x => x.SqlQueryParameters)
                .Returns(queryParametersDbSet);

            ParameterRepository repository = new ParameterRepository(mock.Object);

            // Act
            IEnumerable<Parameter> parameters = repository.GetQueryParametersByQueryId(0);

            // Assert
            Assert.AreEqual(2, parameters.Count());
            Assert.IsTrue(
                queryParametersDbSet
                .Select(i => i.ParameterId)
                .Contains(parameters.First().ParameterId)
            );
            Assert.IsTrue(
                queryParametersDbSet
                .Select(i => i.ParameterId)
                .Contains(parameters.Last().ParameterId)
            );
        }

        [TestMethod]
        public void FindParametersByNameOrderedByCreatedDescMethodTest()
        {
            // Arrange
            TestDbSet<Parameter> parametersDbSet = new TestDbSet<Parameter>()
            {
                new Parameter()
                {
                    ParameterId = 0,
                    ParameterName = "first parameter name",
                    ParameterRuName = "nevermind",
                    ParameterType = "text",
                    ParameterCreatedDate = DateTime.Today
                },
                new Parameter()
                {
                    ParameterId = 1,
                    ParameterName = "second parameter",
                    ParameterRuName = "same",
                    ParameterType = "phone",
                    ParameterCreatedDate = DateTime.Today.AddDays(-1)
                },
                new Parameter()
                {
                    ParameterId = 2,
                    ParameterName = "third parameter name",
                    ParameterRuName = "unclear",
                    ParameterType = "number",
                    ParameterCreatedDate = DateTime.Today.AddDays(-2)
                }
            };

            var mock = new Mock<IContext>();
            mock
                .Setup(x => x.Parameters)
                .Returns(parametersDbSet);

            ParameterRepository repository = new ParameterRepository(mock.Object);

            // Act
            IEnumerable<Parameter> parameters = repository.FindParametersByNameOrderedByCreatedDesc("name");
            int[] expectedParameterIds = new int[] { 0, 2 };

            // Assert
            Assert.AreEqual(2, parameters.Count());
            Assert.IsFalse(expectedParameterIds.Contains(1));
            Assert.IsTrue(parameters.First().ParameterCreatedDate > parameters.Last().ParameterCreatedDate);

            foreach (Parameter param in parameters)
            {
                Assert.IsTrue(param.ParameterName.Contains("name"));
                Assert.IsTrue(expectedParameterIds.Contains(param.ParameterId));
            }

        }
    }
}
