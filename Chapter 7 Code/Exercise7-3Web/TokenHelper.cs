
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using Microsoft.IdentityModel;
using Microsoft.IdentityModel.S2S.Protocols.OAuth2;
using Microsoft.IdentityModel.S2S.Tokens;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

namespace Exercise7_3Web
{

    public class TokenHelper
    {

        #region public methods

        /// <summary>
        /// Configures .Net to trust all certificates when making network calls.  This is used so that calls 
        /// to an https SharePoint server without a valid certificate are not rejected.  This should only be used during 
        /// testing, and should never be used in a production app.
        /// </summary>
        public static void TrustAllCertificates()
        {
            //Trust all certificates
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
        }

        /// <summary>
        /// Retrieves the context token string from the specified request by looking for well-known parameter names in the 
        /// POSTed form parameters and the querystring. Returns null if no context token is found.
        /// </summary>
        /// <param name="request">HttpRequest in which to look for a context token</param>
        /// <returns>The context token string</returns>
        public static string GetContextTokenFromRequest(HttpRequest request)
        {
            string[] paramNames = { "AppContext", "AppContextToken", "AccessToken", "SPAppToken" };
            foreach (string paramName in paramNames)
            {
                if (!string.IsNullOrEmpty(request.Form[paramName])) return request.Form[paramName];
                if (!string.IsNullOrEmpty(request.QueryString[paramName])) return request.QueryString[paramName];
            }
            return null;
        }

        /// <summary>
        /// Validate that a specified context token string is intended for this application based on the parameters 
        /// specified in web.config. Parameters used from web.config used for validation include ClientId, 
        /// HostedAppHostName, ClientSecret, and Realm (if it is specified). If the <paramref name="appHostName"/> is not 
        /// null, it is used for validation instead of the web.config's HostedAppHostName. If the token is invalid, an 
        /// exception is thrown. If the token is valid, TokenHelper's static STS metadata url is updated based on the token contents
        /// and a JsonWebSecurityToken based on the context token is returned.
        /// </summary>
        /// <param name="contextTokenString">The context token to validate</param>
        /// <param name="appHostName">The URL authority, consisting of  Domain Name System (DNS) host name or IP address and the port number, to use for token audience validation.
        /// If null, HostedAppHostName web.config setting is used instead.</param>
        /// <returns>A JsonWebSecurityToken based on the context token.</returns>
        public static SharePointContextToken ReadAndValidateContextToken(string contextTokenString, string appHostName = null)
        {
            JsonWebSecurityTokenHandler tokenHandler = CreateJsonWebSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.ReadToken(contextTokenString);
            JsonWebSecurityToken jsonToken = securityToken as JsonWebSecurityToken;
            SharePointContextToken token = SharePointContextToken.Create(jsonToken);

            string stsAuthority = (new Uri(token.SecurityTokenServiceUri)).Authority;
            int firstDot = stsAuthority.IndexOf('.');

            GlobalEndPointPrefix = stsAuthority.Substring(0, firstDot);
            AcsHostUrl = stsAuthority.Substring(firstDot + 1);

            tokenHandler.ValidateToken(jsonToken);

            if (appHostName == null)
            {
                appHostName = HostedAppHostName;
            }

            string realm = Realm ?? token.Realm;
            string principal = GetFormattedPrincipal(ClientId, appHostName, realm);
            if (!StringComparer.OrdinalIgnoreCase.Equals(token.Audience, principal))
            {
                throw new Microsoft.IdentityModel.Tokens.AudienceUriValidationFailedException(
                    String.Format(CultureInfo.CurrentCulture,
                    "\"{0}\" is not the intended audience \"{1}\"", principal, token.Audience));
            }

            return token;
        }

        /// <summary>
        /// Retrieves an access token from ACS to call the source of the specified context token at the specified 
        /// targetHost. The targetHost must be registered for principal the that sent the context token.
        /// </summary>
        /// <param name="contextToken">Context token issued by the intended access token audience</param>
        /// <param name="targetHost">Url authority of the target principal</param>
        /// <returns>An access token with an audience matching the context token's source</returns>
        public static OAuth2AccessTokenResponse GetAccessToken(SharePointContextToken contextToken, string targetHost)
        {

            string targetPrincipalName = contextToken.TargetPrincipalName;

            // Extract the refreshToken from the context token
            string refreshToken = contextToken.RefreshToken;

            if (String.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            string realm = Realm ?? contextToken.Realm;

            string resource = GetFormattedPrincipal(targetPrincipalName, targetHost, realm);
            string clientId = GetFormattedPrincipal(ClientId, null, realm);

            OAuth2AccessTokenRequest oauth2Request =
                OAuth2MessageFactory.CreateAccessTokenRequestWithRefreshToken(
                    clientId,
                    ClientSecret,
                    refreshToken,
                    resource);

            // Get token
            OAuth2S2SClient client = new OAuth2S2SClient();
            OAuth2AccessTokenResponse oauth2Response;
            try
            {
                oauth2Response =
                    client.Issue(AcsMetadataParser.GetStsUrl(realm), oauth2Request) as OAuth2AccessTokenResponse;
            }
            catch (WebException wex)
            {
                using (StreamReader sr = new StreamReader(wex.Response.GetResponseStream()))
                {
                    string responseText = sr.ReadToEnd();
                    throw new WebException(wex.Message + " - " + responseText, wex);
                }
            }

            return oauth2Response;
        }

        /// <summary>
        /// Uses the specified authorization code to retrieve an access token from ACS to call the specified principal 
        /// at the specified targetHost. The targetHost must be registered for target principal.  If specified realm is 
        /// null, the "Realm" setting in web.config will be used instead.
        /// </summary>
        /// <param name="authorizationCode">Authorization code to exchange for access token</param>
        /// <param name="targetPrincipalName">Name of the target principal to retrieve an access token for</param>
        /// <param name="targetHost">Url authority of the target principal</param>
        /// <param name="targetRealm">Realm to use for the access token's nameid and audience</param>
        /// <returns>An access token with an audience of the target principal</returns>
        public static OAuth2AccessTokenResponse GetAccessToken(
            string authorizationCode,
            string targetPrincipalName,
            string targetHost,
            string targetRealm,
            Uri redirectUri)
        {

            if (targetRealm == null)
            {
                targetRealm = Realm;
            }

            string resource = GetFormattedPrincipal(targetPrincipalName, targetHost, targetRealm);
            string clientId = GetFormattedPrincipal(ClientId, null, targetRealm);

            // Create request for token. The RedirectUri is null here.  This will fail if redirect uri is registered
            OAuth2AccessTokenRequest oauth2Request =
                OAuth2MessageFactory.CreateAccessTokenRequestWithAuthorizationCode(
                    clientId,
                    ClientSecret,
                    authorizationCode,
                    redirectUri,
                    resource);

            // Get token
            OAuth2S2SClient client = new OAuth2S2SClient();
            OAuth2AccessTokenResponse oauth2Response;
            try
            {
                oauth2Response =
                    client.Issue(AcsMetadataParser.GetStsUrl(targetRealm), oauth2Request) as OAuth2AccessTokenResponse;
            }
            catch (WebException wex)
            {
                using (StreamReader sr = new StreamReader(wex.Response.GetResponseStream()))
                {
                    string responseText = sr.ReadToEnd();
                    throw new WebException(wex.Message + " - " + responseText, wex);
                }
            }

            return oauth2Response;
        }

        /// <summary>
        /// Uses the specified refresh token to retrieve an access token from ACS to call the specified principal 
        /// at the specified targetHost. The targetHost must be registered for target principal.  If specified realm is 
        /// null, the "Realm" setting in web.config will be used instead.
        /// </summary>
        /// <param name="refreshToken">Refresh token to exchange for access token</param>
        /// <param name="targetPrincipalName">Name of the target principal to retrieve an access token for</param>
        /// <param name="targetHost">Url authority of the target principal</param>
        /// <param name="targetRealm">Realm to use for the access token's nameid and audience</param>
        /// <returns>An access token with an audience of the target principal</returns>
        public OAuth2AccessTokenResponse GetAccessToken(
            string refreshToken,
            string targetPrincipalName,
            string targetHost,
            string targetRealm)
        {

            if (targetRealm == null)
            {
                targetRealm = Realm;
            }

            string resource = GetFormattedPrincipal(targetPrincipalName, targetHost, targetRealm);
            string clientId = GetFormattedPrincipal(ClientId, null, targetRealm);

            OAuth2AccessTokenRequest oauth2Request = OAuth2MessageFactory.CreateAccessTokenRequestWithRefreshToken(clientId, ClientSecret, refreshToken, resource);

            // Get token
            OAuth2S2SClient client = new OAuth2S2SClient();
            OAuth2AccessTokenResponse oauth2Response;
            try
            {
                oauth2Response =
                    client.Issue(AcsMetadataParser.GetStsUrl(targetRealm), oauth2Request) as OAuth2AccessTokenResponse;
            }
            catch (WebException wex)
            {
                using (StreamReader sr = new StreamReader(wex.Response.GetResponseStream()))
                {
                    string responseText = sr.ReadToEnd();
                    throw new WebException(wex.Message + " - " + responseText, wex);
                }
            }

            return oauth2Response;
        }

        /// <summary>
        /// Retrieves an app-only access token from ACS to call the specified principal 
        /// at the specified targetHost. The targetHost must be registered for target principal.  If specified realm is 
        /// null, the "Realm" setting in web.config will be used instead.
        /// </summary>
        /// <param name="targetPrincipalName">Name of the target principal to retrieve an access token for</param>
        /// <param name="targetHost">Url authority of the target principal</param>
        /// <param name="targetRealm">Realm to use for the access token's nameid and audience</param>
        /// <returns>An access token with an audience of the target principal</returns>
        public static OAuth2AccessTokenResponse GetAppOnlyAccessToken(
            string targetPrincipalName,
            string targetHost,
            string targetRealm)
        {

            if (targetRealm == null)
            {
                targetRealm = Realm;
            }

            string resource = GetFormattedPrincipal(targetPrincipalName, targetHost, targetRealm);
            string clientId = GetFormattedPrincipal(ClientId, HostedAppHostName, targetRealm);

            OAuth2AccessTokenRequest oauth2Request = OAuth2MessageFactory.CreateAccessTokenRequestWithClientCredentials(clientId, ClientSecret, resource);
            oauth2Request.Resource = resource;

            // Get token
            OAuth2S2SClient client = new OAuth2S2SClient();

            OAuth2AccessTokenResponse oauth2Response;
            try
            {
                oauth2Response =
                    client.Issue(AcsMetadataParser.GetStsUrl(targetRealm), oauth2Request) as OAuth2AccessTokenResponse;
            }
            catch (WebException wex)
            {
                using (StreamReader sr = new StreamReader(wex.Response.GetResponseStream()))
                {
                    string responseText = sr.ReadToEnd();
                    throw new WebException(wex.Message + " - " + responseText, wex);
                }
            }

            return oauth2Response;
        }

        /// <summary>
        /// Creates a client context based on the properties of a remote event receiver
        /// </summary>
        /// <param name="properties">Properties of a remote event receiver</param>
        /// <returns>A ClientContext ready to call the web where the event originated</returns>
        public static ClientContext CreateRemoteEventReceiverClientContext(SPRemoteEventProperties properties)
        {
            Uri sharepointUrl = null;
            if (properties.ListEventProperties != null)
            {
                sharepointUrl = new Uri(properties.ListEventProperties.WebUrl);
            }
            else if (properties.ItemEventProperties != null)
            {
                sharepointUrl = new Uri(properties.ItemEventProperties.WebUrl);
            }
            else if (properties.WebEventProperties != null)
            {
                sharepointUrl = new Uri(properties.WebEventProperties.FullUrl);
            }
            else
            {
                return null;
            }

            if (ClientCertificate != null)
            {
                return TokenHelper.GetS2SClientContextWithWindowsIdentity(sharepointUrl, null);
            }
            else
            {
                return CreateAcsClientContextForUrl(properties, sharepointUrl);
            }
        }

        /// <summary>
        /// Creates a client context based on the properties of an app event
        /// </summary>
        /// <param name="properties">Properties of an app event</param>
        /// <param name="useAppWeb">True to target the app web, false to target the host web</param>
        /// <returns>A ClientContext ready to call the app web or the parent web</returns>
        public static ClientContext CreateAppEventClientContext(SPRemoteEventProperties properties, bool useAppWeb)
        {
            if (properties.AppEventProperties == null)
            {
                return null;
            }

            Uri sharepointUrl = useAppWeb ? properties.AppEventProperties.AppWebFullUrl : properties.AppEventProperties.HostWebFullUrl;
            if (ClientCertificate != null)
            {
                return TokenHelper.GetS2SClientContextWithWindowsIdentity(sharepointUrl, null);
            }
            else
            {
                return CreateAcsClientContextForUrl(properties, sharepointUrl);
            }
        }

        /// <summary>
        /// Retrieves an access token from ACS using the specified authorization code, and uses that access token to 
        /// create a client context
        /// </summary>
        /// <param name="targetUrl">Url of the target SharePoint site</param>
        /// <param name="targetPrincipalName">Name of the target SharePoint principal</param>
        /// <param name="authorizationCode">Authorization code to use when retrieving the access token from ACS</param>
        /// <param name="targetRealm">Realm to use for the access token's nameid and audience</param>
        /// <returns>A ClientContext ready to call targetUrl with a valid access token</returns>
        public static ClientContext GetClientContextWithAuthorizationCode(
            string targetUrl,
            string targetPrincipalName,
            string authorizationCode,
            string targetRealm,
            Uri redirectUri)
        {
            Uri targetUri = new Uri(targetUrl);

            string accessToken =
                GetAccessToken(authorizationCode, targetPrincipalName, targetUri.Authority, targetRealm, redirectUri).AccessToken;

            return GetClientContextWithAccessToken(targetUrl, accessToken);
        }

        /// <summary>
        /// Uses the specified access token to create a client context
        /// </summary>
        /// <param name="targetUrl">Url of the target SharePoint site</param>
        /// <param name="accessToken">Access token to be used when calling the specified targetUrl</param>
        /// <returns>A ClientContext ready to call targetUrl with the specified access token</returns>
        public static ClientContext GetClientContextWithAccessToken(string targetUrl, string accessToken)
        {
            Uri targetUri = new Uri(targetUrl);

            ClientContext clientContext = new ClientContext(targetUrl);

            clientContext.AuthenticationMode = ClientAuthenticationMode.Anonymous;
            clientContext.FormDigestHandlingEnabled = false;
            clientContext.ExecutingWebRequest +=
                delegate(object oSender, WebRequestEventArgs webRequestEventArgs)
                {
                    webRequestEventArgs.WebRequestExecutor.RequestHeaders["Authorization"] =
                        "Bearer " + accessToken;
                };

            return clientContext;
        }

        /// <summary>
        /// Retrieves an access token from ACS using the specified context token, and uses that access token to create
        /// a client context
        /// </summary>
        /// <param name="targetUrl">Url of the target SharePoint site</param>
        /// <param name="contextTokenString">Context token received from the target SharePoint site</param>
        /// <param name="appHostUrl">Url authority of the hosted app.  If this is null, the value in the HostedAppHostName
        /// of web.config will be used instead</param>
        /// <returns>A ClientContext ready to call targetUrl with a valid access token</returns>
        public static ClientContext GetClientContextWithContextToken(
            string targetUrl,
            string contextTokenString,
            string appHostUrl)
        {
            SharePointContextToken contextToken = ReadAndValidateContextToken(contextTokenString, appHostUrl);

            Uri targetUri = new Uri(targetUrl);

            string accessToken = GetAccessToken(contextToken, targetUri.Authority).AccessToken;

            return GetClientContextWithAccessToken(targetUrl, accessToken);
        }

        /// <summary>
        /// Returns the SharePoint url to which the app should redirect the browser to request consent and get back
        /// an authorization code.
        /// </summary>
        /// <param name="contextUrl">Absolute Url of the SharePoint site</param>
        /// <param name="scope">Space-delimited permissions to request from the SharePoint site in "shorthand" format 
        /// (e.g. "Web.Read Site.Write")</param>
        /// <returns>Url of the SharePoint site's OAuth authorization page</returns>
        public static string GetAuthorizationUrl(string contextUrl, string scope)
        {
            return string.Format(
                "{0}{1}?IsDlg=1&client_id={2}&scope={3}&response_type=code",
                EnsureTrailingSlash(contextUrl),
                AuthorizationPage,
                ClientId,
                scope);
        }

        /// <summary>
        /// Returns the SharePoint url to which the app should redirect the browser to request consent and get back
        /// an authorization code.
        /// </summary>
        /// <param name="contextUrl">Absolute Url of the SharePoint site</param>
        /// <param name="scope">Space-delimited permissions to request from the SharePoint site in "shorthand" format
        /// (e.g. "Web.Read Site.Write")</param>
        /// <param name="redirectUri">Uri to which SharePoint should redirect the browser to after consent is 
        /// granted</param>
        /// <returns>Url of the SharePoint site's OAuth authorization page</returns>
        public static string GetAuthorizationUrl(string contextUrl, string scope, string redirectUri)
        {
            return string.Format(
                "{0}{1}?IsDlg=1&client_id={2}&scope={3}&response_type=code&redirect_uri={4}",
                EnsureTrailingSlash(contextUrl),
                AuthorizationPage,
                ClientId,
                scope,
                redirectUri);
        }

        /// <summary>
        /// Returns the SharePoint url to which the app should redirect the browser to request a new context token.
        /// </summary>
        /// <param name="contextUrl">Absolute Url of the SharePoint site</param>
        /// <param name="redirectUri">Uri to which SharePoint should redirect the browser to with a context token</param>
        /// <returns>Url of the SharePoint site's context token redirect page</returns>
        public static string GetAppContextTokenRequestUrl(string contextUrl, string redirectUri)
        {
            return string.Format(
                "{0}{1}?client_id={2}&redirect_uri={3}",
                EnsureTrailingSlash(contextUrl),
                RedirectPage,
                ClientId,
                redirectUri);
        }

        /// <summary>
        /// Retrieves an S2S access token signed by the application's private certificate on behalf of the specified 
        /// WindowsIdentity and intended for the SharePoint at the targetApplicationUri. If no Realm is specified in 
        /// web.config, an auth challenge will be issued to the targetApplicationUri to discover it.
        /// </summary>
        /// <param name="targetApplicationUri">Url of the target SharePoint site</param>
        /// <param name="identity">Windows identity of the user on whose behalf to create the access token</param>
        /// <returns>An access token with an audience of the target principal</returns>
        public static string GetS2SAccessTokenWithWindowsIdentity(
            Uri targetApplicationUri,
            WindowsIdentity identity)
        {
            string realm = string.IsNullOrEmpty(Realm) ? GetRealmFromTargetUrl(targetApplicationUri) : Realm;

            JsonWebTokenClaim[] claims = identity != null ? GetClaimsWithWindowsIdentity(identity) : null;

            return GetS2SAccessTokenWithClaims(targetApplicationUri.Authority, realm, claims);
        }

        /// <summary>
        /// Retrieves an S2S client context with an access token signed by the application's private certificate on 
        /// behalf of the specified WindowsIdentity and intended for application at the targetApplicationUri using the 
        /// targetRealm. If no Realm is specified in web.config, an auth challenge will be issued to the 
        /// targetApplicationUri to discover it.
        /// </summary>
        /// <param name="targetApplicationUri">Url of the target SharePoint site</param>
        /// <param name="identity">Windows identity of the user on whose behalf to create the access token</param>
        /// <returns>A ClientContext using an access token with an audience of the target application</returns>
        public static ClientContext GetS2SClientContextWithWindowsIdentity(
            Uri targetApplicationUri,
            WindowsIdentity identity)
        {
            string realm = string.IsNullOrEmpty(Realm) ? GetRealmFromTargetUrl(targetApplicationUri) : Realm;

            JsonWebTokenClaim[] claims = identity != null ? GetClaimsWithWindowsIdentity(identity) : null;

            string accessToken = GetS2SAccessTokenWithClaims(targetApplicationUri.Authority, realm, claims);

            return GetClientContextWithAccessToken(targetApplicationUri.ToString(), accessToken);
        }

        #endregion

        #region private fields

        //
        // Configuration Constants
        //

        private const string SHAREPOINT_PID = "00000003-0000-0ff1-ce00-000000000000";

        private const string AccessTokenParameterName = "AccessToken";
        private const string AuthorizationPage = "_layouts/15/OAuthAuthorize.aspx";
        private const string RedirectPage = "_layouts/15/AppRedirect.aspx";
        private const string AcsPrincipalName = "00000001-0000-0000-c000-000000000000";
        private const string AcsMetadataEndPointRelativeUrl = "metadata/json/1";
        private const string DelegationService = "DelegationService1.0";
        private const string AcsManagementServiceRelativeUrl = "r4/mgmt/service/";
        private const string AcsManagementServiceAppliesTo = "v2/mgmt/service/";
        private const string AcsOAuthRelativeUrl = "v2/OAuth2-13";
        private const string S2SProtocol = "OAuth2";
        private const string DelegationIssuance = "DelegationIssuance1.0";
        private const string DelegationIssuanceRequestTokenUrlSuffix = "RequestAccessToken";
        private const string NameIdentifierClaimType = JsonWebTokenConstants.ReservedClaims.NameIdentifier;
        private const string IdentityProviderClaimType = JsonWebTokenConstants.ReservedClaims.IdentityProvider;
        private const string TrustedForImpersonationClaimType = "trustedfordelegation";
        private const string ApplicationContextClaimType = JsonWebTokenConstants.ReservedClaims.AppContext;
        private const string ActorTokenClaimType = JsonWebTokenConstants.ReservedClaims.ActorToken;
        private const string SmtpClaimType = "smtp";
        private const string SipClaimType = "sip";
        private const int TokenLifetimeMinutes = 1000000;

        //
        // Environment Constants
        //

        private static string GlobalEndPointPrefix = "accounts";
        private static string AcsHostUrl = "accesscontrol.windows.net";

        //
        // Hosted app configuration
        //
        private static readonly string ClientId = string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get("ClientId")) ? WebConfigurationManager.AppSettings.Get("HostedAppName") : WebConfigurationManager.AppSettings.Get("ClientId");
        private static readonly string IssuerId = string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get("IssuerId")) ? ClientId : WebConfigurationManager.AppSettings.Get("IssuerId");
        private static readonly string HostedAppHostName = WebConfigurationManager.AppSettings.Get("HostedAppHostName");
        private static readonly string ClientSecret = string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get("ClientSecret")) ? WebConfigurationManager.AppSettings.Get("HostedAppSigningKey") : WebConfigurationManager.AppSettings.Get("ClientSecret");
        private static readonly string SecondaryClientSecret = WebConfigurationManager.AppSettings.Get("SecondaryClientSecret");
        private static readonly string Realm = WebConfigurationManager.AppSettings.Get("Realm");
        private static readonly string ServiceNamespace = WebConfigurationManager.AppSettings.Get("Realm");

        private static readonly string ClientSigningCertificatePath = WebConfigurationManager.AppSettings.Get("ClientSigningCertificatePath");
        private static readonly string ClientSigningCertificatePassword = WebConfigurationManager.AppSettings.Get("ClientSigningCertificatePassword");
        private static X509Certificate2 ClientCertificate = (string.IsNullOrEmpty(ClientSigningCertificatePath) || string.IsNullOrEmpty(ClientSigningCertificatePassword)) ? null : new X509Certificate2(ClientSigningCertificatePath, ClientSigningCertificatePassword);
        private static Microsoft.IdentityModel.SecurityTokenService.X509SigningCredentials SigningCredentials = (ClientCertificate == null) ? null : new Microsoft.IdentityModel.SecurityTokenService.X509SigningCredentials(ClientCertificate, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest);

        #endregion

        #region private methods

        private static ClientContext CreateAcsClientContextForUrl(SPRemoteEventProperties properties, Uri sharepointUrl)
        {
            string contextTokenString = properties.ContextToken;

            if (String.IsNullOrEmpty(contextTokenString))
            {
                return null;
            }

            HttpRequestMessageProperty requestProperty = (HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name];
            SharePointContextToken contextToken =
                TokenHelper.ReadAndValidateContextToken(contextTokenString, requestProperty.Headers[HttpRequestHeader.Host]);

            string accessToken = TokenHelper.GetAccessToken(contextToken, sharepointUrl.Authority).AccessToken;
            return TokenHelper.GetClientContextWithAccessToken(sharepointUrl.ToString(), accessToken);
        }

        private static string GetAcsMetadataEndpointUrl()
        {
            return Path.Combine(GetAcsGlobalEndpointUrl(), AcsMetadataEndPointRelativeUrl);
        }

        private static string GetFormattedPrincipal(string principalName, string hostName, string realm)
        {
            if (!String.IsNullOrEmpty(hostName))
            {
                return String.Format(CultureInfo.InvariantCulture, "{0}/{1}@{2}", principalName, hostName, realm);
            }
            else
            {
                return String.Format(CultureInfo.InvariantCulture, "{0}@{1}", principalName, realm);
            }
        }

        private static string GetAcsPrincipalName(string realm)
        {
            return GetFormattedPrincipal(AcsPrincipalName, new Uri(GetAcsGlobalEndpointUrl()).Host, realm);
        }

        private static string GetAcsGlobalEndpointUrl()
        {
            return String.Format(CultureInfo.InvariantCulture, "https://{0}.{1}/", GlobalEndPointPrefix, AcsHostUrl);
        }

        private static JsonWebSecurityTokenHandler CreateJsonWebSecurityTokenHandler()
        {
            JsonWebSecurityTokenHandler handler = new JsonWebSecurityTokenHandler();
            handler.Configuration = new Microsoft.IdentityModel.Tokens.SecurityTokenHandlerConfiguration();
            handler.Configuration.AudienceRestriction = new Microsoft.IdentityModel.Tokens.AudienceRestriction(AudienceUriMode.Never);
            handler.Configuration.CertificateValidator = X509CertificateValidator.None;

            List<byte[]> securityKeys = new List<byte[]>();
            securityKeys.Add(Convert.FromBase64String(ClientSecret));
            if (!string.IsNullOrEmpty(SecondaryClientSecret))
            {
                securityKeys.Add(Convert.FromBase64String(SecondaryClientSecret));
            }

            List<SecurityToken> securityTokens = new List<SecurityToken>();
            securityTokens.Add(new MultipleSymmetricKeySecurityToken(securityKeys));

            handler.Configuration.IssuerTokenResolver =
                SecurityTokenResolver.CreateDefaultSecurityTokenResolver(
                new ReadOnlyCollection<SecurityToken>(securityTokens),
                false);
            SymmetricKeyIssuerNameRegistry issuerNameRegistry = new SymmetricKeyIssuerNameRegistry();
            foreach (byte[] securitykey in securityKeys)
            {
                issuerNameRegistry.AddTrustedIssuer(securitykey, GetAcsPrincipalName(ServiceNamespace));
            }
            handler.Configuration.IssuerNameRegistry = issuerNameRegistry;
            return handler;
        }

        private static string GetRealmFromTargetUrl(Uri targetApplicationUri)
        {
            WebRequest request = HttpWebRequest.Create(targetApplicationUri.ToString() + "/_vti_bin/client.svc");
            request.Headers.Add("Authorization: Bearer ");

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                }
            }
            catch (WebException e)
            {
                string bearerResponseHeader = e.Response.Headers["WWW-Authenticate"];
                string realm = bearerResponseHeader.Substring(bearerResponseHeader.IndexOf("Bearer realm=\"") + 14, 36);

                Guid realmGuid;

                if (Guid.TryParse(realm, out realmGuid))
                {
                    return realm;
                }

            }
            return null;
        }

        private static string GetS2SAccessTokenWithClaims(
            string targetApplicationHostName,
            string targetRealm,
            IEnumerable<JsonWebTokenClaim> claims)
        {
            return IssueToken(
                ClientId,
                IssuerId,
                targetRealm,
                SHAREPOINT_PID,
                targetRealm,
                targetApplicationHostName,
                true,
                claims,
                claims == null);
        }

        private static JsonWebTokenClaim[] GetClaimsWithWindowsIdentity(WindowsIdentity identity)
        {
            JsonWebTokenClaim[] claims = new JsonWebTokenClaim[]
            {
                new JsonWebTokenClaim(TokenHelper.NameIdentifierClaimType, identity.User.Value.ToLower()),
                new JsonWebTokenClaim("nii", "urn:office:idp:activedirectory")
            };
            return claims;
        }

        private static string IssueToken(
            string sourceApplication,
            string issuerApplication,
            string sourceRealm,
            string targetApplication,
            string targetRealm,
            string targetApplicationHostName,
            bool trustedForDelegation,
            IEnumerable<JsonWebTokenClaim> claims,
            bool appOnly = false)
        {
            if (null == SigningCredentials)
            {
                throw new InvalidOperationException("SigningCredentials was not initialized");
            }

            #region Actor token

            string issuer = string.IsNullOrEmpty(sourceRealm) ? issuerApplication : string.Format("{0}@{1}", issuerApplication, sourceRealm);
            string nameid = string.IsNullOrEmpty(sourceRealm) ? sourceApplication : string.Format("{0}@{1}", sourceApplication, sourceRealm);
            string audience = string.Format("{0}/{1}@{2}", targetApplication, targetApplicationHostName, targetRealm);

            List<JsonWebTokenClaim> actorClaims = new List<JsonWebTokenClaim>();
            actorClaims.Add(new JsonWebTokenClaim(JsonWebTokenConstants.ReservedClaims.NameIdentifier, nameid));
            if (trustedForDelegation && !appOnly)
            {
                actorClaims.Add(new JsonWebTokenClaim(TokenHelper.TrustedForImpersonationClaimType, "true"));
            }

            // Create token
            JsonWebSecurityToken actorToken = new JsonWebSecurityToken(
                issuer: issuer,
                audience: audience,
                validFrom: DateTime.UtcNow,
                validTo: DateTime.UtcNow.AddMinutes(TokenLifetimeMinutes),
                signingCredentials: SigningCredentials,
                claims: actorClaims);

            string actorTokenString = new JsonWebSecurityTokenHandler().WriteTokenAsString(actorToken);

            if (appOnly)
            {
                // App-only token is the same as actor token for delegated case
                return actorTokenString;
            }

            #endregion Actor token

            #region Outer token

            List<JsonWebTokenClaim> outerClaims = null == claims ? new List<JsonWebTokenClaim>() : new List<JsonWebTokenClaim>(claims);
            outerClaims.Add(new JsonWebTokenClaim(ActorTokenClaimType, actorTokenString));

            JsonWebSecurityToken jsonToken = new JsonWebSecurityToken(
                nameid, // outer token issuer should match actor token nameid
                audience,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(10),
                outerClaims);

            string accessToken = new JsonWebSecurityTokenHandler().WriteTokenAsString(jsonToken);

            #endregion Outer token

            return accessToken;
        }

        private static string EnsureTrailingSlash(string url)
        {
            if (!String.IsNullOrEmpty(url) && url[url.Length - 1] != '/')
            {
                return url + "/";
            }
            else
            {
                return url;
            }
        }

        #endregion

        #region AcsMetadataParser

        // This class is used to get MetaData document from the global STS endpoint. It contains
        // methods to parse the MetaData document and get endpoints and STS certificate.
        public static class AcsMetadataParser
        {
            public static X509Certificate2 GetAcsSigningCert(string realm)
            {
                JsonMetadataDocument document = GetMetadataDocument(realm);

                if (null != document.keys && document.keys.Count > 0)
                {
                    JsonKey signingKey = document.keys[0];

                    if (null != signingKey && null != signingKey.keyValue)
                    {
                        return new X509Certificate2(Encoding.UTF8.GetBytes(signingKey.keyValue.value));
                    }
                }

                throw new Exception("Metadata document does not contain ACS signing certificate.");
            }

            public static string GetDelegationServiceUrl(string realm)
            {
                JsonMetadataDocument document = GetMetadataDocument(realm);

                JsonEndpoint delegationEndpoint = document.endpoints.SingleOrDefault(e => e.protocol == DelegationIssuance);

                if (null != delegationEndpoint)
                {
                    return delegationEndpoint.location;
                }
                else
                {
                    throw new Exception("Metadata document does not contain Delegation Service endpoint Url");
                }
            }

            private static JsonMetadataDocument GetMetadataDocument(string realm)
            {
                string acsMetadataEndpointUrlWithRealm = String.Format(CultureInfo.InvariantCulture, "{0}?realm={1}",
                                                                        GetAcsMetadataEndpointUrl(),
                                                                        realm);
                byte[] acsMetadata;
                using (WebClient webClient = new WebClient())
                {

                    acsMetadata = webClient.DownloadData(acsMetadataEndpointUrlWithRealm);
                }
                string jsonResponseString = Encoding.UTF8.GetString(acsMetadata);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                JsonMetadataDocument document = serializer.Deserialize<JsonMetadataDocument>(jsonResponseString);

                if (null == document)
                {
                    throw new Exception("No metadata document found at the global endpoint " + acsMetadataEndpointUrlWithRealm);
                }

                return document;
            }

            public static string GetStsUrl(string realm)
            {
                JsonMetadataDocument document = GetMetadataDocument(realm);

                JsonEndpoint s2sEndpoint = document.endpoints.SingleOrDefault(e => e.protocol == S2SProtocol);

                if (null != s2sEndpoint)
                {
                    return s2sEndpoint.location;
                }
                else
                {
                    throw new Exception("Metadata document does not contain STS endpoint url");
                }
            }

            private class JsonMetadataDocument
            {
                public string serviceName { get; set; }
                public List<JsonEndpoint> endpoints { get; set; }
                public List<JsonKey> keys { get; set; }
            }

            private class JsonEndpoint
            {
                public string location { get; set; }
                public string protocol { get; set; }
                public string usage { get; set; }
            }

            private class JsonKeyValue
            {
                public string type { get; set; }
                public string value { get; set; }
            }

            private class JsonKey
            {
                public string usage { get; set; }
                public JsonKeyValue keyValue { get; set; }
            }
        }

        #endregion
    }

    /// <summary>
    /// A JsonWebSecurityToken generated by SharePoint to authenticate to a 3rd party application and allow callbacks using a refresh token
    /// </summary>
    public class SharePointContextToken : JsonWebSecurityToken
    {
        public static SharePointContextToken Create(JsonWebSecurityToken contextToken)
        {
            return new SharePointContextToken(contextToken.Issuer, contextToken.Audience, contextToken.ValidFrom, contextToken.ValidTo, contextToken.Claims);
        }

        public SharePointContextToken(string issuer, string audience, DateTime validFrom, DateTime validTo, IEnumerable<JsonWebTokenClaim> claims)
            : base(issuer, audience, validFrom, validTo, claims)
        {
        }

        public SharePointContextToken(string issuer, string audience, DateTime validFrom, DateTime validTo, IEnumerable<JsonWebTokenClaim> claims, SecurityToken issuerToken, JsonWebSecurityToken actorToken)
            : base(issuer, audience, validFrom, validTo, claims, issuerToken, actorToken)
        {
        }

        public SharePointContextToken(string issuer, string audience, DateTime validFrom, DateTime validTo, IEnumerable<JsonWebTokenClaim> claims, SigningCredentials signingCredentials)
            : base(issuer, audience, validFrom, validTo, claims, signingCredentials)
        {
        }

        public string NameId
        {
            get
            {
                return GetClaimValue(this, "nameid");
            }
        }

        /// <summary>
        /// The principal name portion of the context token's "appctxsender" claim
        /// </summary>
        public string TargetPrincipalName
        {
            get
            {
                string appctxsender = GetClaimValue(this, "appctxsender");

                if (appctxsender == null)
                {
                    return null;
                }

                return appctxsender.Split('@')[0];
            }
        }

        /// <summary>
        /// The context token's "refreshtoken" claim
        /// </summary>
        public string RefreshToken
        {
            get
            {
                return GetClaimValue(this, "refreshtoken");
            }
        }

        /// <summary>
        /// The context token's "CacheKey" claim
        /// </summary>
        public string CacheKey
        {
            get
            {
                string appctx = GetClaimValue(this, "appctx");
                if (appctx == null)
                {
                    return null;
                }

                ClientContext ctx = new ClientContext("http://tempuri.org");
                Dictionary<string, object> dict = (Dictionary<string, object>)ctx.ParseObjectFromJsonString(appctx);
                string cacheKey = (string)dict["CacheKey"];

                return cacheKey;
            }
        }

        /// <summary>
        /// The context token's "SecurityTokenServiceUri" claim
        /// </summary>
        public string SecurityTokenServiceUri
        {
            get
            {
                string appctx = GetClaimValue(this, "appctx");
                if (appctx == null)
                {
                    return null;
                }

                ClientContext ctx = new ClientContext("http://tempuri.org");
                Dictionary<string, object> dict = (Dictionary<string, object>)ctx.ParseObjectFromJsonString(appctx);
                string cacheKey = (string)dict["SecurityTokenServiceUri"];

                return cacheKey;
            }
        }

        /// <summary>
        /// The realm portion of the context token's "audience" claim
        /// </summary>
        public string Realm
        {
            get
            {
                string aud = this.Audience;
                if (aud == null)
                {
                    return null;
                }

                string tokenRealm = aud.Substring(aud.IndexOf('@') + 1);

                return tokenRealm;
            }
        }

        private static string GetClaimValue(JsonWebSecurityToken token, string claimType)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            foreach (JsonWebTokenClaim claim in token.Claims)
            {
                if (StringComparer.Ordinal.Equals(claim.ClaimType, claimType))
                {
                    return claim.Value;
                }
            }

            return null;
        }

    }

    public class OAuthTokenPair
    {
        public string AccessToken;
        public string RefreshToken;
    }

    /// <summary>
    /// Represents a security token which contains multiple security keys that are generated using symmetric algorithms.
    /// </summary>
    public class MultipleSymmetricKeySecurityToken : SecurityToken
    {
        /// <summary>
        /// Initializes a new instance of the MultipleSymmetricKeySecurityToken class.
        /// </summary>
        /// <param name="keys">An enumeration of Byte arrays that contain the symmetric keys.</param>
        public MultipleSymmetricKeySecurityToken(IEnumerable<byte[]> keys)
            : this(UniqueId.CreateUniqueId(), keys)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultipleSymmetricKeySecurityToken class.
        /// </summary>
        /// <param name="id">The unique identifier of the security token.</param>
        /// <param name="keys">An enumeration of Byte arrays that contain the symmetric keys.</param>
        public MultipleSymmetricKeySecurityToken(string id, IEnumerable<byte[]> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Value cannot be a null or empty string.", "id");
            }

            foreach (byte[] key in keys)
            {
                if (key.Length <= 0)
                {
                    throw new ArgumentException("The key length must be greater then zero.", "keys");
                }
            }

            this.id = id;
            this.effectiveTime = DateTime.UtcNow;
            this.securityKeys = CreateSymmetricSecurityKeys(keys);
        }

        /// <summary>
        /// Gets the unique identifier of the security token.
        /// </summary>
        public override string Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// Gets the cryptographic keys associated with the security token.
        /// </summary>
        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                return this.securityKeys.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the first instant in time at which this security token is valid.
        /// </summary>
        public override DateTime ValidFrom
        {
            get
            {
                return this.effectiveTime;
            }
        }

        /// <summary>
        /// Gets the last instant in time at which this security token is valid.
        /// </summary>
        public override DateTime ValidTo
        {
            get
            {
                // Never expire
                return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the key identifier for this instance can be resolved to the specified key identifier.
        /// </summary>
        /// <param name="keyIdentifierClause">A SecurityKeyIdentifierClause to compare to this instance</param>
        /// <returns>true if keyIdentifierClause is a SecurityKeyIdentifierClause and it has the same unique identifier as the Id property; otherwise, false.</returns>
        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (keyIdentifierClause == null)
            {
                throw new ArgumentNullException("keyIdentifierClause");
            }

            // Since this is a symmetric token and we do not have IDs to distinguish tokens, we just check for the
            // presence of a SymmetricIssuerKeyIdentifier. The actual mapping to the issuer takes place later
            // when the key is matched to the issuer.
            if (keyIdentifierClause is SymmetricIssuerKeyIdentifierClause)
            {
                return true;
            }
            return base.MatchesKeyIdentifierClause(keyIdentifierClause);
        }

        #region private members

        private List<SecurityKey> CreateSymmetricSecurityKeys(IEnumerable<byte[]> keys)
        {
            List<SecurityKey> symmetricKeys = new List<SecurityKey>();
            foreach (byte[] key in keys)
            {
                symmetricKeys.Add(new InMemorySymmetricSecurityKey(key));
            }
            return symmetricKeys;
        }

        private string id;
        private DateTime effectiveTime;
        private List<SecurityKey> securityKeys;

        #endregion
    }
}
