; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Monyze Windows Agent"
#define MyAppVersion "1.0"
#define MyAppPublisher "Monyze, Inc."
#define MyAppURL "https://monyze.ru/"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{495BE497-94EA-4B83-BD1A-AF8BA9321665}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=Installer
OutputBaseFilename=MonyzeWindowsAgent-{#MyAppVersion}
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Files]
Source: "MonyzeWindowsAgent\bin\Release\MonyzeWindowsAgent.exe"; DestDir: "{app}"; Flags: ignoreversion;
Source: "MonyzeWindowsAgent\bin\Release\log4net.dll"; DestDir: "{app}"; Flags: ignoreversion;
Source: "MonyzeWindowsAgent\bin\Release\log4net.xml"; DestDir: "{app}"; Flags: ignoreversion;
Source: "MonyzeWindowsAgent\bin\Release\OpenHardwareMonitorLib.dll"; DestDir: "{app}"; Flags: ignoreversion;
Source: "MonyzeWindowsAgent\bin\Release\monyze_config.ini"; DestDir: "{app}"; Flags: ignoreversion;

[Run]

Filename: {sys}\sc.exe; Parameters: "stop ""MonyzeAgent""" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "delete ""MonyzeAgent""" ; Flags: runhidden
Filename: {app}\MonyzeWindowsAgent.exe; Parameters: "--install" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "start ""MonyzeAgent""" ; Flags: runhidden

[UninstallRun]
Filename: {sys}\sc.exe; Parameters: "stop ""MonyzeAgent""" ; Flags: runhidden;
Filename: {sys}\timeout.exe; Parameters: "4" ; Flags: runhidden;
Filename: {app}\MonyzeWindowsAgent.exe; Parameters: "--uninstall" ; Flags: runhidden
