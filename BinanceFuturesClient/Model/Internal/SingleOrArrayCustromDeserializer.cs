using GBinanceFuturesClient.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBinanceFuturesClient.Model.Internal
{
    internal class SingleOrArrayCustromDeserializer<T> : ICustomDeserializer<List<T>>
    {
        public List<T> Deserialize(string response)
        {
            if (response.Contains("\"code\":") && response.Contains("\"msg\":")
                || (response.Contains("\"status\":") && response.Contains("404")))
            {
                var errorResponse = JsonTools.DeserializeFromJson<ApiResponseError>(response);
                throw new ApiException(errorResponse.Code, errorResponse.Msg  + ". " + errorResponse.Status);
            }

            List<T> responseDeserialized;

            if (response[0] == '[')
                responseDeserialized = JsonTools.DeserializeFromJson<List<T>>(response);
            else
            {
                responseDeserialized = new List<T>();
                responseDeserialized.Add(JsonTools.DeserializeFromJson<T>(response));
            }

            return responseDeserialized;
        }
    }
}