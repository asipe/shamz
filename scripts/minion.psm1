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
    Remove-Item $path -Verbose -Recurse
  }
}

function Bootstrap() {
  .\thirdparty\nuget\nuget.exe install .\src\Shamz.Nuget.Packages\common\packages.config -OutputDirectory .\thirdparty\packages\common -ExcludeVersion | Write-Host
  CheckLastExitCode
  .\thirdparty\nuget\nuget.exe install .\src\Shamz.Nuget.Packages\net-4.5\packages.config -OutputDirectory .\thirdparty\packages\net-4.5 -ExcludeVersion | Write-Host
  CheckLastExitCode
}

function Clean() {
  TryDelete($config.debug_dir)
}

function CleanAll() {
  TryDelete($config.packages_dir)
}

function SetEnv() {
  $env:PATH += ";.\thirdparty\packages\common\NUnit.Runners\tools"
  Write-Host -ForegroundColor Green 'Path information set'
}

function RunUnitTestsVS() {
  Write-Host -ForegroundColor Cyan '----------VS Unit Tests (3.5)-----------'
  .\thirdparty\packages\common\nunit.runners\tools\nunit-console.exe .\src\Shamz.UnitTests\bin\debug\Shamz.UnitTests.dll /nologo /framework:net-4.0 | Write-Host
  CheckLastExitCode
  Write-Host -ForegroundColor Cyan '----------------------------------'
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
        default { Write-Host -ForegroundColor Red "command not known: $command" }
      }
    }
    
    Write-Host ''
    Write-Host -ForegroundColor Green "Command completed in $($times.TotalSeconds) seconds ($($times.TotalMilliseconds) millis)"
  }
}

export-modulemember -function Minion