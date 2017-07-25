using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Wallet.Balances
{
    public class BalanceConverter
    {
        public async Task<decimal> GetBalance(Storage.WalletType convertFrom, decimal convertValue, Storage.WalletType convertTo, CancellationToken cancellationToken)
        {
            if (convertValue == 0)
                return 0;

            if (convertFrom != Storage.WalletType.Iota)
                throw new ArgumentException($"Currency type '{convertFrom}' not supported");

            if (convertTo != Storage.WalletType.Eur)
                throw new ArgumentException($"Currency type '{convertTo}' not supported");

            var url = $"https://api.coinmarketcap.com/v1/ticker/{convertFrom.ToString()}/?convert={convertTo.ToString()}";
            var marketCapArray = await RequestGetAsync<CoinMarketCapEur[]>(url, cancellationToken);
            var marketCap = marketCapArray.FirstOrDefault();
            if (marketCap == null) return 0;
            decimal rate = 0;
            decimal.TryParse(marketCap.price_eur.Replace('.', ','), out rate);

            decimal value = convertValue * rate / 1000000; // todo change that divide hardcode
            return value;
        }

        private async Task<TResponse> RequestGetAsync<TResponse>(string url, CancellationToken cancellationToken) //where TResponse : new()
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(30);
                client.DefaultRequestHeaders.Add("User-Agent", "borlay.wallet");
                var getResponse = await client.GetAsync(url, cancellationToken);
                var stringResult = await getResponse.Content.ReadAsStringAsync();

                if (getResponse.IsSuccessStatusCode)
                {
                    var objectResult = JsonConvert.DeserializeObject<TResponse>(stringResult);
                    return objectResult;
                }
                else
                {
                    throw new Exception(stringResult);
                }
            }
        }
    }

    public class CoinMarketCapEur
    {

        public string id { get; set; } // iota
        public string name { get; set; } // IOTA
        public string symbol { get; set; } // MIOTA 
        public string rank { get; set; } // 8 
        public string price_usd { get; set; } //": "0.269837", 
        public string price_btc { get; set; } //0.00009652", 
        public string price_eur { get; set; } //0.231323165", 

        //"24h_volume_usd": "5207610.0", 
        //"market_cap_usd": "750020113.0", 
        //"available_supply": "2779530283.0", 
        //"total_supply": "2779530283.0", 
        //"percent_change_1h": "0.71", 
        //"percent_change_24h": "-1.69", 
        //"percent_change_7d": "65.4", 
        //"last_updated": "1500751456", 

        //"24h_volume_eur": "4464327.8247", 
        //"market_cap_eur": "642969742.0"
    }

}
