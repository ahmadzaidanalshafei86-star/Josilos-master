using OfficeOpenXml;

namespace ES.Web.Areas.EsAdmin.Helpers
{
    public class ExcelExportHelper
    {
        // used in form responses
        public static async Task<byte[]> GenerateExcel(Form form, List<FormResponse> responses)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Responses");

            // Header Row
            worksheet.Cells[1, 1].Value = "Response ID";
            worksheet.Cells[1, 2].Value = "Submitted At";


            int colIndex = 3;
            var fields = form.Fields.ToList();

            foreach (var field in fields)
            {
                worksheet.Cells[1, colIndex++].Value = field.FieldName;
            }

            // Response Data
            int rowIndex = 2;
            foreach (var response in responses)
            {
                worksheet.Cells[rowIndex, 1].Value = response.Id;
                worksheet.Cells[rowIndex, 2].Value = response.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss");

                colIndex = 3;
                foreach (var field in fields)
                {
                    var detail = response.ResponseDetails.FirstOrDefault(rd => rd.FieldId == field.Id);
                    worksheet.Cells[rowIndex, colIndex++].Value = detail?.ResponseValue ?? "";
                }

                rowIndex++;
            }

            // Auto-fit columns for better readability
            worksheet.Cells.AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }

        //used in news letters
        public static async Task<byte[]> GenerateExcel(List<NewsLetter> newsletters, string emailHeader, string dateHeader)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("News-Letter-Subscriptions");

            // Header Row
            worksheet.Cells[1, 1].Value = emailHeader;
            worksheet.Cells[1, 2].Value = dateHeader;

            // Data Rows
            int rowIndex = 2;
            foreach (var newsletter in newsletters)
            {
                worksheet.Cells[rowIndex, 1].Value = newsletter.Email;
                worksheet.Cells[rowIndex, 2].Value = newsletter.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                rowIndex++;
            }

            // Auto-fit columns for better readability
            worksheet.Cells.AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }
    }
}
