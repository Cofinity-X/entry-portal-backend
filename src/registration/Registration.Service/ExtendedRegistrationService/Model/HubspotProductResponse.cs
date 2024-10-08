using System.Text.Json.Serialization;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Model;

public class HubspotProductResponse
{
    [JsonPropertyName("allowed_billing_frequencies")]
    public string? AllowedBillingFrequencies { get; set; }

    [JsonPropertyName("catena_x_feature_list")]
    public string? CatenaXFeatureList { get; set; }

    [JsonPropertyName("catena_x_package_id")]
    public string? CatenaXPackageId { get; set; }

    [JsonPropertyName("catena_x_tags")]
    public string? CatenaXTags { get; set; }

    [JsonPropertyName("company_role")]
    public required string CompanyRole { get; set; }

    [JsonPropertyName("createdate")]
    public DateTime? CreateDate { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("hs_lastmodifieddate")]
    public DateTime? LastModifiedDate { get; set; }

    [JsonPropertyName("hs_object_id")]
    public string? ObjectId { get; set; }

    [JsonPropertyName("hs_price_eur")]
    public string? PriceEur { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("price_yearly")]
    public string? PriceYearly { get; set; }

    [JsonPropertyName("sales_package")]
    public required string SalesPackage { get; set; }
}
