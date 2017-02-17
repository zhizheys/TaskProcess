using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Common.ExcelHelper
{
   public class ExcelHelper
    {
        public static IExcelHelper CreateInstance()
        {
            IExcelHelper excelHelper = new NpoiExcelHelper();
            return excelHelper;
        }
    }
}
