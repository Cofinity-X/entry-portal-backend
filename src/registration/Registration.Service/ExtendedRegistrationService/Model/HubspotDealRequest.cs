using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Model;

public class HubspotDealRequest
{
    [JsonPropertyName("dealname")]
    public required string DealName { get; set; }

    [JsonPropertyName("quotename")]
    public required string QuoteName { get; set; }

    [JsonPropertyName("companyId")]
    public required string CompanyId { get; set; }

    [JsonPropertyName("company_role")]
    public required string CompanyRole { get; set; }

    [JsonPropertyName("contactId")]
    public required string ContactId { get; set; }

    [JsonPropertyName("agreementIds")]
    public List<string>? AgreementIds { get; set; }

    [JsonPropertyName("productIds")]
    public List<string>? ProductIds { get; set; }
}
public class HubspotDealCreateResponse
{
    [JsonPropertyName("dealId")]
    public string? DealId { get; set; }

    [JsonPropertyName("quoteId")]
    public string? QuoteId { get; set; }

    [JsonPropertyName("quoteUrl")]
    public string? QuoteUrl { get; set; }
}
