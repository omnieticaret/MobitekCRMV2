namespace MobitekCRMV2.Entity.Entities
{
    public class TemplateItem : EntityBase<string>
    {
        public string Title { get; set; }
        public string Desc { get; set; }

        public int TodoRange { get; set; }

        public string TodoGroupId { get; set; }
        public TodoGroup TodoGroup { get; set; }
    }
}
