MKDIR %Tmp%\MTS_installer 
XCOPY . %Tmp%\MTS_installer\ /S /E /Y 
start %Tmp%\MTS_installer\setup.exe