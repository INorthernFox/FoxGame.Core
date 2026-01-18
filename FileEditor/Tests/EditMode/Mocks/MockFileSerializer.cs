using System;
using Core.FileEditor.Serialization;
using FluentResults;

namespace Core.FileEditor.Tests.EditMode.Mocks
{
    public sealed class MockFileSerializer : IFileSerializer
    {
        private Func<object, Result<string>> _serializeFunc;
        private Func<string, Type, Result<object>> _deserializeFunc;

        public int SerializeCallCount { get; private set; }
        public int DeserializeCallCount { get; private set; }
        public object LastSerializedData { get; private set; }
        public string LastDeserializedContent { get; private set; }

        public MockFileSerializer()
        {
            Reset();
        }

        public void Reset()
        {
            SerializeCallCount = 0;
            DeserializeCallCount = 0;
            LastSerializedData = null;
            LastDeserializedContent = null;
            _serializeFunc = null;
            _deserializeFunc = null;
        }

        public void SetupSerialize(Func<object, Result<string>> func)
        {
            _serializeFunc = func;
        }

        public void SetupSerializeSuccess(string json = "{}")
        {
            _serializeFunc = _ => Result.Ok(json);
        }

        public void SetupSerializeFailure(string error = "Serialization failed")
        {
            _serializeFunc = _ => Result.Fail<string>(error);
        }

        public void SetupDeserialize(Func<string, Type, Result<object>> func)
        {
            _deserializeFunc = func;
        }

        public void SetupDeserializeSuccess<T>(T data) where T : class
        {
            _deserializeFunc = (_, __) => Result.Ok<object>(data);
        }

        public void SetupDeserializeFailure(string error = "Deserialization failed")
        {
            _deserializeFunc = (_, __) => Result.Fail<object>(error);
        }

        public Result<string> Serialize<T>(T data) where T : class
        {
            SerializeCallCount++;
            LastSerializedData = data;

            if (_serializeFunc != null)
            {
                return _serializeFunc(data);
            }

            return Result.Ok($"{{\"type\":\"{typeof(T).Name}\"}}");
        }

        public Result<T> Deserialize<T>(string content) where T : class
        {
            DeserializeCallCount++;
            LastDeserializedContent = content;

            if (_deserializeFunc != null)
            {
                var result = _deserializeFunc(content, typeof(T));
                if (result.IsFailed)
                {
                    return result.ToResult<T>();
                }
                return Result.Ok((T)result.Value);
            }

            return Result.Fail<T>("No deserialize setup configured");
        }
    }
}
