using ControlPositionTerminal.Common.userSettings;
using ControlPositionTerminal.Util.Enums;
using System;

namespace ControlPositionTerminal.Common.UserSettings
{
    [Serializable]
    public class ExchangeServiceManager
    {
        /// <summary>
        /// Создать экземпляр обертки для обмена данными с выбранным сервером.
        /// </summary>
        /// <param name="fullNameSelectServerEnum">Полное имя выбранного сервера.</param>
        /// <returns>Экземпляр IIExchangeServiceWrapper для выбранного сервера.</returns>
        public IExchangeServiceWrapper CreateExchangeService(FullNameSelectServerEnum fullNameSelectServerEnum)
        {
            return ExchangeServiceFactory.CreateExchangeService(fullNameSelectServerEnum);
        }
    }
}