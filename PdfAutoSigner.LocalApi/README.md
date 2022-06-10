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