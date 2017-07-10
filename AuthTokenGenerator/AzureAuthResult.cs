// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureAuthResult.cs" company="Microsoft"> 
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

    /// <summary>
    /// Azure authentication Result
    /// </summary>
    public class AzureAuthResult
    {
        /// <summary>
        /// Gets or sets the authentication token.
        /// </summary>
        /// <value>
        /// The authentication token.
        /// </value>
        public string AuthToken { get; set; }

        /// <summary>
        /// Gets or sets the token expires on.
        /// </summary>
        /// <value>
        /// The token expires on.
        /// </value>
        public DateTimeOffset ExpiresOn { get; set; }
    }
}
