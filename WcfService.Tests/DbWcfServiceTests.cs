namespace WcfService.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.DTOs;
    using Domain.Models;
    using Domain.Services.Abstract;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class DbWcfServiceTests
    {
        private Mock<IDatabaseService> _dbServiceMock;

        [Test]
        public void GetDatabase_DatabaseServiceReturnsDatabase_ReturnsDatabaseDto()
        {
            // Arrange
            Database database = new Database
                { Name = "someDatabase", Tables = { { "firstTable", new Table() }, { "secondTable", new Table() } } };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetDatabase(database.Name))
                .Returns(database);

            // Arrange - create target
            DbWcfService target = new DbWcfService(this._dbServiceMock.Object);

            // Act
            DatabaseDto dbDto = target.GetDatabase(database.Name);

            // Assert
            Assert.IsNotNull(dbDto);

            Assert.AreEqual(dbDto.Name, database.Name);
            Assert.AreEqual(dbDto.TableNames, database.Tables.Keys);
        }

        [Test]
        public void GetDatabase_DatabaseServiceReturnsNull_ReturnsNull()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetDatabase(dbName))
                .Returns((Database)null);

            // Arrange - create target
            DbWcfService target = new DbWcfService(this._dbServiceMock.Object);

            // Act
            DatabaseDto db = target.GetDatabase(dbName);

            // Assert
            Assert.IsNull(db);
        }

        [Test]
        public void GetDatabase_DatabaseServiceThrowsExceptions_ForwardsException()
        {
            // Arrange
            string dbName = "testDatabase";

            Exception exception = new Exception();

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetDatabase(dbName))
                .Throws(exception);

            // Arrange - create target
            DbWcfService target = new DbWcfService(this._dbServiceMock.Object);

            // Act
            Exception ex = Assert.Throws<Exception>(() => target.GetDatabase(dbName));

            Assert.AreSame(exception, ex);
        }

        [Test]
        public void GetDatabasesNames_ReturnsDatabasesNames()
        {
            // Arrange
            string[] testDbNames = { "someDatabase", "anotherDatabase" };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetDatabaseNames())
                .Returns(testDbNames);

            // Arrange - create target
            DbWcfService target = new DbWcfService(this._dbServiceMock.Object);

            // Act
            IEnumerable<string> dbNames = target.GetDatabasesNames();

            // Assert
            Assert.AreEqual(testDbNames, dbNames);
        }

        [Test]
        public void GetTable_DatabaseServiceReturnsNull_ReturnsNull()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTable(dbName, tableName))
                .Returns((Table)null);

            // Arrange - create target
            DbWcfService target = new DbWcfService(this._dbServiceMock.Object);

            // Act
            TableDto table = target.GetTable(dbName, tableName);

            // Assert
            Assert.IsNull(table);
        }

        [Test]
        public void GetTable_DatabaseServiceReturnsTable_ReturnsTableDto()
        {
            // Arrange
            string dbName = "testDatabase";
            Table testTable = new Table
            {
                Name = "testTable",
                Attributes = { new Domain.Models.Attribute { Name = "firstAttribute", Type = "someType" } },
                Rows = { { 0, new Row { Id = 0, Value = { "firstValue" } } } }
            };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTable(dbName, testTable.Name))
                .Returns(testTable);

            // Arrange - create target
            DbWcfService target = new DbWcfService(this._dbServiceMock.Object);

            // Act
            TableDto table = target.GetTable(dbName, testTable.Name);

            // Assert
            Assert.IsNotNull(table);
            Assert.AreEqual(testTable.Name, table.Name);
            Assert.AreEqual(testTable.Attributes, table.Attributes);
            Assert.AreEqual(testTable.Rows.Values, table.Rows);
        }

        [Test]
        public void GetTable_DatabaseServiceThrowsExceptions_ForwardsException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            Exception exception = new Exception();

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTable(dbName, tableName))
                .Throws(exception);

            // Arrange - create target
            DbWcfService target = new DbWcfService(this._dbServiceMock.Object);

            // Act
            Exception ex = Assert.Throws<Exception>(() => target.GetTable(dbName, tableName));

            Assert.AreSame(exception, ex);
        }

        [Test]
        public void GetTableProjection_DatabaseServiceReturnsNull_ReturnsNull()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "firstAttribute" };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTableProjection(dbName, tableName, attributesNames))
                .Returns((Table)null);

            // Arrange - create target
            DbWcfService target = new DbWcfService(this._dbServiceMock.Object);

            // Act
            TableDto table = target.GetTableProjection(dbName, tableName, attributesNames);

            // Assert
            Assert.IsNull(table);
        }

        [Test]
        public void GetTableProjection_DatabaseServiceReturnsTable_ReturnsTableDto()
        {
            // Arrange
            string dbName = "testDatabase";
            string[] attributesNames = { "firstAttribute" };
            Table testTable = new Table
            {
                Name = "testTable",
                Attributes = { new Domain.Models.Attribute { Name = attributesNames.First(), Type = "someType" } },
                Rows = { { 0, new Row { Id = 0, Value = { "firstValue" } } } }
            };

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTableProjection(dbName, testTable.Name, attributesNames))
                .Returns(testTable);

            // Arrange - create target
            DbWcfService target = new DbWcfService(this._dbServiceMock.Object);

            // Act
            TableDto table = target.GetTableProjection(dbName, testTable.Name, attributesNames);

            // Assert
            Assert.IsNotNull(testTable);
            Assert.AreEqual(testTable.Name, table.Name);
            Assert.AreEqual(testTable.Attributes, table.Attributes);
            Assert.AreEqual(testTable.Rows.Values, table.Rows);
        }

        [Test]
        public void GetTableProjection_DatabaseServiceThrowsExceptions_ForwardsException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "firstAttribute" };

            Exception exception = new Exception();

            // Arrange - mock dbService
            this._dbServiceMock.Setup(s => s.GetTableProjection(dbName, tableName, attributesNames))
                .Throws(exception);

            // Arrange - create target
            DbWcfService target = new DbWcfService(this._dbServiceMock.Object);

            // Act
            Exception ex = Assert.Throws<Exception>(() => target.GetTableProjection(dbName, tableName, attributesNames));

            Assert.AreSame(exception, ex);
        }

        [SetUp]
        public void Init()
        {
            this._dbServiceMock = new Mock<IDatabaseService>();
        }
    }
}