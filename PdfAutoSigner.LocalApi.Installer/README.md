Project built based on https://www.youtube.com/watch?v=6Yf-eDsRrnM

Install the WIX Toolset and the Wix Extension for Visual Studio from https://wixtoolset.org/

The client has to run the msi as an administrator.

Change the two ProductCode entries each time you change the version of the application. Never change the UpgradeCode.

The project has some before sections to run customized scripts: in order to see it the project has to be edited in an editor.
In the before section there is a HeatDirectory instruction. This uses heat to automatically generate the ComponentsGenerated.wxs file which contains the files from the publish folder. The first time this was run I needed to first comment out the corresponding lines from the Product.wxs; you should not need to do this any longer.
Regarding the HeatDirectory there is a RunWixToolsOutOfProc line in the PropertyGroup and a RunAsSeparateProcess attribute in the HeatDirectory: this is because heat has some issues running on 64 bit machines. Do not alter this part without properly testing it.
Also, regarding the HeatDirectory: we use a RemoveExeComponentsTransform.xslt. This is because we cannot easily exclude files from the harvesting process. This allows us to remove any executables. We need to manually add the executable for the local api in the Product.wxs file so that we can create a Windows Service that links to this executable.

I recommend not to change the ProductCode at all. Keeping the same ProductCode will force the client to uninstall the previous version, which I think it is the safest approach. Uninstalling or installing the application should first cause the related existing Windows Service to uninstall.