/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010 Thomas Endres, Stephen Hobley, Julien Vinel
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

#include <jni.h>

#ifndef _Included_ardrone_ARDrone
#define _Included_ardrone_ARDrone
#ifdef __cplusplus
extern "C" {
#endif
	JNIEXPORT jint JNICALL Java_ardrone_ARDrone_initDrone(JNIEnv *, jclass);
	JNIEXPORT jboolean JNICALL Java_ardrone_ARDrone_updateDrone(JNIEnv *, jclass);
	JNIEXPORT jboolean JNICALL Java_ardrone_ARDrone_shutdownDrone(JNIEnv *, jclass);
	JNIEXPORT jint JNICALL Java_ardrone_ARDrone_getDroneState(JNIEnv *, jclass);
	JNIEXPORT jint JNICALL Java_ardrone_ARDrone_getBatteryLevel(JNIEnv *, jclass);
	JNIEXPORT jdouble JNICALL Java_ardrone_ARDrone_getTheta(JNIEnv *, jclass);
	JNIEXPORT jdouble JNICALL Java_ardrone_ARDrone_getPhi(JNIEnv *, jclass);
	JNIEXPORT jdouble JNICALL Java_ardrone_ARDrone_getPsi(JNIEnv *, jclass);
	JNIEXPORT jint JNICALL Java_ardrone_ARDrone_getAltitude(JNIEnv *, jclass);
	JNIEXPORT jdouble JNICALL Java_ardrone_ARDrone_getVX(JNIEnv *, jclass);
	JNIEXPORT jdouble JNICALL Java_ardrone_ARDrone_getVY(JNIEnv *, jclass);
	JNIEXPORT jdouble JNICALL Java_ardrone_ARDrone_getVZ(JNIEnv *, jclass);
	JNIEXPORT jint JNICALL Java_ardrone_ARDrone_changeToBottomCamera(JNIEnv *, jclass);
	JNIEXPORT jint JNICALL Java_ardrone_ARDrone_changeToFrontCamera(JNIEnv *, jclass);
	JNIEXPORT jint JNICALL Java_ardrone_ARDrone_sendFlatTrim(JNIEnv *, jclass);
	JNIEXPORT jint JNICALL Java_ardrone_ARDrone_sendEmergency(JNIEnv *, jclass);
	JNIEXPORT jint JNICALL Java_ardrone_ARDrone_sendTakeoff(JNIEnv *, jclass);
	JNIEXPORT jint JNICALL Java_ardrone_ARDrone_sendLand(JNIEnv *, jclass);
	JNIEXPORT jint JNICALL Java_ardrone_ARDrone_setProgressCmd(JNIEnv *, jclass, jboolean, jfloat, jfloat, jfloat, jfloat);
	JNIEXPORT jbyteArray JNICALL Java_ardrone_ARDrone_getCurrentImage(JNIEnv *, jclass);
	JNIEXPORT jint JNICALL Java_ardrone_ARDrone_test(JNIEnv *, jclass);
#ifdef __cplusplus
}
#endif
#endif

extern "C" __declspec(dllimport) int _stdcall TestLibrary();
extern "C" __declspec(dllimport) int _stdcall InitDrone();
extern "C" __declspec(dllimport) BOOL _stdcall UpdateDrone();
extern "C" __declspec(dllimport) BOOL _stdcall ShutdownDrone();
extern "C" __declspec(dllimport) int _stdcall GetBatteryLevel();
extern "C" __declspec(dllimport) double _stdcall GetTheta();
extern "C" __declspec(dllimport) double _stdcall GetPhi();
extern "C" __declspec(dllimport) double _stdcall GetPsi();
extern "C" __declspec(dllimport) int _stdcall GetAltitude();
extern "C" __declspec(dllimport) double _stdcall GetVX();
extern "C" __declspec(dllimport) double _stdcall GetVY();
extern "C" __declspec(dllimport) double _stdcall GetVZ();
extern "C" __declspec(dllimport) int _stdcall ChangeToBottomCamera();
extern "C" __declspec(dllimport) int _stdcall ChangeToFrontCamera();
extern "C" __declspec(dllimport) int _stdcall SendFlatTrim();
extern "C" __declspec(dllimport) int _stdcall SendEmergency();
extern "C" __declspec(dllimport) int _stdcall SendTakeoff();
extern "C" __declspec(dllimport) int _stdcall SendLand();
extern "C" __declspec(dllimport) int _stdcall SetProgressCmd(BOOL bhovering, float roll, float pitch, float gaz, float yaw);
extern "C" __declspec(dllimport) __int8* _stdcall GetCurrentImage();