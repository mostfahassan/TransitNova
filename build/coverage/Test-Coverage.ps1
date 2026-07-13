param(
    [string]$ThresholdFile = "build/coverage/coverage-thresholds.json"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

if (-not (Test-Path -LiteralPath $ThresholdFile)) {
    throw "Coverage threshold configuration was not found at '$ThresholdFile'."
}

$configuration = Get-Content -LiteralPath $ThresholdFile -Raw | ConvertFrom-Json
$failures = [System.Collections.Generic.List[string]]::new()
$results = [System.Collections.Generic.List[object]]::new()
$totalLinesValid = 0.0
$totalLinesCovered = 0.0
$totalBranchesValid = 0.0
$totalBranchesCovered = 0.0
$culture = [System.Globalization.CultureInfo]::InvariantCulture

if ($null -eq $configuration.layers -or $configuration.layers.Count -eq 0) {
    throw "Coverage threshold configuration must contain at least one layer."
}

foreach ($layer in $configuration.layers) {
    $reportPath = Join-Path (Get-Location) $layer.report
    if (-not (Test-Path -LiteralPath $reportPath)) {
        $failures.Add("$($layer.name): missing coverage report at $($layer.report)")
        continue
    }

    [xml]$report = Get-Content -LiteralPath $reportPath
    $coverageNode = $report.DocumentElement
    if ($null -eq $coverageNode -or $coverageNode.Name -ne "coverage") {
        $failures.Add("$($layer.name): invalid Cobertura document at $($layer.report)")
        continue
    }

    $linesValidText = $coverageNode.GetAttribute("lines-valid")
    $linesCoveredText = $coverageNode.GetAttribute("lines-covered")
    $branchesValidText = $coverageNode.GetAttribute("branches-valid")
    $branchesCoveredText = $coverageNode.GetAttribute("branches-covered")
    $lineRateText = $coverageNode.GetAttribute("line-rate")
    $branchRateText = $coverageNode.GetAttribute("branch-rate")
    $linesValid = if ([string]::IsNullOrWhiteSpace($linesValidText)) { 0 } else { [double]::Parse($linesValidText, $culture) }
    $linesCovered = if ([string]::IsNullOrWhiteSpace($linesCoveredText)) { 0 } else { [double]::Parse($linesCoveredText, $culture) }
    $branchesValid = if ([string]::IsNullOrWhiteSpace($branchesValidText)) { 0 } else { [double]::Parse($branchesValidText, $culture) }
    $branchesCovered = if ([string]::IsNullOrWhiteSpace($branchesCoveredText)) { 0 } else { [double]::Parse($branchesCoveredText, $culture) }

    $lineCoverage = if ($linesValid -gt 0) {
        [math]::Round(($linesCovered / $linesValid) * 100, 2)
    }
    elseif (-not [string]::IsNullOrWhiteSpace($lineRateText)) {
        [math]::Round([double]::Parse($lineRateText, $culture) * 100, 2)
    }
    else { 0 }

    $branchCoverage = if ($branchesValid -gt 0) {
        [math]::Round(($branchesCovered / $branchesValid) * 100, 2)
    }
    elseif (-not [string]::IsNullOrWhiteSpace($branchRateText)) {
        [math]::Round([double]::Parse($branchRateText, $culture) * 100, 2)
    }
    else { 0 }

    $totalLinesValid += $linesValid
    $totalLinesCovered += $linesCovered
    $totalBranchesValid += $branchesValid
    $totalBranchesCovered += $branchesCovered

    $lineThreshold = [double]$layer.lineThreshold
    $branchThreshold = [double]$layer.branchThreshold
    $results.Add([pscustomobject]@{
        Layer = [string]$layer.name
        LineCoverage = $lineCoverage
        LineThreshold = $lineThreshold
        BranchCoverage = $branchCoverage
        BranchThreshold = $branchThreshold
    })

    Write-Host ("{0}: lines {1}% (gate {2}%), branches {3}% (gate {4}%)" -f $layer.name, $lineCoverage, $lineThreshold, $branchCoverage, $branchThreshold)
    if ($lineCoverage -lt $lineThreshold) {
        $failures.Add("$($layer.name): line coverage $lineCoverage% is below the $lineThreshold% gate")
    }
    if ($branchCoverage -lt $branchThreshold) {
        $failures.Add("$($layer.name): branch coverage $branchCoverage% is below the $branchThreshold% gate")
    }
}

if ($null -ne $configuration.overall) {
    $overallLineCoverage = if ($totalLinesValid -gt 0) { [math]::Round(($totalLinesCovered / $totalLinesValid) * 100, 2) } else { 0 }
    $overallBranchCoverage = if ($totalBranchesValid -gt 0) { [math]::Round(($totalBranchesCovered / $totalBranchesValid) * 100, 2) } else { 0 }
    $overallLineThreshold = [double]$configuration.overall.lineThreshold
    $overallBranchThreshold = [double]$configuration.overall.branchThreshold
    $results.Add([pscustomobject]@{
        Layer = "Overall"
        LineCoverage = $overallLineCoverage
        LineThreshold = $overallLineThreshold
        BranchCoverage = $overallBranchCoverage
        BranchThreshold = $overallBranchThreshold
    })
    Write-Host ("Overall: lines {0}% (gate {1}%), branches {2}% (gate {3}%)" -f $overallLineCoverage, $overallLineThreshold, $overallBranchCoverage, $overallBranchThreshold)
    if ($overallLineCoverage -lt $overallLineThreshold) {
        $failures.Add("Overall: line coverage $overallLineCoverage% is below the $overallLineThreshold% gate")
    }
    if ($overallBranchCoverage -lt $overallBranchThreshold) {
        $failures.Add("Overall: branch coverage $overallBranchCoverage% is below the $overallBranchThreshold% gate")
    }
}

if (-not [string]::IsNullOrWhiteSpace($env:GITHUB_STEP_SUMMARY)) {
    $summary = [System.Collections.Generic.List[string]]::new()
    $summary.Add("## Test coverage")
    $summary.Add("")
    $summary.Add("| Layer | Line coverage | Line gate | Branch coverage | Branch gate |")
    $summary.Add("| --- | ---: | ---: | ---: | ---: |")
    foreach ($result in $results) {
        $summary.Add("| $($result.Layer) | $($result.LineCoverage)% | $($result.LineThreshold)% | $($result.BranchCoverage)% | $($result.BranchThreshold)% |")
    }
    Add-Content -LiteralPath $env:GITHUB_STEP_SUMMARY -Value ($summary -join [Environment]::NewLine)
}

if ($failures.Count -gt 0) {
    throw "Coverage gates were not met:$([Environment]::NewLine)$($failures -join [Environment]::NewLine)"
}

Write-Host "All coverage gates passed."
