Instructions to build a OSX package:

Create a folder called /Applications/PdfAutoSignerPkg/pkg1
Copy publish, resources and scripts folders to /Applications/PdfAutoSignerPkg
Copy the distribution.xml and com.plantanapp.pdfautosigner.plist files to /Applications/PdfAutoSignerPkg

pkgbuild --root /Applications/PdfAutoSignerPkg/publish --identifier com.plantanapp.pdfautosigner --version 1.0.0.0 --ownership recommended --scripts scripts pkg1/output.pkg

productbuild --distribution distribution.xml --resources resources --package-path pkg1 --version 1.0.0.0 ./PdfAutoSignerPkg.pkg
