Publish as a self-contained app:
dotnet publish -c Release --self-contained -r win-x64 -p:PublishTrimmed=true -p:PublishSingleFile=true
dotnet publish -c Release --self-contained -r osx-x64 -p:PublishTrimmed=true -p:PublishSingleFile=true

Run on mac:
- pkcs11
./PdfAutoSigner.Lib.App ../hello.pdf ../hello-signed.pdf /usr/local/lib/libeTPkcs11.dylib -p [pin]
- cert
./PdfAutoSigner.Lib.App ./hello.pdf ./hello-signed.pdf certSign -p [pin] -c

Run on win:
- pkcs11
./PdfAutoSigner.Lib.App ./hello.pdf ./hello-signed.pdf "C:\Program Files (x86)\Gemalto\IDGo 800 PKCS#11\IDPrimePKCS1164.dll" -p [pin]
- cert
./PdfAutoSigner.Lib.App ./hello.pdf ./hello-signed.pdf certSign -p [pin] -c

Observations:
The pin can also be obtained from UserSecrets (in VS) from the "TokenPin" path.
For ease of use a hello.pdf file was provided in the root of the project and also 2 launch configurations that can be used to quickly test both types of signatures. Bear in mind you would need the appropiate usb token and setup the pin in the "TokenPin" in UserSecrets if you want to use the launch configurations as they are.