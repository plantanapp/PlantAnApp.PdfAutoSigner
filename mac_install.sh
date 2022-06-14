#!/bin/bash

# We need to first create the destination folder
sudo mkdir -p /Applications/PlantAnApp/PdfAutoSigner

# Copy the file to the corresponding folder
sudo cp -a ./publish/. /Applications/PlantAnApp/PdfAutoSigner

# Give the app file execution permission
sudo chmod +x /Applications/PlantAnApp/PdfAutoSigner/PdfAutoSigner.LocalApi

# Copy the plist file in the daemons folder
sudo cp ./com.plantanapp.pdfautosigner.plist /Library/LaunchDaemons/

# -w flag permanently adds the plist to the Launch Daemon
sudo launchctl load -w /Library/LaunchDaemons/com.plantanapp.pdfautosigner.plist