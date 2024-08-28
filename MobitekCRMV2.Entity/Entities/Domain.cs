using System.Collections.Generic;
using MobitekCRMV2.Entity.Entities;

namespace MobitekCRMV2.Entity.Entities
{
    public class Domain : EntityBase<string>
    {

        public string Name { get; set; }


        #region Navigation Properties
        virtual public List<BackLink> BackLinks { get; set; }

        virtual public Project Project { get; set; }
        #endregion
    }
}
