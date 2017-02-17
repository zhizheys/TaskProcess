
namespace MS.Common.ExcelHelper
{
    using System.Collections.Generic;
    using System.IO;

    public interface IExcelHelper
    {
        void CreateExcel<T>(List<T> lists, string workbookFilePath);
        List<T> ImportExcel<T>(ref List<string> resultMessage, Stream excelFileStream);
    }
}
