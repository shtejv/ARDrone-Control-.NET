License:

ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
Copyright (C) 2010, 2011 Thomas Endres, Stephen Hobley, Julien Vinel

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.

-----------------------------------------------------------------
ARDrone.Net library for the Parrot AR Drone
Stephen Hobley, Thomas Endres and Julien Vinel November 2010
-----------------------------------------------------------------
You must have the DirectX and Windows SDK installed in order to compile this solution file.

A few quick tips to get the app up and running:
- This is a Visual Studio 2010 project using .NET framework 4.0
- You need to download and install Windows SDK and a fresh copy of the DirectX SDK
  - Get DirectX from http://msdn.microsoft.com/en-us/directx/aa937788.aspx
  - Get Windows SDK from http://msdn.microsoft.com/de-de/windows/aa904949.aspx
- You need to disable the loader lock exception in Visual Studio (Debug -> Exceptions -> Managed Debugging assistents -> Disable the "Loader lock" checkbox)
- You need to set the start project to ARDRoneUI_WPF (or ARDroneUI_Detection for the detection examples)
  - Do this by right clicking the project and selecting "Set as start project" from the context menu

Once you have completed these steps, the solution should compile.
Since this application is using the Microsoft Speech API, it should not be working with Windows XP versions prior SP2.

For more information, visit our websites:
https://github.com/shtejv/ARDrone-Control-.NET
http://parrotsonjava.com
http://stephenhobley.com/blog

This software uses the following libraries:
- WiiMoteLib:				http://wiimotelib.codeplex.com/
- Aviation Instruments:		http://www.codeproject.com/KB/miscctrl/Avionic_Instruments.aspx
- AviFile Library:			http://www.codeproject.com/KB/audio-video/avifilewrapper.aspx

The software uses various tips and tricks as well as some classes from the internet. Where possible, the names of the original authors were mentioned in the legal notices.



