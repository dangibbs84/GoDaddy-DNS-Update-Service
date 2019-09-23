using System.ServiceProcess;


namespace GD_DNS_Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new UpdateTool()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
