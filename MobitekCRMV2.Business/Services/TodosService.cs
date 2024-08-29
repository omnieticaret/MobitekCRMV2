using MobitekCRMV2.DataAccess.Context;

namespace MobitekCRMV2.Business.Services
{
    public class TodosService
    {
        private readonly CRMDbContext _context;

        public TodosService(CRMDbContext context)
        {
            _context = context;
        }




        public string GetDescIdentity(string content, string username)
        {
            if (string.IsNullOrEmpty(content))
            {
                return "";
            }
            var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].Contains("@"))
                {

                    if (lines[i] != "" && i == lines.Length - 1)
                    {
                        lines[i] += " @" + username + $" ({DateTime.Now.ToString("dd-MM-yyyy")})";
                    }

                }
            }
            return string.Join(Environment.NewLine, lines);
        }

        public string GetDescIdentityWithControl(string content, string username, string oldContent)
        {
            var startPoint = 0;
            if (string.IsNullOrEmpty(content))
            {
                return "";
            }
            if (!string.IsNullOrEmpty(oldContent))
            {
                startPoint = oldContent.Length;
            }

            if (startPoint > content.Length)
            {
                return "unauthorized";
            }

            content = content.Substring(startPoint);
            var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].Contains("@"))
                {

                    if (lines[i] != "" && i == lines.Length - 1)
                    {
                        lines[i] += " @" + username + $" ({DateTime.Now.ToString("dd-MM-yyyy")})";
                    }

                }
            }
            return string.Join(Environment.NewLine, lines);
        }

    }
}
