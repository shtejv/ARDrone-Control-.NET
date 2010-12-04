/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010 Thomas Endres, Stephen Hobley, Julien Vinel
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

#include "stdafx.h"
#include "arDroneDLL.h"

jint JNICALL Java_ardrone_ARDrone_initDrone(JNIEnv *env, jclass cls)
{
	return InitDrone();
}

jboolean JNICALL Java_ardrone_ARDrone_updateDrone(JNIEnv *env, jclass cls)
{
	return UpdateDrone();
}

jboolean JNICALL Java_ardrone_ARDrone_shutdownDrone(JNIEnv *env, jclass cls)
{
	return ShutdownDrone();
}

jint JNICALL Java_ardrone_ARDrone_getDroneState(JNIEnv *env, jclass cls)
{
	return GetDroneState();
}

jint JNICALL Java_ardrone_ARDrone_getBatteryLevel(JNIEnv *env, jclass cls)
{
	return GetBatteryLevel();
}

jdouble JNICALL Java_ardrone_ARDrone_getTheta(JNIEnv *env, jclass cls)
{
	return GetTheta();
}

jdouble JNICALL Java_ardrone_ARDrone_getPhi(JNIEnv *env, jclass cls)
{
	return GetPhi();
}

jdouble JNICALL Java_ardrone_ARDrone_getPsi(JNIEnv *env, jclass cls)
{
	return GetPsi();
}

jint JNICALL Java_ardrone_ARDrone_getAltitude(JNIEnv *env, jclass cls)
{
	return GetAltitude();
}

jdouble JNICALL Java_ardrone_ARDrone_getVX(JNIEnv *env, jclass cls)
{
	return GetVX();
}

jdouble JNICALL Java_ardrone_ARDrone_getVY(JNIEnv *env, jclass cls)
{
	return GetVY();
}

jdouble JNICALL Java_ardrone_ARDrone_getVZ(JNIEnv *env, jclass cls)
{
	return GetVZ();
}

jint JNICALL Java_ardrone_ARDrone_changeToBottomCamera(JNIEnv *env, jclass cls)
{
	return ChangeToBottomCamera();
}

jint JNICALL Java_ardrone_ARDrone_changeToFrontCamera(JNIEnv *env, jclass cls)
{
	return ChangeToFrontCamera();
}

jint JNICALL Java_ardrone_ARDrone_sendFlatTrim(JNIEnv *env, jclass cls)
{
	return SendFlatTrim();
}

jint JNICALL Java_ardrone_ARDrone_sendEmergency(JNIEnv *env, jclass cls)
{
	return SendEmergency();
}

jint JNICALL Java_ardrone_ARDrone_sendTakeoff(JNIEnv *env, jclass cls)
{
	return SendTakeoff();
}

jint JNICALL Java_ardrone_ARDrone_sendLand(JNIEnv *env, jclass cls)
{
	return SendLand();
}

jint JNICALL Java_ardrone_ARDrone_setProgressCmd(JNIEnv *env, jclass cls, jboolean hovering, jfloat roll, jfloat pitch, jfloat gaz, jfloat yaw)
{
	return SetProgressCmd(hovering, roll, pitch, gaz, yaw);
}

jbyteArray JNICALL Java_ardrone_ARDrone_getCurrentImage(JNIEnv *env, jclass cls)
{
	//TODO Does not work ... try to find a way to convert byte* to jbyteArray

	_int8* imageBytes = GetCurrentImage();

	jbyteArray resultingArray = env->NewByteArray(sizeof(imageBytes));
	jbyte *bytes = env->GetByteArrayElements(resultingArray, 0);

	for ( int i = 0; i < sizeof(imageBytes); i++ )
	{
		bytes[i] = imageBytes[i];
	}
	env->SetByteArrayRegion(resultingArray, 0, sizeof(imageBytes), bytes);

	return resultingArray;
}

jint JNICALL Java_ardrone_ARDrone_test(JNIEnv *env, jclass cls)
{
	return TestLibrary();
}