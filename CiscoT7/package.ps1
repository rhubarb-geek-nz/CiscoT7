#!/usr/bin/env pwsh
# Copyright (c) 2025 Roger Brown.
# Licensed under the MIT License.

param($Configuration,$TargetFramework,$Platform,$IntDir,$OutDir,$PublishDir)

$ProjectName = 'CiscoT7'
$CompanyName = 'rhubarb-geek-nz'

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
$DSC = [System.IO.Path]::DirectorySeparatorChar

trap
{
	throw $PSItem
}

$xmlDoc = [System.Xml.XmlDocument](Get-Content "$ProjectName.csproj")

$ModuleId = $xmlDoc.SelectSingleNode("/Project/PropertyGroup/PackageId").FirstChild.Value
$Version = $xmlDoc.SelectSingleNode("/Project/PropertyGroup/Version").FirstChild.Value
$ProjectUri = $xmlDoc.SelectSingleNode("/Project/PropertyGroup/PackageProjectUrl").FirstChild.Value
$Description = $xmlDoc.SelectSingleNode("/Project/PropertyGroup/Description").FirstChild.Value
$Author = $xmlDoc.SelectSingleNode("/Project/PropertyGroup/Authors").FirstChild.Value
$Copyright = $xmlDoc.SelectSingleNode("/Project/PropertyGroup/Copyright").FirstChild.Value
$AssemblyName = $xmlDoc.SelectSingleNode("/Project/PropertyGroup/AssemblyName").FirstChild.Value

$CmdletsToExport = @("ConvertTo-$ProjectName","ConvertFrom-$ProjectName")

New-ModuleManifest -Path "$OutDir/$ModuleId.psd1" `
				-RootModule "$AssemblyName.dll" `
				-ModuleVersion $Version `
				-Guid '78bbd099-ba05-4386-973e-fe1b5a64c981' `
				-Author $Author `
				-CompanyName $CompanyName `
				-Copyright $Copyright `
				-Description $Description `
				-FunctionsToExport @() `
				-CmdletsToExport $CmdletsToExport `
				-VariablesToExport '*' `
				-AliasesToExport @() `
				-ProjectUri $ProjectUri

Import-PowerShellDataFile -LiteralPath "$OutDir/$ModuleId.psd1" | Export-PowerShellDataFile | Set-Content -LiteralPath "$PublishDir$ModuleId.psd1" -Encoding utf8BOM

Remove-Item "$OutDir/$ModuleId.psd1"
