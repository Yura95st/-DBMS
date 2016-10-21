namespace App.Tests.Services.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using App.Dal.Repositories.Abstract;
    using App.Exceptions;
    using App.Models;
    using App.Services.Abstract;
    using App.Services.Concrete;
    using App.Validations.Abstract;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class DatabaseServiceTests
    {
        private Mock<IDbRepository> _dbRepositoryMock;

        private Mock<IDatabaseValidation> _dbValidationMock;

        private Database _testDb;

        private Table _testTable;

        [Test]
        public void AddRow_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row();

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.AddRow(null, tableName, row));
            Assert.Throws<ArgumentNullException>(() => target.AddRow(dbName, null, row));
            Assert.Throws<ArgumentNullException>(() => target.AddRow(dbName, tableName, null));

            Assert.Throws<ArgumentException>(() => target.AddRow("", tableName, row));
            Assert.Throws<ArgumentException>(() => target.AddRow(dbName, "", row));
        }

        [Test]
        public void AddRow_DbRepositoryThrowsDbRepositoryException_ThrowsDbServiceException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row();

            DbRepositoryException innerException = new DbRepositoryException();

            // Arrange - mock dbRepository
            this._dbRepositoryMock.Setup(r => r.GetByName(dbName))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            DbServiceException ex = Assert.Throws<DbServiceException>(() => target.AddRow(dbName, tableName, row));

            Assert.AreSame(innerException, ex.InnerException);
        }

        [Test]
        public void AddRow_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row();

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.AddRow(dbName, tableName, row));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void AddRow_NonexistentTableName_ThrowsTableNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = "testTable";
            Row row = new Row();

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<TableNotFoundException>(() => target.AddRow(dbName, tableName, row));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void AddRow_RowIsInvalid_ThrowsIvalidRowException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            Table table = this._testTable;
            Row row = new Row();

            // Arrange - mock dbValidation
            this._dbValidationMock.Setup(v => v.DoesRowFitTable(table, row))
                .Returns(false);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidRowException>(() => target.AddRow(dbName, table.Name, row));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void AddRow_RowIsValid_AddsNewRowAndUpdatesTableNextRowIdCounter()
        {
            // Arrange
            Database database = new Database
            {
                Name = this._testDb.Name,
                Tables =
                    this._testDb.Tables.ToDictionary(t => t.Key,
                        t =>
                            new Table
                            {
                                Attributes = t.Value.Attributes, Name = t.Value.Name, NextRowId = t.Value.NextRowId,
                                Rows = new Dictionary<int, Row>(t.Value.Rows)
                            })
            };
            Table table = database.Tables.Values.First();

            Row row = new Row { Value = new List<string>() };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            target.AddRow(database.Name, table.Name, row);

            row.Id = table.NextRowId;
            table.Rows.Add(row.Id, row);

            table.NextRowId++;

            // Assert
            this._dbValidationMock.Verify(v => v.DoesRowFitTable(table, row), Times.Once);
            this._dbRepositoryMock.Verify(r => r.Update(database), Times.Once);
        }

        [Test]
        public void CreateDatabase_DatabaseNameIsInvalid_ThrowsInvalidDatabaseNameException()
        {
            // Arrange
            Database database = new Database { Name = "testDatabase" };

            // Arrange - mock dbValidation
            this._dbValidationMock.Setup(v => v.IsValidDatabaseName(database.Name))
                .Returns(false);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidDatabaseNameException>(() => target.CreateDatabase(database.Name));

            this._dbRepositoryMock.Verify(r => r.Create(database), Times.Never);
        }

        [Test]
        public void CreateDatabase_DatabaseNameIsNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.CreateDatabase(null));
            Assert.Throws<ArgumentException>(() => target.CreateDatabase(""));
        }

        [Test]
        public void CreateDatabase_DatabaseNameIsValid_CreatesNewDatabase()
        {
            // Arrange
            Database database = new Database { Name = "testDatabase" };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            target.CreateDatabase(database.Name);

            // Assert
            this._dbValidationMock.Verify(v => v.IsValidDatabaseName(database.Name), Times.Once);

            this._dbRepositoryMock.Verify(r => r.GetAllNames(), Times.Once);
            this._dbRepositoryMock.Verify(r => r.Create(database), Times.Once);
        }

        [Test]
        public void CreateDatabase_DatabaseWithSuchNameAlreadyExists_ThrowsDatabaseAlreadyExistsException()
        {
            // Arrange
            Database database = new Database { Name = this._testDb.Name };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseAlreadyExistsException>(() => target.CreateDatabase(database.Name));

            this._dbRepositoryMock.Verify(r => r.GetAllNames(), Times.Once);
            this._dbRepositoryMock.Verify(r => r.Create(database), Times.Never);
        }

        [Test]
        public void CreateDatabase_DbRepositoryThrowsDbRepositoryException_ThrowsDbServiceException()
        {
            // Arrange
            Database database = new Database { Name = "testDatabase" };
            DbRepositoryException innerException = new DbRepositoryException();

            // Arrange - mock dbRepository
            this._dbRepositoryMock.Setup(r => r.Create(database))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            DbServiceException ex = Assert.Throws<DbServiceException>(() => target.CreateDatabase(database.Name));

            Assert.AreSame(innerException, ex.InnerException);
        }

        [Test]
        public void CreateTable_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.CreateTable(null, tableScheme));
            Assert.Throws<ArgumentNullException>(() => target.CreateTable(dbName, null));

            Assert.Throws<ArgumentException>(() => target.CreateTable("", tableScheme));
        }

        [Test]
        public void CreateTable_ArgumentsAreValid_CreatesNewTable()
        {
            // Arrange
            Database database = new Database
                { Name = this._testDb.Name, Tables = new Dictionary<string, Table>(this._testDb.Tables) };
            Table table = new Table
            {
                Name = "testTable",
                Attributes =
                    new List<Models.Attribute> { new Models.Attribute { Name = "firstAttribute", Type = "someType" } }
            };

            TableScheme tableScheme = new TableScheme(table.Name, table.Attributes);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            target.CreateTable(database.Name, tableScheme);

            database.Tables.Add(table.Name, table);

            // Assert
            this._dbValidationMock.Verify(v => v.CheckTableScheme(tableScheme), Times.Once);
            this._dbRepositoryMock.Verify(r => r.Update(database), Times.Once);
        }

        [Test]
        public void CreateTable_DbRepositoryThrowsDbRepositoryException_ThrowsDbServiceException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            DbRepositoryException innerException = new DbRepositoryException();

            // Arrange - mock dbRepository
            this._dbRepositoryMock.Setup(r => r.Update(It.IsAny<Database>()))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            DbServiceException ex = Assert.Throws<DbServiceException>(() => target.CreateTable(dbName, tableScheme));

            Assert.AreSame(innerException, ex.InnerException);
        }

        [Test]
        public void CreateTable_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            // Arrange - mock dbRepository
            this._dbRepositoryMock.Setup(r => r.GetByName(dbName))
                .Returns((Database)null);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.CreateTable(dbName, tableScheme));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void CreateTable_TableSchemeIsInvalid_ThrowsInvalidTableSchemeException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            Exception innerException = new Exception();

            // Arrange - mock dbValidation
            this._dbValidationMock.Setup(v => v.CheckTableScheme(tableScheme))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            InvalidTableSchemeException exeption =
                Assert.Throws<InvalidTableSchemeException>(() => target.CreateTable(dbName, tableScheme));
            Assert.AreSame(exeption.InnerException, innerException);

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void CreateTable_TableWithSuchNameAlreadyExists_ThrowsTableAlreadyExistsException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            TableScheme tableScheme = new TableScheme(this._testTable.Name, new List<Models.Attribute>());

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<TableAlreadyExistsException>(() => target.CreateTable(dbName, tableScheme));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void DeleteRow_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            int rowId = 1;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.DeleteRow(null, tableName, rowId));
            Assert.Throws<ArgumentNullException>(() => target.DeleteRow(dbName, null, rowId));

            Assert.Throws<ArgumentException>(() => target.DeleteRow("", tableName, rowId));
            Assert.Throws<ArgumentException>(() => target.DeleteRow(dbName, "", rowId));
            Assert.Throws<ArgumentException>(() => target.DeleteRow(dbName, "", -1));
        }

        [Test]
        public void DeleteRow_ArgumentsAreValid_DeletesRow()
        {
            // Arrange
            Database database = new Database
            {
                Name = this._testDb.Name,
                Tables =
                    this._testDb.Tables.ToDictionary(t => t.Key,
                        t =>
                            new Table
                            {
                                Attributes = t.Value.Attributes, Name = t.Value.Name, NextRowId = t.Value.NextRowId,
                                Rows = new Dictionary<int, Row>(t.Value.Rows)
                            })
            };

            string tableName = this._testTable.Name;
            int rowId = this._testTable.Rows.First()
                .Key;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            target.DeleteRow(database.Name, tableName, rowId);

            database.Tables[tableName].Rows.Remove(rowId);

            // Assert
            this._dbRepositoryMock.Verify(r => r.Update(database), Times.Once);
        }

        [Test]
        public void DeleteRow_DbRepositoryThrowsDbRepositoryException_ThrowsDbServiceException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            int rowId = 1;

            DbRepositoryException innerException = new DbRepositoryException();

            // Arrange - mock dbRepository
            this._dbRepositoryMock.Setup(r => r.GetByName(dbName))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            DbServiceException ex = Assert.Throws<DbServiceException>(() => target.DeleteRow(dbName, tableName, rowId));

            Assert.AreSame(innerException, ex.InnerException);
        }

        [Test]
        public void DeleteRow_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            int rowId = 1;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.DeleteRow(dbName, tableName, rowId));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void DeleteRow_NonexistentRowId_ThrowsRowNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = this._testTable.Name;
            int rowId = this._testTable.NextRowId;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<RowNotFoundException>(() => target.DeleteRow(dbName, tableName, rowId));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void DeleteRow_NonexistentTableName_ThrowsTableNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = "testTable";
            int rowId = 1;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<TableNotFoundException>(() => target.DeleteRow(dbName, tableName, rowId));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void DropDatabase_DatabaseNameIsNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.DropDatabase(null));
            Assert.Throws<ArgumentException>(() => target.DropDatabase(""));
        }

        [Test]
        public void DropDatabase_DatabaseNameIsValid_DropsDatabase()
        {
            // Arrange
            string dbName = this._testDb.Name;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            target.DropDatabase(dbName);

            // Assert
            this._dbRepositoryMock.Verify(r => r.GetAllNames(), Times.Once);
            this._dbRepositoryMock.Verify(r => r.Delete(dbName), Times.Once);
        }

        [Test]
        public void DropDatabase_DbRepositoryThrowsDbRepositoryException_ThrowsDbServiceException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            DbRepositoryException innerException = new DbRepositoryException();

            // Arrange - mock dbRepository
            this._dbRepositoryMock.Setup(r => r.Delete(dbName))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            DbServiceException ex = Assert.Throws<DbServiceException>(() => target.DropDatabase(dbName));

            Assert.AreSame(innerException, ex.InnerException);
        }

        [Test]
        public void DropDatabase_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.DropDatabase(dbName));

            this._dbRepositoryMock.Verify(r => r.GetAllNames(), Times.Once);
            this._dbRepositoryMock.Verify(r => r.Delete(dbName), Times.Never);
        }

        [Test]
        public void DropTable_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "tableName";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.DropTable(null, tableName));
            Assert.Throws<ArgumentNullException>(() => target.DropTable(dbName, null));

            Assert.Throws<ArgumentException>(() => target.DropTable("", tableName));
            Assert.Throws<ArgumentException>(() => target.DropTable(dbName, ""));
        }

        [Test]
        public void DropTable_ArgumentsAreValid_DropsTable()
        {
            // Arrange
            Database database = new Database
                { Name = this._testDb.Name, Tables = new Dictionary<string, Table>(this._testDb.Tables) };
            string tableName = this._testTable.Name;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            target.DropTable(database.Name, tableName);

            database.Tables.Remove(tableName);

            // Assert
            this._dbRepositoryMock.Verify(r => r.GetByName(database.Name), Times.Once);
            this._dbRepositoryMock.Verify(r => r.Update(database), Times.Once);
        }

        [Test]
        public void DropTable_DbRepositoryThrowsDbRepositoryException_ThrowsDbServiceException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = this._testTable.Name;

            DbRepositoryException innerException = new DbRepositoryException();

            // Arrange - mock dbRepository
            this._dbRepositoryMock.Setup(r => r.Update(It.IsAny<Database>()))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            DbServiceException ex = Assert.Throws<DbServiceException>(() => target.DropTable(dbName, tableName));

            Assert.AreSame(innerException, ex.InnerException);
        }

        [Test]
        public void DropTable_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.DropTable(dbName, tableName));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void DropTable_NonexistentTableName_ThrowsTableNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = "testTable";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<TableNotFoundException>(() => target.DropTable(dbName, tableName));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void GetDatabase_DatabaseNameIsNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.GetDatabase(null));
            Assert.Throws<ArgumentException>(() => target.GetDatabase(""));
        }

        [Test]
        public void GetDatabase_DatabaseNameIsValid_ReturnsDatabase()
        {
            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            Database db = target.GetDatabase(this._testDb.Name);

            // Assert
            Assert.AreEqual(db, this._testDb);

            this._dbRepositoryMock.Verify(r => r.GetByName(this._testDb.Name), Times.Once);
        }

        [Test]
        public void GetDatabase_DbRepositoryThrowsDbRepositoryException_ThrowsDbServiceException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            DbRepositoryException innerException = new DbRepositoryException();

            // Arrange - mock dbRepository
            this._dbRepositoryMock.Setup(r => r.GetByName(dbName))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            DbServiceException ex = Assert.Throws<DbServiceException>(() => target.GetDatabase(dbName));

            Assert.AreSame(innerException, ex.InnerException);
        }

        [Test]
        public void GetDatabase_NonexistentDatabaseName_ReturnsNull()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            Database db = target.GetDatabase(dbName);

            // Assert
            Assert.IsNull(db);

            this._dbRepositoryMock.Verify(r => r.GetByName(dbName), Times.Once);
        }

        [Test]
        public void GetDatabaseNames_DbRepositoryThrowsDbRepositoryException_ThrowsDbServiceException()
        {
            // Arrange
            DbRepositoryException innerException = new DbRepositoryException();

            // Arrange - mock dbRepository
            this._dbRepositoryMock.Setup(r => r.GetAllNames())
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            DbServiceException ex = Assert.Throws<DbServiceException>(() => target.GetDatabaseNames());

            Assert.AreSame(innerException, ex.InnerException);
        }

        [Test]
        public void GetDatabaseNames_ReturnsListOfDatabaseNames()
        {
            // Arrange
            string[] testDbNames = { this._testDb.Name };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            IEnumerable<string> dbNames = target.GetDatabaseNames();

            // Assert
            Assert.IsTrue(dbNames.SequenceEqual(testDbNames));

            this._dbRepositoryMock.Verify(r => r.GetAllNames(), Times.Once);
        }

        [Test]
        public void GetTable_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "tableName";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.GetTable(null, tableName));
            Assert.Throws<ArgumentNullException>(() => target.GetTable(dbName, null));

            Assert.Throws<ArgumentException>(() => target.GetTable("", tableName));
            Assert.Throws<ArgumentException>(() => target.GetTable(dbName, ""));
        }

        [Test]
        public void GetTable_ArgumetnsAreValid_ReturnsTable()
        {
            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            Table table = target.GetTable(this._testDb.Name, this._testTable.Name);

            // Assert
            Assert.AreEqual(table, this._testTable);

            this._dbRepositoryMock.Verify(r => r.GetByName(this._testDb.Name), Times.Once);
        }

        [Test]
        public void GetTable_DbRepositoryThrowsDbRepositoryException_ThrowsDbServiceException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            DbRepositoryException innerException = new DbRepositoryException();

            // Arrange - mock dbRepository
            this._dbRepositoryMock.Setup(r => r.GetByName(dbName))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            DbServiceException ex = Assert.Throws<DbServiceException>(() => target.GetTable(dbName, tableName));

            Assert.AreSame(innerException, ex.InnerException);
        }

        [Test]
        public void GetTable_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.GetTable(dbName, tableName));
        }

        [Test]
        public void GetTable_NonexistentTableName_ReturnsNull()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = "testTable";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            Table table = target.GetTable(dbName, tableName);

            // Assert
            Assert.IsNull(table);
        }

        [Test]
        public void GetTableProjection_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "someAttribute" };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.GetTableProjection(null, tableName, attributesNames));
            Assert.Throws<ArgumentNullException>(() => target.GetTableProjection(dbName, null, attributesNames));
            Assert.Throws<ArgumentNullException>(() => target.GetTableProjection(dbName, tableName, null));

            Assert.Throws<ArgumentException>(() => target.GetTableProjection("", tableName, attributesNames));
            Assert.Throws<ArgumentException>(() => target.GetTableProjection(dbName, "", attributesNames));
            Assert.Throws<ArgumentException>(() => target.GetTableProjection(dbName, tableName, Enumerable.Empty<string>()));
        }

        [Test]
        public void GetTableProjection_ArgumetnsAreValid_ReturnsTable()
        {
            // Arrange
            List<int> attributesIndexes = new List<int> { this._testTable.Attributes.Count - 1 };

            string dbName = this._testDb.Name;
            Table testTable = new Table
            {
                Attributes = this._testTable.Attributes.ToList(), Name = this._testTable.Name,
                NextRowId = this._testTable.NextRowId,
                Rows =
                    this._testTable.Rows.ToDictionary(kvp => kvp.Key,
                        kvp => new Row { Id = kvp.Value.Id, Value = kvp.Value.Value.ToList() })
            };

            var attributesNames = testTable.Attributes.Where((item, index) => attributesIndexes.Contains(index))
                .Select(item => item.Name)
                .ToList();

            for (int i = testTable.Attributes.Count - 1; i >= 0; i--)
            {
                if (!attributesIndexes.Contains(i))
                {
                    testTable.Attributes.RemoveAt(i);

                    foreach (Row row in testTable.Rows.Values)
                    {
                        row.Value.RemoveAt(i);
                    }
                }
            }

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            Table table = target.GetTableProjection(dbName, testTable.Name, attributesNames);

            // Assert
            Assert.AreEqual(table, testTable);
        }

        [Test]
        public void GetTableProjection_DbRepositoryThrowsDbRepositoryException_ThrowsDbServiceException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "testAttribute" };

            DbRepositoryException innerException = new DbRepositoryException();

            // Arrange - mock dbRepository
            this._dbRepositoryMock.Setup(r => r.GetByName(dbName))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            DbServiceException ex =
                Assert.Throws<DbServiceException>(() => target.GetTableProjection(dbName, tableName, attributesNames));

            Assert.AreSame(innerException, ex.InnerException);
        }

        [Test]
        public void GetTableProjection_NonexistentAttributeName_ThrowsAttributeNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = this._testTable.Name;
            string[] attributesNames =
            {
                this._testTable.Attributes.First()
                    .Name,
                "testAttribute"
            };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<AttributeNotFoundException>(() => target.GetTableProjection(dbName, tableName, attributesNames));
        }

        [Test]
        public void GetTableProjection_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "testAttribute" };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.GetTableProjection(dbName, tableName, attributesNames));
        }

        [Test]
        public void GetTableProjection_NonexistentTableName_ReturnsNull()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = "testTable";
            string[] attributesNames = { "testAttribute" };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            Table table = target.GetTableProjection(dbName, tableName, attributesNames);

            // Assert
            Assert.IsNull(table);
        }

        [SetUp]
        public void Init()
        {
            this.InitDatabase();

            this.InitDbRepositoryMock();

            this.InitDbValidationMock();
        }

        [Test]
        public void UpdateRow_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row();

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.UpdateRow(null, tableName, row));
            Assert.Throws<ArgumentNullException>(() => target.UpdateRow(dbName, null, row));
            Assert.Throws<ArgumentNullException>(() => target.UpdateRow(dbName, tableName, null));

            Assert.Throws<ArgumentException>(() => target.UpdateRow("", tableName, row));
            Assert.Throws<ArgumentException>(() => target.UpdateRow(dbName, "", row));
        }

        [Test]
        public void UpdateRow_DbRepositoryThrowsDbRepositoryException_ThrowsDbServiceException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row();

            DbRepositoryException innerException = new DbRepositoryException();

            // Arrange - mock dbRepository
            this._dbRepositoryMock.Setup(r => r.GetByName(dbName))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            DbServiceException ex = Assert.Throws<DbServiceException>(() => target.UpdateRow(dbName, tableName, row));

            Assert.AreSame(innerException, ex.InnerException);
        }

        [Test]
        public void UpdateRow_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row();

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.UpdateRow(dbName, tableName, row));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void UpdateRow_NonexistentRowId_ThrowsRowNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            Table table = this._testTable;
            Row row = new Row { Id = table.NextRowId };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<RowNotFoundException>(() => target.UpdateRow(dbName, table.Name, row));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void UpdateRow_NonexistentTableName_ThrowsTableNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = "testTable";
            Row row = new Row();

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<TableNotFoundException>(() => target.UpdateRow(dbName, tableName, row));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void UpdateRow_RowIsInvalid_ThrowsIvalidRowException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            Table table = this._testTable;
            Row row = new Row {Id = table.Rows.Keys.First()};

            // Arrange - mock dbValidation
            this._dbValidationMock.Setup(v => v.DoesRowFitTable(table, row))
                .Returns(false);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidRowException>(() => target.UpdateRow(dbName, table.Name, row));

            this._dbRepositoryMock.Verify(r => r.Update(It.Is<Database>(db => db.Name == dbName)), Times.Never);
        }

        [Test]
        public void UpdateRow_RowIsValid_UpdatesRow()
        {
            // Arrange
            Database database = new Database
            {
                Name = this._testDb.Name,
                Tables =
                    this._testDb.Tables.ToDictionary(t => t.Key,
                        t =>
                            new Table
                            {
                                Attributes = t.Value.Attributes, Name = t.Value.Name, NextRowId = t.Value.NextRowId,
                                Rows = new Dictionary<int, Row>(t.Value.Rows)
                            })
            };
            Table table = database.Tables.Values.First();
            
            Row row = new Row
            {
                Id = table.Rows.Keys.First()
            };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbRepositoryMock.Object, this._dbValidationMock.Object);

            // Act
            target.UpdateRow(database.Name, table.Name, row);

            table.Rows[row.Id] = row;

            // Assert
            this._dbValidationMock.Verify(v => v.DoesRowFitTable(table, row), Times.Once);
            this._dbRepositoryMock.Verify(r => r.Update(database), Times.Once);
        }

        private void InitDatabase()
        {
            Row row = new Row { Id = 1, Value = { "someValue", "anotherValue", "thirdValue" } };

            this._testTable = new Table
            {
                Name = "someTable", NextRowId = row.Id + 1,
                Attributes =
                {
                    new Models.Attribute { Name = "firstAttribute", Type = "someType" },
                    new Models.Attribute { Name = "secondAttribute", Type = "anotherType" },
                    new Models.Attribute { Name = "thirdAttribute", Type = "otherType" },
                },
                Rows = { { row.Id, row } }
            };

            this._testDb = new Database
            {
                Name = "someDatabase", Tables = new Dictionary<string, Table> { { this._testTable.Name, this._testTable } }
            };
        }

        private void InitDbRepositoryMock()
        {
            this._dbRepositoryMock = new Mock<IDbRepository>();

            this._dbRepositoryMock.Setup(r => r.GetAllNames())
                .Returns(new List<string> { this._testDb.Name });

            this._dbRepositoryMock.Setup(r => r.GetByName(this._testDb.Name))
                .Returns(this._testDb);
        }

        private void InitDbValidationMock()
        {
            this._dbValidationMock = new Mock<IDatabaseValidation>();

            this._dbValidationMock.Setup(v => v.IsValidDatabaseName(It.IsAny<string>()))
                .Returns(true);

            this._dbValidationMock.Setup(v => v.DoesRowFitTable(It.IsAny<Table>(), It.IsAny<Row>()))
                .Returns(true);
        }
    }
}