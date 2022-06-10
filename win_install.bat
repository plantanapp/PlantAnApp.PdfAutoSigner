set destinationdir=%PROGRAMFILES%\PlantAnApp\PdfAutoSigner\
Xcopy "%~dp0publish\" "%destinationdir%" /E /Y
sc.exe create PlantAnApp.PdfAutoSigner binPath= "\"%destinationdir%\PdfAutoSigner.LocalApi.exe\"" start= auto