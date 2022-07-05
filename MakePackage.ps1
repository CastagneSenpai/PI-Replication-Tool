$ReleaseFolder = Join-Path $PSScriptRoot -ChildPath "Application\bin\release"
$TargetFolder = "D:\PI\Applications\PI Replication Tool"

$Folders = @('Logs', 'Input', 'Output')

#Preparation architecture target

foreach($folder in $Folders) {
$path = Join-Path $TargetFolder -ChildPath $folder
if(Test-Path -Path $path) {
    # existe deja
    write-host("existe deja")
} else {
    New-Item -ItemType Directory -Path (Join-Path $TargetFolder -ChildPath $folder)     
}
}



#write-host($ReleaseFolder)

