namespace App.Tests.Validations.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using App.Exceptions;
    using App.Models;
    using App.Validations.Concrete;

    using NUnit.Framework;

    [TestFixture]
    public class DatabaseValidationTests
    {
        private DatabaseValidationSettings _dbValidationSettings;

        [Test]
        public void CheckAttribute_AttributeHasInvalidName_ThrowsInvalidAttributeException()
        {
            // Arrange
            string[] attributeNames = { null, "", " ", new string(Path.GetInvalidFileNameChars()) };

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            foreach (string attributeName in attributeNames)
            {
                Models.Attribute attribute = new Models.Attribute { Name = attributeName };

                Assert.Throws<InvalidAttributeException>(() => target.CheckAttribute(attribute));
            }
        }

        [Test]
        public void CheckAttribute_AttributeHasUnknownType_ThrowsInvalidAttributeException()
        {
            // Arrange
            Models.Attribute attribute = new Models.Attribute { Name = "testAttribute", Type = "testType" };

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.Throws<InvalidAttributeException>(() => target.CheckAttribute(attribute));
        }

        [Test]
        public void CheckAttribute_AttributeIsNull_ThrowsArgumentNullException()
        {
            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.CheckAttribute(null));
        }

        [Test]
        public void CheckAttribute_AttributeIsValid_DoesNotThrowAnyException()
        {
            // Arrange
            Models.Attribute attribute = new Models.Attribute
                { Name = "testAttribute", Type = this._dbValidationSettings.DataTypes.Keys.First() };

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.DoesNotThrow(() => target.CheckAttribute(attribute));
        }

        [Test]
        public void CheckAttribute_AttributeTypeIsNull_ThrowsInvalidAttributeException()
        {
            // Arrange
            Models.Attribute attribute = new Models.Attribute { Name = "testAttribute", Type = null };

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert

            Assert.Throws<InvalidAttributeException>(() => target.CheckAttribute(attribute));
        }

        [Test]
        public void CheckTable_TableIsNull_ThrowsArgumentNullException()
        {
            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.CheckTable(null));
        }

        [Test]
        public void CheckTable_TableHasInvalidName_ThrowsInvalidTableException()
        {
            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.CheckTable(null));
        }

        [SetUp]
        public void Init()
        {
            this._dbValidationSettings =
                new DatabaseValidationSettings(dataTypes:
                    new Dictionary<string, Regex> { { "someType", new Regex(@"^-*[0-9]+$") } });
        }

        [Test]
        public void IsValidDatabaseName_DatabaseNameIsInvalid_ReturnsFalse()
        {
            // Arrange
            string[] dbNames = { null, "", " ", new string(Path.GetInvalidFileNameChars()) };

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            foreach (string dbName  in dbNames)
            {
                Assert.IsFalse(target.IsValidDatabaseName(dbName));
            }
        }

        [Test]
        public void IsValidDatabaseName_DatabaseNameIsValid_ReturnsTrue()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.IsTrue(target.IsValidDatabaseName(dbName));
        }
    }
}