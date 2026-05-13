# SchoolAccount.Alpha 
## Overview

The project demonstrates 'Single Sign-On' using DfE Sign-In (DSI) Authentication, retrieval of data from the DSI, Academies and Search APIs.

The service allows a user to:
- sign in with DSI
- view a list of their DSI organisations
- view details about an establishment, including school size, phase of education, and free school meals information
- view the establishments in their trusts
- view details about an establishment in their trust
- view the latest DfE documents published on gov.uk
- search DfE documents published on gov.uk 

## Requirements

To run the project a DSI Application registration is required, including an API key. 
An application registration can be initiated via a 'Service Now' request. 

Ensure the project's redirect and logout redirect URLs are added to the DSI application's service configuration.

**Note:** the DSI redirect URLs do not allow `http` or `localhost`, so the project is configured to run on `https://127.0.0.1`.

An Academies API key is also required. Instructions on requesting a key can be found on the service details page at [Find and Use an API](https://beta-find-and-use-an-api.education.gov.uk/api/academies-api-1).

## Configuration

The project requires the following app configuration values to be overridden, ideally using [User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows):
```json
{
  "OpenIDConnectSettings": {
    "Authority": "<URL-OF-DSI-OIDC-SERVER>",
    "ClientId": "<DSI-CLIENT-ID>"
  },
  "DfeSignInApi": {
    "PublicURL": "<URL-OF-DSI-API>",
    "ServiceAudience": "<DSI-SERVICE-AUDIENCE>",
    "Issuer": "<DSI-CLIENT-ID>",
    "ApiSecret": "<DSI-API-SECRET>"
  },
  "AcademiesApi": {
    "PublicURL": "<URL-OF-ACADEMIES-API-ENVIRONMENT>",
    "ApiKey": "<ACADEMIES-API-KEY>"
  }
}
```

Suitable values may be taken from an existing project, i.e. DfE Connect, for investigative purposes.

## Shared Components

The projct utilises the [govuk-frontend-aspnetcore](https://github.com/x-govuk/govuk-frontend-aspnetcore) package, rebranded to DfE using the guidlines at [https://design.education.gov.uk/design-system/govuk-rebrand](https://design.education.gov.uk/design-system/govuk-rebrand).


## DSI Authentication

When authenticating with DSI the client may choose to manage a user's organisation itself, or to delegate that responsibility to DSI.

When delegating responsibility to DSI, the organisation is supplied as an additional 'claim', and the user's organisation cannot be changed without signing out and back in again.

This project manages the organisation itself, using the DSI public APIs to retrieve a list of the user's organisations.

To see the DSI managed organisation flow in action, uncomment line 28 in [Program.cs](./SchoolAccount.Alpha/Program.cs) so it reads:
```csharp   
options.Scope.Add("organisation");
```

## DSI API

The project uses a DSI API Secret to generate a short lived JWT access token to call the DSI API.

The user's DSI ID is obtained from the `sub` claim from the authentication token, and is used to retrieve a list of their organisations from the DSI API.
See the [DSI API 'Get organisations for user' documentation](https://github.com/DFE-Digital/login.dfe.public-api/blob/main/README.md#get-organisations-for-user) for more information.

## Academies API

While the DSI API provides organisation details, richer information is available from the Academies API.

Although named 'the Academies API', the Academies API contains data about all educational establishments, including academies and local authority maintained schools.

>**Note:** The Academies API publishes a NuGet package [Dfe.AcademiesApi.Client](https://github.com/DFE-Digital/academies-api/tree/main/Dfe.AcademiesApi.Client) to allow .NET projects to easily integrate with the API. 
>
>For the sake of simplicity this project calls the API directly, rather than requiring additional setup to use the GitHub NuGet package feed.
> 
>A production application should use the NuGet package instead.

The Academies API is used for two purposes:
- to retrieve in depth details about an establishment
- to retrieve the establishments in a trusts

## Search API

The search API documentation can be found at [https://docs.publishing.service.gov.uk/repos/search-api](https://docs.publishing.service.gov.uk/repos/search-api/using-the-search-api.html).

There are hundreds of fields that can be retrieved from the search API, but not all are used across all documents. The full list can be found in the Search API's [field_definitions.json](https://github.com/alphagov/search-api/blob/main/config/schema/field_definitions.json).

The query in this project uses the following fields:
- title
- description
- link
- updated_at
- format
- taxons
- part_of_taxonomy_tree
- start_date
- end_date
               
It is also possible to retrieve the raw content that was added to the query store via the `indexable_content` field. 
While not very readable due to the lack of whitespace, this could potentially be useful as the source for an AI summary.

Taxons are returned as a list of IDs in the search API, rather than the human readable name. Some effort is required to get details for the ID. 

Taxons themselves are content. Level one taxon details can be retrieved from [https://www.gov.uk/api/content](https://www.gov.uk/api/content). 

The [documentation](https://docs.publishing.service.gov.uk/manual/taxonomy.html#accessing-the-taxonomy) suggests building the structure of the taxonomy tree by following the `child_taxons` links for each top level taxon, and each child link thereof. 

This approach was not undertaken for the spike, a suitable (though possibly out of date) CSV file was obtained from [GitHub user Oscar Wyatt's repository](https://github.com/oscarwyatt/print-the-whole-damn-taxonomy/blob/master/tree.csv) to [build a dictionary](SchoolAccount.Alpha/Services/TaxonService.cs) of taxon ID to name.

To reduce the amount of content returned search results are restricted to only guidance and detailed guides. published by the DfE.

Search results are order by [relevance](https://docs.publishing.service.gov.uk/repos/search-api/relevancy.html#contents), while latest guidance is in descending order of last update. 

Note: there are two timestamps available, `update_date` and `public_timestamp`. `update_date` is the time of the last indexing which may be significantly newer than `public_timestamp`, as that usually only reflects major updates and not minor ones. Update date was selected for the prototype to capture all changes to content.
