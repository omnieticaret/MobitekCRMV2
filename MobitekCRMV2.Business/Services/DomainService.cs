using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Business.Services
{
    public class DomainService
    {
        private readonly IRepository<BackLink> _backLinkRepository;

        public DomainService(IRepository<BackLink> backLinkRepository)
        {
            _backLinkRepository = backLinkRepository;
        }

        public List<Domain> FilterDomainsByStatus(List<Domain> domains, string projectStatus)
        {
            if (Enum.TryParse<Status>(projectStatus, true, out var status))
            {
                return domains.Where(x => x.Project?.Status == status).ToList();
            }
            return domains.Where(x => x.Project?.Status == Status.Active).ToList();
        }

        public int GetDomainsBacklinkCount(string id)
        {
            return _backLinkRepository.Table.Count(x => x.DomainId == id && x.Status != "OK");
        }

        public int GetTotalBacklinkCount(string id)
        {
            return _backLinkRepository.Table.Count(x => x.DomainId == id);
        }
    }
}
