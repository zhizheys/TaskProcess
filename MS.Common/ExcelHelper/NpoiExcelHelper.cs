
namespace MS.Common.ExcelHelper
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.IO;
    using NPOI.XSSF.UserModel;
    using NPOI.SS.UserModel;
    using MS.TaskProcess.Model;

    public class NpoiExcelHelper : IExcelHelper
    {

        /// <summary>
        /// export Excel
        /// </summary>
        /// <param name="lists"></param>
        /// <param name="head">head name</param>
        /// <param name="workbookFilePath">save name</param>
        public void CreateExcel<T>(List<T> lists, string workbookFilePath)
        {
            try
            {
                XSSFWorkbook workbook = new XSSFWorkbook();
                MemoryStream ms = new MemoryStream();
                ISheet sheet = workbook.CreateSheet("sheet1");
                IRow headerRow = sheet.CreateRow(0);
                bool h = false;
                int j = 1;
                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties();
                Dictionary<int, ColumnInfo> dicColumn = new Dictionary<int, ColumnInfo>();
                dicColumn = GetColumnInfoDictionary(properties);

                foreach (T item in lists)
                {
                    IRow dataRow = sheet.CreateRow(j);
                    //int i = 0;
                    foreach (PropertyInfo property in properties)
                    {
                        ColumnInfo tempColumnInfo = new ColumnInfo();
                        foreach (var k in dicColumn)
                        {
                            if (property.Name == k.Value.ColumnPropertyName)
                            {
                                //Console.WriteLine(k.Key);
                                tempColumnInfo = k.Value;

                                if (!h)
                                {
                                    headerRow.CreateCell(tempColumnInfo.ColumnIndex).SetCellValue(tempColumnInfo.TitleName == null ? tempColumnInfo.ColumnPropertyName : tempColumnInfo.TitleName);
                                    dataRow.CreateCell(tempColumnInfo.ColumnIndex).SetCellValue(property.GetValue(item, null) == null ? "" : property.GetValue(item, null).ToString());
                                }
                                else
                                {
                                    dataRow.CreateCell(tempColumnInfo.ColumnIndex).SetCellValue(property.GetValue(item, null) == null ? "" : property.GetValue(item, null).ToString());
                                }

                                break;
                            }
                        }

                        //i++;
                    }
                    h = true;
                    j++;
                }
                workbook.Write(ms);

                FileStream fs = new FileStream(workbookFilePath, FileMode.Create, FileAccess.Write);
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Close();

                ms.Flush();
                sheet = null;
                headerRow = null;
                workbook = null;
                data = null;
                ms = null;
                fs = null;
            }
            catch (Exception ex)
            {
                throw ex;
                //string see = ee.Message;
            }
        }

        /// <summary>
        /// import excel file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resultMessage">error message list</param>
        /// <param name="excelFileStream"></param>
        /// <returns></returns>
        public List<T> ImportExcel<T>(ref List<string> resultMessage, Stream excelFileStream)
        {
            List<T> listImportData = new List<T>();
            Dictionary<int, ColumnInfo> dicColumn = new Dictionary<int, ColumnInfo>();
            //get T property
            PropertyInfo[] propertyArray = typeof(T).GetProperties();

            dicColumn = GetColumnInfoDictionary(propertyArray);

            using (excelFileStream)
            {
                IWorkbook workbook = new XSSFWorkbook(excelFileStream);
                //get first sheet
                ISheet sheet = workbook.GetSheetAt(0);

                //get the firs row, the fist row usually is title
                IRow headerRow = sheet.GetRow(0);
                int cellCount = propertyArray.Length;
                int rowCount = sheet.LastRowNum; //LastRowNum = PhysicalNumberOfRows - 1
                int tempRowNumber = 1;

                for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
                {
                    tempRowNumber++;
                    IRow row = sheet.GetRow(i);

                    if (row != null)
                    {
                        T entity = Activator.CreateInstance<T>();
                        int j = 0;
                        int tempColumnNumber = 0;

                        foreach (PropertyInfo propertyItem in propertyArray)
                        {
                            ColumnInfo tempColumnInfo = new ColumnInfo();
                            foreach (var k in dicColumn)
                            {
                                if (propertyItem.Name == k.Value.ColumnPropertyName)
                                {
                                    int indexNumber = k.Value.ColumnIndex;
                                    tempColumnNumber = k.Value.ColumnIndex + 1;

                                    try
                                    {
                                        //when cell value is not null
                                        if (row.GetCell(indexNumber) != null)
                                        {
                                            //var a = row.GetCell(indexNumber).ToString();
                                            //var b =Convert.ChangeType(row.GetCell(indexNumber).ToString(), propertyItem.PropertyType);
                                            var cellValue = GetCellData(row.GetCell(indexNumber).ToString(), propertyItem.PropertyType);
                                            propertyItem.SetValue(entity, cellValue, null);
                                        }
                                        else
                                        {
                                            //when cell value is null
                                            GetCellData(null, propertyItem.PropertyType);
                                            propertyItem.SetValue(entity, null, null);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //data convert error
                                        j++;
                                        //erro row number and column number
                                        resultMessage.Add(String.Format("data type is error: the row number is: {0} ; the column number is: {1} ; the value is: {2}", tempRowNumber, tempColumnNumber, row.GetCell(indexNumber)));
                                    }
                                }
                            }
                        }

                        if (j == 0)
                        {
                            listImportData.Add(entity);
                        }
                    }
                }

                return listImportData;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyArray"></param>
        /// <returns></returns>
        private static Dictionary<int, ColumnInfo> GetColumnInfoDictionary(PropertyInfo[] propertyArray)
        {
            Dictionary<int, ColumnInfo> dicColumn = new Dictionary<int, ColumnInfo>();

            foreach (PropertyInfo p in propertyArray)
            {
                string name = p.Name;
                Type pType = p.PropertyType;
                string typeName = pType.Name;

                //get the attribute ot the property 
                MsValidationAttribute pAttribute = (MsValidationAttribute)Attribute.GetCustomAttribute(p, typeof(MsValidationAttribute));
                string description = pAttribute.Description;
                int columnIndex = pAttribute.ColumnIndex;

                ColumnInfo columnInfo = new ColumnInfo();
                columnInfo.ColumnIndex = columnIndex;
                columnInfo.TitleName = description;
                columnInfo.ColumnPropertyName = name;

                dicColumn.Add(columnIndex, columnInfo);
            }

            return dicColumn;
        }

        /// <summary>
        /// get Excel cell value
        /// </summary>
        /// <param name="cellValue">cell value</param>
        /// <param name="type">object date type</param>
        /// <returns>object</returns>
        private static object GetCellData(string cellValue, Type type)
        {
            #region convert data type

            if (typeof(string) == type)
            {
                return cellValue;
            }

            if (typeof(char) == type)
            {
                return char.Parse(cellValue);
            }

            if (typeof(byte) == type)
            {
                return byte.Parse(cellValue);
            }

            if (typeof(byte?) == type)
            {
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    return byte.Parse(cellValue);
                }
                return null;
            }

            if (typeof(short) == type)
            {
                return short.Parse(cellValue);
            }

            if (typeof(short?) == type)
            {
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    return short.Parse(cellValue);
                }
                return null;
            }


            if (typeof(int) == type)
            {
                return int.Parse(cellValue);
            }

            if (typeof(int?) == type)
            {
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    return int.Parse(cellValue);
                }
                return null;
            }

            if (typeof(long) == type)
            {
                return long.Parse(cellValue);
            }

            if (typeof(long?) == type)
            {
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    return long.Parse(cellValue);
                }
                return null;
            }


            if (typeof(float) == type)
            {
                return float.Parse(cellValue);
            }

            if (typeof(float?) == type)
            {
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    return float.Parse(cellValue);
                }
                return null;
            }

            if (typeof(double) == type)
            {
                return double.Parse(cellValue);
            }

            if (typeof(double?) == type)
            {
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    return double.Parse(cellValue);
                }
                return null;
            }

            if (typeof(decimal) == type)
            {
                return decimal.Parse(cellValue);
            }

            if (typeof(decimal?) == type)
            {
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    return decimal.Parse(cellValue);
                }
                return null;
            }

            if (typeof(DateTime) == type)
            {
                return DateTime.Parse(cellValue);
            }

            if (typeof(DateTime?) == type)
            {
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    return DateTime.Parse(cellValue);
                }
                return null;
            }

            if (typeof(bool) == type)
            {
                return bool.Parse(cellValue);
            }


            if (typeof(bool?) == type)
            {
                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    return bool.Parse(cellValue);
                }
                return null;
            }

            return null;

            #endregion
        }

    }

    public class ColumnInfo
    {
        public int ColumnIndex { get; set; }
        public string TitleName { get; set; }
        public string ColumnPropertyName { get; set; }

    }
}
