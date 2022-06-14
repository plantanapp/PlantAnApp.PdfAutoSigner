#!/bin/bash

sudo launchctl unload /Library/LaunchDaemons/org.plantanapp.pdfautosigner.plist

sudo rm /Library/LaunchDaemons/org.plantanapp.pdfautosigner.plist

sudo rm -r /Applications/PlantAnApp/PdfAutoSigner

sudo rmdir /Applications/PlantAnApp