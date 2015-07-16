#Assumed to be run from root of project
$nuget = "src\\.nuget\\nuget.exe"
$msbuild = "C:\\Program Files (x86)\\MSBuild\\12.0\\Bin\\MSBuild.exe"
$buildOutput = join-path (convert-Path .) buildoutput
$solutions = get-childitem "src" *.sln 

foreach ($solution in $solutions) {
    & $nuget restore $solution.FullName
    & $msbuild /target:Build $solution.FullName /p:OutputPath=$buildOutput
    if (!$?) {
        exit 1
    }
}


$xunitRunner = "src\packages\xunit.runner.console.2.0.0\tools\xunit.console.exe"
$testAssemblies = get-childitem $buildOutput *.Specifications.dll 

foreach ($testAssembly in $testAssemblies) {
	& $xunitRunner $testAssembly.FullName
	if (!$?) {
			$exitCode = 1
	}
}