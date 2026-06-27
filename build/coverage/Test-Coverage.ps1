param(
    [string]$ThresholdFile = "build/coverage/coverage-thresholds.json"
)

$configuration = Get-Content -LiteralPath $ThresholdFile -Raw | ConvertFrom-Json
$failures = [System.Collections.Generic.List[string]]::new()

foreach ($layer in $configuration.layers) {
    $reportPath = Join-Path (Get-Location) $layer.report
    if (-not (Test-Path -LiteralPath $reportPath)) {
        $failures.Add("$($layer.name): missing coverage report at $($layer.report)")
        continue
    }

    [xml]$report = Get-Content -LiteralPath $reportPath
    $linesValid = [double]$report.coverage.'lines-valid'
    $linesCovered = [double]$report.coverage.'lines-covered'
    $coverage = if ($linesValid -eq 0) { 0 } else { [math]::Round(($linesCovered / $linesValid) * 100, 2) }

    Write-Host ("{0}: {1}% (threshold {2}%)" -f $layer.name, $coverage, $layer.threshold)

    if ($coverage -lt [double]$layer.threshold) {
        $failures.Add("$($layer.name): $coverage% is below the $($layer.threshold)% threshold")
    }
}

if ($failures.Count -gt 0) {
    $message = $failures -join [Environment]::NewLine
    throw "Coverage thresholds not met:`n$message"
}
