; 脚本由 Inno Setup 脚本向导 生成！
; 有关创建 Inno Setup 脚本文件的详细资料请查阅帮助文档！

#define MyAppName "SinbodaSemiAuto"
#define MyAppVersion "1.0"
#define MyAppPublisher "Sinboda"
;#define MyAppURL "https://blog.csdn.net/u012842630"
#define MyAppExeName "SinbodaSemiAuto.exe"
#define OutputBaseFilename2 "SinbodaSemiAuto"+GetDateTimeString('_yyyy_mm_dd', '_', ':')

[Setup]
; 注: AppId的值为单独标识该应用程序。
; 不要为其他安装程序使用相同的AppId值。
; (生成新的GUID，点击 工具|在IDE中生成GUID。)
AppId={{61334835-735D-401B-9C6E-3027B2CA3CE2}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
;AppPublisherURL={#MyAppURL}
;AppSupportURL={#MyAppURL}
;AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=..\Output
OutputBaseFilename=   {#OutputBaseFilename2  }
;R1 第一次发布
;UsePreviousAppDir=no 

;SetupIconFile=E:\文件整理\C#\Start10\Start10\windows8.ico
;Password=975354488-ADB0-43EE-BF18-D6DE0D047423
;Encryption=yes
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; 
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\SinbodaSemiAuto\bin\x64\Debug\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\Config\*"; DestDir: "{app}\Config"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\Data\*"; DestDir: "{app}\Data"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\dll\x64\*"; DestDir: "{app}\dll\x64"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\dll\x86\*"; DestDir: "{app}\dll\x86"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\Help\*"; DestDir: "{app}\Help"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\PrintTemplate\Result_1_CN\*"; DestDir: "{app}\PrintTemplate\Result_1_CN"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\PrintTemplate\Result_1_EN\*"; DestDir: "{app}\PrintTemplate\Result_1_EN"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\PrintTemplate\Result_2_CN\*"; DestDir: "{app}\PrintTemplate\Result_2_CN"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\PrintTemplate\Result_2_EN\*"; DestDir: "{app}\PrintTemplate\Result_2_EN"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\PrintTemplate\Result_All_CN\*"; DestDir: "{app}\PrintTemplate\Result_All_CN"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\PrintTemplate\Result_All_EN\*"; DestDir: "{app}\PrintTemplate\Result_All_EN"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\Python310\*"; DestDir: "{app}\Python310"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\Python310\DLLs\*"; DestDir: "{app}\Python310\DLLs"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\Python310\Doc\*"; DestDir: "{app}\Python310\Doc"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\Python310\include\*"; DestDir: "{app}\Python310\include"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\Python310\include\cpython\*"; DestDir: "{app}\Python310\include\cpython"; Flags: ignoreversion
;Source: "..\SinbodaSemiAuto\bin\x64\Debug\Python310\include\internal\*"; DestDir: "{app}\Python310\include\cpython"; Flags: ignoreversion
; 注意: 不要在任何共享系统文件上使用“Flags: ignoreversion”

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent




