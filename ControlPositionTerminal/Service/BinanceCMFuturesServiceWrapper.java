package com.terminal.position.control.service;//package com.app.service;
//
//
//import com.app.exchangeWrapper.IIExchangeServiceWrapper;
//import com.binance.connector.futures.client.exceptions.BinanceClientException;
//import com.binance.connector.futures.client.exceptions.BinanceConnectorException;
//import com.binance.connector.futures.client.impl.CMFuturesClientImpl;
//import com.terminal.position.common.object.dto.binance.PositionData;
//import java.util.LinkedHashMap;
//import java.util.List;
//import lombok.Getter;
//import lombok.extern.slf4j.Slf4j;
//
//@Getter
//@Slf4j
//public class BinanceCMFuturesServiceWrapper implements IIExchangeServiceWrapper {
//
//  private CMFuturesClientImpl client;
//
//
//  public BinanceCMFuturesServiceWrapper(CMFuturesClientImpl client) {
//    this.client = client;
//  }
//
//  @Override
//  public List<PositionData> retrieveOpenPositions() {
//
//    String result = "";
//    try {
//      result = client.account().positionInformation(new LinkedHashMap<>());
//      log.info(result);
//    } catch (BinanceConnectorException e) {
//      throw new BinanceConnectorException(e.getMessage());
//    } catch (BinanceClientException e) {
//      throw new BinanceClientException(e.getMessage(), e.getErrMsg(), e.getErrorCode(),
//          e.getHttpStatusCode());
//    } finally {
//      log.info(result);
//    }
//
//    return null;
//  }
//
//  @Override
//  public void closeSelectPosition(PositionData positionData) {
////    PositionInforamtionItem PositionInforamtionItem = PositionMapper.positionTableViewToDto(positionForClose);
////
////    CreateNewOrderOrClosePositionUM.closePosition(this, PositionInforamtionItem);
//
//  }
//
//  @Override
//  public void closePartSelectPosition(PositionData positionData, String partSize) {
//
//  }
//
//  @Override
//  public List<PositionData> getOnlyOpenPositionDataFromResult(List<?> positions) {
//    return null;
//  }
//
//
//}
