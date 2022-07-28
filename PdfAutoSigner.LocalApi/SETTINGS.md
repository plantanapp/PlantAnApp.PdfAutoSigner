
# Intro

The PdfAutoSigner program supports PDF file signing via the following two approaches:  
1. [PKCS#11](https://en.wikipedia.org/wiki/PKCS_11) (open standard that specifies an API for cryptographic devices) 
2. [CryptoAPI](https://en.wikipedia.org/wiki/Microsoft_CryptoAPI) (Microsoft's proprietary cryptography subsystem)

In order to sign a PDF file the user must choose a signature.  
To help the user with selecting the correct signature, the program can list the available signatures that can be used.  
To list the available signatures the program uses a json [configuration file](tokensettings.json)

# Configuration file

The configuration file has two sections called "Pkcs11Devices" (for the PKCS#11 mode) and "Certificates" (for the CryptoAPI mode).  
The content for each of these sections is a list of entries.
Each entry will have a property called "Name" which is only used to identify the entry for the user (the Name is not used by the program).  

# PKCS#11 sction sample
```json
"Pkcs11Devices": [{
    "Name": "SafeNet",
    "Pkcs11LibPaths": [{
        "OS": "Windows",
        "Architecture": "X64",
        "LibPath": "C:\\Program Files (x86)\\Gemalto\\IDGo 800 PKCS#11\\IDPrimePKCS1164.dll"
    }]
}]
```

# PKCS#11 section description

The PKCS#11 mode uses native libraries provided by the token manufacturers.  
These libraries are installed on the user's system by the installer that comes with the hardware token.  
The libraries and the paths where they are installed are different for combinations of OS and architecture.  
For example: Windows 64 bit will have one path, while MacOS 64 bit will have another path.
The config file needs to specify the paths for all the libraries that will be used to sign PDF files.  
As such the "Pkcs11Devices" will have a list of entries, one entry per device.
Each device entry will have another subsection called "Pkcs11LibPaths" containing a list of entries defining the library paths for that device on different OS - architecture combinations.  
The program will use the corresponding library paths for the user's OS - architecture combination to identify the available devices that can be used to sign the PDF file.
Valid OS entries: Windows, MacOS and Linux.
Valid Architecture entries: X86 and X64.


# CryptoAPI section sample
```json
"Certificates": [{
    "Name": "CertSign",
    "CertificateIssuerName": "certSIGN"
}]
```

# CryptoAPI section description

The CryptoAPI mode uses certificates installed in the certificate store.
Due to the fact that there can be many certificates installed on user's machine the program uses the certificate issuer name to filter certificates that can be used to sign documents.
The "Certificates" section will have a list of entries and each entry has a "CertificateIssuerName" property.
Only the certificates that contain in their issuer name one of the specified certificate issuer names will be displayed to the user.