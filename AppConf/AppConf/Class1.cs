using System;
using System.Diagnostics;
using GDLogger;

namespace GDAppConf
{
    public class AppConf
    {
        Logger clLogger = new Logger();

        public AppConf(string apikey, string secretkey, string webaddressurl, string godaddyuri, string serviceupdaterate, string ttl, string evnlogging, string txtlogging)
        {
            //init folloing without 'try/catch' as there is sufficient handling in GoDaddyAPIClient & IPCheckHelper
            this.APIKey = apikey;
            this.SecretKey = secretkey;
            this.WebAddressURL = webaddressurl;
            this.GoDaddyURI = ("https://api.godaddy.com/v1/domains/" + godaddyuri + "/records/A/%40");

            try
            {
                if (serviceupdaterate == "" || Convert.ToInt32(serviceupdaterate) < 1)
                {
                    clLogger.WriteEventLog("ServiceUpdateRate minimum value is 5, setting ServiceUpdateRate to 5", EventLogEntryType.Warning);
                    clLogger.WriteErrorLog("ServiceUpdateRate minimum value is 5, setting ServiceUpdateRate to 5");
                    this.ServiceUpdateRate = 1;
                }
                else
                {
                    this.ServiceUpdateRate = Convert.ToInt32(serviceupdaterate);
                }
            }
            catch (FormatException e)
            {
                clLogger.WriteEventLog("ServiceUpdateRate not an Integer, setting ServiceUpdateRate to 5 :", e, EventLogEntryType.Warning);
                clLogger.WriteErrorLog("ServiceUpdateRate not an Integer, setting ServiceUpdateRate to 5 :" + e.Message);
                this.ServiceUpdateRate = 1;
            }


            try
            {
                if (ttl == "" || Convert.ToInt32(ttl) < 600)
                {
                    clLogger.WriteEventLog("TTL minimum value is 600, setting TTL to 600", EventLogEntryType.Warning);
                    clLogger.WriteErrorLog("TTL minimum value is 600, setting TTL to 600");
                    this.TTL = 600;
                }
                else
                {
                    this.TTL = Convert.ToInt32(ttl);
                }

            }
            catch (FormatException e)
            {
                clLogger.WriteEventLog("TTL not an Integer, setting TTL to 600 :", e, EventLogEntryType.Warning);
                clLogger.WriteErrorLog("TTL not an Integer, setting TTL to 600 :" + e.Message);
                this.TTL = 600;
            }


            try
            {
                this.DisableEventLogging = Convert.ToBoolean(evnlogging);
            }
            catch (FormatException e)
            {
                clLogger.WriteEventLog("DisableEventLogging not Boolean, setting DisableEventLogging to 'false' :", e, EventLogEntryType.Warning);
                clLogger.WriteErrorLog("DisableEventLogging not Boolean, setting DisableEventLogging to 'false' :" + e.Message);
                this.DisableEventLogging = false;
            }

            try
            {
                this.DisableLogfileLogging = Convert.ToBoolean(txtlogging);
            }
            catch (FormatException e)
            {
                clLogger.WriteEventLog("DisableLogfileLogging not Boolean, setting DisableEventLogging to 'false' :", e, EventLogEntryType.Warning);
                clLogger.WriteErrorLog("DisableLogfileLogging not Boolean, setting DisableEventLogging to 'false' :" + e.Message);
                this.DisableEventLogging = false;
            }
        }

        public string APIKey { get; set; }
        public string SecretKey { get; set; }
        public string WebAddressURL { get; set; }
        public string GoDaddyURI { get; set; }
        public int ServiceUpdateRate { get; set; }
        public int TTL { get; set; }
        public bool DisableEventLogging { get; set; }
        public bool DisableLogfileLogging { get; set; }
    }
}
