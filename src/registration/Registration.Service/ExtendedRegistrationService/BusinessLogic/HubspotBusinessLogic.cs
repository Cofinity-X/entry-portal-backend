using Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.Model;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Identities;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Net.Mime;

namespace Org.Eclipse.TractusX.Portal.Backend.ExtendedRegistration.Service.BusinessLogic;

public class HubspotBusinessLogic(
    IHubspotService hubspotService, IIdentityService identityService) : IHubspotBusinessLogic
{
    public async Task<IEnumerable<HubspotProductResponse>> GetProductsByCompanyRoleAsync(string[] companyRoles, CancellationToken cancellationToken)
    {
        var response = await hubspotService.GetProductsByCompanyRoleAsync(companyRoles, cancellationToken)
          .ConfigureAwait(ConfigureAwaitOptions.None);
        if (!response.Any())
        {
            throw new NotFoundException($"Access to external system Hubspot did not found the products for these company roles : {string.Join(",", companyRoles)}");
        }
        return response;
    }

    public async Task<HubspotCompanyResponse> GetHubspotCompanyDetailsAsync(CancellationToken cancellationToken)
    {
        return await hubspotService.GetHubspotCompanyDetailsAsync(identityService.IdentityData.CompanyId.ToString(), cancellationToken)
          .ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<IEnumerable<HubspotAgreementResponse>> GetHubspotAgreementsAsync(CancellationToken cancellationToken)
    {
        return await hubspotService.GetHubspotAgreementsAsync(cancellationToken)
          .ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<HubspotCompanyUpdateResponse> CreateUpdateHubspotCompanyAsync(HubspotInboundRequest hubspotRequest, CancellationToken cancellationToken)
    {
        var hubspotOutboundRequest = new HubspotOutboundRequest
        (
           hubspotRequest.Company != null ? new HubspotOutboundCompany
           {

               PortalCompanyId = identityService.IdentityData.CompanyId.ToString(),
               Agreements = hubspotRequest.Company.Agreements,
               Properties = hubspotRequest.Company.Properties

           } : null, hubspotRequest.Contact
        );
        var response = await hubspotService.CreateUpdateHubspotCompanyAsync(hubspotOutboundRequest, cancellationToken)
          .ConfigureAwait(ConfigureAwaitOptions.None);
        return response.CompanyId != null || response.ContactId != null ? response : throw new ServiceException("Access to external system Hubspot did not updated the company or contact");
    }

    public async Task<IEnumerable<HubspotDealCreateResponse>> CreateDealAsync(HubspotDealRequest[] hubspotDealRequest, CancellationToken cancellationToken)
    {
        var hsCompany = await hubspotService.GetHubspotCompanyDetailsAsync(identityService.IdentityData.CompanyId.ToString(), cancellationToken);
        if (hubspotDealRequest.Any(item => hsCompany.Id != item.CompanyId))
        {
            throw new ConflictException("This user is not associated with the Hubspot company sent in the request.");
        }
        return await hubspotService.CreateDealAsync(hubspotDealRequest, cancellationToken)
         .ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<(string FileName, byte[] Content, string MediaType)> GetQuoteCombinedPdfAsync(CancellationToken cancellationToken)
    {
        var quotes = await hubspotService.GetQuotesAsync(identityService.IdentityData.CompanyId.ToString(), cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (quotes == null || !quotes.Any())
        {
            throw new NotFoundException("No quotes found for this company");
        }

        var pdfStreams = new List<MemoryStream>();

        await Parallel.ForEachAsync(quotes, cancellationToken, async (item, ct) =>
        {
            var pdfStream = await hubspotService.GetQuotePdfAsync(item.Quote.HsPdfDownloadLink, ct).ConfigureAwait(false);
            lock (pdfStreams)
            {
                pdfStreams.Add(pdfStream);
            }
        });

        // Merge PDFs
        var mergedPdf = await MergePdfFilesAsync(pdfStreams).ConfigureAwait(false);

        // Dispose of streams
        foreach (var stream in pdfStreams)
        {
            stream.Dispose();
        }

        return ("Quote", mergedPdf, MediaTypeNames.Application.Pdf);
    }

    private static Task<byte[]> MergePdfFilesAsync(List<MemoryStream> pdfStreams)
    {
        using var outputStream = new MemoryStream();
        using (var outputDocument = new PdfDocument())
        {
            // Add the first page from the first PDF as a common page because HS have a common cover page for all the quotes
            var firstDoc = PdfReader.Open(pdfStreams[0], PdfDocumentOpenMode.Import);
            outputDocument.AddPage(firstDoc.Pages[0]);

            // Add remaining pages from each PDF
            foreach (var pdfStream in pdfStreams)
            {
                var document = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Import);
                for (var i = 1; i < document.PageCount; i++)
                {
                    outputDocument.AddPage(document.Pages[i]);
                }
            }

            // Save the merged document to the memory stream
            outputDocument.Save(outputStream);
        }
        return Task.FromResult(outputStream.ToArray());
    }
}
