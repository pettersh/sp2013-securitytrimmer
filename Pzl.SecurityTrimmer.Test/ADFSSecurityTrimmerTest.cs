using Microsoft.IdentityModel.Claims;
using Microsoft.Office.Server.Search.Administration;
using Microsoft.Office.Server.Search.Query;
using Microsoft.SharePoint.Administration.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;

namespace Pzl.SecurityTrimmer.Test
{
    [TestClass]
    public class ADFSSecurityTrimmerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Instantiated by SharePoint Search Host Controller (Query Component)
            ISecurityTrimmerPre trimmer = new ADFSSecurityTrimmer();

            NameValueCollection properties = new NameValueCollection();
            properties["connectionString"] = "dummy";

            trimmer.Initialize(properties, null);

            //var claims = new Mock<ClaimCollection>();
            //claims.Object.Add(new Claim(ClaimTypes.PrimarySid, "S-1-5-21-606747145-796845957-725345543-571903", ClaimValueTypes.String, "Windows"));

            var identityMock = new Mock<WindowsClaimsIdentity>() { DefaultValue = DefaultValue.Mock};
            
            var claimsMock = Mock.Get(identityMock.Object.Claims);
            
            //identity.Setup(windowsClaimsIdentity => windowsClaimsIdentity.Claims).Returns(claims.Object);

            //trimmer.AddAccess(null, identity.Object);
        }
    }
}
