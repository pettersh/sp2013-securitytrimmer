#
# Before running script, ensure you have SSA administrator rights
#
# 1) Install DLL in GAC on each host running Query component
# 2) Register the security trimmer in SharePoint
# 3) Restart host controller on each host running Query component

# ------------------------------------------------------------
# Install custom security trimmer to Global Assembly Cache (GAC)
# ------------------------------------------------------------
[System.Reflection.Assembly]::Load("System.EnterpriseServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
$dll = Resolve-Path -Path "C:\dev-git\sp2013-securitytrimmer\Pzl.SecurityTrimmer\bin\Debug\Pzl.SecurityTrimmer.dll"		
$publish = New-Object System.EnterpriseServices.Internal.Publish
$publish.GacInstall($dll)

# ------------------------------------------------------------
# Register custom security trimmer in SharePoint
# ------------------------------------------------------------
$ssa = Get-SPEnterpriseSearchServiceApplication -Identity "Search Service Application"
$typeName="Pzl.SecurityTrimmer.ADFSSecurityTrimmer, Pzl.SecurityTrimmer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=931bf74279825c08"
$connectionString="data source=sql.contoso.com;Integrated Security=true;Initial Catalog=Extranet"
New-SPEnterpriseSearchSecurityTrimmer -Id 1 -SearchApplication $ssa -TypeName $typeName -Properties connectionString~$($connectionString)	

# ------------------------------------------------------------
# Restart the query component for changes to be effective
# ------------------------------------------------------------
Restart-Service -Name "SPSearchHostController"