using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos
{
    public class DomainDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int BacklinkCount { get; set; }
        public int TotalBacklink { get; set; }
        public string ProjectStatus { get; set; }
    }
}
