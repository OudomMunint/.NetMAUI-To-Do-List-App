# Check Mate ✅
A `To-Do list` app that performs CRUD operations. Made with `.Net MAUI` & `SQLite`

# CI/CD Status & Release
[![CI Build](https://github.com/OudomMunint/.NetMAUI-To-Do-List-App/actions/workflows/ci.yml/badge.svg)](https://github.com/OudomMunint/.NetMAUI-To-Do-List-App/actions/workflows/ci.yml)

# Maintenance Status 🔹<a href="https://github.com/OudomMunint/.NetMAUI-To-Do-List-App/issues">Report Bug</a> &nbsp; &nbsp;
![maintenance-status](https://img.shields.io/badge/maintenance-actively--developed-brightgreen.svg)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)

# Supported Platforms
<table>
  <tr>
    <th>Platform</th>
    <th>Version</th>
    <th>Target</th>
  </tr>
  <tr>
    <td>Android</td>
    <td>API 31+</td>
    <td>API 33</td>
  </tr>
  <tr>
    <td>iOS</td>
    <td>iOS 14.2+</td>
    <td>iOS 16.3 & iOS 17.0</td>
  </tr>
  <tr>
    <td>macOS</td>
    <td>macOS 12+ (Monterey)</td>
    <td>macOS 13.6 (Ventura)</td>
  </tr>
</table>

<h1 float="center">
  <img src="Media/iphone14 (6).png" style="height:700px; width:360" />
  <img src="Media/iphone14 (5).png" style="height:700px; width:360" />
</h1>

<div float="center">
<img src="Media/iphone14 (1).png" style="height:700px; width:370" />
<img src="Media/iphone14 (4).png" style="height:700px; width:370" />
</div>

<div float="center">
<img src="Media/ad1.png" style="height:700px; width:340" />
<img src="Media/ad2.png" style="height:700px; width:340" />
</div>

## Upcoming
- `Bottom Sheets` to pull up edit screen or create task screen
- Data visualization with `Charts` or `Graphs`
- WIP: SwipeView Add, SwipeView Delete

## Getting Started
- Install <a href="https://visualstudio.microsoft.com/downloads/" target="_blank">`Visual Studio`</a> on your machine and while choosing components you must check the <a href="https://dotnet.microsoft.com/en-us/learn/maui/first-app-tutorial/install" target="_blank">`.NetMaui`</a> box to install .NetMaui.
- Install <a href="https://developer.android.com/studio?gclid=Cj0KCQiAnNacBhDvARIsABnDa6-EYNc5MIjFoAruujioi9l-gjeu8JVsJd_aqCGGhImxOZkFyoo_woYaAoOCEALw_wcB&gclsrc=aw.ds" target="_blank">`Android Studio`</a> on your machine.
- Create a virtual device with andoid API 31, 32 or 33.
- Clone, download or fork this repository.
- Delete the `bin` and `obj` folder if present.
- Open the solution file
- ctr + Shift + B to build
- Run the solution.
- If build failed with Dependency errors, please unload the project and reload with dependencies.
- Or `cd` to the project directory and run `dotnet restore {name}.sln` to restore dependencies.
- Has `SQLite` & `XUnit` dependency.

## Clean scripts
### This script finds and deletes `bin` / `obj` folders as well as `.DS_Store` files.
- Place the `build.sh` in the root directory of your project.
- cd to the root directory of your project.
- Open the terminal and run `chmod +x build.sh` to make the script executable.
- Run `./build.sh` to clean the project.
- on macos, you can run `sh build.sh` to clean the project.