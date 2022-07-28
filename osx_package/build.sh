#!/bin/bash

# Copy the license text in the resources folder
mkdir -p ./resources

/bin/cp -rf ../license.rtf ./resources

pkgbuild --root ./publish --identifier com.plantanapp.pdfautosigner --version 1.0.0.0 --ownership recommended --scripts ./scripts ./output.pkg

productbuild --distribution ./distribution.xml --resources ./resources --package-path ./ --version 1.0.0.0 ./PdfAutoSigner.pkg