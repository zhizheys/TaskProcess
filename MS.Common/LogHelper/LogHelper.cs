

namespace MS.Common.LogHelper
{
    public class LogHelper
    {
        public static ILogHelper CreateInstance()
        {
            ILogHelper logHelper = new Log4NetHelper();
            return logHelper;
        }
    }
}
