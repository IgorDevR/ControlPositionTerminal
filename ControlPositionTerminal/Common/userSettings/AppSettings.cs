using System;
using System.IO;
using System.Reflection;
using ControlPositionTerminal.Common.userSettings;
using System.Runtime.Serialization;
using ControlPositionTerminal.Util;
using ControlPositionTerminal.Util.Enums;



namespace ControlPositionTerminal.Common.UserSettings
{
    [Serializable]
    public sealed class AppSettings
    {
        private static AppSettings instance;

        public static AppSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppSettings();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        private IExchangeServiceWrapper _exchangeServiceWrapper;

        private ExchangeServiceManager _exchangeServiceManager = new ExchangeServiceManager();

        private AppSettings()
        {
        }

        public static AppSettings GetInstance()
        {
            return Instance;
        }

        public string MainWindowName { get; set; }

        public string ApiKeyBinanceSpot { get; set; }
        public string SecretKeyBinanceSpot { get; set; }

        public string ApiKeyBinanceFutures { get; set; }
        public string SecretKeyBinanceFutures { get; set; }

        public string ApiKeyBinanceFuturesTestnet { get; set; }
        public string SecretKeyBinanceFuturesTestnet { get; set; }

        public readonly string UM_BASE_URL = "https://fapi.binance.com";
        public readonly string CM_BASE_URL = "https://dapi.binance.com";
        public readonly string TESTNET_BASE_URL = "https://testnet.binancefuture.com";

        public string ApiKeyGateIoSpot { get; set; }
        public string SecretKeyGateIoSpot { get; set; }

        public string ApiKeyGateIoFutures { get; set; }
        public string SecretKeyGateIoFutures { get; set; }


        public TypesServerEnum SelectedServerType { get; set; }
        public TypesMarketEnum SelectedMarketType { get; set; }
        public TypesMarketCoinEnum SelectedCoinType { get; set; }

        public FullNameSelectServerEnum FullNameSelectServerEnum { get; set; }

        public IExchangeServiceWrapper GetExchangeService()
        {
            return _exchangeServiceManager.CreateExchangeService(FullNameSelectServerEnum);
        }
        private void SetFullNameSelectServer()
        {
            try
            {
                string fullNameSelectServer = SelectedServerType + "_" + SelectedCoinType + "_" + SelectedMarketType;
                FullNameSelectServerEnum = Enum.Parse<FullNameSelectServerEnum>(fullNameSelectServer);
            }
            catch (Exception e)
            {
                LogList.AddLog("Ошибка получения выбранного сервера");
            }
        }

        public void SaveObjectFieldsToFile()
        {
            SetFullNameSelectServer();

            string filename = "userSettings.txt";
            try
            {
                using (Stream stream = File.Open(filename, FileMode.Create))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    binaryFormatter.Serialize(stream, this);
                }
                LogList.AddLog("Настройки сохранены в файл " + filename);
            }
            catch (IOException e)
            {
                LogList.AddLog("Ошибка сохранения файла" + filename + ". " + e.Message);
            }
        }
        public void LoadObjectFieldsFromFile()
        {
            string filename = "userSettings.txt";

            try
            {
                using (Stream stream = File.Open(filename, FileMode.Open))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    AppSettings loadedSettings = (AppSettings)binaryFormatter.Deserialize(stream);

                    FieldInfo[] fields = typeof(AppSettings).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    foreach (FieldInfo field in fields)
                    {
                        object value = field.GetValue(loadedSettings);
                        field.SetValue(AppSettings.Instance, value);
                    }
                }
                Console.WriteLine("Файл userSettings.txt найден, настройки загружены.");
                LogList.AddLog("Файл userSettings.txt найден, настройки загружены.");
            }
            catch (IOException e)
            {
                Console.WriteLine("Файл userSettings.txt не найден, вызвана настройка по умолчанию." + e.Message);
                LogList.AddLog("Файл userSettings.txt не найден, вызвана настройка по умолчанию." + e.Message);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Файл userSettings.txt не найден, вызвана настройка по умолчанию." + e.Message);
                LogList.AddLog("Файл userSettings.txt не найден, вызвана настройка по умолчанию." + e.Message);
            }
        }
    }
}