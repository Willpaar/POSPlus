# Anchor paths relative to the script location or project root safely
$scriptDir = $PSScriptRoot
$projectRoot = Resolve-Path "$scriptDir\.."
$sqlPath = Join-Path $projectRoot "Common\DBStructure\MasterCreate.sql"
$dataRowDir = Join-Path $projectRoot "Common\DBEntities\DataRowEntities"
$backupDir = Join-Path $projectRoot "Common\DBEntities\_Backup\$(Get-Date -Format 'yyyyMMdd_HHmmss')"

Write-Host "Starting DataRow Generation..." -ForegroundColor Cyan

if (!(Test-Path $sqlPath)) {
    Write-Error "CRITICAL: SQL file not found at $sqlPath. Please verify the path."
    exit
}

# --- BACKUP & REVERT MECHANISM ---
if ((Test-Path $dataRowDir) -and (Get-ChildItem $dataRowDir -ErrorAction SilentlyContinue)) {
    Write-Host "Backing up existing DataRow files to $backupDir..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Force -Path $backupDir | Out-Null
    Copy-Item "$dataRowDir\*" -Destination $backupDir -Recurse -Force
    Write-Host "Backup completed. (To revert, copy files from $backupDir back into $dataRowDir)" -ForegroundColor DarkYellow
}

# Ensure destination directory exists
New-Item -ItemType Directory -Force -Path $dataRowDir | Out-Null

$sqlContent = Get-Content $sqlPath -Raw

# Helper function to map SQL types to C# types with error safety
function Get-CSharpType {
    param($sqlType)
    if ([string]::IsNullOrWhiteSpace($sqlType)) { return "string" }
    $sqlType = $sqlType.Trim().ToLower()
    
    if ($sqlType -match "uniqueidentifier") { return "Guid" }
    if ($sqlType -match "varchar|text|char|nchar|nvarchar") { return "string" }
    if ($sqlType -match "bit") { return "bool" }
    if ($sqlType -match "decimal|numeric|money|smallmoney") { return "decimal" }
    if ($sqlType -match "int|smallint|tinyint") { return "int" }
    if ($sqlType -match "bigint") { return "long" }
    if ($sqlType -match "datetime|date|time") { return "DateTime" }
    
    Write-Warning "Unknown SQL type '$sqlType' encountered. Defaulting to 'string'."
    return "string"
}

# Bulletproof parser to split SQL definitions by comma without breaking on parentheses
function Split-SqlColumns {
    param($body)
    $lines = @()
    $currentLine = ""
    $depth = 0
    for ($i = 0; $i -lt $body.Length; $i++) {
        $char = $body[$i]
        if ($char -eq '(') { $depth++ }
        if ($char -eq ')') { $depth-- }
        if ($char -eq ',' -and $depth -eq 0) {
            $lines += $currentLine.Trim()
            $currentLine = ""
        } else {
            $currentLine += $char
        }
    }
    if (![string]::IsNullOrWhiteSpace($currentLine)) {
        $lines += $currentLine.Trim()
    }
    return $lines
}

# --- SQL PARSING & ERROR HANDLING ---
try {
    $tableMatches = [regex]::Matches($sqlContent, "Create table\s+(\w+)\s*\((.*?)\);", "IgnoreCase, Singleline")

    if ($tableMatches.Count -eq 0) {
        throw "No 'Create table' statements found. Please check your SQL syntax or name patterns in MasterCreate.sql."
    }

    $tables = [ordered]@{}
    $linkTables = @()

    foreach ($match in $tableMatches) {
        $tableName = $match.Groups[1].Value.Trim()
        $body = $match.Groups[2].Value

        if ([string]::IsNullOrEmpty($tableName)) {
            Write-Warning "Skipping an anonymous or malformed table block."
            continue
        }

        # Check if it's a link table (ends with Link)
        if ($tableName -match "Link$") {
            $linkTables += [PSCustomObject]@{ TableName = $tableName; Body = $body }
            continue
        }

        $columns = @()
        $lines = Split-SqlColumns $body

        foreach ($line in $lines) {
            $line = $line.Trim()
            if ([string]::IsNullOrWhiteSpace($line)) { continue }
            if ($line -match "^\s*(CONSTRAINT|FOREIGN KEY|PRIMARY KEY)") { continue }
            
            $parts = $line -split "\s+"
            if ($parts.Length -ge 2) {
                $colName = $parts[0].Trim('[]`"')
                $sqlType = $parts[1]
                
                $columns += [PSCustomObject]@{
                    Name = $colName
                    Type = Get-CSharpType $sqlType
                }
            }
        }

        $tables[$tableName] = $columns
    }
}
catch {
    Write-Error "Failed to parse SQL structure: $_"
    exit
}

# --- RELATIONSHIP ANALYSIS (STRICTLY FROM FK CONSTRAINTS) ---
$entityRelations = @{}
foreach ($tableName in $tables.Keys) {
    $entityRelations[$tableName] = @{ Parents = @(); Children = @(); ManyToMany = @() }
}

foreach ($match in $tableMatches) {
    $tableName = $match.Groups[1].Value.Trim()
    if ($tableName -match "Link$") { continue }

    $body = $match.Groups[2].Value
    $fkMatches = [regex]::Matches($body, "FOREIGN KEY\s*\(\s*(\w+)\s*\)\s*REFERENCES\s+(\w+)\s*\(\s*(\w+)\s*\)", "IgnoreCase")

    foreach ($fk in $fkMatches) {
        $colName = $fk.Groups[1].Value
        $parentTable = $fk.Groups[2].Value

        if ($tables.Contains($parentTable) -and $parentTable -ne $tableName) {
            $entityRelations[$tableName].Parents += [PSCustomObject]@{
                ParentTable = $parentTable
                ForeignKeyCol = $colName
            }
            if (!($entityRelations[$parentTable].Children -contains $tableName)) {
                $entityRelations[$parentTable].Children += $tableName
            }
        }
    }
}

foreach ($link in $linkTables) {
    $fkMatches = [regex]::Matches($link.Body, "FOREIGN KEY\s*\(\s*(\w+)\s*\)\s*REFERENCES\s+(\w+)\s*\(\s*(\w+)\s*\)", "IgnoreCase")
    $refTables = @()

    foreach ($fk in $fkMatches) {
        $refTable = $fk.Groups[2].Value
        if ($tables.Contains($refTable) -and !($refTables -contains $refTable)) {
            $refTables += $refTable
        }
    }

    if ($refTables.Count -eq 2) {
        $t1 = $refTables[0]
        $t2 = $refTables[1]
        
        $entityRelations[$t1].ManyToMany += [PSCustomObject]@{ TargetTable = $t2; LinkTable = $link.TableName }
        $entityRelations[$t2].ManyToMany += [PSCustomObject]@{ TargetTable = $t1; LinkTable = $link.TableName }
    }
}

# --- GENERATE DATAROW FILES ---
foreach ($tableName in $tables.Keys) {
    $dataRowClassName = "${tableName}DataRow"
    $columns = $tables[$tableName]
    $rels = $entityRelations[$tableName]

    try {
        $sbDataRow = New-Object System.Text.StringBuilder
        [void]$sbDataRow.AppendLine("using System;")
        [void]$sbDataRow.AppendLine("using System.Collections.Generic;")
        [void]$sbDataRow.AppendLine("using System.Linq;")
        [void]$sbDataRow.AppendLine("using Common.Abstract;")
        [void]$sbDataRow.AppendLine("namespace Common.DBEntities.DataRowEntities")
        [void]$sbDataRow.AppendLine()
        [void]$sbDataRow.AppendLine("{")
        [void]$sbDataRow.AppendLine("    public class $dataRowClassName : Entity")
        [void]$sbDataRow.AppendLine("    {")

        foreach ($col in $columns) {
            if ($col.Name -eq "Id") { continue }
            [void]$sbDataRow.AppendLine("        public virtual $($col.Type) $($col.Name) { get; set; }")
        }

        foreach ($parentObj in $rels.Parents) {
            $parent = $parentObj.ParentTable
            $fkCol = $parentObj.ForeignKeyCol
            [void]$sbDataRow.AppendLine("        public virtual $parent $parent => ($parent)GetFKEntity(nameof($parent), $fkCol);")
        }

        foreach ($child in $rels.Children) {
            [void]$sbDataRow.AppendLine("        public virtual List<$child> ${child}s => GetFKEntities(nameof($child), Id).Cast<$child>().ToList();")
        }

        foreach ($m2m in $rels.ManyToMany) {
            $target = $m2m.TargetTable
            $linkTbl = $m2m.LinkTable
            $linkBase = $linkTbl -replace 'Link$', ''
            [void]$sbDataRow.AppendLine("        public virtual List<$target> ${target}s => GetLinkedEntities<$target>(nameof($linkBase), Id).Cast<$target>().ToList();")
        }

        [void]$sbDataRow.AppendLine()
        [void]$sbDataRow.AppendLine("        public $dataRowClassName()")
        [void]$sbDataRow.AppendLine("        {")
        [void]$sbDataRow.AppendLine("        }")

        [void]$sbDataRow.AppendLine("    }")
        [void]$sbDataRow.AppendLine("}")

        $dataRowFile = Join-Path $dataRowDir "${dataRowClassName}.cs"
        Set-Content -Path $dataRowFile -Value $sbDataRow.ToString()
        Write-Host "Generated DataRow: $dataRowClassName" -ForegroundColor Green
    }
    catch {
        Write-Error "Error generating DataRow for table '$tableName': $_"
    }
}

Write-Host "DataRow generation completed successfully!" -ForegroundColor Yellow