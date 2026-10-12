$ErrorActionPreference = "Stop"
$base = "http://localhost:5000"
$xlsx = "c:\Users\yosri\Desktop\VectorysInventaire\scripts\swagger-capture-stock.xlsx"
$outDir = "c:\Users\yosri\Desktop\VectorysInventaire\scripts\swagger-capture-results"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

function Invoke-Api {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Url,
        [string]$Token = "",
        [string]$Body = "",
        [string]$ContentType = "application/json",
        [string[]]$ExtraArgs = @()
    )
    $tmp = Join-Path $outDir "$Name.body.json"
    $hdrs = @("-sS", "-w", " __HTTP__:%{http_code}", "-X", $Method, $Url)
    if ($Token) { $hdrs += @("-H", "Authorization: Bearer $Token") }
    if ($Body) {
        $bodyFile = Join-Path $outDir "$Name.request.json"
        Set-Content -Path $bodyFile -Value $Body -Encoding utf8
        $hdrs += @("-H", "Content-Type: $ContentType", "--data-binary", "@$bodyFile")
    }
    $hdrs += $ExtraArgs
    $raw = & curl.exe @hdrs
    if ($LASTEXITCODE -ne 0) { throw "curl failed for $Name : $LASTEXITCODE" }
    $code = "000"
    if ($raw -match "(?s)(.*)\s__HTTP__:(\d+)\s*$") {
        $payload = $Matches[1].Trim()
        $code = $Matches[2]
    } else {
        $payload = $raw
    }
    Set-Content -Path $tmp -Value $payload -Encoding utf8
    Write-Host ("[{0}] {1} {2} -> {3}" -f $code, $Method, $Url, $Name)
    if ($code -notmatch "^2") {
        Write-Host $payload
        throw "Expected 2xx for $Name, got $code"
    }
    return @{ Code = $code; Body = $payload }
}

$loginSup = Invoke-Api -Name "01-login-superviseur" -Method POST -Url "$base/api/Auth/login" -Body (@{
    email = "superviseur@vectorys.local"
    password = "Super123!"
    codePersonnel = $null
} | ConvertTo-Json)
$sup = $loginSup.Body | ConvertFrom-Json
$tokS = $sup.token
Write-Host "SUPERVISEUR token length=$($tokS.Length) userId=$($sup.userId)"

$loginAg = Invoke-Api -Name "02-login-agent" -Method POST -Url "$base/api/Auth/login" -Body (@{
    email = $null
    password = "AG001"
    codePersonnel = "AG001"
} | ConvertTo-Json)
$ag = $loginAg.Body | ConvertFrom-Json
$tokA = $ag.token

$loginAg2 = Invoke-Api -Name "02b-login-agent2" -Method POST -Url "$base/api/Auth/login" -Body (@{
    email = $null
    password = "AG002"
    codePersonnel = "AG002"
} | ConvertTo-Json)
$ag2 = $loginAg2.Body | ConvertFrom-Json
$tokA2 = $ag2.token

$created = Invoke-Api -Name "03-create-inventaire" -Method POST -Url "$base/api/Inventaires" -Token $tokS -Body (@{
    intitule = "Capture Swagger rapport PFE"
    zone = "alle"
    dateDebut = (Get-Date).ToUniversalTime().ToString("o")
    dateFin = $null
} | ConvertTo-Json)
$inv = ($created.Body | ConvertFrom-Json)
$inventaireId = $inv.inventaire.id
$passageC1Id = $inv.passageC1Id
Write-Host "InventaireId=$inventaireId PassageC1Id=$passageC1Id Ref=$($inv.inventaire.reference)"

$importRaw = & curl.exe -sS -w " __HTTP__:%{http_code}" -X POST "$base/api/Passages/$passageC1Id/import-stock" `
    -H "Authorization: Bearer $tokS" `
    -F "file=@$xlsx"
if ($importRaw -match "(?s)(.*)\s__HTTP__:(\d+)\s*$") {
    $importBody = $Matches[1].Trim(); $importCode = $Matches[2]
} else { $importBody = $importRaw; $importCode = "000" }
Set-Content (Join-Path $outDir "04-import-stock.body.json") $importBody -Encoding utf8
Write-Host "[$importCode] POST /api/Passages/$passageC1Id/import-stock -> 04-import-stock"
if ($importCode -ne "200") { Write-Host $importBody; throw "import failed $importCode" }

$invList = Invoke-Api -Name "05-get-inventaires" -Method GET -Url "$base/api/Inventaires?page=1&pageSize=20" -Token $tokS
$mes1 = Invoke-Api -Name "06-mes-emplacements-ag001" -Method GET -Url "$base/api/Emplacements/mes-emplacements" -Token $tokA
$mes2 = Invoke-Api -Name "07-mes-emplacements-ag002" -Method GET -Url "$base/api/Emplacements/mes-emplacements" -Token $tokA2

$emp1 = ($mes1.Body | ConvertFrom-Json) | Where-Object { $_.inventaireId -eq $inventaireId }
$emp2 = ($mes2.Body | ConvertFrom-Json) | Where-Object { $_.inventaireId -eq $inventaireId }
$a01 = $emp1 | Where-Object { $_.code -eq "CAP-A01" } | Select-Object -First 1
$a02 = $emp1 | Where-Object { $_.code -eq "CAP-A02" } | Select-Object -First 1
$b01 = $emp2 | Where-Object { $_.code -eq "CAP-B01" } | Select-Object -First 1
$b02 = $emp2 | Where-Object { $_.code -eq "CAP-B02" } | Select-Object -First 1
Write-Host "A01=$($a01.id) A02=$($a02.id) B01=$($b01.id) B02=$($b02.id)"
if (-not $a01 -or -not $a02 -or -not $b01 -or -not $b02) { throw "emplacements manquants pour cet inventaire" }

$now = (Get-Date).ToUniversalTime().ToString("o")
$openA01 = Invoke-Api -Name "08-traiter-open-A01" -Method POST -Url "$base/api/Scans/traiter" -Token $tokA -Body (@{
    codeBarresScan = "CAP-A01"
    emplacementActifId = $null
    dateHeureScan = $now
    idLocalFlutter = $null
} | ConvertTo-Json)
$scanUs = Invoke-Api -Name "09-traiter-us-A01" -Method POST -Url "$base/api/Scans/traiter" -Token $tokA -Body (@{
    codeBarresScan = "US-C001"
    emplacementActifId = $a01.id
    dateHeureScan = (Get-Date).ToUniversalTime().ToString("o")
    idLocalFlutter = $null
} | ConvertTo-Json)
$scanExtra = Invoke-Api -Name "10-traiter-surplus-A01" -Method POST -Url "$base/api/Scans/traiter" -Token $tokA -Body (@{
    codeBarresScan = "US-EXTRA"
    emplacementActifId = $a01.id
    dateHeureScan = (Get-Date).ToUniversalTime().ToString("o")
    idLocalFlutter = $null
} | ConvertTo-Json)
$finA01 = Invoke-Api -Name "11-fin-comptage-A01" -Method PUT -Url "$base/api/Scans/$($a01.id)/fin-comptage" -Token $tokA

$openA02 = Invoke-Api -Name "12-traiter-open-A02" -Method POST -Url "$base/api/Scans/traiter" -Token $tokA -Body (@{
    codeBarresScan = "CAP-A02"
    emplacementActifId = $null
    dateHeureScan = (Get-Date).ToUniversalTime().ToString("o")
} | ConvertTo-Json)
Invoke-Api -Name "13-traiter-us-A02a" -Method POST -Url "$base/api/Scans/traiter" -Token $tokA -Body (@{
    codeBarresScan = "US-C003"
    emplacementActifId = $a02.id
    dateHeureScan = (Get-Date).ToUniversalTime().ToString("o")
} | ConvertTo-Json) | Out-Null
Invoke-Api -Name "13b-traiter-us-A02b" -Method POST -Url "$base/api/Scans/traiter" -Token $tokA -Body (@{
    codeBarresScan = "US-C003"
    emplacementActifId = $a02.id
    dateHeureScan = (Get-Date).ToUniversalTime().ToString("o")
} | ConvertTo-Json) | Out-Null
Invoke-Api -Name "14-fin-comptage-A02" -Method PUT -Url "$base/api/Scans/$($a02.id)/fin-comptage" -Token $tokA | Out-Null

$openB02 = Invoke-Api -Name "15-traiter-open-B02" -Method POST -Url "$base/api/Scans/traiter" -Token $tokA2 -Body (@{
    codeBarresScan = "CAP-B02"
    emplacementActifId = $null
    dateHeureScan = (Get-Date).ToUniversalTime().ToString("o")
} | ConvertTo-Json)

$batchBody = @{
    scans = @(
        @{
            codeBarresScan = "US-C201"
            emplacementId = $b02.id
            dateHeureScan = (Get-Date).ToUniversalTime().ToString("o")
            idLocalFlutter = "offline-b02-" + [guid]::NewGuid().ToString("N")
        }
    )
} | ConvertTo-Json -Depth 6
$batch = Invoke-Api -Name "16-scans-batch" -Method POST -Url "$base/api/Scans/batch" -Token $tokA2 -Body $batchBody
Invoke-Api -Name "17-fin-comptage-B02" -Method PUT -Url "$base/api/Scans/$($b02.id)/fin-comptage" -Token $tokA2 | Out-Null

$openB01 = Invoke-Api -Name "18-traiter-open-B01" -Method POST -Url "$base/api/Scans/traiter" -Token $tokA2 -Body (@{
    codeBarresScan = "CAP-B01"
    emplacementActifId = $null
    dateHeureScan = (Get-Date).ToUniversalTime().ToString("o")
} | ConvertTo-Json)
Invoke-Api -Name "19-traiter-us-B01" -Method POST -Url "$base/api/Scans/traiter" -Token $tokA2 -Body (@{
    codeBarresScan = "US-C101"
    emplacementActifId = $b01.id
    dateHeureScan = (Get-Date).ToUniversalTime().ToString("o")
} | ConvertTo-Json) | Out-Null
Invoke-Api -Name "20-fin-comptage-B01" -Method PUT -Url "$base/api/Scans/$($b01.id)/fin-comptage" -Token $tokA2 | Out-Null

$suivi = Invoke-Api -Name "21-suivi-temps-reel" -Method GET -Url "$base/api/Inventaires/$inventaireId/suivi-temps-reel" -Token $tokS
$detect = Invoke-Api -Name "22-detecter-ecarts" -Method POST -Url "$base/api/Ecarts/detecter/$inventaireId" -Token $tokS -Body "{}"
$ecarts = Invoke-Api -Name "23-get-ecarts" -Method GET -Url "$base/api/Ecarts?inventaireId=$inventaireId" -Token $tokS
$ecartList = $ecarts.Body | ConvertFrom-Json
$ecartIds = @($ecartList.items | ForEach-Object { $_.id })
Write-Host ("Ecarts detectes={0} ids={1}" -f $ecartList.totalDetectes, ($ecartIds -join ","))

$brancheA = Invoke-Api -Name "24-branche-a" -Method POST -Url "$base/api/Ecarts/branche-a" -Token $tokS -Body (@{
    inventaireId = $inventaireId
    ecartIds = @($ecartIds)
} | ConvertTo-Json)

$kpi = Invoke-Api -Name "25-kpi-dashboard" -Method GET -Url "$base/api/kpi/dashboard?inventaireId=$inventaireId" -Token $tokS
$kpiP = Invoke-Api -Name "26-kpi-precision" -Method GET -Url "$base/api/kpi/precision?inventaireId=$inventaireId" -Token $tokS

$exportFile = Join-Path $outDir "27-export.xlsx"
$exportHdr = Join-Path $outDir "27-export.headers.txt"
& curl.exe -sS -D $exportHdr -o $exportFile -X GET "$base/api/Inventaires/$inventaireId/export" `
    -H "Authorization: Bearer $tokS" | Out-Null
Write-Host "Export saved: $exportFile size=$((Get-Item $exportFile).Length)"
Get-Content $exportHdr | Select-Object -First 5

$cloture = Invoke-Api -Name "28-cloturer" -Method POST -Url "$base/api/Inventaires/$inventaireId/cloturer" -Token $tokS -Body "{}"

$summary = @"
INVENTAIRE_ID=$inventaireId
PASSAGE_C1_ID=$passageC1Id
REFERENCE=$($inv.inventaire.reference)
EMP_A01=$($a01.id)
EMP_A02=$($a02.id)
EMP_B01=$($b01.id)
EMP_B02=$($b02.id)
ECART_IDS=$($ecartIds -join ",")
TOKEN_SUPERVISEUR=$tokS
TOKEN_AGENT_AG001=$tokA
TOKEN_AGENT_AG002=$tokA2
"@
Set-Content (Join-Path $outDir "00-SUMMARY.txt") $summary -Encoding utf8
Write-Host "==== OK ===="
Write-Host $summary
