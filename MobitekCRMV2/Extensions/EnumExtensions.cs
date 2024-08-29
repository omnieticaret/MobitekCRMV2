using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MobitekCRMV2.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Bu extension enum tipindeki değişkenlerin display değerlerini çekmek için bir fonksiyon ekler.
        /// Örneğin Enums.ProjectType.Seo.GetDisplayName() şeklinde çağrıldığında, 
        /// "Seo Projesi" return edecektir. Bu UI kısmında işe yarayacaktır.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
              .GetMember(enumValue.ToString())
              .First()
              .GetCustomAttribute<DisplayAttribute>()
              ?.GetName();
        }
    }
}
