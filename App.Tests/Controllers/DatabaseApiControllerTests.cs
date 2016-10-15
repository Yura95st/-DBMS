namespace App.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
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
        public void CreateDatabase_DatabaseSeriveCreatesDatabase_ReturnsCreatedAtRouteResult()
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
        public void CreateDatabase_DatabaseSeriveThrowsArgumentException_ReturnsBadRequestResult()
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
        public void CreateDatabase_DatabaseSeriveThrowsDatabaseAlreadyExistsException_ReturnsConflictResult()
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
        public void CreateDatabase_DatabaseSeriveThrowsDbServiceException_ReturnsInternalServerErrorResult()
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
        public void CreateDatabase_DatabaseSeriveThrowsInvalidNameFormatException_ReturnsBadRequestErrorMessageResult()
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
        public void DropDatabase_DatabaseSeriveDropsDatabase_ReturnsOkResult()
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
        public void DropDatabase_DatabaseSeriveThrowsArgumentException_ReturnsBadRequestResult()
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
        public void DropDatabase_DatabaseSeriveThrowsDatabaseNotFoundException_ReturnsNotFoundResult()
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
        public void DropDatabase_DatabaseSeriveThrowsDbServiceException_ReturnsInternalServerErrorResult()
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
        public void GetDatabase_DatabaseSeriveReturnsDatabase_ReturnsDatabaseDto()
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
        public void GetDatabase_DatabaseSeriveReturnsNull_ReturnsNotFoundResult()
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
        public void GetDatabase_DatabaseSeriveThrowsArgumentException_ReturnsBadRequestResult()
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
        public void GetDatabase_DatabaseSeriveThrowsDbServiceException_ReturnsInternalServerErrorResult()
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

        [SetUp]
        public void Init()
        {
            this._dbServiceMock = new Mock<IDatabaseService>();
        }
    }
}