using Microsoft.IdentityModel.Claims;
using Microsoft.Office.Server.Search.Administration;
using Microsoft.Office.Server.Search.Query;
using Microsoft.SharePoint.Administration.Claims;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Security.Principal;

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

        // SQL connection
        private string _connectionString;
        
        /// <summary>
        /// Initialize the custom security trimmer
        /// </summary>
        /// <param name="properties">Static properties configured for the trimmer</param>
        /// <param name="searchApplication">Search Service Application instance</param>
        public void Initialize(NameValueCollection properties, SearchServiceApplication searchApplication)
        {
            if (properties.Get("connectionString") != null)
            {
                _connectionString = properties.Get("connectionString");
            }
            else
            {
                throw new ArgumentException("Missing property \"connectionString\"");
            }
        }

        /// <summary>
        /// Add custom claims to the query tree for current user
        /// </summary>
        /// <param name="sessionProperties">Session properties collection</param>
        /// <param name="windowsIdentity">Identity of the user sending the query</param>
        /// <returns>An enumerable of tuples of additional claims</returns>
        public IEnumerable<Tuple<Claim, bool>> AddAccess(IDictionary<string, object> sessionProperties, IIdentity windowsIdentity)
        {
            var samlClaims = new LinkedList<Tuple<Claim, bool>>();
            if (windowsIdentity == null)
            {
                return samlClaims;
            }

            ClaimCollection windowsClaims = ((IClaimsIdentity)windowsIdentity).Claims;
            foreach (var claim in windowsClaims)
            {

                // Find the current user's identity claim of type
                // http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid
                if (SPClaimTypes.Equals(ClaimTypes.PrimarySid, claim.ClaimType))
                {
                    // Lookup the external id for current user and create an external identity claim using
                    // http://schemas.xmlsoap.org/ws/2005/05/identity/claims/privatepersonalidentifier
                    var externalId = Lookup(claim.Value);
                    if (null != externalId)
                    {
                        var pidClaim = new Claim(ClaimTypes.PPID, externalId, ClaimValueTypes.String, _claimIssuer);
                        samlClaims.AddLast(new Tuple<Claim, bool>(pidClaim, false));
                    }
                    else
                    {
                        // "Current user with sid '{0}' doesn't have an external Id", claim.Value
                        // or we have a database connectivity issue.
                    }
                }

                // Find the current user's windows group claims of type
                // http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid
                if (SPClaimTypes.Equals(ClaimTypes.GroupSid, claim.ClaimType))
                {
                    // Use the group sids and create external group claims of type
                    // http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid
                    var groupSidClaim = new Claim(ClaimTypes.GroupSid, claim.Value, ClaimValueTypes.String, _claimIssuer);
                    samlClaims.AddLast(new Tuple<Claim, bool>(groupSidClaim, false));
                }
            }
            return samlClaims;
        }

        /// <summary>
        /// Lookup external id based on CONTOSO sid
        /// </summary>
        /// <param name="key">sid</param>
        /// <returns>external id</returns>
        public string Lookup(string sid)
        {
            string externalId = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(
                    "SELECT TOP 1 Id from Users WHERE Sid = @Sid AND Domain = 'CONTOSO'", connection))
                {
                    command.Parameters.AddWithValue("@Sid", sid);
                    try
                    {
                        connection.Open();                        
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            externalId = reader["Id"].ToString();
                        }
                    }
                    catch (Exception)
                    {
                        // Log the event, but don't throw the exception 
                        // to allow query to continue - with to few claims (potentially)
                    }
                }
            }
            return externalId;
        }
    }
}
