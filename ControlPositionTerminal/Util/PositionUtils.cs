using System.Collections.Generic;
using System.Linq;
using ControlPositionTerminal.Common.Model;

namespace ControlPositionTerminal.Util
{
    public class PositionUtils
    {
        /// <summary>
        /// Получить только открытые позиции.
        /// </summary>
        /// <param name="allPos">Список всех позиций.</param>
        /// <returns>Список только открытых позиций.</returns>
        public static List<PositionData> GetOpenPositions(List<PositionData> allPos)
        {
            if (allPos != null && allPos.Any())
            {
                allPos = allPos.Where(p => p.Amount != "0").ToList();
            }
            return allPos;
        }

        /// <summary>
        /// Проверить список позиций и добавить пустую позицию, если список пуст.
        /// </summary>
        /// <param name="positionData">Список позиций.</param>
        public static List<PositionData> CheckAndAddEmptyPosition(List<PositionData> positionData)
        {
            if (positionData.Count == 0)
            {
                positionData.Add(new PositionData("1", "открытых", "позиций", "нет", "None"));
            }
            return positionData;
        }
        /// <summary>
        /// Проверить список позиций и добавить пустую позицию, если список пуст.
        /// </summary>
        /// <param name="positionData">Список позиций.</param>
        public static List<OrderData> CheckAndAddEmptyOrder(List<OrderData> positionData)
        {
            if (positionData.Count == 0)
            {
                positionData.Add(new OrderData("1", "открытых", "ордеров", "нет", "None", "None", "None"));
            }
            return positionData;
        }


    }
}