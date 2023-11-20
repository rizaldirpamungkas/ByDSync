using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ExcelDataReader;
using CsvHelper;
using System.Globalization;
using TaxIdnProject.Data.EFakturStructure;
using TaxIdnProject.Data;
using System.Net;
using System.Data;

namespace TaxIdnProject.Pages.UnggahEfaktur
{
    public class IndexModel : PageModel
    {
        private IWebHostEnvironment Environment;

        public void OnGet()
        {
        }

        public IndexModel(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }

        public async void OnPostUpload(IFormFile fileUpload)
        {
            string wwwPath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;
            
            string path = Path.Combine(this.Environment.WebRootPath, "Uploads");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            string fileName = Path.GetFileName(fileUpload.FileName);

            CookieContainer cookieContainer = new CookieContainer();

            string? token = ByDSync.OnGetSrfToken(cookieContainer);
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("parentObjectId");
            dataTable.Columns.Add("nomorFaktur");

            using (Stream stream = fileUpload.OpenReadStream())
            {
                StreamReader reader = null;

                try
                {
                    reader = new StreamReader(stream);
                    int index = 0;
                    string? csvLine = reader.ReadLine();

                    while (csvLine != null)
                    {
                        string[] csvColumn = csvLine.Split(',');

                        if (index > 2)
                        {
                            if (csvColumn.First() == "FK")
                            {
                                dataTable.Rows.Add();

                                string parentObjectId = csvColumn.Last();
                                string nomorFaktur = csvColumn[3];

                                dataTable.Rows[dataTable.Rows.Count - 1][0] = parentObjectId;
                                dataTable.Rows[dataTable.Rows.Count - 1][1] = nomorFaktur;
                                
                                //await ByDSync.insertFakturNumber(parentObjectId, nomorFaktur, token, cookieContainer);
                            }
                        }

                        index++;

                        csvLine = reader.ReadLine();
                    }
                }
                catch { }
            }


            foreach(DataRow row in dataTable.Rows)
            {
                try
                {
                    await ByDSync.insertFakturNumber(row[0].ToString(), row[1].ToString(), token, cookieContainer);
                }
                catch
                {
                    continue;
                }
            }

                //LocalRedirect("/UnggahEfaktur");
        }
    }
}
