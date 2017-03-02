properties {
  $buildVersion
  $projectName = "MicroLite.Extensions.WebApi"
  $baseDir = Resolve-Path .
  $buildDir = "$baseDir\build"
  $helpDir = "$buildDir\help\"
  $msbuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"

  $builds = @(
    @{ Name = "NET45"; BuildDir="$buildDir\4.5\"; Framework="v4.5" }
  )
}

Task Default -depends RunTests

Task Clean {
  Write-Host "Cleaning $projectName Build Directory" -ForegroundColor Green
  foreach ($build in $builds) {
    $outDir = $build.BuildDir
    Remove-Item -force -recurse $outDir -ErrorAction SilentlyContinue
  }
  Remove-Item -force -recurse $helpDir -ErrorAction SilentlyContinue
  Write-Host
}

Task Build -Depends Clean {
  foreach ($build in $builds) {
    $name = $build.Name
    Write-Host "Building $projectName.sln" -ForegroundColor Green

    $outDir = $build.BuildDir
    $netVer = $build.Framework
    &"$msbuild" "$projectName.sln" "/target:Clean;Rebuild" "/property:Configuration=Release;WarningLevel=1;OutDir=$outDir;TargetFrameworkVersion=$netVer" /verbosity:quiet
  }
  Write-Host
}

Task RunTests -Depends Build {
  foreach ($build in $builds) {
    Write-Host "Running $projectName.Tests" -ForegroundColor Green

    $outDir = $build.BuildDir
    Exec { & $baseDir\packages\xunit.runners.1.9.2\tools\xunit.console.clr4.exe "$outDir\$projectName.Tests.dll" }
  }
  Write-Host
}