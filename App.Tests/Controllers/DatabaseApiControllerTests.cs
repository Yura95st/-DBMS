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
        public void GetDatabase_DatabaseSeriveThrowsArgumentException_BadRequestResult()
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