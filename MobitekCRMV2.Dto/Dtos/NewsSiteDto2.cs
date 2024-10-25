using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos
{
    public class NewsSiteDto2
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int OrganicTraffic { get; set; }
        public string UserName { get; set; }
        public int Pa { get; set; }
        public int Da { get; set; }

        public string EditorMail { get; set; }
        public string PostData { get; set; }

        public string Sr_traffic { get; set; }
        public string Sr_kwords { get; set; }
    }
}
