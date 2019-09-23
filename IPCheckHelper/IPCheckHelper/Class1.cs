using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using System.Collections;
using System.ServiceModel;
using GDLogger;
using GDAPIClient;
using GDAppConf;

namespace IPCheckHelper
{
    public class GetPubIP
    {
        Logger clLogger = new Logger();
        public string GetIP(string url)
        {
            try
            {
                string IP = "";
                List<System.Text.RegularExpressions.Match> IPList = new List<System.Text.RegularExpressions.Match>();
                // Web Request 
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    // Fill address with entire HTML page as a char stream
                    url = stream.ReadToEnd();
                    // Regular Expression to find the IP address in char stream ---->
                    // This should now work with many services that return the public IP such as:
                    //    icanhazip.com         // 
                    //    checkip.dyndns.org    //
                    //    www.displaymyip.com   //
                    // AVOID USING ANY PAGE WITH MORE THAN ONE ADDRESS AS WE ALWAYS EXPECT ONLY ONE OBJECT IN COLLECTION!!
                    IPList.Add(Regex.Match(url, @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b"));
                    IP = IPList[0].ToString();

                    if (IP == null || IP == "")
                    {
                        clLogger.WriteErrorLog("WebAddressURL does not contain an IP address. Operation failed!");
                        clLogger.WriteEventLog("WebAddressURL does not contain an IP address. Operation failed!", EventLogEntryType.Error);
                        return null;
                    }
                    else
                        return IP;
                }

            }
            catch (System.Configuration.ConfigurationErrorsException e)
            {
                clLogger.WriteErrorLog(e);
                clLogger.WriteEventLog(e);
                return null;
            }
            catch (System.SystemException e)
            {
                clLogger.WriteErrorLog(e);
                clLogger.WriteEventLog(e);
                return null;
            }
        }
    }

    public class IsIPNew
    {
        Logger clLogger = new Logger();
        public async void Run(GetPubIP GetPubIP, GoDaddyAPIClient gd_API, AppConf ConfigObj)
        {
            try
            {
                ArrayList arList = gd_API.API_GetARecordAsync(ConfigObj.GoDaddyURI).Result;
                if (arList[2].Equals(true))
                {
                    if (!GetPubIP.GetIP(ConfigObj.WebAddressURL).Equals(arList[1].ToString(), StringComparison.Ordinal))
                    {
                        await gd_API.API_UpdateARecordAsync(ConfigObj.GoDaddyURI, GetPubIP.GetIP(ConfigObj.WebAddressURL), ConfigObj.TTL);
                    }
                    else
                    {
                        clLogger.WriteErrorLog("No IP Change detected");
                        clLogger.WriteEventLog("No IP Change detected", EventLogEntryType.Information);
                    }
                }
            }
            catch (ProtocolException e)
            {
                clLogger.WriteErrorLog(e);
                clLogger.WriteEventLog(e);
            }
            catch (System.SystemException e)
            {
                clLogger.WriteErrorLog(e);
                clLogger.WriteEventLog(e);
            }
        }

    }
}
