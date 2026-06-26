[Setup]
AppName=BIM Ninja - Sheet Exporter
AppVersion=1.0.0
AppPublisher=BIM Ninja
DefaultDirName={autopf64}\BIM Ninja\Sheet Exporter
DefaultGroupName=BIM Ninja
OutputDir=..
OutputBaseFilename=BimNinjaSheetExporter_Setup
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
WizardStyle=modern
SetupIconFile=Icons\appicon.ico
UninstallDisplayIcon={app}\appicon.ico
LicenseFile=..\LICENSE.txt

[Files]
Source: "..\BimNinja.SheetExporter.Addin\bin\Release\BimNinja.SheetExporter.Addin.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BimNinja.SheetExporter.Addin\bin\Release\BimNinja.Licensing.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BimNinja.SheetExporter.Addin\bin\Release\BimNinja.SheetExporter.Addin.addin"; DestDir: "{app}"; Flags: ignoreversion
Source: "Icons\appicon.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\BIM Ninja Sheet Exporter"; Filename: "{app}\appicon.ico"
Name: "{group}\Uninstall"; Filename: "{uninstallexe}"

[Registry]
Root: HKLM64; Subkey: "SOFTWARE\Autodesk\Revit\Addins\2024"; ValueType: string; ValueName: "BimNinjaSheetExporter"; ValueData: "{app}\BimNinja.SheetExporter.Addin.addin"; Flags: uninsdeletevalue
Root: HKLM64; Subkey: "SOFTWARE\Autodesk\Revit\Addins\2023"; ValueType: string; ValueName: "BimNinjaSheetExporter"; ValueData: "{app}\BimNinja.SheetExporter.Addin.addin"; Flags: uninsdeletevalue
Root: HKLM64; Subkey: "SOFTWARE\Autodesk\Revit\Addins\2022"; ValueType: string; ValueName: "BimNinjaSheetExporter"; ValueData: "{app}\BimNinja.SheetExporter.Addin.addin"; Flags: uninsdeletevalue

[Run]
Filename: "{app}\BimNinja.SheetExporter.Addin.addin"; StatusMsg: "Registering addin..."; Flags: shellexec waituntilterminated