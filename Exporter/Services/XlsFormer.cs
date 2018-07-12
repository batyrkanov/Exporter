using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

using Exporter.Models;
using Exporter.Models.Interfaces;
using System.Data.Common;
using System.Reflection;

namespace Exporter.Services
{
    public class XlsFormer : FileFormer, IFormer
    {
        IUnitOfWork unitOfWork;
        int queryId;
        List<string> parameters = null;
        Dictionary<string, string> queryParameters = null;
        HttpPostedFileBase file;

        public XlsFormer(IUnitOfWork unitOfWork, string queryId, string[] parameters = null, HttpPostedFileBase file = null)
        {
            this.unitOfWork = unitOfWork;
            this.queryId = int.Parse(queryId);
            if (parameters != null)
                this.parameters = parameters.ToList();
            this.file = file;
        }

        public void FormDocument()
        {
            string fullQuery = unitOfWork
                .SqlQueries
                .Get(queryId)
                .SqlQueryContent;

            this.queryParameters = FormQueryParametersDictionary(this.parameters);
            List<string> queries = this.SplitQueryIntoPieces(fullQuery);

            this.CreateFile("xls");

            if (this.file != null && this.file.ContentLength > 0)
                this.file.SaveAs(this.FilePath);
            else
                this.CreateXlsFile();

            ServerDbContext server;
            DbCommand command;
            DbDataReader reader;

            Excel.Application xls;
            Excel.Workbook workbook;
            Excel.Worksheet worksheet;
            int? row = null;
            int col;

            using (server = new ServerDbContext())
            {
                foreach (string query in queries)
                {
                    using (command = server.Database.Connection.CreateCommand())
                    {
                        server.Database.Connection.Open();

                        this.AddParametersToQuery(ref command, query, this.queryParameters);

                        command.CommandText = query;

                        using (reader = command.ExecuteReader())
                        {
                            xls = new Excel.Application();
                            workbook = xls.Workbooks.Open(this.FilePath);
                            worksheet = (Excel.Worksheet)workbook.Worksheets[1];
                            
                            if (row == null)
                            {
                                object misValue = Missing.Value;
                                row = 0;
                                Excel.Range count = worksheet.UsedRange.Columns[1, misValue] as Excel.Range;

                                foreach (Excel.Range cell in count.Cells)
                                    row++;
                            }

                            while(reader.Read())
                            {
                                row++;
                                col = 1;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string value = reader[i].ToString();
                                    worksheet.Cells[row, col] = value;
                                    col++;
                                }
                            }

                            workbook.Save();
                            workbook.Close();
                            xls.Quit();

                            Marshal.ReleaseComObject(worksheet);
                            Marshal.ReleaseComObject(workbook);
                            Marshal.ReleaseComObject(xls);
                        }
                        server.Database.Connection.Close();
                    }
                }
            }
        }

        private void CreateXlsFile()
        {
            Excel.Application app = new Excel.Application()
            {
                Visible = false,
                DisplayAlerts = false
            };

            Excel.Workbook book = app.Workbooks.Add(Type.Missing);
            Excel.Worksheet sheet = (Excel.Worksheet)book.ActiveSheet;
            sheet.Name = "Лист 1";
            sheet.Cells.Font.Size = 12;

            book.SaveAs(this.FilePath);
            book.Close(true, Type.Missing, Type.Missing);
            app.Quit();

            Marshal.ReleaseComObject(sheet);
            Marshal.ReleaseComObject(book);
            Marshal.ReleaseComObject(app);
        }
    }
}