@echo 程序编译中。。。

::开始编译
set devnev="C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\devenv.exe"
%devnev%  "..\SinbodaSemiAuto.sln"  /rebuild "Debug|x64"

::删除生成目录中的 log文件夹
rd /s /q  "..\SinbodaSemiAuto\bin\x64\Debug\Log"

::开始打包
compil32 /cc "SinbodaSemiAuto_SetUp.iss"

pause