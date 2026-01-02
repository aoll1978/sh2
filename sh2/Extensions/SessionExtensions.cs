using System.Text.Json;

namespace sh2.Extensions
{
    // Статический класс расширения
    public static class SessionExtensions
    {
        // Метод для записи сложного объекта в сессию (как JSON)
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Метод для получения сложного объекта из сессии
        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}