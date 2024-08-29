using Microsoft.EntityFrameworkCore;
using Mobitek.CRM.Models.NewsSiteModels;
using MobitekCRMV2.DataAccess.Context;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using System.Net.Http.Json;

namespace MobitekCRMV2.Business.Services
{
    public class NewsSitesService
    {
        private readonly CRMDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public NewsSitesService(CRMDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }


        // Initializes a list to store filtering criteria, capturing minimum and maximum values for specific properties of NewsSite entities.
        public async Task<List<NewsSite>> GetFilteredNewsSites(NewsSiteViewFilter model)
        {
            IQueryable<NewsSite> newsSites = _context.NewsSites.AsNoTracking().Include(x => x.User);

            if (model.PriceMin != null || model.PriceMax != null)
            {
                if (model.PriceMin == null) model.PriceMin = "0";
                if (model.PriceMax == null) model.PriceMax = _context.NewsSites.Max(ns => ns.Price).ToString();
                newsSites = newsSites.Where(x => x.Price >= StringToInt(model.PriceMin) && x.Price <= StringToInt(model.PriceMax));
            }
            if (model.PaMin != null || model.PaMax != null)
            {
                if (model.PaMin == null) model.PaMin = "0";
                if (model.PaMax == null) model.PaMax = _context.NewsSites.Max(ns => ns.PAScore).ToString();
                newsSites = newsSites.Where(x => x.PAScore >= StringToInt(model.PaMin) && x.PAScore <= StringToInt(model.PaMax));
            }
            if (model.DaMin != null || model.DaMax != null)
            {
                if (model.DaMin == null) model.DaMin = "0";
                if (model.DaMax == null) model.DaMax = _context.NewsSites.Max(ns => ns.DAScore).ToString();
                newsSites = newsSites.Where(x => x.DAScore >= StringToInt(model.DaMin) && x.DAScore <= StringToInt(model.DaMax));
            }
            if (model.DRMin != null || model.DRMax != null)
            {
                if (model.DRMin == null) model.DRMin = "0";
                if (model.DRMax == null) model.DRMax = _context.NewsSites.Max(ns => ns.DRScore).ToString();
                newsSites = newsSites.Where(x => x.DRScore >= StringToInt(model.DRMin) && x.DRScore <= StringToInt(model.DRMax));
            }
            if (model.LDomainMin != null || model.LDomainMax != null)
            {
                if (model.LDomainMin == null) model.LDomainMin = "0";
                if (model.LDomainMax == null) model.LDomainMax = _context.NewsSites.Max(ns => ns.LinkedDomains).ToString();
                newsSites = newsSites.Where(x => x.LinkedDomains >= StringToInt(model.LDomainMin) && x.LinkedDomains <= StringToInt(model.LDomainMax));
            }
            if (model.BacklinksMin != null || model.BacklinksMax != null)
            {
                if (model.BacklinksMin == null) model.BacklinksMin = "0";
                if (model.BacklinksMax == null) model.BacklinksMax = _context.NewsSites.Max(ns => ns.TotalBacklinks).ToString();
                newsSites = newsSites.Where(x => x.TotalBacklinks >= StringToInt(model.BacklinksMin) && x.TotalBacklinks <= StringToInt(model.BacklinksMax));
            }
            if (model.OTrafficMin != null || model.OTrafficMax != null)
            {
                if (model.OTrafficMin == null) model.OTrafficMin = "0";
                if (model.OTrafficMax == null) model.OTrafficMax = _context.NewsSites.Max(ns => ns.OrganicTraffic).ToString();
                newsSites = newsSites.Where(x => x.OrganicTraffic >= StringToInt(model.OTrafficMin) && x.OrganicTraffic <= StringToInt(model.OTrafficMax));
            }
            if (model.AllTrafficMin != null || model.AllTrafficMax != null)
            {
                if (model.AllTrafficMin == null) model.AllTrafficMin = "0";
                if (model.AllTrafficMax == null) model.AllTrafficMax = _context.NewsSites.Max(ns => ns.AllTraffic).ToString();
                newsSites = newsSites.Where(x => x.AllTraffic >= StringToInt(model.AllTrafficMin) && x.AllTraffic <= StringToInt(model.AllTrafficMax));
            }
            if (model.SScoreMin != null || model.SScoreMax != null)
            {
                if (model.SScoreMin == null) model.SScoreMin = "0";
                if (model.SScoreMax == null) model.SScoreMax = _context.NewsSites.Max(ns => ns.SpamScore).ToString();
                newsSites = newsSites.Where(x => x.SpamScore >= StringToInt(model.SScoreMin) && x.SpamScore <= StringToInt(model.SScoreMax));
            }
            if (model.NewRecords != null)
            {
                newsSites = newsSites.Where(x => x.CreatedAt > DateTime.Now.AddDays(-30));
            }
            return await newsSites.ToListAsync();
        }


        public int StringToInt(string str)
        {
            var isInt = int.TryParse(str.Trim(), out int value);
            if (isInt)
                return value;
            else
                return -1;
        }


        public int ParseInt(string value)
        {
            if (int.TryParse(value.Trim(), out int result))
            {
                return result;
            }
            throw new FormatException("Invalid integer format");
        }


        public string GetOldDataValue(NewsSite newsSite, User user, int newPrice, string newMail)
        {

            if (newsSite.Price != newPrice || user.Id != newsSite.UserId)
            {
                return $"{newsSite.Price} - {newsSite.User?.Email} - {DateTime.Now.ToString("dd/MM/yyyy")}";

            }
            else
            {
                if (newsSite.OldData != null)
                {
                    return newsSite.OldData;
                }
                else
                {
                    return "";
                }
            }


            return "";

        }


        public User CreateUserFromMailAddress(string mail)
        {

            var exist = _context.Users.Where(x => x.Email.Equals(mail)).FirstOrDefault();
            if (exist != null)
                return null;


            var username = "";
            var user = new User();
            if (mail.Contains('@'))
                username = mail.Split('@')[0];

            user.Email = mail;
            user.UserName = username;
            user.UserType = UserType.Editor;
            _context.Users.Add(user);
            _context.SaveChanges();

            var returnUser = _context.Users.Where(x => x.Email == mail).FirstOrDefault();

            return returnUser;
        }


        public SeoResult GetApiData(string url)
        {

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var fullUrl = "https://seo-rank.my-addr.com/api2/moz+sr+fb/3B04AAF5510D6958D62677D1735E37F6/" + url;

                var json = httpClient.GetFromJsonAsync<SeoResult>(fullUrl).Result;

                if (json == null || json.da == 1 && json.pa == 1)
                {
                    return new SeoResult();
                }
                else
                {
                    json.name = url;
                    return json;
                }
            }

        }

    }
}
