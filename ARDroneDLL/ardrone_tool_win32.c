/********************************************************************
 *                    COPYRIGHT PARROT 2010
 ********************************************************************
 *       PARROT - A.R.Drone SDK Windows Client Example
 *-----------------------------------------------------------------*/
/**
 * @file ardrone_tool_win32.c 
 * @brief Main program of all client programs using ARDroneTool.
 *
 * @author Stephane Piskorski <stephane.piskorski.ext@parrot.fr>
 * @date   Sept, 8. 2010
 * @warning  This file is a modified version of the 'ardrone_tool.c'
 *			 of the A.R. Drone SDK
 *
 *******************************************************************/


#pragma warning( disable : 4996 ) // disable deprecation warning 

#include <stdlib.h>

#include <VP_Os/vp_os_malloc.h>
#include <VP_Os/vp_os_print.h>
#include <VP_Api/vp_api_thread_helper.h>

#include <VP_Com/vp_com.h>

#include <ardrone_tool/ardrone_tool.h>
#include <ardrone_tool/ardrone_time.h>
#include <ardrone_tool/Control/ardrone_control.h>
#include <ardrone_tool/Control/ardrone_control_ack.h>
#include <ardrone_tool/Navdata/ardrone_navdata_client.h>
#include <ardrone_tool/UI/ardrone_input.h>
#include <ardrone_tool/Com/config_com.h>


int32_t MiscVar[NB_MISC_VARS] = { 
               DEFAULT_MISC1_VALUE, 
               DEFAULT_MISC2_VALUE,
               DEFAULT_MISC3_VALUE, 
               DEFAULT_MISC4_VALUE
                                };

static bool_t need_update   = TRUE;
static ardrone_timer_t ardrone_tool_timer;
static int ArdroneToolRefreshTimeInMs = ARDRONE_REFRESH_MS;

/* Those should be defined in ArDroneLib, but there are compilation issues to solve */
unsigned long bswap(unsigned long x) { return _byteswap_ulong(x); }
/*
#include <cmnintrin.h>
int clz(unsigned long x) { return _CountLeadingZeros(x); }
*/

int clz(unsigned long x)
{
	/* Barbarian counting method if no instrinsic is available */
	int i; const int L=sizeof(x)*8-1;
	const unsigned long mask = ( 1 << L );
	if (x==0) { return L+1; }
	for (i=0;i<L;i++) { if (x&mask) return i; x<<=1; } 
	return i;
}


char wifi_ardrone_ip[256] = { WIFI_ARDRONE_IP };


/*---------------------------------------------------------------------------------------------------------------------
Array containing parameters to be sent to the drone before starting flight.
Those parameters are sent one by one with the use of an acknowledgment to make sure they
were properly set on the drone.
---------------------------------------------------------------------------------------------------------------------*/
/// Remote data configuration ///
ardrone_tool_configure_data_t configure_data[] = {
  { "general:navdata_demo", "FALSE" },
  { NULL, NULL }
};



static int32_t configure_index = 0;
static ardrone_control_ack_event_t ack_config;
static bool_t send_com_watchdog = FALSE;



/*---------------------------------------------------------------------------------------------------------------------
Makes ardrone tool send a keepalive command to the drone, when the drone detects inactivity for too long.
---------------------------------------------------------------------------------------------------------------------*/
void ardrone_tool_send_com_watchdog( void )
{
  send_com_watchdog = TRUE;
}


/*---------------------------------------------------------------------------------------------------------------------
Callback function used by 'ardrone_tool_configure' and by itself.
This function uses the drone control socket and its acknowlegment system to securely
set some parameters on the drone.
The parameters to set are stored in the 'configure_data' array.
---------------------------------------------------------------------------------------------------------------------*/

static void ardrone_tool_end_configure( struct _ardrone_control_event_t* event )
{
  if( event->status == ARDRONE_CONTROL_EVENT_FINISH_SUCCESS )
    configure_index ++;

  if( configure_data[configure_index].var != NULL && configure_data[configure_index].value != NULL )
  {
    ack_config.event                        = ACK_CONTROL_MODE;
    ack_config.num_retries                  = 20;
    ack_config.status                       = ARDRONE_CONTROL_EVENT_WAITING;
    ack_config.ardrone_control_event_start  = NULL;
    ack_config.ardrone_control_event_end    = ardrone_tool_end_configure;
    ack_config.ack_state                    = ACK_COMMAND_MASK_TRUE;

    ardrone_at_set_toy_configuration( configure_data[configure_index].var, configure_data[configure_index].value );
    ardrone_at_send();

    ardrone_control_send_event( (ardrone_control_event_t*)&ack_config );
  }
}


/*---------------------------------------------------------------------------------------------------------------------
See 'ardrone_tool_end_configure'
---------------------------------------------------------------------------------------------------------------------*/

static C_RESULT ardrone_tool_configure()
{
  if( configure_data[configure_index].var != NULL && configure_data[configure_index].value != NULL )
  {
    ack_config.event                        = ACK_CONTROL_MODE;
    ack_config.num_retries                  = 20;
    ack_config.status                       = ARDRONE_CONTROL_EVENT_WAITING;
    ack_config.ardrone_control_event_start  = NULL;
    ack_config.ardrone_control_event_end    = ardrone_tool_end_configure;
    ack_config.ack_state                    = ACK_COMMAND_MASK_TRUE;

    ardrone_at_set_toy_configuration( configure_data[configure_index].var, configure_data[configure_index].value );
    ardrone_at_send();

    ardrone_control_send_event( (ardrone_control_event_t*)&ack_config );
  }

  return C_OK;
}


/*---------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------*/
static void ardrone_toy_network_adapter_cb( const char* name )
{
  strcpy( COM_CONFIG_NAVDATA()->itfName, name );
}


/*---------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------*/
C_RESULT ardrone_tool_setup_com( const char* ssid )
{
  C_RESULT res = C_OK;

  vp_com_init(COM_NAVDATA());
  vp_com_local_config(COM_NAVDATA(), COM_CONFIG_NAVDATA());
  vp_com_connect(COM_NAVDATA(), COM_CONNECTION_NAVDATA(), NUM_ATTEMPTS);
  ((vp_com_wifi_connection_t*)wifi_connection())->is_up=1;
  return res;
}


/*---------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------*/
C_RESULT ardrone_tool_init(int argc, char **argv)
{
	C_RESULT res;

	//Fill structure AT codec and built the library AT commands.
	ardrone_at_init( wifi_ardrone_ip, strlen( wifi_ardrone_ip) );

	// Init subsystems
	ardrone_timer_reset(&ardrone_tool_timer);

	ardrone_tool_input_init();
	ardrone_control_init();
	ardrone_navdata_client_init();

	// Init custom tool
	res = ardrone_tool_init_custom(argc, argv);

   //Opens a connection to AT port.
	ardrone_at_open();


	START_THREAD(navdata_update, 0);
	START_THREAD(ardrone_control, 0);

	ardrone_tool_configure();

	// Send start up configuration
	ardrone_at_set_pmode( MiscVar[0] );
	ardrone_at_set_ui_misc( MiscVar[0], MiscVar[1], MiscVar[2], MiscVar[3] );
	
	return res;
}


C_RESULT ardrone_tool_set_refresh_time(int refresh_time_in_ms)
{
  ArdroneToolRefreshTimeInMs = refresh_time_in_ms;

  return C_OK;
}

C_RESULT ardrone_tool_pause( void )
{
   ardrone_navdata_client_suspend();

   return C_OK;
}

C_RESULT ardrone_tool_resume( void )
{
   ardrone_navdata_client_resume();

   return C_OK;
}



/*---------------------------------------------------------------------------------------------------------------------
Updates the AT command client by flushing pending commands.
---------------------------------------------------------------------------------------------------------------------*/
C_RESULT ardrone_tool_update()
{
	int delta;

	C_RESULT res = C_OK;

	// Update subsystems & custom tool
	if( need_update )
	{
	
		ardrone_timer_update(&ardrone_tool_timer);

		ardrone_tool_input_update();
		res = ardrone_tool_update_custom();

		if( send_com_watchdog == TRUE )
		{
			ardrone_at_reset_com_watchdog();
			send_com_watchdog = FALSE;
		}
		// Send all pushed messages
		ardrone_at_send();

		need_update = FALSE;
	}

	delta = ardrone_timer_delta_ms(&ardrone_tool_timer);
	if( delta >= ArdroneToolRefreshTimeInMs)
	{
		// Render frame
		res = ardrone_tool_display_custom();
		need_update = TRUE;
	}
	else
	{
		Sleep((ArdroneToolRefreshTimeInMs - delta));
	}

	return res;
}




/*---------------------------------------------------------------------------------------------------------------------
Stops the drone controlling subsystem.
---------------------------------------------------------------------------------------------------------------------*/
C_RESULT ardrone_tool_shutdown()
{
  C_RESULT res = C_OK;
  
#ifndef NO_ARDRONE_MAINLOOP
  res = ardrone_tool_shutdown_custom();
#endif

  // Shutdown subsystems
  ardrone_navdata_client_shutdown();
  ardrone_control_shutdown();
  ardrone_tool_input_shutdown();
 
  JOIN_THREAD(ardrone_control); 
  JOIN_THREAD(navdata_update);

  // Shutdown AT Commands
  ATcodec_exit_thread();
  ATcodec_Shutdown_Library();

  vp_com_disconnect(COM_NAVDATA());
  vp_com_shutdown(COM_NAVDATA());

  PRINT("Custom ardrone tool ended\n");

  return res;
}



/*---------------------------------------------------------------------------------------------------------------------
Tests the network connection to the drone by fetching the drone version number
through the FTP server embedded on the drone.
This is how FreeFlight checks if a drone sofware update is required.

The FTP connection process is a quick and (very)dirty one. It uses FTP passive mode.
---------------------------------------------------------------------------------------------------------------------*/
int test_drone_connection()
{
	const char * passivdeModeHeader = "\r\n227 PASV ok (";
	vp_com_socket_t ftp_client,ftp_client2;
	char buffer[1024];
	static Write ftp_write = NULL;
	static Read  ftp_read = NULL;
	int bytes_to_send,received_bytes;
	int i,L,x[6],port;
	
	vp_os_memset(buffer,0,sizeof(buffer));

	/* Connects to the FTP server */
		wifi_config_socket(&ftp_client,VP_COM_CLIENT,FTP_PORT,WIFI_ARDRONE_IP);
		ftp_client.protocol = VP_COM_TCP;
		if(VP_FAILED(vp_com_init(wifi_com()))) return -1;
		if(VP_FAILED(vp_com_open(wifi_com(), &ftp_client, &ftp_read, &ftp_write))) return -2;

	/* Request version file */
		bytes_to_send = _snprintf(buffer,sizeof(buffer),"%s",
			"USER anonymous\r\nCWD /\r\nPWD\r\nTYPE A\r\nPASV\r\nRETR version.txt\r\n");
		ftp_write(&ftp_client, (const int8_t*)buffer,&bytes_to_send);
		/* Dirty. We should wait for data to arrive with some kind of synchronization
		or make the socket blocking.*/
		Sleep(1000);

	/* Gets the data port */
		received_bytes = sizeof(buffer);
		ftp_read(&ftp_client,(int8_t*)buffer,&received_bytes);
		if (received_bytes<1) { vp_com_close(wifi_com(), &ftp_client); return -3; }
		L=received_bytes-strlen(passivdeModeHeader);

	/* Searches for the passive mode acknowlegment from the FTP server */
		for (i=0;i<L;i++) {
			if (strncmp((buffer+i),passivdeModeHeader,strlen(passivdeModeHeader))==0)  break; 
		}
		if (i==L) {
			vp_com_close(wifi_com(), &ftp_client); return -4; 
		}
		i+=strlen(passivdeModeHeader);
		if (sscanf(buffer+i,"%i,%i,%i,%i,%i,%i)",&x[0],&x[1],&x[2],&x[3],&x[4],&x[5])!=6)
			{ vp_com_close(wifi_com(), &ftp_client); return -5; }
		port=(x[4]<<8)+x[5];

	/* Connects to the FTP server data port */
		wifi_config_socket(&ftp_client2,VP_COM_CLIENT,port,"192.168.1.1");
		ftp_client2.protocol = VP_COM_TCP;
		if(VP_FAILED(vp_com_init(wifi_com()))) 
				{ vp_com_close(wifi_com(), &ftp_client2); return -6; }
		if(VP_FAILED(vp_com_open(wifi_com(), &ftp_client2, &ftp_read, &ftp_write)))
			{ vp_com_close(wifi_com(), &ftp_client2); return -7; }

	/* Clean up */
		vp_com_close(wifi_com(), &ftp_client);
		vp_com_close(wifi_com(), &ftp_client2);

	return 0;
}




/*---------------------------------------------------------------------------------------------------------------------
Main application function
---------------------------------------------------------------------------------------------------------------------*/

int sdk_demo_stop=0;

//int main(int argc, char **argv)
//{
//	  C_RESULT res;					// functions return value
//	  //const char* old_locale=NULL;
//	  const char* appname = argv[0];
//	  //int argc_backup = argc;
//	  //char** argv_backup = argv;
//
//	  WSADATA wsaData = {0};
//	  int iResult = 0;
//
//
//	/* Initializes Windows socket subsystem */
//		iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
//		if (iResult != 0) {	wprintf(L"WSAStartup failed: %d\n", iResult);	return 1;	}
//			
//	/* Includes the Pthread for Win32 Library if necessary */
//		#include <VP_Os/vp_os_signal.h>
//			#if defined USE_PTHREAD_FOR_WIN32
//			#pragma comment (lib,"pthreadVC2.lib")
//			#endif
// 
//	/* 	Initializes communication sockets	*/	
//		res = test_drone_connection();
//		if(res!=0){
//			printf("%s","Could not detect the drone ... press <Enter> to quit the application.\n");
//			getchar();
//			WSACleanup(); exit(-1);
//		}
//
//		res = ardrone_tool_setup_com( NULL );
//		if( FAILED(res) ){  PRINT("Wifi initialization failed.\n");  return -1;	}
//
//	/* Initialises ARDroneTool */
//	   res = ardrone_tool_init(vidWnd, argc, argv);
//
//   /* Keeps sending AT commands to control the drone as long as 
//		everything is OK */
//      while( VP_SUCCEEDED(res) && ardrone_tool_exit() == FALSE ) {
//        res = ardrone_tool_update();     }
//
//   /**/
//      res = ardrone_tool_shutdown();
//    
//	  WSACleanup();
//
//  /* Bye bye */
//	system("cls");
//	  printf("End of SDK Demo for Windows\n");
//	  getchar();
//	  return VP_SUCCEEDED(res) ? 0 : -1;
//}


	// Default implementation for weak functions
	C_RESULT ardrone_tool_update_custom() { return C_OK; }
	C_RESULT ardrone_tool_display_custom() { return C_OK; }
	C_RESULT ardrone_tool_check_argc_custom( int32_t argc) { return C_OK; }
	void ardrone_tool_display_cmd_line_custom( void ) {}
	bool_t ardrone_tool_parse_cmd_line_custom( const char* cmd ) { return TRUE; }

	// -- DLL EXPORTS BEGIN HERE ------------------------------------------------------------------------------------

	// Flight data - TODO - move to struct?
	char* DroneState;
	int BatteryLevel;
	double Theta, Phi, Psi;
	int Altitude;
	double vX, vY, vZ;

	float roll = 0, pitch = 0, gaz = 0, yaw = 0;
	int hovering = 0;
	int start = 0;

	int _stdcall InitDrone() // TODO : Should SSID be passed in here?
	{
		
	  C_RESULT res;					// functions return value
	  const char* appname = "droneapp";
	  WSADATA wsaData = {0};
	  int iResult = 0;


	/* Initializes Windows socket subsystem */
		iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
		
		if (iResult != 0) 
		{	
			wprintf(L"WSAStartup failed: %d\n", iResult);	
			return -3;	
		}
			
	/* Includes the Pthread for Win32 Library if necessary */
		#include <VP_Os/vp_os_signal.h>
			#if defined USE_PTHREAD_FOR_WIN32
			#pragma comment (lib,"pthreadVC2.lib")
			#endif

		
		res = test_drone_connection();
		
		if( res !=0 )
		{
			// Failed;
			WSACleanup(); 
			return -1;
		}

		res = ardrone_tool_setup_com(NULL); // Can pass in the SSID here
		
		if(FAILED(res))
		{  
			// Wifi init has failed
			return -2;	
		}

		/* Initialises ARDroneTool */
		res = ardrone_tool_init(0, NULL);
		if(FAILED(res))
		{
			return -4;
		}

		// Success
		return 0;

		//-----------------------------------
		//while (VP_SUCCEEDED(res) && ardrone_tool_exit() == FALSE)
		//	res = ardrone_tool_update(); 

	  //res = ardrone_tool_shutdown();
    
	  //WSACleanup();

      /* Bye bye */
	  return VP_SUCCEEDED(res) ? 0 : -1;
	}
	

	
	//added by miguelb:
	int _stdcall SetLedAnimation ( int iAnimation , float32_t fFrequency , uint32_t iDuration )
	{
		
		LED_ANIMATION_IDS iAnimationID;

		//miguelb: There are more elegant ways to do this, but for now this should be enough
		switch ( iAnimation )

		{
		case 1:
			iAnimationID = BLINK_GREEN_RED ;
			break;
		case 2:
            iAnimationID = BLINK_GREEN ;
			break;    
		case 3:
			iAnimationID = BLINK_RED ;
			break;       
		case 4:
			iAnimationID = BLINK_ORANGE;
			break;    
		case 5:
			iAnimationID = SNAKE_GREEN_RED;
			break;
		case 6:
			iAnimationID = FIRE;
			break;   
		case 7:
			iAnimationID = STANDARD;
			break;
		case 8:
			iAnimationID = RED;
			break;     
		case 9:
			iAnimationID = GREEN;
			break;   
		case 10:
			iAnimationID = RED_SNAKE;
			break;       
		case 11:
			iAnimationID = BLANK;
			break;  
		case 12:
			iAnimationID = RIGHT_MISSILE;
			break;   
		case 13:
			iAnimationID = LEFT_MISSILE;
			break;  
		case 14:
			iAnimationID = DOUBLE_MISSILE;
			break;
		default:
			return 0;
		}
		ardrone_at_set_led_animation ( iAnimationID , fFrequency , iDuration);
		return 1;
	}


	BOOL _stdcall UpdateDrone()
	{
	   /* Keeps sending AT commands to control the drone as long as 
		everything is OK */
        C_RESULT res = ardrone_tool_update(); 
		printf("Sent update\n");
		return  (VP_SUCCEEDED(res) && ardrone_tool_exit() == FALSE); 
	}

	int _stdcall ChangeToFrontCamera() {
		ardrone_at_zap(ZAP_CHANNEL_HORI);
		return 0;
	}

	int _stdcall ChangeToBottomCamera() {
		ardrone_at_zap(ZAP_CHANNEL_VERT);
		return 0;
	}


	int _stdcall SendFlatTrim()
	{
		ardrone_at_set_flat_trim();
		printf("Sent flat trim\n");
		return 0;

	}

	int _stdcall SendEmergency()
	{
		ardrone_tool_set_ui_pad_start(0); 
		ardrone_tool_set_ui_pad_select(1);  
		need_update = TRUE;
		printf("Sent Emergency\n");
		return 0;
	}


	int _stdcall SendTakeoff()
	{
		ardrone_tool_set_ui_pad_start(1);
		need_update = TRUE;

		printf("Sent takeoff\n");
		return 0;
	}
	
	int _stdcall SendLand()
	{
		ardrone_tool_set_ui_pad_start(0);  
		need_update = TRUE;
		printf("Sent land\n");
		return 0;
	}

	int _stdcall SetProgressCmd(BOOL bhovering, float roll, float pitch, float gaz, float yaw)
	{
		ardrone_at_set_progress_cmd((hovering)? 0:1, roll, pitch, gaz, yaw);
		printf("Sent Progress Cmd\n");
		return 0;
	}

	BOOL _stdcall ShutdownDrone()
	{
      C_RESULT res = ardrone_tool_shutdown();
    
	  WSACleanup();



		//start^=1; 
		//ardrone_tool_set_ui_pad_start(start);  
		//printf("Sending start %i.%s\n",start,linefiller); 

     	// * @param enable 1,with pitch,roll and 0,without pitch,roll.
		// * @param pitch Using floating value between -1 to +1. 
		// * @param roll Using floating value between -1 to +1.
		// * @param gaz Using floating value between -1 to +1.
		// * @param yaw Using floating value between -1 to +1.

		//ardrone_at_set_progress_cmd((hovering)? 0:1, roll, pitch, gaz, yaw);
	  
	  
	  return VP_SUCCEEDED(res) ? 0 : -1;
	}

	char* _stdcall GetDroneState() { return "CTRL_LANDED";}
	int _stdcall GetBatteryLevel() { return BatteryLevel;}

	double _stdcall GetTheta() { return Theta;}
	double _stdcall GetPhi() { return Phi;}
	double _stdcall GetPsi() { return Psi;}

	int _stdcall GetAltitude() { return Altitude;}

	double _stdcall GetVX() { return vX;}
	double _stdcall GetVY() { return vY;}
	double _stdcall GetVZ() { return vZ;}