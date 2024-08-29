using ClosedXML.Excel;
using MobitekCRMV2.Entity.Entities;


namespace MobitekCRMV2.Business.Services
{
    public class KeywordsService
    {
        public byte[] ProjectKeywordsExportExcel(List<Keyword> keywords)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Proje Kelime Listesi");

                int RowCount = 1;
                foreach (var item in keywords)
                {
                    worksheet.Cell(RowCount, 1).Value = item.KeywordName;
                    worksheet.Cell(RowCount, 2).Value = item.TargetURL;
                    RowCount++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }

            }
        }



    }
}
