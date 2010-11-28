#include <VP_Os/vp_os_malloc.h>
#include <VP_Os/vp_os_print.h>
#include <VP_Api/vp_api_thread_helper.h>

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
static vp_os_mutex_t ardrone_tool_mutex;
static bool_t ardrone_tool_in_pause = FALSE;
char wifi_ardrone_ip[256] = { WIFI_ARDRONE_IP };

int usleep(unsigned int usec);

/// Remote data configuration ///
ardrone_tool_configure_data_t configure_data[] = {
  { "general:navdata_demo", "FALSE" },
  { NULL, NULL }
};

static int32_t configure_index = 0;
static ardrone_control_ack_event_t ack_config;
static bool_t send_com_watchdog = FALSE;

void ardrone_tool_send_com_watchdog( void )
{
  send_com_watchdog = TRUE;
}

static void ardrone_tool_usage( const char* appname )
{
  printf("%s based on ARDrone Tool\n", appname);
  printf("Be aware to not insert space in your options\n");
  printf("\t-ssid=wifinetworkname : tells which wifi network we want to join\n");
  printf("\t-mobile_ip=ip : configures local ip\n");
  printf("\t-ardrone_ip=ip : configures ardrone ip\n");
  printf("\t-bcast_ip=ip : configures broadcast ip\n");

  ardrone_tool_display_cmd_line_custom();
}

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

static void ardrone_toy_network_adapter_cb( const char* name )
{
  strcpy( COM_CONFIG_NAVDATA()->itfName, name );
}

C_RESULT ardrone_tool_setup_com( const char* ssid )
{
  C_RESULT res = C_OK;

#ifdef CHECK_WIFI_CONFIG
  if( FAILED(vp_com_init(COM_NAVDATA())) )
  {
	  DEBUG_PRINT_SDK("VP_Com : Failed to init com for navdata\n");
	  vp_com_shutdown(COM_NAVDATA());
	  res = C_FAIL;
  }

  vp_com_network_adapter_lookup(COM_NAVDATA(), ardrone_toy_network_adapter_cb);

  if( SUCCEED(res) && FAILED(vp_com_local_config(COM_NAVDATA(), COM_CONFIG_NAVDATA())) )
  {
	  DEBUG_PRINT_SDK("VP_Com : Failed to configure com for navdata\n");
	  vp_com_shutdown(COM_NAVDATA());
	  res = C_FAIL;
  }

  if( ssid != NULL )
  {
	  strcpy( ((vp_com_wifi_connection_t*)wifi_connection())->networkName, ssid );
  }

  if( SUCCEED(res) && FAILED(vp_com_connect(COM_NAVDATA(), COM_CONNECTION_NAVDATA(), NUM_ATTEMPTS)))
  {
	  DEBUG_PRINT_SDK("VP_Com: Failed to connect for navdata\n");
	  vp_com_shutdown(COM_NAVDATA());
	  res = C_FAIL;
  }
#else  
  vp_com_init(COM_NAVDATA());
  vp_com_network_adapter_lookup(COM_NAVDATA(), ardrone_toy_network_adapter_cb);
  vp_com_local_config(COM_NAVDATA(), COM_CONFIG_NAVDATA());

  if( ssid != NULL )
  {
	  strcpy( ((vp_com_wifi_connection_t*)wifi_connection())->networkName, ssid );
  }

  vp_com_connect(COM_NAVDATA(), COM_CONNECTION_NAVDATA(), NUM_ATTEMPTS);
  ((vp_com_wifi_connection_t*)wifi_connection())->is_up=1;
#endif

  return res;
}

#ifdef NO_ARDRONE_MAINLOOP
C_RESULT ardrone_tool_init( const char* ardrone_ip, size_t n, AT_CODEC_FUNCTIONS_PTRS *ptrs)
{	
	// Initalize mutex and condition
	vp_os_mutex_init(&ardrone_tool_mutex);
	ardrone_tool_in_pause = FALSE;
	
	//Fill structure AT codec and built the library AT commands.
   if( ptrs != NULL )
	   ardrone_at_init_with_funcs( ardrone_ip, n, ptrs );
   else	
      ardrone_at_init( ardrone_ip, n );

	// Init subsystems
	ardrone_timer_reset(&ardrone_tool_timer);
	
	ardrone_tool_input_init();
	ardrone_control_init();
	ardrone_navdata_client_init();

   //Opens a connection to AT port.
	ardrone_at_open();

	START_THREAD(navdata_update, 0);
	START_THREAD(ardrone_control, 0);

	// Send start up configuration
	ardrone_at_set_pmode( MiscVar[0] );
	ardrone_at_set_ui_misc( MiscVar[0], MiscVar[1], MiscVar[2], MiscVar[3] );

	return C_OK;
}
#else
C_RESULT ardrone_tool_init(int argc, char **argv)
{
	C_RESULT res;

	// Initalize mutex and condition
	vp_os_mutex_init(&ardrone_tool_mutex);
	ardrone_tool_in_pause = FALSE;
	
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
#endif

C_RESULT ardrone_tool_set_refresh_time(int refresh_time_in_ms)
{
  ArdroneToolRefreshTimeInMs = refresh_time_in_ms;

  return C_OK;
}

C_RESULT ardrone_tool_pause( void )
{
	ardrone_navdata_client_suspend();

	vp_os_mutex_lock(&ardrone_tool_mutex);
	ardrone_tool_in_pause = TRUE;
	vp_os_mutex_unlock(&ardrone_tool_mutex);	
	
	return C_OK;
}

C_RESULT ardrone_tool_resume( void )
{
   ardrone_navdata_client_resume();

	vp_os_mutex_lock(&ardrone_tool_mutex);
	ardrone_tool_in_pause = FALSE;
	vp_os_mutex_unlock(&ardrone_tool_mutex);	
	
   return C_OK;
}

C_RESULT ardrone_tool_update()
{
	int delta;

	C_RESULT res = C_OK;

	// Update subsystems & custom tool
	if( need_update )
	{
		ardrone_timer_update(&ardrone_tool_timer);

		if(!ardrone_tool_in_pause)
		{
			ardrone_tool_input_update();
			res = ardrone_tool_update_custom();
		}
		
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
		usleep(1000 * (ArdroneToolRefreshTimeInMs - delta));
	}

	return res;
}

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
#ifndef NO_ARDRONE_MAINLOOP

#include <locale.h>

int main(int argc, char **argv)
{
  C_RESULT res;
  const char* old_locale;
  const char* appname = argv[0];
  int argc_backup = argc;
  char** argv_backup = argv;

  bool_t show_usage = FAILED( ardrone_tool_check_argc_custom(argc) ) ? TRUE : FALSE;

  argc--; argv++;
  while( argc && *argv[0] == '-' )
  {
    if( !strcmp(*argv, "-?") || !strcmp(*argv, "-h") || !strcmp(*argv, "-help") || !strcmp(*argv, "--help") )
    {
      ardrone_tool_usage( appname );
      exit( 0 );
    }
    else if( !strncmp(*argv, "-ssid=", strlen("-ssid=")) || !strncmp(*argv, "--ssid=", strlen("--ssid=")) )
    {
      strcpy( ((vp_com_wifi_connection_t*)wifi_connection())->networkName, strchr( *argv, '=' )+1 );
    }
    else if( !strncmp(*argv, "-mobile_ip=", strlen("-mobile_ip=")) || !strncmp(*argv, "--mobile_ip=", strlen("--mobile_ip=")) )
    {
      strcpy( ((vp_com_wifi_config_t*)wifi_config())->localHost, strchr( *argv, '=' )+1 );
    }
    else if( !strncmp(*argv, "-ardrone_ip=", strlen("-ardrone_ip=")) || !strncmp(*argv, "--ardrone_ip=", strlen("--ardrone_ip=")) )
    {
      const char* ardrone_ip = strchr( *argv, '=' )+1;

      vp_os_memset( &wifi_ardrone_ip[0], 0, sizeof(wifi_ardrone_ip) );

      strcpy( &wifi_ardrone_ip[0], ardrone_ip );
      strcpy( ((vp_com_wifi_config_t*)wifi_config())->gateway, ardrone_ip );
      strcpy( ((vp_com_wifi_config_t*)wifi_config())->server, ardrone_ip );
    }
    else if( !strncmp(*argv, "-bcast_ip=", strlen("-bcast_ip=")) || !strncmp(*argv, "--bcast_ip=", strlen("--bcast_ip=")) )
    {
      strcpy( ((vp_com_wifi_config_t*)wifi_config())->broadcast, strchr( *argv, '=' )+1 );
    }
    else if( !ardrone_tool_parse_cmd_line_custom( *argv ) )
    {
      printf("Option %s not recognized\n", *argv);
      show_usage = TRUE;
    }

    argc--; argv++;
  }

  if( show_usage || (argc != 0) )
  {
    ardrone_tool_usage( appname );
    exit(-1);
  }
  
  /* After a first analysis, the arguments are restored so they can be passed to the user-defined functions */
  argc=argc_backup;
  argv=argv_backup;
  
  old_locale = setlocale(LC_NUMERIC, "en_GB.UTF-8");

  if( old_locale == NULL )
  {
    PRINT("You have to install new locales in your dev environment! (avoid the need of conv_coma_to_dot)\n");
    PRINT("As root, do a \"dpkg-reconfigure locales\" and add en_GB.UTF8 to your locale settings\n");
    PRINT("If you have any problem, feel free to contact Pierre Eline (pierre.eline@parrot.com)\n");
  }
  else
  {
    PRINT("Setting locale to %s\n", old_locale);
  }

  if( &custom_main )
  {
    return custom_main(argc, argv);
  }
  else
  {
    res = ardrone_tool_setup_com( NULL );

    if( FAILED(res) )
    {
      PRINT("Wifi initialization failed. It means either:\n");
      PRINT("\t* you're not root (it's mandatory because you can set up wifi connection only as root)\n");
      PRINT("\t* wifi device is not present (on your pc or on your card)\n");
      PRINT("\t* you set the wrong name for wifi interface (for example rausb0 instead of wlan0) \n");
      PRINT("\t* ap is not up (reboot card or remove wifi usb dongle)\n");
      PRINT("\t* wifi device has no antenna\n");
    }
    else
    {
      res = ardrone_tool_init(argc, argv);

      while( SUCCEED(res) && ardrone_tool_exit() == FALSE )
      {
        res = ardrone_tool_update();
      }

      res = ardrone_tool_shutdown();
    }
  }

  if( old_locale != NULL )
  {
    setlocale(LC_NUMERIC, old_locale);
  }

  return SUCCEED(res) ? 0 : -1;
}
#endif // ! WITH_ARDRONE_MAIN_LOOP

// Default implementation for weak functions
#ifndef _WIN32
	C_RESULT ardrone_tool_init_custom(int argc, char **argv) { return C_OK; }
	C_RESULT ardrone_tool_update_custom() { return C_OK; }
	C_RESULT ardrone_tool_display_custom() { return C_OK; }
	C_RESULT ardrone_tool_shutdown_custom() { return C_OK; }
	bool_t   ardrone_tool_exit() { return FALSE; }
	C_RESULT ardrone_tool_check_argc_custom( int32_t argc) { return C_OK; }
	void ardrone_tool_display_cmd_line_custom( void ) {}
	bool_t ardrone_tool_parse_cmd_line_custom( const char* cmd ) { return TRUE; }
#endif


