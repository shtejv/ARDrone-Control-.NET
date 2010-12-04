/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010 Thomas Endres, Stephen Hobley, Julien Vinel
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

package expose;

import ardrone.*;

public class Expose {
	public static void main(String[] args) {
		Integer connectedState = ARDrone.initDrone();
		System.out.println("Connected state: " + connectedState.toString());
		
		if (connectedState == 0) {
			UpdateThread updateThread = new UpdateThread();
			updateThread.start();
			
			//sendTakeOffAndLandAgain();
			
			while (true) {
				//Integer currentAltitude = ARDrone.getAltitude();
				//System.out.println("Current altitude: " + currentAltitude.toString());
				
				byte[] imageBytes = ARDrone.getCurrentImage();
				System.out.println("Byte array size: " + imageBytes.length);
				
				sleep(100);
			}
		}
	}
	
	public static void sendTakeOffAndLandAgain() {
		sleep(500);
		ARDrone.sendTakeoff();
		sleep(1000);
		ARDrone.sendLand();
	}
	
	public static void sleep(int milliseconds) {
		try {
			Thread.sleep(milliseconds);
		} catch (Exception e) { }
	}
}
