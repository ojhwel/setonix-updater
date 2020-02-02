# Setonix Updater

A small executable allowing (semi-) automatic updating of .NET applications.

It works in tests on a few machines, but is by no means production-ready in the traditional sense. (In other words,
you can use it to update your application but be ready to do a lot of tech support on users' machines.)


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
  System.Reflection.Assembly.GetExecutingAssembly().Location);` (That's the process ID of your application and the path to your application's executable)')
- Quit your application

### What Happens Then

- Setonix Updater makes sure the process ID is not longer pointing to a running process. If it still does after a few seconds, the user gets a "Please close" message 
  and can retry
- All files in the `update.manifest` are copied from the temp folder to the application folder
- Setonix Updater starts your application and quits
  - An extra command line argument is sent to your application to allow cleaning up the temp folder. You can do this by adding the following statement to your
    application: `args = UpdateHelper.HandleTempFolderCleanup(args);` This removes both the temp folder from the disk **and** the extra argument from `args`. 
    (If you don't do this, your application will have to ignore a `--setonix-cleanup=C:\Users\user\Temp\...` argument.)


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

You can theoretically have more than one version in the file, using it as a basis for a download page of older versions or some such, but currently only the 
entry with the highest version number **and** the newest release date is used. If the version with the highest version number is not the one with the newest 
release date, nothing will happen.


### Update ZIP file

This must contain:
- sn `update.manifest` file (see below)
- all files referenced in the manifest

Both the `update.manifest` and the ZIP file must contain `SetonixUpdater.exe` as the unzipped copy in the temp folder is used to perform the update. This allows
deploying a newer version of the upater and using it immediately.

### Update Manifest

Another XML file that has to be in the ZIP file of the new application version and which must be called `update.manifest`. This declares which files to overwrite 
in which folders under your application folder. 

```xml
<?xml version="1.0" encoding="utf-8" ?>
<manifest>
  <folder path=".">
    <file name="YourApp.exe" />
    <file name="Referenced.dll" />
    <file name="setonix_updater.exe" /> <!-- always include this -->
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
- `appname` - the name of your application, currently only used in a message box shown if the application is still running
- `title` - the window title
- `wait` - a short message telling the user what's happening


### Logging

There is some rudimentary logging, which is always done to the system temp folder into a file called `setonix_updater_<date>.log`. The log level defaults to
ERROR and can be set by placing a file called `setonix_loglevel.<loglevel>` in the temp folder where the extension is one of the following: `error`, `warn`, 
`info`, `debug`. (If there are multiple of these files, the most verbose log level wins.)