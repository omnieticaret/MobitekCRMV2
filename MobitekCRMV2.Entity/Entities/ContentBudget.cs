namespace MobitekCRMV2.Entity.Entities
{
    public class ContentBudget : EntityBase<string>
    {

        public string Name { get; set; }
        public int Price { get; set; }
        public string SelectedDate { get; set; }

        #region Navigation Properties
        public string ProjectId { get; set; }
        virtual public Project Project { get; set; }
        #endregion

    }
}
