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
        public void CreateDatabase_DatabaseServiceThrowsArgumentException_ReturnsBadRequestResult()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.CreateDatabase(dbName))
                .Throws<ArgumentException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.CreateDatabase(dbName);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public void CreateDatabase_DatabaseServiceThrowsDatabaseAlreadyExistsException_ReturnsConflictResult()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.CreateDatabase(dbName))
                .Throws<DatabaseAlreadyExistsException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.CreateDatabase(dbName);

            // Assert
            Assert.IsInstanceOf<ConflictResult>(actionResult);
        }

        [Test]
        public void CreateDatabase_DatabaseServiceThrowsDbServiceException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.CreateDatabase(dbName))
                .Throws<DbServiceException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.CreateDatabase(dbName);

            // Assert
            Assert.IsInstanceOf<InternalServerErrorResult>(actionResult);
        }

        [Test]
        public void CreateDatabase_DatabaseServiceThrowsInvalidNameFormatException_ReturnsBadRequestErrorMessageResult()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.CreateDatabase(dbName))
                .Throws<InvalidNameFormatException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.CreateDatabase(dbName);

            // Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actionResult);
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
        public void CreateTable_DatabaseServiceThrowsArgumentException_ReturnsBadRequestResult()
        {
            // Arrange
            string dbName = "testDatabase";
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.CreateTable(dbName, tableScheme))
                .Throws<ArgumentException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.CreateTable(dbName, tableScheme);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public void CreateTable_DatabaseServiceThrowsDatabaseNotFoundException_ReturnsBadRequestErrorMessageResult()
        {
            // Arrange
            string dbName = "testDatabase";
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.CreateTable(dbName, tableScheme))
                .Throws<DatabaseNotFoundException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.CreateTable(dbName, tableScheme);

            // Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actionResult);
        }

        [Test]
        public void CreateTable_DatabaseServiceThrowsDbServiceException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            string dbName = "testDatabase";
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.CreateTable(dbName, tableScheme))
                .Throws<DbServiceException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.CreateTable(dbName, tableScheme);

            // Assert
            Assert.IsInstanceOf<InternalServerErrorResult>(actionResult);
        }

        [Test]
        public void CreateTable_DatabaseServiceThrowsInvalidTableSchemeException_ReturnsBadRequestErrorMessageResult()
        {
            // Arrange
            string dbName = "testDatabase";
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.CreateTable(dbName, tableScheme))
                .Throws<InvalidTableSchemeException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.CreateTable(dbName, tableScheme);

            // Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actionResult);
        }

        [Test]
        public void CreateTable_DatabaseServiceThrowsTableAlreadyExistsException_ReturnsConflictResult()
        {
            // Arrange
            string dbName = "testDatabase";
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.CreateTable(dbName, tableScheme))
                .Throws<TableAlreadyExistsException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.CreateTable(dbName, tableScheme);

            // Assert
            Assert.IsInstanceOf<ConflictResult>(actionResult);
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
        public void DropDatabase_DatabaseServiceThrowsArgumentException_ReturnsBadRequestResult()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.DropDatabase(dbName))
                .Throws<ArgumentException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.DropDatabase(dbName);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public void DropDatabase_DatabaseServiceThrowsDatabaseNotFoundException_ReturnsNotFoundResult()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.DropDatabase(dbName))
                .Throws<DatabaseNotFoundException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.DropDatabase(dbName);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public void DropDatabase_DatabaseServiceThrowsDbServiceException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.DropDatabase(dbName))
                .Throws<DbServiceException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.DropDatabase(dbName);

            // Assert
            Assert.IsInstanceOf<InternalServerErrorResult>(actionResult);
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
        public void DropTable_DatabaseServiceThrowsArgumentException_ReturnsBadRequestResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.DropTable(dbName, tableName))
                .Throws<ArgumentException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.DropTable(dbName, tableName);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public void DropTable_DatabaseServiceThrowsDatabaseNotFoundException_ReturnsNotFoundResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.DropTable(dbName, tableName))
                .Throws<DatabaseNotFoundException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.DropTable(dbName, tableName);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public void DropTable_DatabaseServiceThrowsDbServiceException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.DropTable(dbName, tableName))
                .Throws<DbServiceException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.DropTable(dbName, tableName);

            // Assert
            Assert.IsInstanceOf<InternalServerErrorResult>(actionResult);
        }

        [Test]
        public void DropTable_DatabaseServiceThrowsTableNotFoundException_ReturnsNotFoundResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.DropTable(dbName, tableName))
                .Throws<TableNotFoundException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.DropTable(dbName, tableName);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
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
        public void GetDatabase_DatabaseServiceThrowsArgumentException_ReturnsBadRequestResult()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetDatabase(dbName))
                .Throws<ArgumentException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetDatabase(dbName);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public void GetDatabase_DatabaseServiceThrowsDbServiceException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetDatabase(dbName))
                .Throws<DbServiceException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetDatabase(dbName);

            // Assert
            Assert.IsInstanceOf<InternalServerErrorResult>(actionResult);
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
        public void GetTable_DatabaseServiceThrowsArgumentException_ReturnsBadRequestResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTable(dbName, tableName))
                .Throws<ArgumentException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetTable(dbName, tableName);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public void GetTable_DatabaseServiceThrowsDatabaseNotFoundException_ReturnsNotFoundResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTable(dbName, tableName))
                .Throws<DatabaseNotFoundException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetTable(dbName, tableName);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public void GetTable_DatabaseServiceThrowsDbServiceException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTable(dbName, tableName))
                .Throws<DbServiceException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetTable(dbName, tableName);

            // Assert
            Assert.IsInstanceOf<InternalServerErrorResult>(actionResult);
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
        public void GetTableProjection_DatabaseServiceThrowsArgumentException_ReturnsBadRequestResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "firstAttribute" };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTableProjection(dbName, tableName, attributesNames))
                .Throws<ArgumentException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetTableProjection(dbName, tableName, attributesNames);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public void GetTableProjection_DatabaseServiceThrowsAttributeNotFoundException_ReturnsBadRequestErrorMessageResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "firstAttribute" };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTableProjection(dbName, tableName, attributesNames))
                .Throws<AttributeNotFoundException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetTableProjection(dbName, tableName, attributesNames);

            // Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actionResult);
        }

        [Test]
        public void GetTableProjection_DatabaseServiceThrowsDatabaseNotFoundException_ReturnsNotFoundResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "firstAttribute" };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTableProjection(dbName, tableName, attributesNames))
                .Throws<DatabaseNotFoundException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetTableProjection(dbName, tableName, attributesNames);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public void GetTableProjection_DatabaseServiceThrowsDbServiceException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "firstAttribute" };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTableProjection(dbName, tableName, attributesNames))
                .Throws<DbServiceException>();

            // Arrange - create target
            DatabaseApiController target = new DatabaseApiController(this._dbServiceMock.Object);

            // Act
            IHttpActionResult actionResult = target.GetTableProjection(dbName, tableName, attributesNames);

            // Assert
            Assert.IsInstanceOf<InternalServerErrorResult>(actionResult);
        }

        [SetUp]
        public void Init()
        {
            this._dbServiceMock = new Mock<IDatabaseService>();
        }
    }
}