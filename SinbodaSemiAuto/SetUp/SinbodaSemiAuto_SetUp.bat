@echo ��������С�����

::��ʼ����
set devnev="C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\devenv.exe"
%devnev%  "..\SinbodaSemiAuto.sln"  /rebuild "Debug|x64"

::ɾ������Ŀ¼�е� log�ļ���
rd /s /q  "..\SinbodaSemiAuto\bin\x64\Debug\Log"

::��ʼ���
compil32 /cc "SinbodaSemiAuto_SetUp.iss"

pause