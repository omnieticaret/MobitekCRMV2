using System.Collections.Generic;

namespace Mobitek.CRM.Entities
{
    /// <summary>
    /// Projelerin hangi platform üzerinde olduğu bilgisini tutan tablo
    /// Platform, bussiness katmanında altyapı olarak da anılıyor.
    /// </summary>
    public class Platform : EntityBase<string>
    {
        public string Name { get; set; }

        #region NavigationProperties
        public List<Project> Projects { get; set; }
        #endregion
    }
}
