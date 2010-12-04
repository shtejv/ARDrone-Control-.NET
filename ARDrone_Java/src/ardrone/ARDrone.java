/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010 Thomas Endres, Stephen Hobley, Julien Vinel
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

package ardrone;

public class ARDrone {
	native public static int initDrone();
	native public static boolean updateDrone();
	native public static boolean shutdownDrone();
	native public static int getDroneState();
	native public static int getBatteryLevel();
	native public static double getTheta();
	native public static double getPhi();
	native public static double getPsi();
	native public static int getAltitude();
	native public static double getVX();
	native public static double getVY();
	native public static int getVZ();
	native public static int changeToBottomCamera();
	native public static int changeToFrontCamera();
	native public static int sendFlatTrim();
	native public static int sendEmergency();
	native public static int sendTakeoff();
	native public static int sendLand();
	native public static int setProgressCmd(boolean hovering, float roll, float pitch, float gaz, float yaw);
	//TODO implemented incorrectly
	native public static byte[] getCurrentImage();		// Encoded in RGB24
	native public static int test();
	
    static
    {
    	System.loadLibrary("ARDroneDLL");
        System.loadLibrary("ARDroneDLL_JNI");
    }
}