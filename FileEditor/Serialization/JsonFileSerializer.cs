using System;
using FluentResults;
using Newtonsoft.Json;

namespace Core.FileEditor.Serialization
{
    public sealed class JsonFileSerializer : IFileSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonFileSerializer()
        {
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public Result<string> Serialize<T>(T data) where T : class
        {
            try
            {
                var json = JsonConvert.SerializeObject(data, _settings);
                return Result.Ok(json);
            }
            catch (Exception ex)
            {
                return Result.Fail<string>($"Failed to serialize: {ex.Message}");
            }
        }

        public Result<T> Deserialize<T>(string content) where T : class
        {
            try
            {
                var data = JsonConvert.DeserializeObject<T>(content, _settings);
                if (data == null)
                {
                    return Result.Fail<T>("Deserialization returned null");
                }
                return Result.Ok(data);
            }
            catch (Exception ex)
            {
                return Result.Fail<T>($"Failed to deserialize: {ex.Message}");
            }
        }
    }
}
