using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AuthTokenGenerator
{
    public class AuthTokenValidator
    {
        /// <summary>
        /// The issuer
        /// </summary>
        private static string issuer = string.Empty;

        /// <summary>
        /// The signing tokens
        /// </summary>
        private static List<X509SecurityToken> signingTokens = null;

        /// <summary>
        /// The metadata retrieval time
        /// </summary>
        private static DateTime stsMetadataRetrievalTime = DateTime.MinValue;

        /// <summary>
        /// The Token handler
        /// </summary>
        private static ISecurityTokenValidator tokenHandler;

        /// <summary>
        /// Parses the federation metadata document and gets issuer Name and Signing Certificates
        /// </summary>
        /// <param name="metadataAddress">URL of the Federation Metadata document</param>
        private static void GetTenantInformation(string metadataAddress, string certIssuerName)
        {
            // The issuer and signingTokens are cached for 24 hours. They are updated if any of the conditions in the if condition is true.            
            if ((DateTime.UtcNow.Subtract(stsMetadataRetrievalTime).TotalHours > 24)
                 || string.IsNullOrEmpty(issuer)
                 || signingTokens == null)
            {
                var serializer = new MetadataSerializer()
                {
                    CertificateValidationMode = X509CertificateValidationMode.Custom,
                    CertificateValidator = new TrustedServiceX509CertificateValidator(certIssuerName)
                };

                MetadataBase metadata = serializer.ReadMetadata(XmlReader.Create(metadataAddress));

                var entityDescriptor = (EntityDescriptor)metadata;

                // get the issuer name
                if (!string.IsNullOrWhiteSpace(entityDescriptor.EntityId.Id))
                {
                    issuer = entityDescriptor.EntityId.Id;
                }

                // get the signing certs
                signingTokens = ReadSigningCertsFromMetadata(entityDescriptor);

                stsMetadataRetrievalTime = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// The method to read the signing certs from metadata
        /// </summary>
        /// <param name="entityDescriptor">The entity descriptor</param>
        /// <returns>The security token</returns>
        private static List<X509SecurityToken> ReadSigningCertsFromMetadata(EntityDescriptor entityDescriptor)
        {
            var stsSigningTokens = new List<X509SecurityToken>();

            SecurityTokenServiceDescriptor stsd = entityDescriptor.RoleDescriptors.OfType<SecurityTokenServiceDescriptor>().First();

            if (stsd != null)
            {
                IEnumerable<X509RawDataKeyIdentifierClause> x509DataClauses = stsd.Keys.Where(key => key.KeyInfo != null && (key.Use == KeyType.Signing || key.Use == KeyType.Unspecified)).Select(key => key.KeyInfo.OfType<X509RawDataKeyIdentifierClause>().First());

                stsSigningTokens.AddRange(x509DataClauses.Select(token => new X509SecurityToken(new X509Certificate2(token.GetX509RawData()))));
            }
            else
            {
                throw new InvalidOperationException("There is no RoleDescriptor of type SecurityTokenServiceType in the metadata");
            }

            return stsSigningTokens;
        }

        /// <summary>
        /// Authenticates the authorization header
        /// </summary>
        /// <param name="authHeader">The authorization header</param>
        /// <param name="authority">The authority.</param>
        /// <param name="audience">The audience</param>
        /// <returns>True if the user is authorized</returns>
        public static bool Authenticate(string jwtToken, string authority, string audience, string certIssuerName)
        {
            string stsMetadataAddress = string.Format("{0}/federationmetadata/2007-06/federationmetadata.xml", authority);

            // Get tenant information that's used to validate incoming jwt tokens
            GetTenantInformation(stsMetadataAddress, certIssuerName);

            if (tokenHandler == null)
            {
                tokenHandler = new JwtSecurityTokenHandler();
            }

            SecurityToken validatedToken;

            var validationParameters =
                 new TokenValidationParameters()
                 {
                     ValidAudience = audience,
                     ValidIssuer = issuer,
                     IssuerSigningTokens = signingTokens
                 };

            // Validate token
            ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(jwtToken, validationParameters, out validatedToken);
                        
            // if the token is scoped, verify that required permission is set in the scope claim
            if ((ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/scope") != null) && (ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/scope").Value != "user_impersonation"))
            {
                return false;
            }

            return true;
        }
    }
}
