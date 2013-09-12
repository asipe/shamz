function Configure() {
  $root = '.'
  $src = Join-Path $root 'src'
  $thirdparty = Join-Path $root 'thirdparty'
  $debug = Join-Path $root 'debug'

  return @{
    thirdparty_dir = $thirdparty
    src_dir = $src
    packages_dir = Join-Path $thirdparty 'packages'
    debug_dir = $debug
    deploy_dir = Join-Path $root 'deploy'
  }
}

$config = Configure

function CheckLastExitCode() {
  if ($? -eq $false) {
    throw 'Command Failed'
  }
}

function TryDelete($path) {
  if (Test-Path($path)) {
    Remove-Item $path -Verbose -Recurse -Force
  }
}

function Bootstrap() {
  .\thirdparty\nuget\nuget.exe install .\src\Shamz.Nuget.Packages\common\packages.config -OutputDirectory .\thirdparty\packages\common -ExcludeVersion | Write-Host
  CheckLastExitCode
  .\thirdparty\nuget\nuget.exe install .\src\Shamz.Nuget.Packages\net-3.5\packages.config -OutputDirectory .\thirdparty\packages\net-3.5 -ExcludeVersion | Write-Host
  CheckLastExitCode  
  .\thirdparty\nuget\nuget.exe install .\src\Shamz.Nuget.Packages\net-4.5\packages.config -OutputDirectory .\thirdparty\packages\net-4.5 -ExcludeVersion | Write-Host
  CheckLastExitCode
  .\thirdparty\nuget\nuget.exe install .\src\Shamz.Nuget.Packages\net-4.0\packages.config -OutputDirectory .\thirdparty\packages\net-4.0 -ExcludeVersion | Write-Host
  CheckLastExitCode
}

function Clean() {
  TryDelete($config.debug_dir)
}

function CleanAll() {
  Clean
  TryDelete($config.packages_dir)
}

function SetEnv() {
  $env:PATH += ";.\thirdparty\packages\common\NUnit.Runners\tools"
  Write-Host -ForegroundColor Green 'Path information set'
}

function RunUnitTestsVS() {
  Write-Host -ForegroundColor Cyan '----------VS Unit Tests (4.5)-----------'
  .\thirdparty\packages\common\nunit.runners\tools\nunit-console.exe .\src\Shamz.UnitTests\bin\debug\Shamz.UnitTests.dll /nologo /framework:net-4.5 | Write-Host
  CheckLastExitCode
  Write-Host -ForegroundColor Cyan '----------------------------------'
}

function RunUnitTests() {
  Write-Host -ForegroundColor Cyan '-------Debug Unit Tests (3.5)-----------'
  .\thirdparty\packages\common\nunit.runners\tools\nunit-console.exe .\debug\net-3.5\Shamz.UnitTests\Shamz.UnitTests.dll /nologo /framework:net-3.5 | Write-Host
  CheckLastExitCode
  Write-Host -ForegroundColor Cyan '----------------------------------'
  
  Write-Host -ForegroundColor Cyan '-------Debug Unit Tests (4.0)-----------'
  .\thirdparty\packages\common\nunit.runners\tools\nunit-console.exe .\debug\net-4.0\Shamz.UnitTests\Shamz.UnitTests.dll /nologo /framework:net-4.0 | Write-Host
  CheckLastExitCode
  Write-Host -ForegroundColor Cyan '----------------------------------'
  
  Write-Host -ForegroundColor Cyan '-------Debug Unit Tests (4.5)-----------'
  .\thirdparty\packages\common\nunit.runners\tools\nunit-console.exe .\debug\net-4.5\Shamz.UnitTests\Shamz.UnitTests.dll /nologo /framework:net-4.5 | Write-Host
  CheckLastExitCode
  Write-Host -ForegroundColor Cyan '----------------------------------'
}

function RunIntegrationTestsVS() {
  Write-Host -ForegroundColor Cyan '----------VS Integration Tests (4.5)-----------'
  .\thirdparty\packages\common\nunit.runners\tools\nunit-console.exe .\src\Shamz.IntegrationTests\bin\debug\Shamz.IntegrationTests.dll /nologo /framework:net-4.5 | Write-Host
  CheckLastExitCode
  Write-Host -ForegroundColor Cyan '----------------------------------'
}

function RunIntegrationTests() {
  Write-Host -ForegroundColor Cyan '-------Debug Integration Tests (3.5)-----------'
  .\thirdparty\packages\common\nunit.runners\tools\nunit-console.exe .\debug\net-3.5\Shamz.IntegrationTests\Shamz.IntegrationTests.dll /nologo /framework:net-3.5 | Write-Host
  CheckLastExitCode
  Write-Host -ForegroundColor Cyan '----------------------------------'
  
  Write-Host -ForegroundColor Cyan '-------Debug Integration Tests (4.0)-----------'
  .\thirdparty\packages\common\nunit.runners\tools\nunit-console.exe .\debug\net-4.0\Shamz.IntegrationTests\Shamz.IntegrationTests.dll /nologo /framework:net-4.0 | Write-Host
  CheckLastExitCode
  Write-Host -ForegroundColor Cyan '----------------------------------'
  
  Write-Host -ForegroundColor Cyan '-------Debug Integration Tests (4.5)-----------'
  .\thirdparty\packages\common\nunit.runners\tools\nunit-console.exe .\debug\net-4.5\Shamz.IntegrationTests\Shamz.IntegrationTests.dll /nologo /framework:net-4.5 | Write-Host
  CheckLastExitCode
  Write-Host -ForegroundColor Cyan '----------------------------------'
}

function RunAllTests() {
  RunUnitTestsVS
  RunIntegrationTestsVS
  RunUnitTests
  RunIntegrationTests
}

function RunAllUnitTests() {
  RunUnitTestsVS
  RunUnitTests
}

function RunAllIntegrationTests() {
  RunIntegrationTestsVS
  RunIntegrationTests
}

function BuildAll() {
  C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe .\src\Shamz.Build\Shamz.proj /ds /maxcpucount:6 | Write-Host
  CheckLastExitCode
}

function DeployVersion($version, $target) {
  $dir = Join-Path $config.deploy_dir $version
  New-Item $dir -ItemType directory -Verbose
  $rawdir = Join-Path $dir 'Shamz.Core\raw'
  New-Item $rawdir -ItemType directory -Verbose
  $source = Join-Path $config.debug_dir "$version\Shamz.Core"
  Copy-Item $source\*.* $rawdir -Verbose
  $mergeddir = Join-Path $dir 'Shamz.Core\merged'
  New-Item $mergeddir -ItemType directory -Verbose
  .\thirdparty\packages\common\ilmerge\ilmerge.exe /t:library /out:.\deploy\$version\shamz.core\merged\Shamz.Core.dll /targetplatform:$target .\deploy\$version\shamz.core\raw\shamz.core.dll .\deploy\$version\shamz.core\raw\supacharge.core.dll | Write-Host
  CheckLastExitCode  
}

function Deploy() {
  TryDelete($config.deploy_dir)
  New-Item $config.deploy_dir -ItemType directory -Verbose
  DeployVersion 'net-3.5' 'v2'
  DeployVersion 'net-4.0' 'v4'
  DeployVersion 'net-4.5' 'v4'
}

function Minion {
  param([string[]] $commands)

  $ErrorActionPreference = 'Stop'

  $commands | ForEach {
    $command = $_
    $times = Measure-Command {
      
      Write-Host -ForegroundColor Green "command: $command"
      Write-Host ''
      
      switch ($command) {
        'help' { Help }
        'bootstrap' { Bootstrap }
        'set.env' { SetEnv }
        'run.unit.tests.vs' { RunUnitTestsVS }
        'run.unit.tests' { RunUnitTests }
        'run.integration.tests.vs' { RunIntegraitonTestsVS }
        'run.integration.tests' { RunIntegrationTests }
        'run.all.integration.tests' { RunAllIntegrationTests }
        'run.all.unit.tests' { RunAllUnitTests }
        'run.all.tests' { RunAllTests }        
        'build.all' { BuildAll }
        'clean' { Clean }
        'clean.all' { CleanAll }
        'deploy' { Deploy }
        default { Write-Host -ForegroundColor Red "command not known: $command" }
      }
    }
    
    Write-Host ''
    Write-Host -ForegroundColor Green "Command completed in $($times.TotalSeconds) seconds ($($times.TotalMilliseconds) millis)"
  }
}

export-modulemember -function Minion