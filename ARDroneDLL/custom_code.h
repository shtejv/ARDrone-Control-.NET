/********************************************************************
 *                    COPYRIGHT PARROT 2010
 ********************************************************************
 *       PARROT - A.R.Drone SDK Windows Client Example
 *-----------------------------------------------------------------*/
/**
 * @file custom_code.h 
 * @brief Header file for user-added code.
 *
 * @author Stephane Piskorski <stephane.piskorski.ext@parrot.fr>
 * @date   Sept, 8. 2010
 *
 *******************************************************************/


#ifndef _MYKONOS_TESTING_TOOL_H_
#define _MYKONOS_TESTING_TOOL_H_

#ifdef __cplusplus
extern "C" {
#endif

#include <stdio.h>
#include <VP_Os/vp_os_types.h>

C_RESULT signal_exit();

void ARWin32Demo_SetConsoleCursor(int x,int y);
void ARWin32Demo_AcquireConsole();
void ARWin32Demo_ReleaseConsole();

#ifdef __cplusplus
}
#endif


#endif // _MYKONOS_TESTING_TOOL_H_
