using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ControlPositionTerminal.Util
{
    /// <summary>
    /// Обработка JSON данных и другие операции.
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Десериализация JSON в список указанного типа.
        /// </summary>
        /// <typeparam name="T">Тип элементов списка.</typeparam>
        /// <param name="json">Строка JSON для разбора.</param>
        /// <returns>Список указанного типа.</returns>
        public static List<T> ParseJsonToList<T>(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<T>>(json);
                }
                catch (JsonException e)
                {
                    // Здесь вы можете обработать исключение, возникшее при разборе JSON
                    Console.WriteLine("Error parsing JSON: " + e.Message);
                }
            }
            return new List<T>();
        }
    }
}