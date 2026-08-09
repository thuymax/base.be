using NPOI.XSSF.UserModel;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Xml;
using NPOI.SS.UserModel;
using System.Data;
using NPOI.HSSF.Record.Chart;
//using ExtendedNumerics.Reflection;
using SEEDONE.SERVICE.Attributes;

namespace SEEDONE.SERVICE.Helpers
{
    public class ExportLib<T>
    {
        public static Stream ToExcel(List<T> items, List<string> cols)
        {
            Type t = typeof(T);
            var props = t.GetProperties();

            // create a workbook
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet(typeof(T).Name);
            var titleRow = sheet.CreateRow(0);

            // create font
            IFont font = workbook.CreateFont();
            font.FontName = "Arial";
            font.FontHeightInPoints = 11;

            List<int> dateColumn = new List<int>();

            IRow detailHeaderRow = null;
            var headerRowCount = props.Any(p => typeof(IEnumerable).IsAssignableFrom(p.PropertyType) && p.PropertyType != typeof(string)) ? 2 : 1;

            if (headerRowCount == 2)
            {
                detailHeaderRow = sheet.CreateRow(1);
            }

            int childColHeaderCount = 0;

            // để sinh header
            for (var i = 0; i < cols.Count; i++)
            {
                var firstEle = items[0];
                var prop = props.FirstOrDefault(p => String.Equals(p.Name, cols[i], StringComparison.OrdinalIgnoreCase));
                
                // bỏ qua nếu là cột có attribute là ignoreExport
                if (prop.GetCustomAttribute<IgnoreExportAttribute>() != null)
                {
                    continue;
                }
                var title = prop.GetDisplayName();
                if (prop.PropertyType == typeof(DateTime?))
                {
                    dateColumn.Add(i);
                }

                // xử lý cột có nhiều cột con
                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    var propType = TypeHelpers.GetAnyElementType(prop.PropertyType);
                    var childPropList = propType.GetProperties().Select(x => x.Name).ToList();
                    if (childPropList.Count > 0)
                    {
                        var propValue = (ICollection)prop.GetValue(firstEle) ?? new object[] { };
                        var childPropInListCount = 0;
                        for (int ci = 0; ci < propValue.Count; ci++)
                        {
                            for (int cpi = 0; cpi < childPropList.Count; cpi++)
                            {
                                titleRow.CreateCell(i + childColHeaderCount + childPropInListCount + cpi);
                                detailHeaderRow.CreateCell(i + childColHeaderCount + childPropInListCount + cpi).SetCellValue(childPropList[cpi]);
                            }
                            var cra = new NPOI.SS.Util.CellRangeAddress(0, 0, i + childColHeaderCount + childPropInListCount, i + childColHeaderCount + childPropInListCount + childPropList.Count - 1);
                            sheet.AddMergedRegion(cra);

                            var mergedHeaderCell = sheet.GetRow(0).GetCell(i + childColHeaderCount + childPropInListCount);
                            mergedHeaderCell.SetCellValue(title + " #" + (ci + 1));
                            childPropInListCount += childPropList.Count;
                        }
                        childColHeaderCount += childPropInListCount;
                        continue;
                    }
                    else
                    {
                        var propValue = (IList)prop.GetValue(firstEle);

                        for (int ci = 0; ci < propValue.Count; ci++)
                        {
                            titleRow.CreateCell(i + childColHeaderCount + ci);
                            detailHeaderRow.CreateCell(i + childColHeaderCount + ci).SetCellValue(ci + 1);
                        }
                        var cra = new NPOI.SS.Util.CellRangeAddress(0, 0, i + childColHeaderCount, i + propValue.Count + childColHeaderCount - 1);
                        sheet.AddMergedRegion(cra);

                        var mergedHeaderCell = sheet.GetRow(0).GetCell(i + childColHeaderCount);
                        mergedHeaderCell.SetCellValue(title);
                        childColHeaderCount += propValue.Count - 1;

                        continue;
                    }
                }


                IFont fontTitle = workbook.CreateFont();
                fontTitle.FontName = "Arial";
                fontTitle.FontHeightInPoints = 11;
                fontTitle.Color = IndexedColors.Black.Index;


                var titleCell = titleRow.CreateCell(i + childColHeaderCount);
                ICellStyle style = workbook.CreateCellStyle();
                style.WrapText = true;
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                // font.IsBold = true;

                style.SetFont(fontTitle);
                style.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                style.FillPattern = FillPattern.SolidForeground;
                titleCell.SetCellValue(title);
                titleCell.CellStyle = style;

            }

            // để sinh các cell
            for (var i = 0; i < items.Count; i++)
            {
                var e = items[i];
                var dataRow = sheet.CreateRow(i + headerRowCount);
                int childColCount = 0;

                for (var j = 0; j < cols.Count; j++)
                {
                    var nt = cols[j];
                    var ntWithoutSpace = nt.Replace(" ", "");
                    var prop = props.FirstOrDefault(p => String.Equals(p.Name, ntWithoutSpace, StringComparison.OrdinalIgnoreCase));
                    if (prop != null)
                    {
                        var v = prop.GetValue(e);
                        if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                        {
                            var childPropList = TypeHelpers.GetAnyElementType(prop.PropertyType).GetProperties().Where(p => !p.GetIndexParameters().Any()).Select(x => x.Name).ToList();
                            if (childPropList.Count > 0)
                            {
                                var propValue = v == null ? new List<object> { } : ((IEnumerable)v).Cast<object>().ToList();

                                var childPropInListCount = 0;
                                for (int ci = 0; ci < propValue.Count; ci++)
                                {
                                    var properties = propValue[ci].GetType().GetProperties().Where(p => !p.GetIndexParameters().Any()).ToList();
                                    for (int cpi = 0; cpi < childPropList.Count; cpi++)
                                    {
                                        var currentValue = properties[cpi].GetValue(propValue[ci]);
                                        var type = properties[cpi].Name;
                                        var cellValue = currentValue == null ? null : currentValue.ToString();
                                        dataRow.CreateCell(j + childColCount + childPropInListCount + cpi).SetCellValue(cellValue);
                                    }
                                    childPropInListCount += childPropList.Count;
                                }
                                childColCount += childPropInListCount;
                                continue;
                            }
                            else
                            {
                                var propValue = (IList)v;
                                if (propValue != null)
                                {
                                    for (int ci = 0; ci < propValue.Count; ci++)
                                    {
                                        var cellValue = propValue[ci] == null ? null : propValue[ci].ToString();
                                        dataRow.CreateCell(j + childColCount + ci).SetCellValue(cellValue);
                                    }
                                    childColCount += propValue.Count - 1;
                                }
                                continue;
                            }
                        }

                        ICell cellData = dataRow.CreateCell(j + childColCount);

                        // custom style for cells
                        CustomStyleCell(cellData, workbook, prop, v, dataRow);
                    }
                }
            }

            // auto size date column
            foreach (int i in dateColumn)
            {
                sheet.SetColumnWidth(i, 12 * 256);
            }

            var stream = new MemoryStream();
            {
                workbook.Write(stream, true);
                stream.Flush();
                stream.Position = 0;
            }
            return stream;
        }
        public static MemoryStream ToTemplateExcel(List<T> items, List<string> cols, string filePath)
        {
            string templateFilePath = AppDomain.CurrentDomain.BaseDirectory + filePath;
            //string outputFilePath = "Output.xlsx";


            using (FileStream templateFile = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read))
            {
                Type t = typeof(T);
                var props = t.GetProperties();
                var workbook = new XSSFWorkbook(templateFile);

                // list lưu trữ các style của các cột
                List<ICellStyle> listCellStyle = new List<ICellStyle>();

                // lấy ra
                ISheet templateSheet = workbook.GetSheetAt(0);
                IRow templateRow = templateSheet.GetRow(1);

                for (int k = 0; k < cols.Count; k++)
                {
                    var cellStyle = workbook.CreateCellStyle();
                    ICell templateCell = templateRow.GetCell(k);

                    // clone style từ template ra cho cellStyle
                    cellStyle.CloneStyleFrom(templateCell.CellStyle);
                    listCellStyle.Add(cellStyle);
                }

                ISheet sheet = workbook.GetSheetAt(0);
                for (var i = 0; i < items.Count; i++)
                {
                    var e = items[i];
                    var dataRow = sheet.CreateRow(i + 1);
                    int childColCount = 0;

                    for (var j = 0; j < cols.Count; j++)
                    {

                        var nt = cols[j];
                        var ntWithoutSpace = nt.Replace(" ", "");
                        var prop = props.FirstOrDefault(p => String.Equals(p.Name, ntWithoutSpace, StringComparison.OrdinalIgnoreCase));
                        if (prop != null)
                        {
                            var v = prop.GetValue(e);
                            if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                            {
                                var childPropList = TypeHelpers.GetAnyElementType(prop.PropertyType).GetProperties().Where(p => !p.GetIndexParameters().Any()).Select(x => x.Name).ToList();
                                if (childPropList.Count > 0)
                                {
                                    var propValue = v == null ? new List<object> { } : ((IEnumerable)v).Cast<object>().ToList();

                                    var childPropInListCount = 0;
                                    for (int ci = 0; ci < propValue.Count; ci++)
                                    {
                                        var properties = propValue[ci].GetType().GetProperties().Where(p => !p.GetIndexParameters().Any()).ToList();
                                        for (int cpi = 0; cpi < childPropList.Count; cpi++)
                                        {
                                            var currentValue = properties[cpi].GetValue(propValue[ci]);
                                            var type = properties[cpi].Name;
                                            var cellValue = currentValue == null ? null : currentValue.ToString();
                                            dataRow.CreateCell(j + childColCount + childPropInListCount + cpi).SetCellValue(cellValue);
                                        }
                                        childPropInListCount += childPropList.Count;
                                    }
                                    childColCount += childPropInListCount;
                                    continue;
                                }
                                else
                                {
                                    var propValue = (IList)v;
                                    if (propValue != null)
                                    {
                                        for (int ci = 0; ci < propValue.Count; ci++)
                                        {
                                            var cellValue = propValue[ci] == null ? null : propValue[ci].ToString();
                                            dataRow.CreateCell(j + childColCount + ci).SetCellValue(cellValue);
                                        }
                                        childColCount += propValue.Count - 1;
                                    }
                                    continue;
                                }
                            }

                            ICell cellData = dataRow.CreateCell(j + childColCount);
                            double doubleValue;
                            DateTime dateTimeValue;


                            if ((prop.PropertyType == typeof(DateTime?) || prop.PropertyType == typeof(DateTime)) && v != null)
                            {

                                var a = (DateTime)v;
                                if (a.Year < 1000)
                                {
                                    cellData.SetCellValue(a.ToShortDateString());
                                }
                                else
                                {
                                    cellData.SetCellValue(a);
                                }
                                //DateOnly dateOnlydata = new DateOnly(a.Year, a.Month, a.Day);

                                //ICellStyle dateStyle = workbook.CreateCellStyle();
                                //dateStyle.DataFormat = 14;

                                //cellData.CellStyle = dateStyle;

                            }
                            else if (prop.PropertyType == typeof(string) && v != null)
                            {
                                //ICellStyle dateStyle = workbook.CreateCellStyle();
                                //dateStyle.DataFormat = 49;

                                //cellData.CellStyle = dateStyle;
                                cellData.SetCellValue(v.ToString());
                            }
                            else if (prop.PropertyType == typeof(double) && v != null)
                            {
                                cellData.SetCellValue((double)v);
                            }
                            else if ((prop.PropertyType == typeof(int)) && v != null)
                            {
                                cellData.SetCellValue((int)v);
                            }
                            else if (prop.PropertyType == typeof(long) && v != null)
                            {
                                cellData.SetCellValue((long)v);
                            }
                            else
                            {
                                cellData.SetCellValue(v == null ? "" : v.ToString());
                            }
                            cellData.CellStyle = listCellStyle[j];
                        }
                    }
                }

                var stream = new MemoryStream();
                {
                    workbook.Write(stream, true);
                    stream.Flush();
                    stream.Position = 0;
                }
                return stream;
            }
        }

        public static void FillData(ISheet sheet)
        {
            IRow sampleRow = sheet.GetRow(1);
            for (int i = 0; i < 10; i++)
            {
                IRow newRow = sheet.CreateRow(i + 1);
                for (int j = 0; j < sampleRow.LastCellNum; j++)
                {
                    ICell cell = newRow.CreateCell(j);
                    cell.SetCellValue($"Data {i} - {j}");
                }
            }
        }

        public static Stream ToExcelXml(List<T> invoiceItems, List<string> cols)
        {
            var document = new XmlDocument();
            document.AppendChild(document.CreateXmlDeclaration("1.0", null, null));

            var workbook = document.CreateElement("Workbook");
            workbook.SetAttribute("xmlns:o", "urn:schemas-microsoft-com:office:office");
            workbook.SetAttribute("xmlns:x", "urn:schemas-microsoft-com:office:excel");
            workbook.SetAttribute("xmlns:ss", "urn:schemas-microsoft-com:office:spreadsheet");
            workbook.SetAttribute("xmlns:html", "http://www.w3.org/TR/REC-html40");
            workbook.SetAttribute("xmlns", "urn:schemas-microsoft-com:office:spreadsheet");
            document.AppendChild(workbook);

            var worksheet = document.CreateElement("Worksheet");
            var sheetName = document.CreateAttribute("ss", "Name", "urn:schemas-microsoft-com:office:spreadsheet");
            sheetName.Value = "Sheet1";
            worksheet.Attributes.Append(sheetName);
            workbook.AppendChild(worksheet);

            var table = document.CreateElement("Table");
            worksheet.AppendChild(table);

            var titleRow = document.CreateElement("Row");

            Type t = typeof(T);
            var props = t.GetProperties();

            foreach (var c in cols)
            {
                var prop = props.FirstOrDefault(p => String.Equals(p.Name, c, StringComparison.OrdinalIgnoreCase));
                var title = prop.Name;

                var cell = document.CreateElement("Cell");
                var data = document.CreateElement("Data");
                var typeAttr = document.CreateAttribute("ss", "Type", "urn:schemas-microsoft-com:office:spreadsheet");
                typeAttr.Value = "String";
                data.Attributes.Append(typeAttr);
                var nodeValue = document.CreateNode(XmlNodeType.CDATA, "", "");
                nodeValue.Value = title;
                data.AppendChild(nodeValue);
                cell.AppendChild(data);
                titleRow.AppendChild(cell);
            }
            table.AppendChild(titleRow);

            foreach (var e in invoiceItems)
            {
                var dataRow = document.CreateElement("Row");

                foreach (var c in cols)
                {
                    var prop = props.FirstOrDefault(p => String.Equals(p.Name, c, StringComparison.OrdinalIgnoreCase));
                    if (prop != null)
                    {
                        var v = prop.GetValue(e);
                        var cell = document.CreateElement("Cell");
                        var data = document.CreateElement("Data");
                        var typeAttr = document.CreateAttribute("ss", "Type", "urn:schemas-microsoft-com:office:spreadsheet");
                        typeAttr.Value = "String";
                        data.Attributes.Append(typeAttr);
                        var nodeValue = document.CreateNode(XmlNodeType.CDATA, "", "");
                        nodeValue.Value = v?.ToString();
                        data.AppendChild(nodeValue);
                        cell.AppendChild(data);
                        dataRow.AppendChild(cell);
                    }
                }
                table.AppendChild(dataRow);
            }

            var stream = new MemoryStream();
            {
                document.Save(stream);
                stream.Flush();
                stream.Position = 0;
                return stream;
            }
        }

        public static Stream ToCsv(List<T> invoices, List<string> cols)
        {
            Type t = typeof(T);
            var props = t.GetProperties();

            var sb = new StringBuilder();
            var titles = cols.Select(c => props.FirstOrDefault(p => String.Equals(p.Name, c, StringComparison.OrdinalIgnoreCase)).Name).ToList();
            sb.AppendLine(string.Join(",", titles));

            for (var i = 0; i < invoices.Count; i++)
            {
                var e = invoices[i];
                var dataRow = new string[titles.Count];

                for (var j = 0; j < cols.Count; j++)
                {
                    var nt = cols[j];
                    var prop = props.FirstOrDefault(p => String.Equals(p.Name, nt, StringComparison.OrdinalIgnoreCase));
                    if (prop != null)
                    {
                        var v = prop.GetValue(e);
                        dataRow[j] = EscapeCsvCell(v == null ? "" : v.ToString());
                    }
                }
                sb.AppendLine(string.Join(",", dataRow));
            }

            var csvString = sb.ToString();
            return new MemoryStream(Encoding.UTF8.GetBytes(csvString));
        }

        public static string EscapeCsvCell(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\r") || value.Contains("\n"))
            {
                value.Replace("\"", "\"\"");
                value = $"\"{value}\"";
            }

            return value;
        }

        private static void CustomStyleCell(ICell cellData, XSSFWorkbook workbook, PropertyInfo prop, object v, IRow dataRow)
        {
            // create font
            IFont font = workbook.CreateFont();
            font.FontName = "Arial";
            font.FontHeightInPoints = 11;
            ICellStyle style = workbook.CreateCellStyle();
            font.IsBold = false;


            // setvalue
            if ((prop.PropertyType == typeof(double) || prop.PropertyType == typeof(decimal)) && v != null)
            {

                style.DataFormat = 44; // kiểu tiền tệ ( ví dụ value = 20000,3333 --> $ 20,000.33)
                cellData.SetCellValue((double)v);
            }
            else if ((prop.PropertyType == typeof(DateTime?) || prop.PropertyType == typeof(DateTime)) && v != null)
            {
                var a = (DateTime)v;
                //DateOnly dateOnlydata = new DateOnly(a.Year, a.Month, a.Day);

                style.DataFormat = 14; // định dạng kiểu datetime trong excel

                //cellData.CellStyle = style;
                cellData.SetCellValue(a);
            }
            else if (prop.PropertyType == typeof(int) && v != null)
            {

                cellData.SetCellValue((int)v);
                style.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");
            }
            else if (prop.PropertyType == typeof(string) && v != null)
            {
                style.DataFormat = 49;

                //cellData.CellStyle = style;
                cellData.SetCellValue(v.ToString());

            }
            else
            {
                cellData.SetCellValue(v == null ? "" : v.ToString());
            }

            style.SetFont(font);
            cellData.CellStyle = style;
        }
    }
}
