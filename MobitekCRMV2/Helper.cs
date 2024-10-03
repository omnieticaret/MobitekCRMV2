using System;
using System.Collections.Generic;

namespace MobitekCRMV2
{
    public class Helper
    {
        #region Url
        public static string GetAuthoritativeUrl(string url)
        {

            if (!(url.Substring(0, 4) == "http"))
            {
                url = "https://" + url;
            }
            Uri myUri = new Uri(url);
            string host = myUri.Host;  // host is "www.contoso.com"
            if (host.Substring(0, 3) == "www")
            {
                host = host.Substring(4, host.Length - 4);
            }
            return host;
        }
        #endregion

        #region Date Conversion
        public static DateTime MonthAndYearToDate(string url)
        {
            var words = url.Split(' ');
            var month = "";
            var year = "";
            month = words[0];
            year = words[1];
            int yearAsInt = short.Parse(year);
            DateTime date = new DateTime();
            date = date.AddMonths(TurkishMonthToNumber(month) - 1);
            date = date.AddYears(yearAsInt - 1);
            return date;
        }


        public static int TurkishMonthToNumber(string month)
        {
            var months = new Dictionary<string, int>(){
                {"Ocak", 1},
                {"Şubat", 2},
                {"Mart", 3},
                {"Nisan", 4},
                {"Mayıs", 5},
                {"Haziran", 6},
                {"Temmuz", 7},
                {"Ağustos", 8},
                {"Eylül", 9},
                {"Ekim", 10},
                {"Kasım", 11},
                {"Aralık", 12}
            };
            int intMonth = months[month];
            return intMonth;
        }
        public static string NumberToTurkishMonth(int month)
        {
            var months = new Dictionary<int, string>(){
                {1, "Ocak"},
                {2, "Şubat"},
                {3, "Mart"},
                {4, "Nisan"},
                {5, "Mayıs"},
                {6, "Haziran"},
                {7, "Temmuz"},
                {8, "Ağustos"},
                {9, "Eylül"},
                {10, "Ekim"},
                {11, "Kasım"},
                {12, "Aralık"}
            };
            string stringMonth = months[month];
            return stringMonth;
        }
        public static string DateToTurkishString(DateTime date)
        {
            int month = date.Month;
            int year = date.Year;
            string monthAsString = NumberToTurkishMonth(month);
            string yearAsString = year.ToString();
            return monthAsString + " " + yearAsString;
        }
        #endregion

    }
}
