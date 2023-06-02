using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ControlPositionTerminal.Common;
using ControlPositionTerminal.Common.mapper;
using ControlPositionTerminal.Common.Model;
using ControlPositionTerminal.Exceptions;
using ControlPositionTerminal.GateIo;
using ControlPositionTerminal.Util;
using GBinanceFuturesClient.Model.Trade;
using Io.Gate.GateApi.Api;
using Io.Gate.GateApi.Client;
using Io.Gate.GateApi.Model;

namespace ControlPositionTerminal.Service
{
	[Serializable]
	public class GateIoServiceFuturesWrapper : IExchangeServiceWrapper
	{
		private readonly FuturesApi _client;
		private string _settle = "usdt";

		public GateIoServiceFuturesWrapper(string key, string secret)
		{
			Configuration configuration = new Configuration();
			configuration.ApiV4Key = key;
			configuration.ApiV4Secret = secret;
			this._client = new FuturesApi(configuration);

		}

		public async Task<List<PositionData>> RetrieveOpenPositionsAsync()
		{
			List<Position> positions = await ExecuteWithExceptionHandling.Execute(() => _client.ListPositionsAsync(_settle));
			return GetOnlyOpenPositionDataFromResult(positions);
		}

		public async Task<List<InitialLeverage>> RetrieveAllLeverageCoinsAsync()
		{
			List<Position> positions = ExecuteWithExceptionHandling.Execute(() => _client.ListPositions(_settle));

			List<InitialLeverage> nameAndLeverageAllCoins = GetNameAndLeverageAllCoins(positions);
			return nameAndLeverageAllCoins;
		}

		public async Task CloseSelectPositionAsync(PositionData closePos, string partSize)
		{
			FuturesOrder futuresOrder = PositionMapper.PositionDataToGateIoFuturesOrder(closePos);
			FuturesOrder closeOrderMarket = BuildNewFuturesPositionForGateIo.BuildCloseOrderMarket(futuresOrder, partSize);

			FuturesOrder orderResponse = await ExecuteWithExceptionHandling.Execute(() => _client.CreateFuturesOrderAsync("usdt", closeOrderMarket));
			if (orderResponse != null)
			{
				LogList.AddLog("Выполено закрытие позиции. " + orderResponse.Size);
			}
		}

		public async Task SetNewValueLeverage<T>(List<T> symbols, string newLeverage)
		{
			int i = 0;
			foreach (InitialLeverage symbolAndLeverage in symbols.Cast<InitialLeverage>())
			{
				if (newLeverage == "0")
				{
					LogList.AddLog($"Неверное значение плеча.");
					continue;
				}

				if (symbolAndLeverage.Leverage_limit.ToString() == newLeverage)
				{
					LogList.AddLog($"Для символа: {symbolAndLeverage.Symbol}. Плечо уже равно: {newLeverage}");
					continue;
				}
				if (symbols.Count > 1)
				{
					LogList.AddLog("Изменение плеча у всех символов. Пожалуйста ожидайте... " + "Текущий: " + symbolAndLeverage.Symbol + ".Осталось: " + (symbols.Count - i++));
				}
				await ExecuteWithExceptionHandling.Execute(() =>
					_client.UpdateDualModePositionLeverageAsync(_settle, symbolAndLeverage.Symbol, "0",
						newLeverage));
			}
			LogList.AddLog("Изменение плечей завершено.");
		}



		public List<PositionData> GetOnlyOpenPositionDataFromResult<T>(List<T> positions)
		{
			List<PositionData> positionData = PositionMapper.GateIoPositionsToPositionData(positions);
			positionData = PositionUtils.GetOpenPositions(positionData);
			return positionData;
		}

		public List<InitialLeverage> GetNameAndLeverageAllCoins(List<Position> result)
		{
			//List<PositionInforamtionItem> binancePosDto = Parser.ParseJsonToList<PositionInforamtionItem>((string)result[0]);
			List<InitialLeverage> symbolAndLeverage = result
				.Select(dto => new InitialLeverage(dto.Contract, Convert.ToInt32(dto.CrossLeverageLimit)))
				.Distinct()
				.ToList();
			return symbolAndLeverage;
		}
	}
}
