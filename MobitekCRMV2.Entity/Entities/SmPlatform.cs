namespace MobitekCRMV2.Entity.Entities
{
    public class SmPlatform : EntityBase<string>
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public int PostCount { get; set; }
        public string Note { get; set; }


        #region Navigation Properties

        public string ProjectId { get; set; }
        public Project Project { get; set; }

        #endregion
    }
}
