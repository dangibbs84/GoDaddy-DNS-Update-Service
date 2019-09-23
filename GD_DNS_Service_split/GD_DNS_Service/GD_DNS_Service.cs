using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using System.Configuration;
using GDAPIClient;
using GDLogger;
using IPCheckHelper;
using GDAppConf;

namespace GD_DNS_Service
{
    public partial class UpdateTool : ServiceBase
    {
        // class variables
        AppConf ConfigObj;
        GetPubIP clGetPubIP;

        public UpdateTool()
        {
            InitializeComponent();
            // Create the source, if it does not already exist.
            if (!EventLog.SourceExists("GoDaddy DNS Update Serice"))
            {
                //An event log source should not be created and immediately used.
                //There is a latency time to enable the source, it should be created
                //prior to executing the application that uses the source.
                EventLog.CreateEventSource("GoDaddy DNS Update Serice", "GoDaddy Update Tool");
            }

        }

        Logger clLogger = new Logger();
        protected override void OnStart(string[] args)
        {
            // Send start message to text log and event viewer 
            clLogger.WriteErrorLog("GoDaddy DNS Service Started");
            clLogger.WriteEventLog("GoDaddy DNS Service Started", EventLogEntryType.Information);
            System.Threading.Thread.Sleep(1000);

            // init classes
            clGetPubIP = new GetPubIP();
            // pull data from App.config 
            ConfigObj = new AppConf(
            ConfigurationManager.AppSettings["APIKey"],
            ConfigurationManager.AppSettings["SecretKey"],
            ConfigurationManager.AppSettings["WebAddressURL"],
            ConfigurationManager.AppSettings["GoDaddyDomain"],
            ConfigurationManager.AppSettings["ServiceUpdateRate"],
            ConfigurationManager.AppSettings["TTL"],
            ConfigurationManager.AppSettings["DisableEventLogging"],
            ConfigurationManager.AppSettings["DisableLogfileLogging"]
            );

            // init timer
            // setup service timer loop, multiply value by 60000(ms)
            Timer timer1 = null;
            timer1 = new Timer();
            timer1.Interval = ConfigObj.ServiceUpdateRate * 60000;
            timer1.Elapsed += new ElapsedEventHandler(timer1_tick);
            timer1.Enabled = true;
        }

        // Each timer tick -> run Update method 
        //public void timer1_tick(object sender, ElapsedEventArgs e)
        public void timer1_tick(object sender, ElapsedEventArgs e)
        {

            IsIPNew ServiceRun = new IsIPNew();
            GoDaddyAPIClient APIobj = new GoDaddyAPIClient(ConfigObj.APIKey, ConfigObj.SecretKey, ConfigObj);
            ServiceRun.Run(clGetPubIP, APIobj, ConfigObj);
        }

        // Send to text file and even viewer service stop
        protected override void OnStop()
        {
            clLogger.WriteErrorLog("GoDaddy DNS Service Stopped <--!");
            clLogger.WriteEventLog("GoDaddy DNS Service Stopped", EventLogEntryType.Warning);
        }
    }

}

