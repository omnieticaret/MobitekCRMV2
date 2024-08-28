using MobitekCRMV2.Entity.Enums;
using System.Collections.Generic;

namespace MobitekCRMV2.Entity.Entities
{
    public class TodoGroup : EntityBase<string>
    {

        public string Name { get; set; }
        public ProjectType ProjectType { get; set; }

        public Status Status { get; set; }

        public int TodoRange { get; set; }


        virtual public List<Todo> Todo { get; set; }
        virtual public List<TemplateItem> TemplateItems { get; set; }

    }
}
