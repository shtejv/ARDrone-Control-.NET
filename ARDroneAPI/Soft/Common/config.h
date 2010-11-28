/**
***************************************************************************
*
* Copyright (C) 2007 Parrot S.A.
*
***************************************************************************
*/

#ifndef _CONFIG_H_
#define _CONFIG_H_

#include <VP_Os/vp_os_types.h>
#ifdef _WIN32
	#include <win32_custom.h>
#else
	#include <generated_custom.h>
	#include <autoconf.h>
#endif

#undef ARDRONE_PIC_VERSION

#define USE_NAVDATA_IP
#define USE_AT_IP
#define USE_VIDEO_IP

///////////////////////////////////////////////
// Video configuration
#define VIDEO_ENABLE            1

///////////////////////////////////////////////
// Vision configuration

#define VISION_ENABLE           1
// #define VISION_TEST_MODE
#define ARDRONE_VISION_DETECT

///////////////////////////////////////////////
// Navdata configuration

#define NAVDATA_ENABLE          1
#define ND_WRITE_TO_FILE

# define NAVDATA_SUBSAMPLING     13 /* 200 / 15 fps = 13.3333 */

#if defined(NAVDATA_ENABLE)

# define NAVDATA_VISION_DETECT_INCLUDED
# define NAVDATA_TRIMS_INCLUDED
# define NAVDATA_WATCHDOG
# define NAVDATA_EULER_ANGLES_INCLUDED
# define NAVDATA_PHYS_MEASURES_INCLUDED
# define NAVDATA_TIME_INCLUDED
# define NAVDATA_RAW_MEASURES_INCLUDED
# define NAVDATA_GYROS_OFFSETS_INCLUDED
# define NAVDATA_REFERENCES_INCLUDED
# define NAVDATA_RC_REFERENCES_INCLUDED
# define NAVDATA_PWM_INCLUDED
# define NAVDATA_ALTITUDE_INCLUDED
# define NAVDATA_VISION_INCLUDED
# define NAVDATA_VISION_PERF_INCLUDED
# define NAVDATA_TRACKERS_SEND

#endif // ! NAVDATA_ENABLE

#ifndef ARDRONE_VISION_DETECT
# undef NAVDATA_VISION_DETECT_INCLUDED
#endif // ! ARDRONE_VISION_DETECT

///////////////////////////////////////////////
// Wifi configuration

#define USE_AUTOIP              VP_COM_AUTOIP_DISABLE /* VP_COM_AUTOIP_ENABLE */

#define WIFI_NETMASK            "255.255.255.0"
#define WIFI_GATEWAY            WIFI_ARDRONE_IP
#define WIFI_SERVER             WIFI_ARDRONE_IP
#define WIFI_SECURE             0

// Configure infrastructure mode given wifi driver compilation
#define WIFI_INFRASTRUCTURE     0

#define WIFI_PASSKEY            "9F1C3EE11CBA230B27BF1C1B6F"

#define FTP_PORT				     5551
#define NAVDATA_PORT            5554
#define VIDEO_PORT              5555
#define AT_PORT                 5556
#define RAW_CAPTURE_PORT        5557
#define PRINTF_PORT             5558
#define CONTROL_PORT            5559

///////////////////////////////////////////////
// Wired configuration

#define WIRED_MOBILE_IP         WIFI_MOBILE_IP

///////////////////////////////////////////////
// Serial link configuration

#ifdef USE_ELINUX

#define SERIAL_LINK_0           "/dev/ttyPA0"
#define SERIAL_LINK_1           "/dev/ttyPA1"
#define SERIAL_LINK_2           "/dev/ttyPA2"

#endif

#ifdef USE_LINUX

#ifdef USE_MINGW32

#define SERIAL_LINK_0           ""
#define SERIAL_LINK_1           ""
#define SERIAL_LINK_2           ""

#else

// Only USE_LINUX is defined
#define SERIAL_LINK_0           "/dev/ttyUSB0" /* Serial link for navdata & ATCmd */
#define SERIAL_LINK_1           "/dev/ttyUSB1" /* Serial link for video */
#define SERIAL_LINK_2           "/dev/ser2" /* Serial link for adc */

#endif // USE_MINGW32

#endif // USE_LINUX

#define SL0_BAUDRATE             VP_COM_BAUDRATE_460800 /* baud rate for serial link 0 */
#define SL1_BAUDRATE             VP_COM_BAUDRATE_460800 /* baud rate for serial link 1 */
#define SL2_BAUDRATE             VP_COM_BAUDRATE_460800 /* baud rate for serial link 2 */

///////////////////////////////////////////////
// Defines & types used in shared data

#define ARDRONE_ORIENTATION_HISTORY_SIZE  256

#define TSECDEC     21 /* Defines used to format time ( seconds << 21 + useconds ) */
#define TUSECMASK   ((1 << TSECDEC) - 1)
#define TSECMASK    (0xffffffff & ~TUSECMASK)
#define TIME_TO_USEC    (0xffffffff & ~TUSECMASK)

#define VBAT_POWERING_OFF   9000    /* Minimum Battery Voltage [mV] to prevent damaging */

/* Syslog Configuration */
#define SYSLOG_NUM_BUFFERS        4     /* Number of actives buffers. When a buffer is full, it's dumped in file */
#define SYSLOG_BUFFER_SIZE        2048  /* Max number of bytes in a syslog buffer */
#define SYSLOG_BUFFER_DUMP_SIZE   128   /* Max number of bytes wrote at once during dump */


#define DEFAULT_MISC1_VALUE 2
#define DEFAULT_MISC2_VALUE 20
#define DEFAULT_MISC3_VALUE 2000
#define DEFAULT_MISC4_VALUE 3000


typedef enum {
  MISC_VAR1 = 0,
  MISC_VAR2,
  MISC_VAR3,
  MISC_VAR4,
  NB_MISC_VARS
} misc_var_t;

// Mayday scenarii
typedef enum {
  ANIM_PHI_M30_DEG= 0,
	ANIM_PHI_30_DEG,	
  ANIM_THETA_M30_DEG,
	ANIM_THETA_30_DEG,
  ANIM_THETA_20DEG_YAW_200DEG,
  ANIM_THETA_20DEG_YAW_M200DEG,
  ANIM_TURNAROUND,
  ANIM_TURNAROUND_GODOWN,
	ANIM_YAW_SHAKE,
  NB_ANIM_MAYDAY
} anim_mayday_t;

// Bitfield definition for user input

typedef enum {
  ARDRONE_UI_BIT_AG             = 0,
  ARDRONE_UI_BIT_AB             = 1,
  ARDRONE_UI_BIT_AD             = 2,
  ARDRONE_UI_BIT_AH             = 3,
  ARDRONE_UI_BIT_L1             = 4,
  ARDRONE_UI_BIT_R1             = 5,
  ARDRONE_UI_BIT_L2             = 6,
  ARDRONE_UI_BIT_R2             = 7,
  ARDRONE_UI_BIT_SELECT         = 8,
  ARDRONE_UI_BIT_START          = 9,
  ARDRONE_UI_BIT_TRIM_THETA     = 18,
  ARDRONE_UI_BIT_TRIM_PHI       = 20,
  ARDRONE_UI_BIT_TRIM_YAW       = 22,
  ARDRONE_UI_BIT_X              = 24,
  ARDRONE_UI_BIT_Y              = 28,
} ardrone_ui_bitfield_t;


/// \enum def_ardrone_state_mask_t is a bit field representing ARDrone' state


// Define masks for ARDrone state
// 31                                                             0
//  x x x x x x x x x x x x x x x x x x x x x x x x x x x x x x x x -> state
//  | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | |
//  | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | FLY MASK : (0) ardrone is landed, (1) ardrone is flying
//  | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | VIDEO MASK : (0) video disable, (1) video enable
//  | | | | | | | | | | | | | | | | | | | | | | | | | | | | | VISION MASK : (0) vision disable, (1) vision enable
//  | | | | | | | | | | | | | | | | | | | | | | | | | | | | CONTROL ALGO : (0) euler angles control, (1) angular speed control
//  | | | | | | | | | | | | | | | | | | | | | | | | | | | ALTITUDE CONTROL ALGO : (0) altitude control inactive (1) altitude control active
//  | | | | | | | | | | | | | | | | | | | | | | | | | | USER feedback : Start button state
//  | | | | | | | | | | | | | | | | | | | | | | | | | Control command ACK : (0) None, (1) one received
//  | | | | | | | | | | | | | | | | | | | | | | | | Trim command ACK : (0) None, (1) one received
//  | | | | | | | | | | | | | | | | | | | | | | | Trim running : (0) none, (1) running
//  | | | | | | | | | | | | | | | | | | | | | | Trim result : (0) failed, (1) succeeded
//  | | | | | | | | | | | | | | | | | | | | | Navdata demo : (0) All navdata, (1) only navdata demo
//  | | | | | | | | | | | | | | | | | | | | Navdata bootstrap : (0) options sent in all or demo mode, (1) no navdata options sent
//  | | | | | | | | | | | | | | | | | | |
//  | | | | | | | | | | | | | | | | | |
//  | | | | | | | | | | | | | | | | | Bit means that there's an hardware problem with gyrometers
//  | | | | | | | | | | | | | | | | VBat low : (1) too low, (0) Ok
//  | | | | | | | | | | | | | | | VBat high (US mad) : (1) too high, (0) Ok
//  | | | | | | | | | | | | | | Timer elapsed : (1) elapsed, (0) not elapsed
//  | | | | | | | | | | | | | Power : (0) Ok, (1) not enough to fly
//  | | | | | | | | | | | | Angles : (0) Ok, (1) out of range
//  | | | | | | | | | | | Wind : (0) Ok, (1) too much to fly
//  | | | | | | | | | | Ultrasonic sensor : (0) Ok, (1) deaf
//  | | | | | | | | | Cutout system detection : (0) Not detected, (1) detected
//  | | | | | | | | PIC Version number OK : (0) a bad version number, (1) version number is OK
//  | | | | | | | ATCodec thread ON : (0) thread OFF (1) thread ON
//  | | | | | | Navdata thread ON : (0) thread OFF (1) thread ON
//  | | | | | Video thread ON : (0) thread OFF (1) thread ON
//  | | | | Acquisition thread ON : (0) thread OFF (1) thread ON
//  | | | CTRL watchdog : (1) delay in control execution (> 5ms), (0) control is well scheduled // Check frequency of control loop
//  | | ADC Watchdog : (1) delay in uart2 dsr (> 5ms), (0) uart2 is good // Check frequency of uart2 dsr (com with adc)
//  | Communication Watchdog : (1) com problem, (0) Com is ok // Check if we have an active connection with a client
//  Emergency landing : (0) no emergency, (1) emergency

typedef enum {
  ARDRONE_FLY_MASK            = 1 << 0,  /*!< FLY MASK : (0) ardrone is landed, (1) ardrone is flying */
  ARDRONE_VIDEO_MASK          = 1 << 1,  /*!< VIDEO MASK : (0) video disable, (1) video enable */
  ARDRONE_VISION_MASK         = 1 << 2,  /*!< VISION MASK : (0) vision disable, (1) vision enable */
  ARDRONE_CONTROL_MASK        = 1 << 3,  /*!< CONTROL ALGO : (0) euler angles control, (1) angular speed control */
  ARDRONE_ALTITUDE_MASK       = 1 << 4,  /*!< ALTITUDE CONTROL ALGO : (0) altitude control inactive (1) altitude control active */
  ARDRONE_USER_FEEDBACK_START = 1 << 5,  /*!< USER feedback : Start button state */
  ARDRONE_COMMAND_MASK        = 1 << 6,  /*!< Control command ACK : (0) None, (1) one received */
  ARDRONE_FW_FILE_MASK        = 1 << 7,  /* Firmware file is good (1) */
  ARDRONE_FW_VER_MASK         = 1 << 8,  /* Firmware update is newer (1) */
//  ARDRONE_FW_UPD_MASK         = 1 << 9,  /* Firmware update is ongoing (1) */
  ARDRONE_NAVDATA_DEMO_MASK   = 1 << 10, /*!< Navdata demo : (0) All navdata, (1) only navdata demo */
  ARDRONE_NAVDATA_BOOTSTRAP   = 1 << 11, /*!< Navdata bootstrap : (0) options sent in all or demo mode, (1) no navdata options sent */
  ARDRONE_MOTORS_MASK  	      = 1 << 12, /*!< Motors status : (0) Ok, (1) Motors problem */
  ARDRONE_COM_LOST_MASK       = 1 << 13, /*!< Communication Lost : (1) com problem, (0) Com is ok */
  ARDRONE_VBAT_LOW            = 1 << 15, /*!< VBat low : (1) too low, (0) Ok */
  ARDRONE_USER_EL             = 1 << 16, /*!< User Emergency Landing : (1) User EL is ON, (0) User EL is OFF*/
  ARDRONE_TIMER_ELAPSED       = 1 << 17, /*!< Timer elapsed : (1) elapsed, (0) not elapsed */
  ARDRONE_ANGLES_OUT_OF_RANGE = 1 << 19, /*!< Angles : (0) Ok, (1) out of range */
  ARDRONE_ULTRASOUND_MASK     = 1 << 21, /*!< Ultrasonic sensor : (0) Ok, (1) deaf */
  ARDRONE_CUTOUT_MASK         = 1 << 22, /*!< Cutout system detection : (0) Not detected, (1) detected */
  ARDRONE_PIC_VERSION_MASK    = 1 << 23, /*!< PIC Version number OK : (0) a bad version number, (1) version number is OK */
  ARDRONE_ATCODEC_THREAD_ON   = 1 << 24, /*!< ATCodec thread ON : (0) thread OFF (1) thread ON */
  ARDRONE_NAVDATA_THREAD_ON   = 1 << 25, /*!< Navdata thread ON : (0) thread OFF (1) thread ON */
  ARDRONE_VIDEO_THREAD_ON     = 1 << 26, /*!< Video thread ON : (0) thread OFF (1) thread ON */
  ARDRONE_ACQ_THREAD_ON       = 1 << 27, /*!< Acquisition thread ON : (0) thread OFF (1) thread ON */
  ARDRONE_CTRL_WATCHDOG_MASK  = 1 << 28, /*!< CTRL watchdog : (1) delay in control execution (> 5ms), (0) control is well scheduled */
  ARDRONE_ADC_WATCHDOG_MASK   = 1 << 29, /*!< ADC Watchdog : (1) delay in uart2 dsr (> 5ms), (0) uart2 is good */
  ARDRONE_COM_WATCHDOG_MASK   = 1 << 30, /*!< Communication Watchdog : (1) com problem, (0) Com is ok */
  ARDRONE_EMERGENCY_MASK      = 1 << 31  /*!< Emergency landing : (0) no emergency, (1) emergency */
} def_ardrone_state_mask_t;

static INLINE uint32_t ardrone_set_state_with_mask( uint32_t state, uint32_t mask, bool_t value )
{
  state &= ~mask;
  if( value )
    state |= mask;

  return state;
}

/** Returns a bit value of state from a mask
 * This function is used to test bits from a bit field like def_ardrone_state_mask_t
 *
 * @param state a 32 bits word we want to test
 * @param mask a mask that tells the bit to test
 * @return TRUE if bit is set, FALSE otherwise
 */
static INLINE bool_t ardrone_get_mask_from_state( uint32_t state, uint32_t mask )
{
  return state & mask ? TRUE : FALSE;
}

/** Convert time value from proprietary format to (unsigned int) micro-second value
 *
 * @param time value in proprietary format
 * @return time value in micro-second value (unsigned int)
 */
static INLINE uint32_t ardrone_time_to_usec( uint32_t time )
{
  return ((uint32_t)(time >> TSECDEC) * 1000000 + (uint32_t)(time & TUSECMASK));
}


#ifdef DEBUG_MODE
#define POSIX_DEBUG
#endif // DEBUG_MODE

#endif // _CONFIG_H_

/*! \mainpage ARDRone

<h2> Introduction </h2>

The ARDrone SDK provides the tools and ressources needed to create native applications for hardware platforms (PC, iPhone, iPad, Android and any host device supporting the Wifi ad-hoc mode). The ARDrone SDK provides reference functionalities that you can customize and extend. The iPhone reference application uses the ARDroneEngine Xcode project. This project provides a reference framework that makes it possible to create a functional application in a matter of minutes.

<h2> The core application </h2>

Every ARDrone application built using the ARDrone SDK shares the same core architecture. The ARDrone SDK provides the key objects needed to run the application and to coordinate the handling of user input, display the camera video and handling the tags detection. Where applications deviate from one another is in how they configure these default objects and also where they incorporate custom objects to enhance their application’s user interface and behavior.

When you create your own application you should re-use the high level APIs of the SDK. As each application depends on the platform and device used, it is important to understand when it needs customization and when the high level APIs are sufficient. This chapter provides an overview of the core application architecture and the high-level APIs customization points.

<h2> Core Application Architecture </h2>

Once connected to the AR.Drone, you can re-use the ARDrone SDK to initiate communication with the AR.Drone. Your application should manage the various devices (wifi connection, HMI). The AR.Drone SDK provides tools and methods to send commands, receive information from the AR.Drone and manage the video stream. To manage devices you need to understand the cycles of an AR.Drone application. The following sections describe these cycles and also provide a summary of some of the key design patterns used throughout the development of an ARDrone application.

<h2> The Application Life Cycle </h2>

The application life cycle is the minimum sequence of events that take place between the launch and termination of your AR.Drone application. You can compile the SDK with or without the main entry point provided with the SDK but your application must comply with the reference AR.Drone application life cycle to work correctly. The application is launched by calling it's main function. At this point, the application life cycle can be started. It initializes different user interfaces and calls custom SDK initialization objects from your application. During the event loop, the AR.Drone SDK calls the registered delegate objects. When the user performs an action that would cause your application to quit, the AR.Drone SDK notifies your application and begins the termination process.

Figure 1  Application life cycle

<img src="../schema_bloc_ardronemainloop.png">

The Main Function

The main function of an ARDrone application
<pre>
#include <ardrone_api.h>
#include <ardrone_tool/ardrone_tool.h>

C_RESULT res;

res = ardrone_tool_setup_com( NULL );

if( VP_FAILED(res) )
{
   PRINT("Wifi initialization failed. It means either:\n");
   PRINT("\t* you're not root (you can set up wifi connection only as root)\n");
   PRINT("\t* wifi device is not present (on your pc or on your card)\n");
   PRINT("\t* you set the wrong name for wifi interface (for example rausb0 instead of wlan0) \n");
   PRINT("\t* ap is not up (reboot card or remove wifi usb dongle)\n");
   PRINT("\t* wifi device has no antenna\n");
}
else
{
   res = ardrone_tool_init(argc, argv);

   while( VP_SUCCEEDED(res) && ardrone_tool_exit() == FALSE )
   {
      res = ardrone_tool_update();
   }

   res = ardrone_tool_shutdown();
}
</pre>

<h2> Minimum requirement to compile your AR.Drone application </h2>

To compile your application with the SDK you must integrate some MACRO.

Listing 1.1 Threads table
<pre>
#include <VP_Api/vp_api_thread_helper.h>

BEGIN_THREAD_TABLE
END_THREAD_TABLE
</pre>

Listing 1.2 Navdata table
<pre>
#include <ardrone_tool/Navdata/ardrone_navdata_client.h>

BEGIN_NAVDATA_HANDLER_TABLE
END_NAVDATA_HANDLER_TABLE
</pre>

<h2> Implementing Custom TOOL Schemes </h2>

You can register TOOL types for your application that include custom TOOL schemes. A custom TOOL scheme is a mechanism through which you customize specific code for your application and handle registering objects for the SDK.    

h3. Registering Custom TOOL Schemes

To register a TOOL type for your application you specify the custom tool methods declared in ardrone_tool.h file which is introduced in "The application life cycle". The following list identifies some methods that you might want to implement in your application.
* ardrone_tool_init_custom
* ardrone_tool_update_custom
* ardrone_tool_display_custom
* ardrone_tool_shutdown_custom
* ardrone_tool_exit

Methods for parsing command line
* ardrone_tool_check_argc_custom
* ardrone_tool_display_cmd_line_custom
* ardrone_tool_parse_cmd_line_custom

Details about these methods are explained in more detail later in this document.

<h2> Managing threads </h2>

You must declare a threads table in vp_api_thread_helper.h file. You can add threads to your application and default threads implemented in the SDK. The listing of the default threads will be described later in this document.

Listing 1.3 Implementing threads table 
<pre>
#include <VP_Api/vp_api_thread_helper.h>
PROTO_THREAD_ROUTINE(your_thread, private_data);

DEFINE_THREAD_ROUTINE(your_thread, private_data)
{
   //your code
}

BEGIN_THREAD_TABLE
  THREAD_TABLE_ENTRY( your_thread, 20 )
  THREAD_TABLE_ENTRY( navdata_update, 20 )
END_THREAD_TABLE
</pre>

To run and stop the threads, you declare MACRO in vp_api_thread_helper.h file. Use START_THREAD macro to run the thread and JOIN_THREAD macro to stop the thread. START_THREAD must be called in custom implemented method named ardrone_tool_init_custom which was introduced in "Implementing custom tool schemes". JOIN_THREAD is called in custom implemented method named ardrone_tool_shutdown_custom which was introduced in "Implementing custom tool schemes".

Listing 1.4 Managing threads 
<pre>
C_RESULT ardrone_tool_init_custom(int argc, char **argv)
{
  START_THREAD(your_thread, &private_data);

  return C_OK;
}

C_RESULT ardrone_tool_shutdown_custom()
{
  JOIN_THREAD( novadem );
}                  
</pre>

Activating default threads by adding in the threads table. The delegate object handles the default threads. 

<h2> Handling input events </h2>

The delegates for game controller events are handled by implementing methods included into input_device_t structure declared in ardrone_input.h file :

<pre>
typedef struct _input_device_t {
  char name[MAX_NAME_LENGTH];

  C_RESULT (*init)(void);
  C_RESULT (*update)(void);
  C_RESULT (*shutdown)(void);
} input_device_t;
</pre>

A method named ardrone_tool_input_add allowing to register methods by passing the pointer to the input_device_t structure is declared in ardrone_input.h file. This method is called in a custom method named ardrone_tool_init_custom which was introduced in "Implementing custom tool schemes". Such methods represent a game controller device. The SDK uses these methods to send pilot commands to the AR.Drone. Before we show how to implement these methods, let us describe the pilot's commands interface for the AR.Drone.

<h2> Description of the pilot's commands for the AR.Drone </h2>

The SDK provides means to emulate a gamepad. The following list identifies some gamepad emulation features.

* Left button : Deprecated
* Right button : Deprecated
* Up/Down button : push “up” button to go upward at a predetermined maximum vertical speed and push “down” button to go downward at a predetermined maximum vertical speed. Otherwise the AR.Drone stays at the same altitude. The maximum vertical speed can be configured.
* L1/R1 buttons: push “L1” button to turn left at a predetermined maximum angluar velocity and push “R1” button to turn right at a predetermined maximum angular velocity. Otherwise the AR.Drone keeps the same orientation. The maximum angular velocity can be configured.
* R2 button : provides means for trimming the AR.Drone with gamepad keys combination: 

    * Push "R2" button + left stick X-axis: decreases trim phi at 1°/s.
    * Push "R2" button + right stick X-axis: increases trim phi at 1°/s.
    * Push "R2" button + left stick Y-axis: decreases trim theta at 1°/s. 
    * Push "R2" button + right stick Y-axis: increases trim theta at 1°/s. 
    * Push "R2" button + push "L1" button : decreases trim yaw at 1°/s. 
    * Push "R2" button + push "R1" button : increases trim yaw at 1°/s.

* L2 button : Deprecated 
* select button : reset AR.Drone emergency signal.  
* start button : send take-off/land command to the AR.Drone.
* X-axis : the top button of the X-axis is to go forward at a predetermined max bending angle, the bottom button of the X-axis to strafe backward at a predetermined max bending angle. Otherwise keeps the same pitch angle. The max angle can be configured. 
* Y-axis : the left button of the Y-axis is to bend the AR.Drone to the left at a predetermined max angle; the right button of the Y-axis is to bend the AR.Drone to the right at a predetermined max angle. Otherwise it keeps the same bending angle. The max angle can be configured. 


The following figure shows the Gamepads functions
<img src="../schema_gamepad.png">

Declare these methods in ardrone_tool_input.h file for handling input change. 

* ardrone_tool_set_ui_pad_ag
* [[ardrone_tool_set_ui_pad_ab]]
* ardrone_tool_set_ui_pad_ad
* [[ardrone_tool_set_ui_pad_ah]]
* [[ardrone_tool_set_ui_pad_l1]]
* [[ardrone_tool_set_ui_pad_r1]]
* [[ardrone_tool_set_ui_pad_l2]]
* [[ardrone_tool_set_ui_pad_r2]]
* [[ardrone_tool_set_ui_pad_select]]
* [[ardrone_tool_set_ui_pad_start]]
* [[ ardrone_tool_set_ui_pad_xy ]]

This methods will be used by the "update" method introduced in "Handling input events".

<h2> Implementing input events methods </h2>

The delegate object handles your device by implementing init, update and close methods in your application. The delegate object calls the init method when you register a new device in the SDK. During the event loop, the delegate object calls the update method. The delegate object calls shutdown method when the application exit or your application can directly call the ardrone_tool_input_remove method (declared in ardrone_tool_input.h file) to remove your device from the SDK.

Listing 1.3 Implementing input events methods 
<pre>
#include <ardrone_api.h>
#include <VP_Os/vp_os_types.h>
#include <ardrone_tool/UI/ardrone_input.h>

input_device_t gamepad = {
  "Gamepad",
  open_gamepad,
  update_gamepad,
  close_gamepad
};

C_RESULT open_gamepad(void)
{
  C_RESULT res = C_FAIL;

  FILE* f = fopen("/proc/bus/input/devices", "r");

  if( f != NULL )
  {
    res = parse_proc_input_devices( f, GAMEPAD_LOGICTECH_ID);

    fclose( f );

    if( VP_SUCCEEDED( res ) && strcmp(gamepad.name, "Gamepad")!=0)
    {
         char dev_path[20]="/dev/input/";
         strcat(dev_path, gamepad.name);
      joy_dev = open(dev_path, O_NONBLOCK | O_RDONLY);
    }
      else
      {
         return C_FAIL;
      }
  }

  return res;
}
C_RESULT update_gamepad(void)
{
  static int32_t x = 0, y = 0;
  static bool_t refresh_values = FALSE;
  ssize_t res;
  static struct js_event js_e_buffer[64];

  res = read(joy_dev, js_e_buffer, sizeof(struct js_event) * 64);

  if( !res || (res < 0 && errno == EAGAIN) )
    return C_OK;

  if( res < 0 )
    return C_FAIL;

  if (res < (int) sizeof(struct js_event))// If non-complete bloc: ignored
    return C_OK;

  // Buffer décomposition in blocs (if the last is incomplete, it's ignored)
  int32_t idx = 0;
  refresh_values = FALSE;
  for (idx = 0; idx < res / sizeof(struct js_event); idx++)
  {
    if(js_e_buffer[idx].type & JS_EVENT_INIT )// If Init, the first values are ignored
    {
      break;
    }
    else if(js_e_buffer[idx].type & JS_EVENT_BUTTON )// Event Button detected
    {
      switch( js_e_buffer[idx].number )
      {
        case PAD_AG :
      ardrone_tool_set_ui_pad_ag(js_e_buffer[idx].value);
      break;
        case PAD_AB :
      ardrone_tool_set_ui_pad_ab(js_e_buffer[idx].value);
      break;
        case PAD_AD :
      ardrone_tool_set_ui_pad_ad(js_e_buffer[idx].value);
      break;
        case PAD_AH :
      ardrone_tool_set_ui_pad_ah(js_e_buffer[idx].value);
      break;
        case PAD_L1 :
      ardrone_tool_set_ui_pad_l1(js_e_buffer[idx].value);
      break;
        case PAD_R1 :
      ardrone_tool_set_ui_pad_r1(js_e_buffer[idx].value);
      break;
        case PAD_L2 :
      ardrone_tool_set_ui_pad_l2(js_e_buffer[idx].value);
      break;
        case PAD_R2 :
      ardrone_tool_set_ui_pad_r2(js_e_buffer[idx].value);
      break;
        case PAD_SELECT :
      ardrone_tool_set_ui_pad_select(js_e_buffer[idx].value);
      break;
        case PAD_START :
      ardrone_tool_set_ui_pad_start(js_e_buffer[idx].value);
      break;
        default:
      break;
      }
    }
    else if(js_e_buffer[idx].type & JS_EVENT_AXIS )// Event Axis detected
    {
      refresh_values = TRUE;
      switch( js_e_buffer[idx].number )
      {
        case PAD_X:
          x = ( js_e_buffer[idx].value + 1 ) >> 15;
          break;
        case PAD_Y:
          y = ( js_e_buffer[idx].value + 1 ) >> 15;
          break;
        default:
          break;
      }
    }
    else
    {// TODO: default: ERROR (non-supported)
    }
  }

  if(refresh_values)// Axis values to refresh
    {
      ardrone_tool_set_ui_pad_xy( x, y );
    }
  return C_OK;
}

C_RESULT close_gamepad(void)
{
  close( joy_dev );

  return C_OK;
}
</pre>

<h2> Using progressive commands </h2>

The [[ardrone_at_set_progress_cmd_with_pitch]] method enables you to pilot the AR.Drone with better control. It replaces [[ ardrone_tool_set_ui_pad_xy ]] methods for controlling pitch and roll movements and [[ardrone_tool_set_ui_pad_ab]] and [[ardrone_tool_set_ui_pad_ah]]. For example, you can use this method to send the accelerometer values or to send radiogp commands. 

<h2> Registering a game controller device </h2>

To register a new device for your application, you must call ardrone_tool_input_add method. Call ardrone_tool_input_remove method to unregister device. To add a new device call ardrone_tool_input_add method in your custom implemented method named ardrone_tool_init_custom (see "Implementing custom tool schemes"). To remove your device call ardrone_tool_input_remove method in your custom implemented method named ardrone_tool_shutdown_custom (see "Implementing custom tool schemes").

Listing 1.4 Registering a game controller device
<pre>
C_RESULT ardrone_tool_init_custom(int argc, char **argv)
{
  // Add inputs
  ardrone_tool_input_add( &gamepad );

  return C_OK;
}

C_RESULT ardrone_tool_shutdown_custom()
{
  ardrone_tool_input_remove( &gamepad );

  return C_OK;
}
</pre>

A fully implemented custom method is described later in this document.

<h2> Displaying the video stream </h2>

The video stream data are handled using a pipeline. This SDK provides some stages that you can sequentially connect.
The life cycle of a pipeline must realize a minimum sequence.

Listing 1.5 Processing of a pipeline
<pre>
    #include <VP_Api/vp_api.h>

    res = vp_api_open(&pipeline, &pipeline_handle);

    if( VP_SUCCEEDED(res) )
    {
      int loop = VP_SUCCESS;
      out.status = VP_API_STATUS_PROCESSING;

      while( !ardrone_tool_exit() && (loop == VP_SUCCESS) )
      {
        if( image_vision_window_view == WINDOW_VISIBLE ) {
          if( VP_SUCCEEDED(vp_api_run(&pipeline, &out)) ) {
            if( (out.status == VP_API_STATUS_PROCESSING || out.status == VP_API_STATUS_STILL_RUNNING) ) {
              loop = VP_SUCCESS;
            }
          }
          else loop = -1; // Finish this thread
        }
      }

      vp_api_close(&pipeline, &pipeline_handle);
</pre>
Your application needs to call this in a thread introduced in "Managing threads".

Listing 1.6 Handling the video stream data for an iPhone application

<pre>
Header files in VP_SDK
#include <VP_Api/vp_api.h> 
#include <VP_Api/vp_api_error.h>
#include <VP_Api/vp_api_stage.h>
#include <VP_Api/vp_api_picture.h>
#include <VP_Stages/vp_stages_yuv2rgb.h>

Header files in VLIB
#include <VLIB/Stages/vlib_stage_decode.h>

Header files in ARDroneLib
#include <ardrone_tool/ardrone_tool.h>
#include <ardrone_tool/Com/config_com.h>
#include <ardrone_tool/Video/video_com_stage.h>

include in header file
PROTO_THREAD_ROUTINE(video_stage, data);

Implementing in source code file 
DEFINE_THREAD_ROUTINE(video_stage, data)
{
C_RESULT res;

[[vp_api_io_pipeline_t]] pipeline;
[[vp_api_io_data_t]] out;
[[vp_api_io_stage_t]] stages[NB_STAGES];

[[vp_api_picture_t]] picture;

[[vlib_stage_decoding_config_t]] vec;
[[video_com_config_t]]           icc;

video_stage_started = TRUE;

vp_os_memset(&icc, 0, sizeof( icc ));
vp_os_memset(&vec, 0, sizeof( vec ));

/// Picture configuration
picture.format = PIX_FMT_RGB565;

picture.width = 512;
picture.height = 512;
picture.framerate = 15;

picture.y_buf = vp_os_malloc( picture.width * picture.height );
picture.cr_buf = vp_os_malloc( picture.width * picture.height / 2 );
picture.cb_buf = vp_os_malloc( picture.width * picture.height / 2 );

picture.y_line_size = picture.width * 2;
picture.cb_line_size = 0;
picture.cr_line_size = 0;

icc.com = COM_VIDEO();
icc.buffer_size = 100000;
icc.protocol = VP_COM_UDP;
COM_CONFIG_SOCKET_VIDEO(&icc.socket, VP_COM_CLIENT, VIDEO_PORT, wifi_ardrone_ip);

vec.width = 512;
vec.height = 512;
vec.picture = &picture;
vec.luma_only = FALSE;
vec.block_mode_enable = TRUE;

pipeline.nb_stages = 0;

stages[pipeline.nb_stages].type = VP_API_INPUT_SOCKET;
stages[pipeline.nb_stages].cfg = (void *)&icc;
stages[pipeline.nb_stages].funcs = video_com_funcs;
pipeline.nb_stages++;

stages[pipeline.nb_stages].type = VP_API_FILTER_DECODER;
stages[pipeline.nb_stages].cfg = (void*)&vec;
stages[pipeline.nb_stages].funcs = vlib_decoding_funcs;
pipeline.nb_stages++;

stages[pipeline.nb_stages].type = VP_API_OUTPUT_LCD;
stages[pipeline.nb_stages].cfg = (void*)&vec;
stages[pipeline.nb_stages].funcs = opengl_video_stage_funcs;
pipeline.nb_stages++;

pipeline.stages = &stages[0];

if( !ardrone_tool_exit() )
{
res = vp_api_open(&pipeline, &pipeline_handle);

if( VP_SUCCEEDED(res) )
{
int loop = VP_SUCCESS;
out.status = VP_API_STATUS_PROCESSING;

while( !ardrone_tool_exit() && (loop == VP_SUCCESS) )
{
if(!video_stage_started)
{
vp_os_mutex_lock(&video_stage_mutex);
icc.num_retries = VIDEO_MAX_RETRIES;
vp_os_cond_wait(&video_stage_condition);
video_stage_started = TRUE;
vp_os_mutex_unlock(&video_stage_mutex);
}

if( VP_SUCCEEDED(vp_api_run(&pipeline, &out)) ) {
if( (out.status == VP_API_STATUS_PROCESSING || out.status == VP_API_STATUS_STILL_RUNNING) ) {
loop = VP_SUCCESS;
}
}
else loop = -1; // Finish this thread
}

vp_api_close(&pipeline, &pipeline_handle);
}
}

return (THREAD_RET)0;
}
</pre>
    
<h2> Toggling the video camera </h2>

The [[ardrone_at_zap]] method will enable you to select the video from either the horizontal or the vertical camera.

The following figure displays the video channels

<img src="../horizontal_camera.png">
Displaying of horizontal video camera

<img src="../vertical_camera.png">
Displaying of vertical video camera

<img src="../horizontal_vertical_camera.png">
Both, "large horizontal" and "small vertical" video camera

<img src="../vertical_horizontal_camera.png">
Both, "large vertical" and "small horizontal" video camera

<h2> Playing a led animation </h2>

The [[ardrone_at_set_led_animation]] method enables you to play a led animation.

<h2> Playing a flight animation </h2>

The [[ardrone_at_set_mayday]] method will enable you to play flight animations with the AR.Drone.

<h2> Receiving Navdata Information </h2>

To control the AR.Drone you need to retrieve informations containing alert messages. The following Navdata are important to the user.

* Communication Lost
* Emergency landing
* VBat low 
* Motors status
* Cutout system detection

The following picture displays a control panel showing alert messages.
<img src="../control_panel.png">

Navdata gives you informations about settings of the AR.Drone. The following list identifies some parameters.
* Yaw speed (°/s)
* Vert speed (°/s)
* Trim Pitch (°)
* Trim Yaw (°/s)
* Tilt (°)

With these settings, you can adjust the control of the AR.Drone to your driving style. The following picture displays a settings panel.
<img src="../settings_panel3.png">
 
To change the setting parameters of the AR. Drone, you must use the corresponding AT commands located in the file ardrone_api.h file. The following list identifies some methods to configure the AR.Drone.
* ardrone_at_set_tilt, indicates to the enemy AR.Drone that it was hit.
* ardrone_at_set_vert_speed 
* ardrone_at_set_manual_trim, to change in manual mode to configure trim parameters.
* ardrone_at_set_max_angle
* ardrone_at_set_yaw
...

The Navdata are sent periodically (15 times per second). A mechanism is provided to retrieve this data. To handle Navdata you should implement methods (init, process and release) that are embedded in the ardrone_navdata_handler_t structure declared in ardrone_navdata_client.h. A delegate object calls the init method when launching the Navdata client thread. During the event loop, the delegate object calls the process method. The delegate object calls release method when the thread exits. In order to register this method in the client, your application should define a Navdata table which was introduced in "Minimum requirement to compile your AR.Drone application". The following listing code shows an example to handle Navdata.

<pre>
#include <ardrone_tool/Navdata/ardrone_navdata_client.h>

C_RESULT navdata_init( void* private_data )
{
   //Your code to initialize your local variables.
}

C_RESULT navdata_process( const navdata_unpacked_t* const pnd )
{
  //Retrieves current Navdata unpacked.
}

C_RESULT navdata_release( void )
{
  //Free local variables.
}

//Defines Navdata table in your application
BEGIN_NAVDATA_HANDLER_TABLE
  NAVDATA_HANDLER_TABLE_ENTRY(navdata_init, navdata_process, navdata_release, NULL) //Register your methods to the client
END_NAVDATA_HANDLER_TABLE
</pre>

The client calls process method with a reference to a structure navdata_unpacked_t declared in ardrone_api.h file. This structure contains other structures. The following list identifies some structures.
* navdata_demo_t navdata_demo; This structure contains alert messages, settings parameters and informations about single player mode. 
* navdata_vision_detect_t  navdata_vision_detect; This structure contains informations about multi-player mode. 

<h2> Single player mode </h2>

The goal is to create a game using augmented reality gaming mechanics. The 3D-tag can be detected by the AR.Drone using its frontal or below camera. The AR.Drone then sends the used camera position parameters in the 3D-tag coordinate frame to the client via the NavData. The following list identifies some detection parameters.
* Camera rotation, 3D rotation coefficients.
* Camera translation, algebraic coordinates.
* tag index, tag type. 
* Camera type, horizontal or vertical.

Figure showing the 3D-tag detected by the AR.Drone
<img src="../view_3dtag.png">

These parameters are accessible in the Navdata_demo structure. A reference to this structure was introduced in the preceding section "Receiving Navdata Information".
*/
