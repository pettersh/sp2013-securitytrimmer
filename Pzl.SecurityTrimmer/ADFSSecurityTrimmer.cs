using Microsoft.IdentityModel.Claims;
using Microsoft.Office.Server.Search.Administration;
using Microsoft.Office.Server.Search.Query;
using Microsoft.SharePoint.Administration.Claims;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Pzl.SecurityTrimmer
{
    /// <summary>
    /// Security trimmer that allows internal users to search for external content, 
    /// by adding ADFS/SAML claims for current user's windows claims.
    /// </summary>
    public class ADFSSecurityTrimmer : ISecurityTrimmerPre
    {
        // Trusted Provider name
        private string _claimIssuer = SPOriginalIssuers.Format(SPOriginalIssuerType.TrustedProvider, "ADFS");

        /// <summary>
        /// Initialize the custom security trimmer
        /// </summary>
        /// <param name="properties">Static properties configured for the trimmer</param>
        /// <param name="searchApplication">Search Service Application instance</param>
        public void Initialize(NameValueCollection properties, SearchServiceApplication searchApplication)
        {
        }

        /// <summary>
        /// Add custom claims to the query tree for current user
        /// </summary>
        /// <param name="sessionProperties">Session properties collection</param>
        /// <param name="windowsIdentity">Identity of the user sending the query</param>
        /// <returns>An enumerable of tuples of additional claims</returns>
        public IEnumerable<Tuple<Claim, bool>> AddAccess(IDictionary<string, object> sessionProperties, IIdentity windowsIdentity)
        {
            return null;
        }
    }
}
