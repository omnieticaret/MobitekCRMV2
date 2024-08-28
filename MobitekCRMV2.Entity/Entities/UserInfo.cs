namespace MobitekCRMV2.Entity.Entities
{
    public class UserInfo : EntityBase<string>
    {



        public string Iban { get; set; }

        public string BankName { get; set; }

        public string Tc { get; set; }

        public decimal Salary { get; set; }
        public bool Kdv { get; set; }



        #region Navigation Properties

        public string UserId { get; set; }
        virtual public User User { get; set; }

        #endregion
    }
}
