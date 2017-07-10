// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureAuthenticationHelper.cs" company="Microsoft"> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AuthTokenGenerator
{
    using System;
    using System.Globalization;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// Helper Class for enabling Azure AD Token operations
    /// </summary>
    public static class AzureAuthenticationHelper
    {
        /// <summary>
        /// The method for getting the authorization token from Azure AD
        /// </summary>
        /// <param name="clientId">The client ID of the AD application.</param>
        /// <param name="clientSecret">The client secret of the AD application</param>
        /// <param name="apiResourceId">The resource id of the API.</param>
        /// <param name="tenant">The Azure AD tenant</param>
        /// <param name="aadInstance">The Azure AD instance</param>
        /// <returns>
        /// Azure authorization result
        /// </returns>
        public static AzureAuthResult ObtainAuthToken(string clientId, string clientSecret, string apiResourceId, string tenant, string aadInstance)
        {
            var azureAuthResult = new AzureAuthResult();

            if (string.IsNullOrEmpty(tenant) || string.IsNullOrEmpty(aadInstance) || string.IsNullOrEmpty(apiResourceId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentException(
                    string.Format(
                        "{0}, {1}, {2}, {3}, {4} must be present in configuration.",
                        tenant,
                        aadInstance,
                        apiResourceId,
                        clientId,
                        clientSecret));
            }

            if (!aadInstance.Contains("{0}"))
            {
                throw new ArgumentException("AAD instance configuration should have a placeholder for tenant");
            }

            var authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
            var authContext = new AuthenticationContext(authority);
            var cred = new ClientCredential(clientId, clientSecret);
            authContext.TokenCache.Clear();

            var authResult = authContext.AcquireToken(apiResourceId, cred);
            azureAuthResult.AuthToken = authResult.AccessToken;
            azureAuthResult.ExpiresOn = authResult.ExpiresOn;
            return azureAuthResult;
        }
    }
}
