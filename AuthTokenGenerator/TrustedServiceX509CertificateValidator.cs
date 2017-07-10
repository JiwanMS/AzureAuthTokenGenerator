
namespace AuthTokenGenerator
{
    using System;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.Security.Cryptography.X509Certificates;

    public class TrustedServiceX509CertificateValidator : X509CertificateValidator
    {
        /// <summary>
        /// The allowed issuer name for the certificate to validate
        /// </summary>
        private readonly string allowedIssuerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrustedServiceX509CertificateValidator" /> class.
        /// </summary>
        /// <param name="allowedIssuerName">The allowed issuer name</param>
        public TrustedServiceX509CertificateValidator(string allowedIssuerName)
        {
            if (string.IsNullOrEmpty(allowedIssuerName))
            {
                throw new ArgumentNullException("allowedIssuerName");
            }

            this.allowedIssuerName = allowedIssuerName;
        }

        /// <summary>
        /// Validates the certificate
        /// </summary>
        /// <param name="certificate">The certificate to validate</param>
        public override void Validate(X509Certificate2 certificate)
        {
            // Check that there is a certificate.
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }

            // Check that the certificate issuer matches the configured issuer
            if (this.allowedIssuerName != certificate.IssuerName.Name)
            {
                throw new SecurityTokenValidationException("Certificate was not issued by a trusted issuer");
            }
        }
    }
}
