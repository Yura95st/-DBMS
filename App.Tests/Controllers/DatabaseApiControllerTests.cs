namespace App.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Results;

    using App.Controllers;
    using App.DTOs;
    using App.Exceptions;
    using App.Models;
    using App.Services.Abstract;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class DatabaseApiControllerTests
    {
        private Mock<IDatabaseService> _dbServiceMock;

        [Test]
        public void AddRow_DatabaseServiceAddsRow_ReturnsOkResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row { Value = { "someValue" } };

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.AddRow(dbName, tableName, row);

            // Assert
            Assert.IsInstanceOf<OkResult>(actionResult);

            this._dbServiceMock.Verify(s => s.AddRow(dbName, tableName, row), Times.Once);
        }

        [Test]
        public void AddRow_DatabaseServiceThrowsExceptions_ReturnsValidResults()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row { Value = { "someValue" } };

            Dictionary<Exception, Type> resultsDictionary = new Dictionary<Exception, Type>
            {
                { new ArgumentException(), typeof(BadRequestResult) },
                { new DatabaseNotFoundException(), typeof(BadRequestErrorMessageResult) },
                { new TableNotFoundException(), typeof(BadRequestErrorMessageResult) },
                { new InvalidRowException(), typeof(BadRequestErrorMessageResult) },
                { new DbServiceException(), typeof(InternalServerErrorResult) }
            };

            foreach (KeyValuePair<Exception, Type> result in resultsDictionary)
            {
                // Arrange - mock dbService
                this._dbServiceMock.Setup(s => s.AddRow(dbName, tableName, row))
                    .Throws(result.Key);

                // Arrange - create target
                DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

                // Act
                IHttpActionResult actionResult = target.AddRow(dbName, tableName, row);

                // Assert
                Assert.IsInstanceOf(result.Value, actionResult);
            }
        }

        [Test]
        public void CreateDatabase_DatabaseServiceCreatesDatabase_ReturnsCreatedAtRouteResult()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.CreateDatabase(dbName);

            CreatedAtRouteNegotiatedContentResult<string> createdResult =
                actionResult as CreatedAtRouteNegotiatedContentResult<string>;

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("GetDatabase", createdResult.RouteName);
            Assert.AreEqual(dbName, createdResult.RouteValues["dbName"]);

            this._dbServiceMock.Verify(s => s.CreateDatabase(dbName), Times.Once);
        }

        [Test]
        public void CreateDatabase_DatabaseServiceThrowsExceptions_ReturnsValidResults()
        {
            // Arrange
            string dbName = "testDatabase";

            Dictionary<Exception, Type> resultsDictionary = new Dictionary<Exception, Type>
            {
                { new ArgumentException(), typeof(BadRequestResult) },
                { new InvalidNameFormatException(), typeof(BadRequestErrorMessageResult) },
                { new DatabaseAlreadyExistsException(), typeof(ConflictResult) },
                { new DbServiceException(), typeof(InternalServerErrorResult) }
            };

            foreach (KeyValuePair<Exception, Type> result in resultsDictionary)
            {
                // Arrange - mock dbService
                this._dbServiceMock.Setup(s => s.CreateDatabase(dbName))
                    .Throws(result.Key);

                // Arrange - create target
                DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

                // Act
                IHttpActionResult actionResult = target.CreateDatabase(dbName);

                // Assert
                Assert.IsInstanceOf(result.Value, actionResult);
            }
        }

        [Test]
        public void CreateTable_DatabaseServiceCreatesTable_ReturnsCreatedAtRouteResult()
        {
            // Arrange
            string dbName = "testDatabase";
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.CreateTable(dbName, tableScheme);

            CreatedAtRouteNegotiatedContentResult<TableScheme> createdResult =
                actionResult as CreatedAtRouteNegotiatedContentResult<TableScheme>;

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("GetTable", createdResult.RouteName);
            Assert.AreEqual(dbName, createdResult.RouteValues["dbName"]);
            Assert.AreEqual(tableScheme.Name, createdResult.RouteValues["tableName"]);

            this._dbServiceMock.Verify(s => s.CreateTable(dbName, tableScheme), Times.Once);
        }

        [Test]
        public void CreateTable_DatabaseServiceThrowsExceptions_ReturnsValidResults()
        {
            // Arrange
            string dbName = "testDatabase";
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            Dictionary<Exception, Type> resultsDictionary = new Dictionary<Exception, Type>
            {
                { new ArgumentException(), typeof(BadRequestResult) },
                { new DatabaseNotFoundException(), typeof(BadRequestErrorMessageResult) },
                { new InvalidTableSchemeException(), typeof(BadRequestErrorMessageResult) },
                { new TableAlreadyExistsException(), typeof(ConflictResult) },
                { new DbServiceException(), typeof(InternalServerErrorResult) }
            };

            foreach (KeyValuePair<Exception, Type> result in resultsDictionary)
            {
                // Arrange - mock dbService
                this._dbServiceMock.Setup(s => s.CreateTable(dbName, tableScheme))
                    .Throws(result.Key);

                // Arrange - create target
                DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

                // Act
                IHttpActionResult actionResult = target.CreateTable(dbName, tableScheme);

                // Assert
                Assert.IsInstanceOf(result.Value, actionResult);
            }
        }

        [Test]
        public void DeleteRow_DatabaseServiceDeletesRow_ReturnsOkResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            int rowId = 1;

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.DeleteRow(dbName, tableName, rowId);

            // Assert
            Assert.IsInstanceOf<OkResult>(actionResult);

            this._dbServiceMock.Verify(s => s.DeleteRow(dbName, tableName, rowId), Times.Once);
        }

        [Test]
        public void DeleteRow_DatabaseServiceThrowsExceptions_ReturnsValidResults()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            int rowId = 1;

            Dictionary<Exception, Type> resultsDictionary = new Dictionary<Exception, Type>
            {
                { new ArgumentException(), typeof(BadRequestResult) },
                { new DatabaseNotFoundException(), typeof(NotFoundResult) },
                { new TableNotFoundException(), typeof(NotFoundResult) },
                { new RowNotFoundException(), typeof(NotFoundResult) },
                { new DbServiceException(), typeof(InternalServerErrorResult) }
            };

            foreach (KeyValuePair<Exception, Type> result in resultsDictionary)
            {
                // Arrange - mock dbService
                this._dbServiceMock.Setup(s => s.DeleteRow(dbName, tableName, rowId))
                    .Throws(result.Key);

                // Arrange - create target
                DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

                // Act
                IHttpActionResult actionResult = target.DeleteRow(dbName, tableName, rowId);

                // Assert
                Assert.IsInstanceOf(result.Value, actionResult);
            }
        }

        [Test]
        public void DropDatabase_DatabaseServiceDropsDatabase_ReturnsOkResult()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.DropDatabase(dbName);

            // Assert
            Assert.IsInstanceOf<OkResult>(actionResult);

            this._dbServiceMock.Verify(s => s.DropDatabase(dbName), Times.Once);
        }

        [Test]
        public void DropDatabase_DatabaseServiceThrowsExceptions_ReturnsValidResults()
        {
            // Arrange
            string dbName = "testDatabase";

            Dictionary<Exception, Type> resultsDictionary = new Dictionary<Exception, Type>
            {
                { new ArgumentException(), typeof(BadRequestResult) },
                { new DatabaseNotFoundException(), typeof(NotFoundResult) },
                { new DbServiceException(), typeof(InternalServerErrorResult) }
            };

            foreach (KeyValuePair<Exception, Type> result in resultsDictionary)
            {
                // Arrange - mock dbService
                this._dbServiceMock.Setup(s => s.DropDatabase(dbName))
                    .Throws(result.Key);

                // Arrange - create target
                DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

                // Act
                IHttpActionResult actionResult = target.DropDatabase(dbName);

                // Assert
                Assert.IsInstanceOf(result.Value, actionResult);
            }
        }

        [Test]
        public void DropTable_DatabaseServiceDropsTable_ReturnsOkResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.DropTable(dbName, tableName);

            // Assert
            Assert.IsInstanceOf<OkResult>(actionResult);

            this._dbServiceMock.Verify(s => s.DropTable(dbName, tableName), Times.Once);
        }

        [Test]
        public void DropTable_DatabaseServiceThrowsExceptions_ReturnsValidResults()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            Dictionary<Exception, Type> resultsDictionary = new Dictionary<Exception, Type>
            {
                { new ArgumentException(), typeof(BadRequestResult) },
                { new DatabaseNotFoundException(), typeof(NotFoundResult) },
                { new TableNotFoundException(), typeof(NotFoundResult) },
                { new DbServiceException(), typeof(InternalServerErrorResult) }
            };

            foreach (KeyValuePair<Exception, Type> result in resultsDictionary)
            {
                // Arrange - mock dbService
                this._dbServiceMock.Setup(s => s.DropTable(dbName, tableName))
                    .Throws(result.Key);

                // Arrange - create target
                DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

                // Act
                IHttpActionResult actionResult = target.DropTable(dbName, tableName);

                // Assert
                Assert.IsInstanceOf(result.Value, actionResult);
            }
        }

        [Test]
        public void GetDatabase_DatabaseServiceReturnsDatabase_ReturnsDatabaseDto()
        {
            // Arrange
            Database database = new Database
                { Name = "someDatabase", Tables = { { "firstTable", new Table() }, { "secondTable", new Table() } } };

            DatabaseDto dbDto = new DatabaseDto { Name = database.Name, TableNames = database.Tables.Keys };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetDatabase(database.Name))
                .Returns(database);

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetDatabase(database.Name);
            OkNegotiatedContentResult<DatabaseDto> contentResult = actionResult as OkNegotiatedContentResult<DatabaseDto>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);

            Assert.AreEqual(dbDto.Name, contentResult.Content.Name);
            Assert.AreEqual(dbDto.TableNames, contentResult.Content.TableNames);
        }

        [Test]
        public void GetDatabase_DatabaseServiceReturnsNull_ReturnsNotFoundResult()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetDatabase(dbName))
                .Returns((Database)null);

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetDatabase(dbName);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public void GetDatabase_DatabaseServiceThrowsExceptions_ReturnsValidResults()
        {
            // Arrange
            string dbName = "testDatabase";

            Dictionary<Exception, Type> resultsDictionary = new Dictionary<Exception, Type>
            {
                { new ArgumentException(), typeof(BadRequestResult) },
                { new DbServiceException(), typeof(InternalServerErrorResult) }
            };

            foreach (KeyValuePair<Exception, Type> result in resultsDictionary)
            {
                // Arrange - mock dbService
                this._dbServiceMock.Setup(s => s.GetDatabase(dbName))
                    .Throws(result.Key);

                // Arrange - create target
                DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

                // Act
                IHttpActionResult actionResult = target.GetDatabase(dbName);

                // Assert
                Assert.IsInstanceOf(result.Value, actionResult);
            }
        }

        [Test]
        public void GetDatabasesNames_ReturnsDatabasesNames()
        {
            // Arrange
            string[] dbNames = { "someDatabase", "anotherDatabase" };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetDatabaseNames())
                .Returns(dbNames);

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetDatabasesNames();
            OkNegotiatedContentResult<IEnumerable<string>> contentResult =
                actionResult as OkNegotiatedContentResult<IEnumerable<string>>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(dbNames, contentResult.Content);
        }

        [Test]
        public void GetTable_DatabaseServiceReturnsNull_ReturnsNotFoundResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTable(dbName, tableName))
                .Returns((Table)null);

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetTable(dbName, tableName);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public void GetTable_DatabaseServiceReturnsTable_ReturnsTableDto()
        {
            // Arrange
            string dbName = "testDatabase";
            Table table = new Table
            {
                Name = "testTable", Attributes = { new Models.Attribute { Name = "firstAttribute", Type = "someType" } },
                Rows = { { 0, new Row { Id = 0, Value = { "firstValue" } } } }
            };

            TableDto tableDto = new TableDto { Name = table.Name, Attributes = table.Attributes, Rows = table.Rows.Values };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTable(dbName, table.Name))
                .Returns(table);

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetTable(dbName, table.Name);
            OkNegotiatedContentResult<TableDto> contentResult = actionResult as OkNegotiatedContentResult<TableDto>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);

            Assert.AreEqual(tableDto.Name, contentResult.Content.Name);
            Assert.AreEqual(tableDto.Attributes, contentResult.Content.Attributes);
            Assert.AreEqual(tableDto.Rows, contentResult.Content.Rows);
        }

        [Test]
        public void GetTable_DatabaseServiceThrowsExceptions_ReturnsValidResults()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            Dictionary<Exception, Type> resultsDictionary = new Dictionary<Exception, Type>
            {
                { new ArgumentException(), typeof(BadRequestResult) },
                { new DatabaseNotFoundException(), typeof(NotFoundResult) },
                { new DbServiceException(), typeof(InternalServerErrorResult) }
            };

            foreach (KeyValuePair<Exception, Type> result in resultsDictionary)
            {
                // Arrange - mock dbService
                this._dbServiceMock.Setup(s => s.GetTable(dbName, tableName))
                    .Throws(result.Key);

                // Arrange - create target
                DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

                // Act
                IHttpActionResult actionResult = target.GetTable(dbName, tableName);

                // Assert
                Assert.IsInstanceOf(result.Value, actionResult);
            }
        }

        [Test]
        public void GetTableProjection_DatabaseServiceReturnsNull_ReturnsNotFoundResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "firstAttribute" };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTableProjection(dbName, tableName, attributesNames))
                .Returns((Table)null);

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetTableProjection(dbName, tableName, attributesNames);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public void GetTableProjection_DatabaseServiceReturnsTable_ReturnsTableDto()
        {
            // Arrange
            string dbName = "testDatabase";
            string[] attributesNames = { "firstAttribute" };
            Table table = new Table
            {
                Name = "testTable",
                Attributes = { new Models.Attribute { Name = attributesNames.First(), Type = "someType" } },
                Rows = { { 0, new Row { Id = 0, Value = { "firstValue" } } } }
            };

            TableDto tableDto = new TableDto { Name = table.Name, Attributes = table.Attributes, Rows = table.Rows.Values };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTableProjection(dbName, table.Name, attributesNames))
                .Returns(table);

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetTableProjection(dbName, table.Name, attributesNames);
            OkNegotiatedContentResult<TableDto> contentResult = actionResult as OkNegotiatedContentResult<TableDto>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);

            Assert.AreEqual(tableDto.Name, contentResult.Content.Name);
            Assert.AreEqual(tableDto.Attributes, contentResult.Content.Attributes);
            Assert.AreEqual(tableDto.Rows, contentResult.Content.Rows);
        }

        [Test]
        public void GetTableProjection_DatabaseServiceThrowsExceptions_ReturnsValidResults()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "firstAttribute" };

            Dictionary<Exception, Type> resultsDictionary = new Dictionary<Exception, Type>
            {
                { new ArgumentException(), typeof(BadRequestResult) },
                { new AttributeNotFoundException(), typeof(BadRequestErrorMessageResult) },
                { new DatabaseNotFoundException(), typeof(NotFoundResult) },
                { new DbServiceException(), typeof(InternalServerErrorResult) }
            };

            foreach (KeyValuePair<Exception, Type> result in resultsDictionary)
            {
                // Arrange - mock dbService
                this._dbServiceMock.Setup(s => s.GetTableProjection(dbName, tableName, attributesNames))
                    .Throws(result.Key);

                // Arrange - create target
                DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

                // Act
                IHttpActionResult actionResult = target.GetTableProjection(dbName, tableName, attributesNames);

                // Assert
                Assert.IsInstanceOf(result.Value, actionResult);
            }
        }

        [SetUp]
        public void Init()
        {
            this._dbServiceMock = new Mock<IDatabaseService>();
        }

        [Test]
        public void UpdateRow_DatabaseServiceThrowsExceptions_ReturnsValidResults()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row { Id = 1 };

            Dictionary<Exception, Type> resultsDictionary = new Dictionary<Exception, Type>
            {
                { new ArgumentException(), typeof(BadRequestResult) },
                { new DatabaseNotFoundException(), typeof(NotFoundResult) },
                { new TableNotFoundException(), typeof(NotFoundResult) },
                { new RowNotFoundException(), typeof(NotFoundResult) },
                { new InvalidRowException(), typeof(BadRequestErrorMessageResult) },
                { new DbServiceException(), typeof(InternalServerErrorResult) }
            };

            foreach (KeyValuePair<Exception, Type> result in resultsDictionary)
            {
                // Arrange - mock dbService
                this._dbServiceMock.Setup(s => s.UpdateRow(dbName, tableName, row))
                    .Throws(result.Key);

                // Arrange - create target
                DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

                // Act
                IHttpActionResult actionResult = target.UpdateRow(dbName, tableName, row);

                // Assert
                Assert.IsInstanceOf(result.Value, actionResult);
            }
        }

        [Test]
        public void UpdateRow_DatabaseServiceUpdatesRow_ReturnsOkResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row { Id = 1 };

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.UpdateRow(dbName, tableName, row);

            // Assert
            Assert.IsInstanceOf<OkResult>(actionResult);

            this._dbServiceMock.Verify(s => s.UpdateRow(dbName, tableName, row), Times.Once);
        }
    }
}