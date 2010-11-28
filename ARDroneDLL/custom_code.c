/********************************************************************
 *                    COPYRIGHT PARROT 2010
 ********************************************************************
 *       PARROT - A.R.Drone SDK Windows Client Example
 *-----------------------------------------------------------------*/
/**
 * @file custom_code.c 
 * @brief User added code
 *
 * @author sylvain.gaeremynck@parrot.com
 * @date 2009/07/01
 *
 * @author Stephane Piskorski <stephane.piskorski.ext@parrot.fr>
 * @date   Sept, 8. 2010
 *
 *******************************************************************/



#include <custom_code.h>

//ARDroneLib
	#include <ardrone_tool/ardrone_time.h>
	#include <ardrone_tool/Navdata/ardrone_navdata_client.h>
	#include <ardrone_tool/Control/ardrone_control.h>
	#include <ardrone_tool/UI/ardrone_input.h>

//Common
	#include <config.h>
	#include <ardrone_api.h>

//VP_SDK
	#include <ATcodec/ATcodec_api.h>
	#include <VP_Os/vp_os_print.h>
	#include <VP_Api/vp_api_thread_helper.h>
	#include <VP_Os/vp_os_signal.h>

//Local project
	#include <Video/video_stage.h>
	#include <UI/directx_rendering.h>

//Global variables
	int32_t exit_ihm_program = 1;
	vp_os_mutex_t consoleMutex;


/* Implementing Custom methods for the main function of an ARDrone application */



/*--------------------------------------------------------------------
The delegate object calls this method during initialization of an 
ARDrone application 
--------------------------------------------------------------------*/
C_RESULT ardrone_tool_init_custom(int argc, char **argv)
{
	/* Change the console title */
		vp_os_mutex_init(&consoleMutex);
		//system("cls");

	/* Start all threads of your application */
		START_THREAD( video_stage, NULL );
  
  return C_OK;
}



/*--------------------------------------------------------------------
The delegate object calls this method when the event loop exit     
--------------------------------------------------------------------*/
C_RESULT ardrone_tool_shutdown_custom()
{
  /* Relinquish all threads of your application */
  JOIN_THREAD( video_stage );

  vp_os_mutex_destroy(&consoleMutex);

  return C_OK;
}

/*--------------------------------------------------------------------
The event loop calls this method for the exit condition            
--------------------------------------------------------------------*/
bool_t ardrone_tool_exit()
{
  return exit_ihm_program == 0;
}

C_RESULT signal_exit()
{
  exit_ihm_program = 0;

  return C_OK;
}

int custom_main(int argc,char**argv) { return 0;};


/* Implementing thread table in which you add routines of your application and those provided by the SDK */
BEGIN_THREAD_TABLE
  THREAD_TABLE_ENTRY( ardrone_control, 50 )
  THREAD_TABLE_ENTRY( navdata_update, 50 )
  THREAD_TABLE_ENTRY( video_stage, 50 )
END_THREAD_TABLE



/* Function to change the cursor place in the console window */

	HANDLE hStdout =  NULL;  /* Handle to the output console */
	CONSOLE_SCREEN_BUFFER_INFO csbiInfo;				/* Information about the output console */



void ARWin32Demo_SetConsoleCursor(int x,int y)
{
	if (hStdout==NULL) hStdout=GetStdHandle(STD_OUTPUT_HANDLE);

	if (hStdout != INVALID_HANDLE_VALUE){
			GetConsoleScreenBufferInfo(hStdout, &csbiInfo);
			csbiInfo.dwCursorPosition.X=x;
			csbiInfo.dwCursorPosition.Y=y;
			SetConsoleCursorPosition(hStdout,csbiInfo.dwCursorPosition);
	}
}

void ARWin32Demo_AcquireConsole(int x,int y)
{
	vp_os_mutex_lock(&consoleMutex);
}
void ARWin32Demo_ReleaseConsole(int x,int y)
{
	vp_os_mutex_unlock(&consoleMutex);
}