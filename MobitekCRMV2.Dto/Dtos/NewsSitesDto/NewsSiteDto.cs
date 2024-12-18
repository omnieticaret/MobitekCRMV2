﻿using MobitekCRMV2.Dto.Dtos;
using MobitekCRMV2.Dto.Dtos.BackLinskDto;
using MobitekCRMV2.Dto.Dtos.PromotionsDto;
using MobitekCRMV2.Dto.Dtos.UsersDto;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Dto.Dtos.NewsSitesDto
{
    public class NewsSiteDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string GoogleNews { get; set; }
        public int DRScore { get; set; }
        public int LinkedDomains { get; set; }
        public int TotalBacklinks { get; set; }
        public int OrganicTraffic { get; set; }
        public int AllTraffic { get; set; }
        public int PAScore { get; set; }
        public int DAScore { get; set; }
        public int SpamScore { get; set; }
        public string EditorMail { get; set; }
        public string EditorPhone { get; set; }
        public int MozDA { get; set; }
        public string LastUpdateDate { get; set; }
        public string OldData { get; set; }
        public string Note { get; set; }

        public string PromotionId { get; set; }
        public PromotionSummaryDto Promotion { get; set; }
        public string UserId { get; set; }
        public UserSummaryDto User { get; set; }
        public List<BackLinkSummaryDto> BackLinks { get; set; }

        public int BackLinksCount { get; set; }
        public string TrafficQuality { get; set; }
    }

    public class NewsSiteListDto
    {
        public List<NewsSiteDto> NewsSites { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<string, double> AverageMetrics { get; set; }
    }

    public class NewsSiteSummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int DAScore { get; set; }
        public string Email { get; set; }
        public Status Status { get; set; }
        public int PAScore { get; set; }
        public string EditorMail { get; set; }
        public string LastUpdateDate { get; set; }
        public int BackLinksCount { get; set; }
    }
    public class NewsSiteCreateUpdateDto
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string GoogleNews { get; set; }
        public int DRScore { get; set; }
        public int LinkedDomains { get; set; }
        public int TotalBacklinks { get; set; }
        public int OrganicTraffic { get; set; }
        public int AllTraffic { get; set; }
        public int PAScore { get; set; }
        public int DAScore { get; set; }
        public int SpamScore { get; set; }
        public string EditorMail { get; set; }
        public string EditorPhone { get; set; }
        public int MozDA { get; set; }
        public string Note { get; set; }
        public string PromotionId { get; set; }
        public string UserId { get; set; }
    }

    public class NewsSiteFilterDto
    {
        public string? Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinDAScore { get; set; }
        public int? MaxDAScore { get; set; }
        public int? MinPAScore { get; set; }
        public int? MaxPAScore { get; set; }
        public int? MinDRScore { get; set; }
        public int? MaxDRScore { get; set; }
        public int? MinLinkedDomains { get; set; }
        public int? MaxLinkedDomains { get; set; }
        public int? MinBacklinks { get; set; }
        public int? MaxBacklinks { get; set; }
        public int? MinOrganicTraffic { get; set; }
        public int? MaxOrganicTraffic { get; set; }
        public int? MinAllTraffic { get; set; }
        public int? MaxAllTraffic { get; set; }
        public int? MinSpamScore { get; set; }
        public int? MaxSpamScore { get; set; }
        public bool NewRegistrations { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }


}