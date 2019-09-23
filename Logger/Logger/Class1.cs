using System;
using System.IO;
using System.Diagnostics;


namespace GDLogger
{
    public class Logger
    {
        // Write to text file, an exception
        public void WriteErrorLog(SystemException ex)
        {
            StreamWriter sw = null;
            try
            {
                // get value from application config
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logfile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Message.ToString().Trim());
                // flush and close stream reader
                sw.Flush();
                sw.Close();
            }

            catch (UnauthorizedAccessException e)
            {
                WriteEventLog(e);
            }
            catch (SystemException e)
            {
                // write any exception to both logs
                WriteEventLog(e);
                WriteErrorLog(e);
            }
        }

        // Write to text file, information 
        public void WriteErrorLog(string Message)
        {
            StreamWriter sw = null;
            try
            {
                // get value from application config
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logfile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                // flush and close stream reader
                sw.Flush();
                sw.Close();
            }
            catch (UnauthorizedAccessException e)
            {
                WriteEventLog(e);
            }
            catch (SystemException e)
            {
                // write any exception to both logs
                WriteEventLog(e);
                WriteErrorLog(e);
            }
        }

        // Write to text file, information and exception
        public void WriteErrorLog(string Message, SystemException ex)
        {
            StreamWriter sw = null;
            try
            {
                // get value from application config
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logfile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                // flush and close stream reader
                sw.Flush();
                sw.Close();
            }
            catch (UnauthorizedAccessException e)
            {
                WriteEventLog(e);
            }
            catch (SystemException e)
            {
                // write any exception to both logs
                WriteEventLog(e);
                WriteErrorLog(e);
            }
        }

        // overloaded methods for writing to event viewer
        // First method to allow specific message type
        public void WriteEventLog(string Message, EventLogEntryType type)
        {
            EventLog myLog = new EventLog
            {
                Source = "GoDaddy Update Tool"
            };
            myLog.WriteEntry(Message, type);
        }

        // overloaded methods for writing to event viewer
        // First method to allow specific message type
        public void WriteEventLog(string Message, SystemException ex, EventLogEntryType type)
        {
            EventLog myLog = new EventLog
            {
                Source = "GoDaddy Update Tool"
            };
            myLog.WriteEntry(Message + " " + ex.Message, type);
        }

        // Second method, system exception
        public void WriteEventLog(SystemException ex)
        {
            EventLog myLog = new EventLog
            {
                Source = "GoDaddy Update Tool"
            };
            myLog.WriteEntry(ex.Message, EventLogEntryType.Error);
        }

        // Third method, UnauthorizedAccessException
        public void WriteEventLog(UnauthorizedAccessException ex)
        {
            EventLog myLog = new EventLog
            {
                Source = "GoDaddy Update Tool"
            };
            myLog.WriteEntry(ex.Message, EventLogEntryType.Error);
        }
    }
}
