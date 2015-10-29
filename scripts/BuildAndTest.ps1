#Assumed to be run from root of project
trap
{
    $ErrorActionPreference = "Continue";   
    Write-Error $_
    exit 1
}
$nuget = "src\\.nuget\\nuget.exe"
$msbuild = "C:\\Program Files (x86)\\MSBuild\\12.0\\Bin\\MSBuild.exe"
$buildOutput = join-path (convert-Path .) buildoutput
$solutions = get-childitem "src" *.sln 

if(test-path $buildOutput){
	rm $buildOutput -Force -Recurse
}

foreach ($solution in $solutions) {
    & $nuget restore $solution.FullName
    & $msbuild /target:Build $solution.FullName /p:OutputPath=$buildOutput
    if (!$?) {
        exit 1
    }
}

$xunitRunner = "src\packages\xunit.runner.console.2.0.0\tools\xunit.console.exe"
$testAssemblies = get-childitem $buildOutput *.Tests.dll 
write-output "Running tests..."
foreach ($testAssembly in $testAssemblies) {
	& $xunitRunner $testAssembly.FullName
	if (!$?) {
			$exitCode = 1
	}
}
if(test-path ("$buildOutput\BDDfy.html")){
	write-output "Running tests complete."
	write-output "Results available at $buildOutput\BDDfy.html"
}
else{
	write-error"Running tests complete. Results file is missing"	
}
