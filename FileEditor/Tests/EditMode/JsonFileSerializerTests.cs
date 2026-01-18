using Core.FileEditor.Serialization;
using Core.FileEditor.Tests.EditMode.TestData;
using NUnit.Framework;

namespace Core.FileEditor.Tests.EditMode
{
    [TestFixture]
    public sealed class JsonFileSerializerTests
    {
        private JsonFileSerializer _serializer;

        [SetUp]
        public void SetUp()
        {
            _serializer = new JsonFileSerializer();
        }

        #region Serialize Tests

        [Test]
        public void Serialize_SimpleData_ReturnsSuccessWithJson()
        {
            var data = TestDataFactory.CreateSimpleData();

            var result = _serializer.Serialize(data);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Contains("\"Name\""));
            Assert.IsTrue(result.Value.Contains("\"TestName\""));
            Assert.IsTrue(result.Value.Contains("\"Value\""));
            Assert.IsTrue(result.Value.Contains("42"));
        }

        [Test]
        public void Serialize_NestedData_ReturnsSuccessWithNestedJson()
        {
            var data = TestDataFactory.CreateNestedData();

            var result = _serializer.Serialize(data);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Contains("\"Id\""));
            Assert.IsTrue(result.Value.Contains("\"Inner\""));
            Assert.IsTrue(result.Value.Contains("\"Description\""));
            Assert.IsTrue(result.Value.Contains("\"Tags\""));
        }

        [Test]
        public void Serialize_CircularReference_ReturnsSuccessWithReferenceLoopIgnored()
        {
            var data = CircularData.CreateWithCircularReference();

            var result = _serializer.Serialize(data);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Contains("\"Name\""));
            Assert.IsTrue(result.Value.Contains("\"root\""));
        }

        [Test]
        public void Serialize_NullableDataWithNulls_OmitsNullFields()
        {
            var data = TestDataFactory.CreateNullableDataWithNulls();

            var result = _serializer.Serialize(data);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Contains("\"RequiredField\""));
            Assert.IsFalse(result.Value.Contains("\"OptionalField\""));
            Assert.IsFalse(result.Value.Contains("\"NullableInt\""));
            Assert.IsFalse(result.Value.Contains("\"OptionalList\""));
        }

        [Test]
        public void Serialize_NullableDataFull_IncludesAllFields()
        {
            var data = TestDataFactory.CreateNullableDataFull();

            var result = _serializer.Serialize(data);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Contains("\"RequiredField\""));
            Assert.IsTrue(result.Value.Contains("\"OptionalField\""));
            Assert.IsTrue(result.Value.Contains("\"NullableInt\""));
            Assert.IsTrue(result.Value.Contains("\"OptionalList\""));
        }

        [Test]
        public void Serialize_EmptyData_ReturnsEmptyObject()
        {
            var data = new EmptyData();

            var result = _serializer.Serialize(data);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("{}", result.Value.Trim());
        }

        [Test]
        public void Serialize_ComplexData_ReturnsSuccessWithAllFields()
        {
            var data = TestDataFactory.CreateComplexData();

            var result = _serializer.Serialize(data);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Contains("\"Id\""));
            Assert.IsTrue(result.Value.Contains("\"Name\""));
            Assert.IsTrue(result.Value.Contains("\"Score\""));
            Assert.IsTrue(result.Value.Contains("\"IsActive\""));
            Assert.IsTrue(result.Value.Contains("\"Items\""));
            Assert.IsTrue(result.Value.Contains("\"Metadata\""));
        }

        #endregion

        #region Deserialize Tests

        [Test]
        public void Deserialize_ValidSimpleJson_ReturnsSuccess()
        {
            var json = TestDataFactory.SimpleDataJson;

            var result = _serializer.Deserialize<SimpleData>(json);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("TestName", result.Value.Name);
            Assert.AreEqual(42, result.Value.Value);
        }

        [Test]
        public void Deserialize_InvalidJson_ReturnsFailure()
        {
            var json = TestDataFactory.InvalidJson;

            var result = _serializer.Deserialize<SimpleData>(json);

            Assert.IsTrue(result.IsFailed);
        }

        [Test]
        public void Deserialize_EmptyJson_ReturnsSuccessWithDefaults()
        {
            var json = TestDataFactory.EmptyJson;

            var result = _serializer.Deserialize<SimpleData>(json);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.Value.Name);
            Assert.AreEqual(0, result.Value.Value);
        }

        [Test]
        public void Deserialize_NullJson_ReturnsFailure()
        {
            var json = TestDataFactory.NullJson;

            var result = _serializer.Deserialize<SimpleData>(json);

            Assert.IsTrue(result.IsFailed);
            Assert.IsTrue(result.Errors[0].Message.Contains("null"));
        }

        [Test]
        public void Deserialize_EmptyString_ReturnsFailure()
        {
            var json = string.Empty;

            var result = _serializer.Deserialize<SimpleData>(json);

            Assert.IsTrue(result.IsFailed);
        }

        #endregion

        #region Round-trip Tests

        [Test]
        public void RoundTrip_SimpleData_PreservesData()
        {
            var original = TestDataFactory.CreateSimpleData();

            var serializeResult = _serializer.Serialize(original);
            Assert.IsTrue(serializeResult.IsSuccess);

            var deserializeResult = _serializer.Deserialize<SimpleData>(serializeResult.Value);
            Assert.IsTrue(deserializeResult.IsSuccess);

            Assert.AreEqual(original.Name, deserializeResult.Value.Name);
            Assert.AreEqual(original.Value, deserializeResult.Value.Value);
        }

        #endregion
    }
}
