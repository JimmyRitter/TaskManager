# PowerShell script for running code coverage on Windows

# Navigate to the API directory
Set-Location $PSScriptRoot\..

Write-Host "🧪 Running tests with code coverage..." -ForegroundColor Green

# Create coverage output directory
if (!(Test-Path "coverage")) {
    New-Item -ItemType Directory -Path "coverage"
}

# Run tests with coverage collection
dotnet test `
  --collect:"XPlat Code Coverage" `
  --results-directory ./coverage `
  --settings ./coverlet.runsettings `
  --verbosity normal

# Find the most recent coverage file
$coverageFile = Get-ChildItem -Path "./coverage" -Filter "coverage.cobertura.xml" -Recurse | 
                Sort-Object LastWriteTime -Descending | 
                Select-Object -First 1

if ($coverageFile) {
    Write-Host "📊 Generating HTML coverage report..." -ForegroundColor Yellow
    
    # Generate HTML report using ReportGenerator
    $reportGeneratorPath = Get-ChildItem -Path "$env:USERPROFILE\.nuget\packages\reportgenerator" -Filter "ReportGenerator.dll" -Recurse | 
                          Where-Object { $_.FullName -like "*tools*net6.0*" } | 
                          Select-Object -First 1
    
    if ($reportGeneratorPath) {
        dotnet $reportGeneratorPath.FullName `
            -reports:$coverageFile.FullName `
            -targetdir:./coverage/html `
            -reporttypes:Html
        
        Write-Host "✅ Coverage report generated at: ./coverage/html/index.html" -ForegroundColor Green
        
        # Try to extract coverage percentage (basic XML parsing)
        try {
            [xml]$coverageXml = Get-Content $coverageFile.FullName
            $lineRate = $coverageXml.coverage.'line-rate'
            $coveragePercent = [math]::Round([double]$lineRate * 100, 1)
            Write-Host "📈 Line Coverage: $coveragePercent%" -ForegroundColor Cyan
        }
        catch {
            Write-Host "Could not parse coverage percentage" -ForegroundColor Yellow
        }
    }
    else {
        Write-Host "❌ ReportGenerator not found!" -ForegroundColor Red
    }
}
else {
    Write-Host "❌ Coverage file not found!" -ForegroundColor Red
    exit 1
} 