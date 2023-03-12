param (
    [Parameter(Mandatory)]
    [string] $Runtime,

    [switch] $BuildConsole,
    [switch] $BuildConsoleTools,
    [switch] $BuildServer,
    [switch] $BuildAll,

    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $MSBuildArgs
)

function MakeProjectPath {
    param ([string] $ProjectName)
    return $ProjectName
}

# Check there's a build task
if (!($BuildConsole -or $BuildConsoleTools -or $BuildServer -or $BuildAll)) {
    Write-Error "请指定一个编译目标。"
    Get-Help $PSCommandPath
    return
}

# Check current path
if ((Get-Location).Path -eq $PSScriptRoot) {
    $RepoRoot = ".."
}
elseif (Test-Path "YukiChan.sln") {
    $RepoRoot = "."
}
else {
    Write-Error "请在仓库根目录执行本脚本。"
    return
}

# Project source directories
$Src = Join-Path $RepoRoot "src"
$ServerSrc = Join-Path $RepoRoot "server"

# Project file directories
$ConsoleProjectPath = Join-Path $Src (MakeProjectPath("YukiChan"))
$ConsoleToolsProjectPath = Join-Path $Src (MakeProjectPath("YukiChan.Tools"))
$ServerProjectPath = Join-Path $ServerSrc (MakeProjectPath("YukiChan.Server"))

# Build output directories
$Output = Join-Path $RepoRoot "build"
$ConsoleOutput = Join-Path $Output "console"
$ServerOutput = Join-Path $Output "server"

$MSBuildArgs = "-c", "Release", "-r", $Runtime, "--no-self-contained" + $MSBuildArgs

# Build console
if ($BuildConsole -or $BuildAll) {
    dotnet publish `
        -o (Join-Path $ConsoleOutput $Runtime) `
        @MSBuildArgs `
        $ConsoleProjectPath
}

# Build console tools
if ($BuildConsoleTools -or $BuildAll) {
    dotnet publish `
        -o (Join-Path $ConsoleOutput $Runtime) `
        @MSBuildArgs `
        $ConsoleToolsProjectPath
}

# Build server
if ($BuildServer -or $BuildAll) {
    dotnet publish `
        -o (Join-Path $ServerOutput $Runtime) `
        @MSBuildArgs `
        $ServerProjectPath
}
