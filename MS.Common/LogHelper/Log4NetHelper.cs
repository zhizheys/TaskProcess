using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.IO;
using System.Reflection;

namespace MS.Common.LogHelper
{
   public class Log4NetHelper : ILogHelper
    {
        log4net.ILog log;

        public Log4NetHelper()
        {
            string logConfigPath = AppDomain.CurrentDomain.BaseDirectory + @"Config\log4net.config";

            if (!File.Exists(logConfigPath))
            {
                throw new Exception(string.Format("the log4net config file: {0} is not exists, please check ", logConfigPath));
            }

            FileInfo logCfg = new FileInfo(logConfigPath);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(logCfg);
            Type type = MethodBase.GetCurrentMethod().DeclaringType;
            log = log4net.LogManager.GetLogger(type);
        }

        public void Debug(object message)
        {
            log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            log.Debug(message, exception);
        }

        public void Info(object message)
        {
            log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            log.Info(message, exception);
        }

        public void Warn(object message)
        {
            log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            log.Warn(message, exception);
        }

        public void Error(object message)
        {
            log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            log.Error(message, exception);
        }

        public void Fatal(object message)
        {
            log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            log.Fatal(message, exception);
        }
    }
}
