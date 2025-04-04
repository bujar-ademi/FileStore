using System.Text.Json;

namespace FileStore.Application.Helpers
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public static T Deserialize<T>(this string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return default(T);
            }
            try
            {
                var t = JsonSerializer.Deserialize<T>(jsonString, jsonOptions);
                return t;
            }
            catch
            {
                return default(T);
            }
        }

        public static string Serialize(this object t)
        {
            return JsonSerializer.Serialize(t, jsonOptions);
        }

        public static bool IsValidJson(this string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    JsonDocument.Parse(strInput);
                    return true;
                }
                catch (JsonException)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
