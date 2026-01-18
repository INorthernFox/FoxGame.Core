using System.Collections.Generic;

namespace Core.FileEditor.Tests.EditMode.TestData
{
    public sealed class SimpleData
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public SimpleData() { }

        public SimpleData(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }

    public sealed class NestedData
    {
        public int Id { get; set; }
        public InnerData Inner { get; set; }

        public NestedData() { }

        public NestedData(int id, InnerData inner)
        {
            Id = id;
            Inner = inner;
        }

        public sealed class InnerData
        {
            public string Description { get; set; }
            public List<string> Tags { get; set; }

            public InnerData() { }

            public InnerData(string description, List<string> tags)
            {
                Description = description;
                Tags = tags;
            }
        }
    }

    public sealed class CircularData
    {
        public string Name { get; set; }
        public CircularData Reference { get; set; }

        public CircularData() { }

        public CircularData(string name)
        {
            Name = name;
        }

        public static CircularData CreateWithCircularReference()
        {
            var data = new CircularData("root");
            data.Reference = data;
            return data;
        }
    }

    public sealed class NullableData
    {
        public string RequiredField { get; set; }
        public string OptionalField { get; set; }
        public int? NullableInt { get; set; }
        public List<string> OptionalList { get; set; }

        public NullableData() { }

        public NullableData(string requiredField)
        {
            RequiredField = requiredField;
        }
    }

    public sealed class ComplexData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Score { get; set; }
        public bool IsActive { get; set; }
        public List<SimpleData> Items { get; set; }
        public Dictionary<string, int> Metadata { get; set; }

        public ComplexData() { }

        public ComplexData(int id, string name, double score, bool isActive)
        {
            Id = id;
            Name = name;
            Score = score;
            IsActive = isActive;
            Items = new List<SimpleData>();
            Metadata = new Dictionary<string, int>();
        }
    }

    public sealed class EmptyData
    {
    }

    public static class TestDataFactory
    {
        public static SimpleData CreateSimpleData()
        {
            return new SimpleData("TestName", 42);
        }

        public static NestedData CreateNestedData()
        {
            return new NestedData(1, new NestedData.InnerData("Test description", new List<string> { "tag1", "tag2" }));
        }

        public static NullableData CreateNullableDataWithNulls()
        {
            return new NullableData("required")
            {
                OptionalField = null,
                NullableInt = null,
                OptionalList = null
            };
        }

        public static NullableData CreateNullableDataFull()
        {
            return new NullableData("required")
            {
                OptionalField = "optional",
                NullableInt = 100,
                OptionalList = new List<string> { "item1", "item2" }
            };
        }

        public static ComplexData CreateComplexData()
        {
            var data = new ComplexData(1, "Complex", 95.5, true);
            data.Items.Add(new SimpleData("Item1", 10));
            data.Items.Add(new SimpleData("Item2", 20));
            data.Metadata["key1"] = 100;
            data.Metadata["key2"] = 200;
            return data;
        }

        public const string SimpleDataJson = "{\"Name\":\"TestName\",\"Value\":42}";
        public const string InvalidJson = "{\"Name\":\"Test\"";
        public const string EmptyJson = "{}";
        public const string NullJson = "null";
    }
}
