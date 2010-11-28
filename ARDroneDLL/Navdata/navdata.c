/********************************************************************
 *                    COPYRIGHT PARROT 2010
 ********************************************************************
 *       PARROT - A.R.Drone SDK Windows Client Example
 *-----------------------------------------------------------------*/
/**
 * @file navdata.c 
 * @brief Navdata handling code
 *
 * @author sylvain.gaeremynck@parrot.com
 * @date 2009/07/01
 *
 * @author Stephane Piskorski <stephane.piskorski.ext@parrot.fr>
 * @date   Sept, 8. 2010
 *
 *******************************************************************/


/* Includes required to handle navigation data */
	#include <ardrone_tool/Navdata/ardrone_navdata_client.h>
	#include <Navdata/navdata.h>

#include <custom_code.h>

/*---------------------------------------------------------------------------------------------------------------------
Function taking the drone control state stored as an integer, and prints in a string 
the names of the set control bits.

The drone control state is an integer sent in the navdata which says if the drone is landed, flying,
hovering, taking-off, crashed, etc ...
---------------------------------------------------------------------------------------------------------------------*/

	#define CTRL_STATES_STRING
	#include "control_states.h"

const char* ctrl_state_str(uint32_t ctrl_state)
{
  #define MAX_STR_CTRL_STATE 256
  static char str_ctrl_state[MAX_STR_CTRL_STATE];

  ctrl_string_t* ctrl_string;
  uint32_t major = ctrl_state >> 16;
  uint32_t minor = ctrl_state & 0xFFFF;

  if( strlen(ctrl_states[major]) < MAX_STR_CTRL_STATE )
  {
    vp_os_memset(str_ctrl_state, 0, sizeof(str_ctrl_state));

    strcat_s(str_ctrl_state, sizeof(str_ctrl_state),ctrl_states[major]);
    ctrl_string = control_states_link[major];

    if( ctrl_string != NULL && (strlen(ctrl_states[major]) + strlen(ctrl_string[minor]) < MAX_STR_CTRL_STATE) )
    {
      strcat_s( str_ctrl_state,sizeof(str_ctrl_state), " | " );
      strcat_s( str_ctrl_state, sizeof(str_ctrl_state),ctrl_string[minor] );
    }
  }
  else
  {
    vp_os_memset( str_ctrl_state, '#', sizeof(str_ctrl_state) );
  }

  return str_ctrl_state;
}



/*---------------------------------------------------------------------------------------------------------------------
Initialization local variables before event loop  
---------------------------------------------------------------------------------------------------------------------*/
inline C_RESULT demo_navdata_client_init( void* data )
{
	/**	======= INSERT USER CODE HERE ========== **/
	/* Initialize your navdata handler */
	/**	======= INSERT USER CODE HERE ========== **/

  return C_OK;
}





/*---------------------------------------------------------------------------------------------------------------------
Navdata handling function, which is called every time navdata are received
---------------------------------------------------------------------------------------------------------------------*/
inline C_RESULT demo_navdata_client_process( const navdata_unpacked_t* const navdata )
{
	static int cpt=0;

    const navdata_demo_t* const nd = &navdata->navdata_demo;


		/**	======= INSERT USER CODE HERE ========== **/
		// Update the global nav variables
		// TODO - implement locking
		extern int DroneState;
		extern int BatteryLevel;
		extern double Theta, Phi, Psi;
		extern int Altitude;
		extern double vX, vY, vZ;
		extern double Pitch, Roll, Yaw, Gaz;

				//ARWin32Demo_AcquireConsole();
				//if ((cpt++)==90) { system("cls"); cpt=0; }
				//
				//ARWin32Demo_SetConsoleCursor(0,0);  // Print at the top of the console
				//printf("=================================\n");
				//printf("Navdata for flight demonstrations\n");
				//printf("=================================\n");

				//printf("Control state : %s                                      \n",ctrl_state_str(nd->ctrl_state));
				//printf("Battery level : %i/100          \n",nd->vbat_flying_percentage);
				//printf("Orientation   : [Theta] %4.3f  [Phi] %4.3f  [Psi] %4.3f          \n",nd->theta,nd->phi,nd->psi);
				//printf("Altitude      : %i          \n",nd->altitude);
				//printf("Speed         : [vX] %4.3f  [vY] %4.3f  [vZ] %4.3f          \n",nd->vx,nd->vy,nd->vz);
				//ARWin32Demo_ReleaseConsole();

				DroneState = nd->ctrl_state;
				Altitude = nd->altitude;
				BatteryLevel = nd->vbat_flying_percentage;
				Theta = nd->theta;
				Phi = nd->phi;
				Psi = nd->psi;
				vX = nd->vx;
				vY = nd->vy;
				vZ = nd->vz;
				
		/** ======= INSERT USER CODE HERE ========== **/

		return C_OK;
}






/*---------------------------------------------------------------------------------------------------------------------
 Relinquish the local resources after the event loop exit 
---------------------------------------------------------------------------------------------------------------------*/
inline C_RESULT demo_navdata_client_release( void )
{
	/**	======= INSERT USER CODE HERE ========== **/
	/* Clean up your navdata handler */
	/**	======= INSERT USER CODE HERE ========== **/
  return C_OK;
}





/* 
Registering the navdata handling function to 'navdata client' which is part 
of the ARDroneTool.
You can add as many navdata handlers as you want.
Terminate the table with a NULL pointer.
*/
BEGIN_NAVDATA_HANDLER_TABLE
  NAVDATA_HANDLER_TABLE_ENTRY(demo_navdata_client_init, demo_navdata_client_process, demo_navdata_client_release, NULL)
END_NAVDATA_HANDLER_TABLE

