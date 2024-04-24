# One-liner from https://stackoverflow.com/questions/7690994/running-a-command-as-administrator-using-powershell
if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) { Start-Process powershell.exe "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs; exit }

Clear-Host

# VARIABLES
$BuildPackage = $true
$DeployPackage = $false
$DeployPackageInHQOnly = $false
$ReleaseDir = "D:\Romain_dev\Applications\PI-Replication-Tool\Application\bin\Release"
$PackageDir = Join-Path $ReleaseDir "PI-Replication-Tool"
$DirectoriesToCreate = "Application", "Input", "Output", "Log", "Documentation"
$MSBuildPath = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\"
$CSProjPath = "D:\Romain_dev\Applications\PI-Replication-Tool\Application\PI-Replication-Tool.csproj.user"
$BatFile = "D:\Romain_dev\Applications\PI-Replication-Tool\Déploiement\PI Replication Tool.bat"
$DocFile = "D:\Romain_dev\Applications\PI-Replication-Tool\Documentation\EP_PI-ReplicationTool_User-Manual.pdf"
[System.Collections.ArrayList]$UNCPathList= @(
    "\\OPEPPA-WPPIHQ05\PI-Replication-Tool",
	"\\OPEPPA-WRPIAO01\PI-Replication-Tool",
    "\\OPEPPA-WRPIAR01\PI-Replication-Tool",
	"\\OPEPPA-WRPIBR01\PI-Replication-Tool",
	"\\OPEPPA-WRPICG01\PI-Replication-Tool",
	"\\OPEPPA-WRPIDK01\PI-Replication-Tool",
	"\\OPEPPA-WRPIGB01\PI-Replication-Tool",
	"\\OPEPPA-WRPIIT01\PI-Replication-Tool",
	"\\OPEPPA-WRPING01\PI-Replication-Tool",
	"\\OPEPPA-WRPING02\PI-Replication-Tool",
	"\\OPEPPA-WRPINL01\PI-Replication-Tool",
	"\\OPEPPA-WRPIQA01\PI-Replication-Tool",
	#"\\GBEPABZ-APMIS03\PI-Replication-Tool"  #Identity "PIPerfmon" n'existe pas sur le serveur : déploiement manuel pour ne pas casser la réplication.
	"\\AOEPTTA-APPIL01\PI-Replication-Tool",
	"\\AREPBUE-APPI01\PI-Replication-Tool",
	"\\NGEPLOS-APPIS01\PI-Replication-Tool",
	"\\NGEPPHC-APPI01\PI-Replication-Tool",
	"\\BREPRIO-APPI01\PI-Replication-Tool"
)

# TREATMENT
if($BuildPackage)
{
	Write-Host "Starting packaging of PI Replication Tool using directory : $SourcePathToCopy"
     
    #Delete Old package
    Write-Host "Removing old $PackageDir ..." -ForegroundColor Gray
    if(Test-Path -Path $PackageDir) { Remove-Item $PackageDir -recurse}
    Write-Host "Removing old $PackageDir OK" -ForegroundColor Green

    #Building new package with MSBuild
    Write-Host "Building new package using MSBuild ..." -ForegroundColor Gray
    cd $MSBuildPath
    .\MSBuild "D:\Romain_dev\Applications\PI-Replication-Tool\Application\PI-Replication-Tool.sln" /property:Configuration=Release | Out-Null
    Write-Host "Building new package using MSBuild OK" -ForegroundColor Green
    
    #Create folders in the package
    Write-Host "Creating folders in the package (Application, Input, Output, Log) ..." -ForegroundColor Gray
	$Package = Get-ChildItem -Path $ReleaseDir
    New-Item -Path $ReleaseDir -Name "PI-Replication-Tool" -ItemType "directory" | Out-Null
	Foreach ($CurrentDir in $DirectoriesToCreate)
	{
		New-Item -Path $PackageDir -Name $CurrentDir -ItemType "directory"| Out-Null
	}
    Write-Host "Creating folders in the package (Application, Input, Output, Log) OK" -ForegroundColor Green

    #Move files from release to package
    Write-Host "Moving Files from the release to the package ..." -ForegroundColor Gray
	Copy-Item $BatFile -Destination $PackageDir 
    New-Item (Join-Path $PackageDir "Input\Input.txt") | Out-Null
	Foreach ($p in $Package)
    {
		Move-Item (Join-Path $ReleaseDir $p.Name) -Destination (Join-Path $PackageDir "Application")
	}
	Copy-Item $DocFile -Destination (Join-Path $PackageDir "Documentation") 
    Write-Host "Moving Files from the release to the package OK" -ForegroundColor Green

    #Provide Everyone access to the package
    Write-Host "Configuring security access in the package ..." -ForegroundColor Gray
    $fileSystemAccessRuleArgumentList = "Everyone", "FullControl" , "Allow"
    $fileSystemAccessRule = New-Object -TypeName System.Security.AccessControl.FileSystemAccessRule -ArgumentList $fileSystemAccessRuleArgumentList

    $Files = Get-ChildItem -Path $PackageDir -Recurse
    foreach($File in $Files) 
    { 
        $NewAcl = Get-Acl -Path $File.FullName
        $NewAcl.SetAccessRule($fileSystemAccessRule)
        Set-Acl -Path $File.FullName -AclObject $NewAcl
    }
    Write-Host "Configuring security access in the package OK" -ForegroundColor Green

}

if($DeployPackageInHQOnly)
{
	#Robocopy the package to HQ
	Write-Host "`nStarting Deployement of PI Replication Tool in HQ"
    Robocopy $PackageDir "\\OPEPPA-WPPIHQ05\PI-Replication-Tool" /MIR /Z /MT /COPYALL | OUT-Null
    Write-Host "Processing Robocopy for" $UNCpath "OK" -ForegroundColor Green  
}
elseif($DeployPackage)
{
    Write-Host "`nStarting Deployement of PI Replication Tool"
    foreach($UNCpath in $UNCPathList)
    {
        #Robocopy the package to servers
        Robocopy $PackageDir $UNCpath /MIR /Z /MT /COPYALL | OUT-Null
        Write-Host "Processing Robocopy for" $UNCpath "OK" -ForegroundColor Green  
    }
}

Set-Location -path $PSScriptRoot
Read-Host 'End of script. Press Enter to end.'