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
    }
}
