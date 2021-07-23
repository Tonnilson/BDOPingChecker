# Black Desert Ping Checker

Simple C# WPF Application for checking the ping of the connected server for Black Desert Online (PC) [NA/EU]

## Description

As unfortunate as it is that this is not natively built into the game I decided to make a quick app that you can drag over the top of your game in Full Screen Window mode to display current ping to whatever server you may be connected to. Will not sit over top of the game while using Full Screen due to DirectX taking priority for top most view, in the event you're using Full screen you can drag it off to another screen to see it from there.

Yes it is safe to use, it does not interfere with the game in any way. It's equivalent to using Resource Monitor -> Network -> TCP Connections to check your ping but without the headache and guessing.

![Example Image](https://i.imgur.com/ZyBnDNx.png)

## Getting Started

### Dependencies

* [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472)
* Game running and connected to a server

### Installing

* Download from [My personal website](http://tonic.pw/files/bdo/BDOPingChecker.exe), [Releases section](https://github.com/Tonnilson/BDOPingChecker/releases) or compile your own

## Features
* Dragging anywhere on the screen while saving the last position on application close
* Change background opacity from the settings window accessed via the Tasktray
* Lock the window from being moved

## Help

### It just says NCON
As stated above the game must be running and you must be in a server. This is only tested for NA but the ports should be the same for EU, other regions are unknown. Each region/version of the game utilizes different ports.

## Version History

* 1.0
    * Initial Release
