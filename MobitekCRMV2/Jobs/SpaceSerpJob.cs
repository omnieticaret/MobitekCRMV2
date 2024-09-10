using Microsoft.EntityFrameworkCore;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using MobitekCRMV2.Model.Models;
using Newtonsoft.Json;
using Quartz;

namespace MobitekCRMV2.Jobs
{
    public class SpaceSerpJob : IJob
    {

        private readonly IRepository<Keyword> _keywordRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<KeywordInfo> _keywordInfoRepository;
        private readonly IRepository<KeywordValue> _keywordValue;
        private readonly IRepository<LogMessage> _logsMessageRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUnitOfWork _unitOfWork;


        public SpaceSerpJob(IRepository<Keyword> keywordRepository, IRepository<Project> projectRepository, IHttpClientFactory httpClientFactory,
        IRepository<KeywordInfo> keywordInfoRepository, IUnitOfWork unitOfWork, IRepository<KeywordValue> keywordValue, IRepository<LogMessage> logsMessageRepository)
        {
            _keywordRepository = keywordRepository;
            _projectRepository = projectRepository;
            _httpClientFactory = httpClientFactory;
            _keywordInfoRepository = keywordInfoRepository;
            _unitOfWork = unitOfWork;
            _keywordValue = keywordValue;
            _logsMessageRepository = logsMessageRepository;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return myJob2();
        }

        public async Task<Task> myJob2()
        {
            return Task.CompletedTask;
        }
        public async Task<Task> myJob()
        {

            //işlemler bunun içinde
            var seoProject = await _projectRepository.Table.Include(x => x.Keywords).ThenInclude(y => y.KeywordInfo)
            .Where(x => x.ProjectType == ProjectType.Seo && x.Status == Status.Active).ToListAsync();


            var msg = new LogMessage();
            msg.Name = "START SPACESERP JOB";
            await _logsMessageRepository.AddAsync(msg);
            await _unitOfWork.CommitAsync();
            //seoProject.Count
            for (var i = 0; i < seoProject.Count; i++)
            {

                for (var j = 0; j < seoProject[i].Keywords.Count; j++)
                {
                    var keywordName = seoProject[i].Keywords[j].KeywordName;

                    var url = seoProject[i].Url;
                    //var keyword = "Profesyonel Arapça Tercüme";
                    //var url = "erdentercume.com";

                    var countryCode = seoProject[i].CountryCode;
                    try
                    {
                        var result = await GetKeywordValuesAsync(keywordName, url, countryCode);
                        await SetResultToDb(result, seoProject[i].Keywords[j].Id, url);
                    }
                    catch (Exception e)
                    {//do nothing
                        var ms2 = new LogMessage();
                        ms2.Name = "ERROR keyword : " + keywordName;
                        ms2.Description = e.Message;
                        await _logsMessageRepository.AddAsync(ms2);
                        await _unitOfWork.CommitAsync();
                    }

                    // await Task.Delay(1000);
                }
                var ms4 = new LogMessage();
                ms4.Name = seoProject[i].Id.ToString();
                ms4.Description = seoProject[i].Url.ToString() + " - finished " + "KeywordCount :" + seoProject[i].Keywords.Count;
                await _logsMessageRepository.AddAsync(ms4);
                await _unitOfWork.CommitAsync();



            }
            var msg3 = new LogMessage();
            msg3.Name = "FINISH SPACESERP JOB";
            await _logsMessageRepository.AddAsync(msg3);
            await _unitOfWork.CommitAsync();


            return Task.CompletedTask;
        }

        public async Task<KeywordJsonModel> GetKeywordValuesAsync(string keyword, string domain, string countryCode)
        {
            keyword = keyword.Trim();
            keyword = keyword.Replace(" ", "+");
            string fullUrl = $"https://api.spaceserp.com/google/search?apiKey=bcb0a583-0fdf-45a3-9b3b-b89202e97c9f&q={keyword}&domain=google.com.tr&gl=tr&hl=tr&pageSize=100&resultBlocks=";
            string json = "";


            if (countryCode != null)
            {
                var countries = new Dictionary<string, string> {
                        {"tr", "domain=google.com.tr&gl=tr&hl=tr" },
                        {"us", "domain=google.com&gl=us&hl=en" },
                        {"uk", "domain=google.co.uk&gl=uk&hl=en" },
                        {"de", "domain=google.de&gl=de&hl=de" },
                        {"fr", "domain=google.fr&gl=fr&hl=fr" },
                        {"es", "domain=google.es&gl=es&hl=es" },
                    };
                fullUrl = $"https://api.spaceserp.com/google/search?apiKey=bcb0a583-0fdf-45a3-9b3b-b89202e97c9f&q={keyword}&{countries[countryCode]}&pageSize=100&resultBlocks=";

            }
            // eğer bağlantıda hata olursa veya zaman aşımı yerse 3 kere deneyecek
            var Loop = true;
            var inc = 0;
            while (Loop)
            {
                try
                {
                    using (var httpClient = _httpClientFactory.CreateClient())
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(20);// zaman aşımında catche düşer
                        var stream = httpClient.GetStreamAsync(fullUrl).Result;
                        var reader = new StreamReader(stream);
                        json = reader.ReadToEnd();
                        reader.Close();
                    }



                }
                catch (Exception ex)
                {

                }
                if (json != "")//json dolu gelirse(cevap) loop bitir
                {
                    Loop = false;
                }
                inc += 1;
                if (inc == 2)
                {
                    Loop = false;
                }
            }


            //data model dönüşümü
            Root data = JsonConvert.DeserializeObject<Root>(json);
            KeywordJsonModel result = new KeywordJsonModel();

            if (data == null)
            {
                result.position = 101;
                result.domain = domain;
                result.CountryCode = countryCode;
                return result;
            }
            if (data.organic_results == null)
            {
                result.position = 101;
                result.domain = domain;
                result.CountryCode = countryCode;
                return result;
            }

            //data içerisinde verilen domain aranıyor

            domain = domain.Split('/')[0];
            var isFound = false;
            foreach (var item in data.organic_results)
            {
                if (item.domain == domain || item.domain == "www." + domain)//aranan domain varsa resulta ekleniyor
                {

                    result.domain = domain;// dışarıdan verilen proje domaini
                    result.position = item.position;
                    result.link = item.link;
                    result.CountryCode = countryCode;

                    isFound = true;
                }
            }

            if (!isFound)//bulunamadıysa position 101 yapılıyor
            {
                result.position = 101;
                result.CountryCode = countryCode;
            }

            return result;
        }

        public async Task<int> SetResultToDb(KeywordJsonModel result, string keywordId, string domain)
        {
            if (result == null)
            {
                return 0;
            }


            var keywordUpdate = await _keywordRepository.Table.Include(x => x.KeywordInfo).FirstOrDefaultAsync(x => x.Id == keywordId);

            if (result.position != 0)
            {

                var values = new KeywordValue();
                values.Position = result.position;
                values.Domain = domain;
                values.Link = result.link;
                values.KeywordId = keywordUpdate.Id;
                values.CountryCode = result.CountryCode;

                await _keywordValue.AddAsync(values);
                await _unitOfWork.CommitAsync();
            }


            return 1;
        }


    }
}
