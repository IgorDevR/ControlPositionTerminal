using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ControlPositionTerminal.Common.Model;

namespace ControlPositionTerminal.Common
{
    /// <summary>
    /// Интерфейс-обертка для объединения всех сервисов.
    /// </summary>
    public interface IExchangeServiceWrapper
    {
        /// <summary>
        /// Получить все открытые позиции.
        /// </summary>
        /// <returns>Список объектов PositionData, представляющих открытые позиции.</returns>
        Task<List<PositionData>> RetrieveOpenPositionsAsync();

        /// <summary>
        /// Получить все позиции и их плечи.
        /// </summary>
        /// <typeparam name="T">Тип данных для позиций.</typeparam>
        /// <returns>Список символов и их значения плеча.</returns>
        Task<List<InitialLeverage>> RetrieveAllLeverageCoinsAsync();

        /// <summary>
        /// Установить новое значение плеча.
        /// </summary>
        /// <typeparam name="T">Тип данных символов.</typeparam>
        /// <param name="symbols">Список символов. Если список пуст, плечо будет изменено для всех доступных символов.</param>
        /// <param name="leverageStr">Новое значение плеча.</param>
        Task SetNewValueLeverage<T>(List<T> symbols, string leverageStr);

        /// <summary>
        /// Закрыть выбранную позицию.
        /// </summary>
        /// <param name="positionData">Объект PositionData, представляющий позицию для закрытия.</param>
        /// <param name="partSize">Размер позиции для закрытия.</param>
        Task CloseSelectPositionAsync(PositionData positionData, string partSize);

        /// <summary>
        /// Получить только открытые позиции с объемом, отличным от 0.
        /// </summary>
        /// <typeparam name="T">Тип данных позиций.</typeparam>
        /// <param name="positions">Список позиций разных типов от разных бирж.</param>
        /// <returns>Список открытых позиций.</returns>
        List<PositionData> GetOnlyOpenPositionDataFromResult<T>(List<T> positions);


    }
}
