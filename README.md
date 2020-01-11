# Setonix Updater

A small executable allowing (semi-) automatic updating of .NET applications.

It's currently a work in progress. 


## Overview

### What You'll Need

- A web server containing:
  - an XML file declaring the current version
  - a ZIP of the current version, as referenced in the XML

### How to Use in Your Application

- Add `SetonixUpdater` to your project as a reference
- Create an instance of `SetonixUpdater.Download.UpdateHelper` specifying the version currently installed locally and the URL of the XML file with the version
  information
- Call `UpdateHelper.CheckForUpdate()`; if this returns `true`, ask your users whether they want to update (or just do it during startup or shutdown, depending
  on how draconian you want/need to be)
- Call `UpdateHelper.DownloadUpdate()`; this returns a `DirectoryInfo` instance pointing to a temp folder where the ZIP has been decompessed to
- Start the update by calling `updateHelper.StartUpdate(System.Diagnostics.Process.GetCurrentProcess().Id, 
  System.Reflection.Assembly.GetExecutingAssembly().Location);`
- Quit your application:
  ```C#
  Application.Exit();
  return;
  ```
- If you add the line `args = UpdateHelper.HandleTempFolderCleanup(args)` to the startup code of your application before you start doing anything with the
  command line arguments, the temp folder will be cleaned. (If you **don't** do this, you will find a `--setonix-cleanup` argument which 
  `HandleTempFolderCleanup()` removes.)

### What Happens Then

- Setonix Updater makes sure the process ID is not longer pointing to a running process. If it still does after a few seconds, you get a "Please close" message 
  and can retry.
- All files in the `update.manifest` are copied from the temp folder to the application folder
- Setonix Updater starts your application and quits


## More Details

### Versions List

An XML file on your server:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<versions>
    <version id="1.0.0.1138">
        <name>1.0 beta</name>
        <release-date>2010-01-08</release-date>
        <url>http://your_link_here/app_1.0.0.1138.zip</url>
    </version>
</versions>    
```

You can theretically have more than one version in the file. The one with the highest version number **and** the newest release date is used. If the version 
with the highest version number is not the one with the newest release date, nothing is done.


### Update ZIP file

This must contain:
- `update.manifest` (see below)
- all files referenced in the manifest


### Update Manifest

Another XML file that has to be in the ZIP of the new application version and which must be called `update.manifest`. This declares which files to overwrite 
in which folders under your application folder. 

```xml
<?xml version="1.0" encoding="utf-8" ?>
<manifest>
  <folder path=".">
    <file name="YourApp.exe" />
    <file name="Referenced.dll" />
    <file name="setonix_updater.exe" /> <!-- might be a good idea to include -->
  </folder>
  <folder path="Data">
    <file name="SomeFile.xml" />
  </folder>
  <messages>
    <message key="appname" language="en">Your Application Name</message>
    <message key="title" language="en">Update</message>
    <message key="wait" language="en">Your Application is being updated. Please wait...</message>
  </messages>
</manifest>
```

It also declares some bits of text for the updater window:
- `title` - the window title
- `wait` - a short message telling the user what's happening
- `appname` - the name of your application, currently only used in a message box shown if the application is still running
