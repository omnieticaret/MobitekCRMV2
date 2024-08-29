using MobitekCRMV2.DataAccess.Context;

namespace MobitekCRMV2.Business.Services
{
    public class BacklinksService
    {
        private readonly CRMDbContext _context;

        public BacklinksService(CRMDbContext context)
        {
            _context = context;
        }


        public string FixSelectDate(string SelectDate)
        {
            var fixedDate = "";
            try
            {
                var month = SelectDate.Split(".")[1];
                var year = SelectDate.Split(".")[2].Split(" ")[0];
                fixedDate = month + "-" + year;
            }
            catch (Exception ex) { }

            return fixedDate;
        }

    }
}
