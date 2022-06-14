Build the the PdfSigner.LocalApi.exe as a self contained single file following the instructions on the corresponding project.
Copy the publish folder at the same level with the corresponding installation file.
The installation file needs to be run with admin rights.

Windows:
The installation file is win_install.bat. This will copy the publish folder content into a fixed folder (C:\Program Files\PlantAnApp\PdfAutoSigner) and will create a windows service called PlantAnApp.PdfAutoSigner with startup type automatic.

MacOS:
The Unix line endings must be preserved
Besides the publish folder and the mac_install.sh file, also copy the plist file next to the installation file.

