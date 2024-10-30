using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Model;

public class HubspotQuoteResponse
{
    [JsonPropertyName("name")]
    public required string Id { get; set; }

    [JsonPropertyName("quote")]
    public required HubspotQuote Quote { get; set; }
}

public class HubspotQuote
{
    [JsonPropertyName("hs_quote_link")]
    public string? HsQuoteLink { get; set; }

    [JsonPropertyName("hs_pdf_download_link")]
    public required string HsPdfDownloadLink { get; set; }
}
