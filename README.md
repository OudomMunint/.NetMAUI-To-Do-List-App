# Check Mate ✅

A cross platform `To-Do list` shell application that performs CRUD operations and stores data locally. Made with `.Net MAUI`, `SQLite` & `.NET 9.0` for iOS & Android.

- No Login required.
- No Internet connection required.

<h1 float="center">
  <img src="Media/mainlist.png" style="height:350px; width:175" />
  <img src="Media/dashboard.png" style="height:350px; width:175" />
  <img src="Media/iphone14-1.png" style="height:350px; width:175" />
  <img src="Media/iphone14-3.png" style="height:350px; width:175" />
</h1>

# CI/CD Status & Release

[![CI Build](https://github.com/OudomMunint/.NetMAUI-To-Do-List-App/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/OudomMunint/.NetMAUI-To-Do-List-App/actions/workflows/ci.yml)

# Maintenance Status 🔹<a href="https://github.com/OudomMunint/.NetMAUI-To-Do-List-App/issues">Report Bug</a> &nbsp; &nbsp;

![maintenance-status](https://img.shields.io/badge/maintenance-passively--maintained-yellowgreen.svg)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)

# Supported Platforms

<table>
  <tr>
    <th>Platform</th>
    <th>Version</th>
    <th>Target</th>
    <th>Latest Stable</th>
  </tr>
  <tr>
    <td>Android</td>
    <td>API 22+</td>
    <td>API 34 / Android 14</td>
    <td>Android 14</td>
  </tr>
  <tr>
    <td>iOS</td>
    <td>iOS 15+</td>
    <td>iOS 16 & 17</td>
    <td>iOS 16.7.8 & 18.2</td>
  </tr>
</table>

# Required SDKs
- .NET & Xcode

## Minimum SDKs
- .Net 8.0 or newer from <a href="https://dotnet.microsoft.com/download/dotnet/8.0" target="_blank">`here`</a>
- XCode 15 or newer from <a href="https://developer.apple.com/xcode/" target="_blank">`here`</a>

## Recommended SDKs
- .Net 9.0 or newer from <a href="https://dotnet.microsoft.com/download/dotnet/9.0" target="_blank">`here`</a>
- XCode 16 or newer from <a href="https://developer.apple.com/xcode/" target="_blank">`here`</a>

# Screenshots

### Android & iOS, iPhone 15 / Galaxy Z Flip 4
<h1 float="center">
  <img src="Media/iosapplocked.png" style="height:350px; width:165" />
  <img src="Media/adapplocked.jpg" style="height:350px; width:150" />
</h1>

### Android & iOS, Galaxy Z Flip 4 / iPhone 15
<h1 float="center">
  <img src="Media/adatt.jpeg" style="height:350px; width:155" />
  <img src="Media/iosatt.jpeg" style="height:350px; width:165" />
</h1>

## Upcoming

- [ ] Swipe gestures
- [x] Dark mode
- [ ] Bottom Sheets
- [x] Attachments
- [x] Charts, Data Visualization
- [x] Foldable device support
- [x] .Net 8 Support
- [x] Biomertic Authentication
- [x] .Net 9 Support
- [x] Shell migration

## Getting Started

- Install `.NET 9` SDK from <a href="https://dotnet.microsoft.com/download/dotnet/9.0" target="_blank">`here`</a> on your machine.
- Install <a href="https://visualstudio.microsoft.com/downloads/" target="_blank">`Visual Studio`</a> on your machine and while choosing components you must check the <a href="https://dotnet.microsoft.com/en-us/learn/maui/first-app-tutorial/install" target="_blank">`.NetMaui`</a> box to install .NetMaui.
- Install <a href="https://developer.android.com/studio?gclid=Cj0KCQiAnNacBhDvARIsABnDa6-EYNc5MIjFoAruujioi9l-gjeu8JVsJd_aqCGGhImxOZkFyoo_woYaAoOCEALw_wcB&gclsrc=aw.ds" target="_blank">`Android Studio`</a> on your machine.
- Create a virtual device with andoid API 31 || 32 || 33 ||34.
- Clone, download or fork this repository.
- Open the solution file, build then run with selected device.
- If build failed with Dependency errors, please unload the project and reload with dependencies.
- Or `cd` to the project directory and run `dotnet restore {name}.sln` to restore dependencies.
- Has `SQLite` & `XUnit` dependency.

## Getting Started iOS

- Install `.NET 9` SDK from <a href="https://dotnet.microsoft.com/download/dotnet/9.0" target="_blank">`here`</a> on your machine.
- Install <a href="https://visualstudio.microsoft.com/vs/mac/" target="_blank">`Visual Studio for mac`</a> on your machine and while choosing components you must check the `.NetMaui` box.

> [!WARNING]
> Visual Studio for mac has been retired and does not support .NET 9.0 <br>
> Please use Visual Studio Code for .NET 9.0
> Or Rider IDE for .NET 9.0

- Install <a href="https://developer.apple.com/xcode/" target="_blank">`XCode 16+`</a> on your machine.
- Clone, download or fork this repository.
- Open the solution file, build then run with selected device iOS 15+.
- If build failed with Dependency errors, please unload the project and reload with dependencies.
- Or `cd` to the project directory and run `dotnet restore {name}.sln` to restore dependencies.
- Has `SQLite` & `XUnit` dependency same with android.

## Permissions
- Android: `Read & Write External Storage`, `Read & Write Internal Storage`, `Camera access`, `Haptic feedback`, `Biometric information`.
- iOS: `Camera access`, `Photo Library access`, `Read & Write External Storage`, `Read & Write Internal Storage`, `Haptic feedback`, `Biometric information`.

> [!NOTE]  
> FaceID, TouchID and AndroidOS equivalent must be enrolled or settings will be disabled.
> Biomertic information access is required.

## Clean scripts

### This script finds and deletes `bin` / `obj` folders as well as `.DS_Store` files.

- Place the `build.sh` in the root directory of your project.
- cd to the root directory of your project.
- Open the terminal and run `chmod +x build.sh` to make the script executable.
- Run `./build.sh` to clean the project.
- on macos, you can run `sh build.sh` to clean the project.