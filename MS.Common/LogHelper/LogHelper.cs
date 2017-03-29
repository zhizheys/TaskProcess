

namespace MS.Common.LogHelper
{
    public class LogHelper
    {
        //public static ILogHelper CreateInstance()
        //{
        //    ILogHelper logHelper = new Log4NetHelper();
        //    return logHelper;
        //}

        private static ILogHelper _log;

        public static ILogHelper CreateInstance
        {
            get
            {
                if (_log == null)
                {
                    _log =  new Log4NetHelper();
                }
                return _log;
            }
        }
    }
}
