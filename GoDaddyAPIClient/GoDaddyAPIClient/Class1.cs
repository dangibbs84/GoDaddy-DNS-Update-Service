using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections;
using System.Threading.Tasks;
using System.Text;
using System.ServiceModel;
using GDLogger;
using GDAppConf;


namespace GDAPIClient
{
    public class GoDaddyAPIClient : HttpClient
    {
        Logger clLogger = new Logger();

        public GoDaddyAPIClient(string apiKey, string apiSecret, AppConf confObj)
        {
            DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("sso-key", $"{apiKey}:{apiSecret}");
        }

        public async Task<ArrayList> API_GetARecordAsync(string uri)
        {
            try
            {
                bool response_type = true;
                ArrayList arList = new ArrayList();
                HttpResponseMessage response = await GetAsync(uri);
                var result = Regex.Match(await response.Content.ReadAsStringAsync(), @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b");
                if (response.ReasonPhrase != "OK")
                {
                    clLogger.WriteErrorLog("API request failed with the reponse: " + response.ReasonPhrase);
                    clLogger.WriteEventLog("API request failed with the reponse: " + response.ReasonPhrase, EventLogEntryType.Error);
                    response_type = false;
                }
                else
                {
                    response_type = true;
                }
                arList.Add(response.ReasonPhrase);
                arList.Add(result.ToString());
                arList.Add(response_type);
                response.Dispose();
                return arList;
            }
            catch (ProtocolException e)
            {
                clLogger.WriteEventLog(e);
                clLogger.WriteErrorLog(e);
                return null;
            }
        }

        public async Task<ArrayList> API_UpdateARecordAsync(string uri, string param_ip, int param_ttl)
        {
            try
            {
                bool response_type = true;
                ArrayList arList = new ArrayList();
                var payload = JsonConvert.SerializeObject(new[] { new { data = param_ip, ttl = param_ttl } });
                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await PutAsync(uri, content);
                arList.Add(response.ReasonPhrase);
                if (response.ReasonPhrase != "OK")
                {
                    clLogger.WriteErrorLog("API request failed with the reponse: " + response.ReasonPhrase);
                    clLogger.WriteEventLog("API request failed with the reponse: " + response.ReasonPhrase, EventLogEntryType.Error);
                    response_type = false;
                    arList.Add(response_type);
                }
                else
                {
                    clLogger.WriteErrorLog("GoDaddy DNS A Record updated: " + param_ip.ToString());
                    clLogger.WriteEventLog("GoDaddy DNS A Record updated: " + param_ip.ToString(), EventLogEntryType.Information);
                    response_type = true;
                    arList.Add(response_type);
                }
                response.Dispose();
                return arList;
            }
            catch (ProtocolException e)
            {
                clLogger.WriteEventLog(e);
                clLogger.WriteErrorLog(e);
                return null;
            }
        }
    }
}