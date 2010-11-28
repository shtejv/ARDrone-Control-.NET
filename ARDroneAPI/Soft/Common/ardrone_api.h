#ifndef _ARDRONE_API_H_
#define _ARDRONE_API_H_

#include <ardrone_common_config.h>
#include <ATcodec/ATcodec_api.h>
#include <navdata_common.h>
#include <vision_common.h>
#include <VP_Os/vp_os_malloc.h>

#if defined(USE_LINUX) && defined(_EMBEDDED) && !defined(USE_MINGW32)
#define API_WEAK WEAK
#else
#define API_WEAK
#endif

#define ARDRONE_CONFIGURATION_SET(NAME, VALUE) 				ardrone_at_configuration_set_##NAME(VALUE)
#define ARDRONE_CONFIGURATION_PROTOTYPE(C_TYPE, NAME) 		C_RESULT ardrone_at_configuration_set_##NAME(C_TYPE NAME)

enum {
  NO_CONTROL                           = 0,
  ALTITUDE_CONTROL                     = 1,
  ALTITUDE_VISION_CONTROL              = 2,
  ALTITUDE_VISION_CONTROL_TAKEOFF_TRIM = 3,
};

enum {
  NO_CONTROL_MODE = 0,          // Doing nothing
  ARDRONE_UPDATE_CONTROL_MODE,  // Ardrone software update reception (update is done next run)
                                // After event completion, card should power off
  PIC_UPDATE_CONTROL_MODE,      // Ardrone pic software update reception (update is done next run)
                                // After event completion, card should power off
  LOGS_GET_CONTROL_MODE,        // Send previous run's logs
  CFG_GET_CONTROL_MODE,         // Send activ configuration
  ACK_CONTROL_MODE              // Reset command mask in navdata
};

// define video channel
typedef enum
{
	ZAP_CHANNEL_HORI=0,
	ZAP_CHANNEL_VERT,
	ZAP_CHANNEL_LARGE_HORI_SMALL_VERT,
	ZAP_CHANNEL_LARGE_VERT_SMALL_HORI,
	ZAP_CHANNEL_NEXT,
} ZAP_VIDEO_CHANNEL;

typedef enum
{
	CAD_TYPE_HORIZONTAL = 0, // Not used but keep to compatibility
	CAD_TYPE_VERTICAL,		 // Not used but keep to compatibility
	CAD_TYPE_VISION_DETECT,
    CAD_TYPE_NONE,
	CAD_TYPE_NUM,
} CAD_TYPE;

typedef enum
{
  ORANGE_GREEN = 1,
  ORANGE_YELLOW,
  ORANGE_BLUE
} COLORS_DETECTION_TYPE, ENEMY_COLORS_TYPE;

typedef enum
{
	// This is a bit to shift
	CONTROL_LEVEL_ACCELERO_DISABLED	= 0,
	CONTROL_LEVEL_COMBINED_YAW		= 1,
	CONTROL_LEVEL_CONTROL_MODE		= 2,
	//4 used for CONTROL_LEVEL_CONTROL_MODE_BIT
} CONTROL_LEVEL;

// define led animation
typedef enum LED_ANIMATION_IDS_
{
   #define LED_ANIMATION(NAME, ... ) NAME ,
   #include <led_animation.h>
   #undef LED_ANIMATION
   NUM_LED_ANIMATION
} LED_ANIMATION_IDS;

/**
 * \struct _euler_angles_t
 * \brief  Euler angles in float32_t format expressed in radians.
 */
typedef struct _euler_angles_t {
  float32_t theta;
  float32_t phi;
  float32_t psi;
} euler_angles_t;

/**
 * \struct _euler_angles_t
 * \brief  Euler angles in int32_t format expressed in radians.
 */
typedef struct _iEuler_angles_t {
  int32_t theta;
  int32_t phi;
  int32_t psi;
} iEuler_angles_t;

/**
 * \struct _angular_rates_t
 * \brief Angular rates in float32_t format
 */
typedef struct _angular_rates_t {
  float32_t p;
  float32_t q;
  float32_t r;
} angular_rates_t;

/**
 * \struct _velocities_t
 * \brief Velocities in float32_t format
 */
/*
typedef struct _velocities_t {
  float32_t x;
  float32_t y;
  float32_t z;
} velocities_t;
*/
/**
 * \struct _acq_vision_t
 * \brief Vision params in float32_t
 */
typedef struct _acq_vision_t {
  float32_t tx;
  float32_t ty;
  float32_t tz;

  int32_t turn_angle;
  int32_t height;
  int32_t turn_finished;

  bool_t flag_25Hz;
} acq_vision_t;

typedef struct _polaris_data_t {
  float32_t x;
  float32_t y;
  float32_t psi;
  bool_t    defined;
  int32_t   time_us;
} polaris_data_t;

/**
 * \enum api_control_gains_t
 * \brief gain structure
*/
typedef struct _api_control_gains_t {
  int32_t     pq_kp;            /**<    Gain for proportionnal correction in pitch (p) and roll (q) angular rate control */
  int32_t     r_kp;             /**<    Gain for proportionnal correction in yaw (r) angular rate control */
  int32_t     r_ki;             /**<    Gain for integral correction in yaw (r) angular rate control */
  int32_t     ea_kp;            /**<    Gain for proportionnal correction in Euler angle control */
  int32_t     ea_ki;            /**<    Gain for integral correction in Euler angle control */
  int32_t     alt_kp;           /**<    Gain for proportionnal correction in Altitude control */
  int32_t     alt_ki;           /**<    Gain for integral correction in Altitude control */
  int32_t     vz_kp;            /**<    Gain for proportional correction in Vz control */
  int32_t     vz_ki;            /**<   Gain for integral correction in Vz control */
  int32_t     hovering_kp;      /**<    Gain for proportionnal correction in hovering control */
  int32_t     hovering_ki;      /**<    Gain for integral correction in hovering control */
} api_control_gains_t;

typedef struct _api_vision_tracker_params_t {
  int32_t coarse_scale;         /**<    scale of current picture with respect to original picture */
  int32_t nb_pair;              /**<    number of searched pairs in each direction */
  int32_t loss_per;             /**<    authorized lost pairs percentage for tracking */
  int32_t nb_tracker_width;     /**<    number of trackers in width of current picture */
  int32_t nb_tracker_height;    /**<    number of trackers in height of current picture */
  int32_t scale;                /**<    distance between two pixels in a pair */
  int32_t trans_max;            /**<    largest value of trackers translation between two adjacent pictures */
  int32_t max_pair_dist;        /**<    largest distance of pairs research from tracker location */
  int32_t noise;                /**<    threshold of significative contrast */
} api_vision_tracker_params_t;

/********************************************************************
*                        NAVDATA FUNCTIONS
********************************************************************/
typedef struct _navdata_unpacked_t {
  uint32_t  ardrone_state;
  bool_t    vision_defined;

  navdata_demo_t           navdata_demo;
  navdata_time_t           navdata_time;
  navdata_raw_measures_t   navdata_raw_measures;
  navdata_phys_measures_t  navdata_phys_measures;
  navdata_gyros_offsets_t  navdata_gyros_offsets;
  navdata_euler_angles_t   navdata_euler_angles;
  navdata_references_t     navdata_references;
  navdata_trims_t          navdata_trims;
  navdata_rc_references_t  navdata_rc_references;
  navdata_pwm_t            navdata_pwm;
  navdata_altitude_t       navdata_altitude;
  navdata_vision_raw_t     navdata_vision_raw;
  navdata_vision_of_t      navdata_vision_of;
  navdata_vision_t         navdata_vision;
  navdata_vision_perf_t    navdata_vision_perf;
  navdata_trackers_send_t  navdata_trackers_send;
  navdata_vision_detect_t  navdata_vision_detect;
  navdata_watchdog_t       navdata_watchdog;
  navdata_adc_data_frame_t navdata_adc_data_frame;
} navdata_unpacked_t;

#define ardrone_navdata_pack( navdata_ptr, option ) (navdata_option_t*) navdata_pack_option( (uint8_t*) navdata_ptr,      \
                                                                                     (uint8_t*) &option,          \
                                                                                     option.size )

#define ardrone_navdata_unpack( navdata_ptr, option ) (navdata_option_t*) navdata_unpack_option( (uint8_t*) navdata_ptr,  \
                                                                                         (uint8_t*) &option,      \
                                                                                         navdata_ptr->size )

static INLINE uint8_t* navdata_pack_option( uint8_t* navdata_ptr, uint8_t* data, uint32_t size )
{
  vp_os_memcpy(navdata_ptr, data, size);

  return (navdata_ptr + size);
}

static INLINE uint8_t* navdata_unpack_option( uint8_t* navdata_ptr, uint8_t* data, uint32_t size )
{
  vp_os_memcpy(data, navdata_ptr, size);

  return (navdata_ptr + size);
}

static INLINE navdata_option_t* navdata_next_option( navdata_option_t* navdata_options_ptr )
{
  uint8_t* ptr;

  ptr  = (uint8_t*) navdata_options_ptr;
  ptr += navdata_options_ptr->size;

  return (navdata_option_t*) ptr;
}

/********************************************************************
 * ardrone_navdata_compute_cks:
 *-----------------------------------------------------------------*/
/**
 * @param nv Data to calculate the checksum.
 *
 * @param size Size of data calculate as follow : size-sizeof(navdata_cks_t).
 *
 * @return Retrieve the checksum from nv.
 *
 *******************************************************************/
uint32_t ardrone_navdata_compute_cks( uint8_t* nv, int32_t size ) API_WEAK;

/********************************************************************
 * ardrone_navdata_unpack_all:
 *-----------------------------------------------------------------*/
/**
 * @param navdata_unpacked  navdata_unpacked in which to store the navdata.
 *
 * @param navdata One packet read from the port NAVDATA.
 *
 * @param Checksum of navdata
 *
 * @brief Disassembles buffer of navdata and completed it in the structure navdata_unpacked.
 *
 * @DESCRIPTION
 *
 *******************************************************************/
C_RESULT ardrone_navdata_unpack_all(navdata_unpacked_t* navdata_unpacked, navdata_t* navdata, uint32_t* cks) API_WEAK;

/********************************************************************
 * ardrone_navdata_search_option:
 *-----------------------------------------------------------------*/
/**
 * @param navdata_options_ptr
 *
 * @param tag
 *
 * @brief
 *
 * @DESCRIPTION
 *
 *******************************************************************/
navdata_option_t* ardrone_navdata_search_option( navdata_option_t* navdata_options_ptr, uint32_t tag ) API_WEAK;

/********************************************************************
*                        AT FUNCTIONS
********************************************************************/

/********************************************************************
 * ardrone_at_init: Init at command.
 *-----------------------------------------------------------------*/
/**
 * @param void
 *
 * @brief Fill structure AT codec
 *        and built the library AT commands.
 *
 * @DESCRIPTION
 *
 *******************************************************************/
void ardrone_at_init( const char* ip, size_t ip_len ) API_WEAK;

/****************************************************************************
 * ardrone_at_init_with_funcs: Init at command with particular ATCodec funcs
 *-----------------------------------------------------------------**********/
/**
 * @param funcs
 *
 * @brief Fill structure AT codec
 *        and built the library AT commands.
 *
 * @DESCRIPTION
 *
 *******************************************************************/
void ardrone_at_init_with_funcs ( const char* ip, size_t ip_len, AT_CODEC_FUNCTIONS_PTRS *funcs) API_WEAK;

/********************************************************************
 * ardrone_at_open: Open at command socket.
 *-----------------------------------------------------------------*/
/**
 * @param void
 *
 * @brief Open at command socket.
 *
 * @DESCRIPTION
 *
 *******************************************************************/
ATCODEC_RET ardrone_at_open ( void ) API_WEAK;

/********************************************************************
 * ardrone_at_send: Send all pushed messages.
 *-----------------------------------------------------------------*/
/**
 * @param void
 *
 * @brief Send all pushed messages.
 *
 * @DESCRIPTION
 *
 *******************************************************************/
ATCODEC_RET ardrone_at_send ( void ) API_WEAK;

/**
 * @fn     Send input sequence number to avoid reception of old data
 * @param  value : sequence number
 * @return void
 */
//void ardrone_at_set_sequence( uint32_t sequence ) API_WEAK;

/**
 * @fn     Send ui state to toy
 * @param  value : encoded version of user inputs
 * @return void
 */
void ardrone_at_set_ui_value( uint32_t value ) API_WEAK;

/**
 * @fn     Send Misc data to toy
 * @param  data are used to configure control
 * @return void
 */
void ardrone_at_set_pmode( int32_t pmode ) API_WEAK;

/**
 * @fn     Tell to keep trim result
 * @param  yes or no
 * @return C_RESULT
 */
void ardrone_at_keep_trim(bool_t keep) API_WEAK;

/**
 * @fn     Reset trim/misc0 related ack's
 * @return void
 */
void ardrone_at_trim_ack_reset(void) API_WEAK;

/**
 * @fn     Send Misc data to toy
 * @param  data are used to configure control (for instance)
 * @return void
 */
void ardrone_at_set_ui_misc( int32_t m1, int32_t m2, int32_t m3, int32_t m4 ) API_WEAK;

/**
 * @fn     Play an animation
 * @param  type : type of animation
 * @param  timeout : duration of the animation
 * @return void
 */
void ardrone_at_set_anim(int32_t type, int32_t timeout) API_WEAK;

/**
 * @fn     Send to drone a command to set flat trim
 * @return void
 */
void ardrone_at_set_flat_trim(void) API_WEAK;

/**
 * @fn     Send to drone a command to set manual trims
 * @return void
 */
void ardrone_at_set_manual_trims(float32_t trim_pitch, float32_t trim_roll, float32_t trim_yaw) API_WEAK;

/// Functions used during developpment

/**
 * @fn     Change gain values according to user input
 * @param  user_ctrl_gains : gains to be set
 * @return void
 */
void ardrone_at_set_control_gains( api_control_gains_t* user_ctrl_gains ) API_WEAK;

/**
 * @fn     Change tracking params (only in UE_IHM_PO mode)
 * @param  params : new params
 * @return void
 */
void ardrone_at_set_vision_track_params( api_vision_tracker_params_t* params ) API_WEAK;

/**
 * @fn     Start a raw capture
 * @return void
 */
void ardrone_at_start_raw_capture(void) API_WEAK;

/**
 * @fn     Change video channel
 * @return void
 */
void ardrone_at_cad( CAD_TYPE type, float32_t tag_size ) API_WEAK;

/**
 * @fn     Change video channel
 * @return void
 */
void ardrone_at_zap( ZAP_VIDEO_CHANNEL channel ) API_WEAK;

/**
 * @fn     Playing led animation
 * @return void
 */
void ardrone_at_set_led_animation ( LED_ANIMATION_IDS anim_id, float32_t freq, uint32_t duration_sec ) API_WEAK;

/**
 * @fn     Set vision update options (only in UE_IHM_PO mode)
 * @param  user_vision_option : new option
 * @return void
 */
void ardrone_at_set_vision_update_options(int32_t user_vision_option) API_WEAK;

/**
 * @fn     Set drone's position as seen by polaris
 * @param  x_polaris : x of ardrone position seen by polaris
 * @param  y_polaris : y of ardrone position seen by polaris
 * @param  defined : tells if polaris data are valid or not
 * @param  time_us : time in us
 * @return void
 */
void ardrone_at_set_polaris_pos(float32_t x_polaris, float32_t y_polaris, float32_t psi_polaris, bool_t defined, int32_t time_us) API_WEAK;

/**
 * @fn     Send to drone a configuration
 * @param  param : parameter to set or update
 * @param  value : value to apply to the parameter
 * @return void
 */
void ardrone_at_set_toy_configuration(const char* param, const char* value) API_WEAK;

/**
 * @fn     Ask the drone to reset com watchdog
 * @return void
 */
void ardrone_at_reset_com_watchdog(void) API_WEAK;

/**
 * @fn     Ask the drone to purge log files
 * @return void
 */
void ardrone_at_reset_logs(void) API_WEAK;

/**
 * @fn     Ask the drone we receive a plf with a size filesize
 * @return void
 */
void ardrone_at_update_control_mode(uint32_t what_to_do, uint32_t filesize) API_WEAK;

/**
 * @fn     Ask the drone to send control mode
 * @return void
 */
void ardrone_at_configuration_get_ctrl_mode(void) API_WEAK;

/**
 * @fn     Ask the drone we receive control mode
 * @return void
 */
void ardrone_at_configuration_ack_ctrl_mode(void) API_WEAK;

/**
 * @fn     Set drone's pwm's directly
 * @return void
 */
void ardrone_at_set_pwm(int32_t p1, int32_t p2, int32_t p3, int32_t p4) API_WEAK;

/**
 * @fn     Send to drone progressive command
 * @param  enable
 * @param  phi
 * @param  theta
 * @param  gaz
 * @param yaw
 *
 * @return void
 */
void ardrone_at_set_progress_cmd(int32_t enable, float32_t phi, float32_t theta, float32_t gaz, float32_t yaw );


/*****************************************************************
*                       AT CONFIG FUNCTIONS
*****************************************************************/
#undef ARDRONE_CONFIG_KEY_IMM
#undef ARDRONE_CONFIG_KEY_REF
#undef ARDRONE_CONFIG_KEY_STR
#define ARDRONE_CONFIG_KEY_IMM(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#define ARDRONE_CONFIG_KEY_REF(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#define ARDRONE_CONFIG_KEY_STR(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#include <config_keys.h> // must be included before to have types

#undef ARDRONE_CONFIG_KEY_IMM
#undef ARDRONE_CONFIG_KEY_REF
#undef ARDRONE_CONFIG_KEY_STR
#define ARDRONE_CONFIG_KEY_IMM(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) ARDRONE_CONFIGURATION_PROTOTYPE(C_TYPE,NAME);
#define ARDRONE_CONFIG_KEY_REF(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#define ARDRONE_CONFIG_KEY_STR(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) ARDRONE_CONFIGURATION_PROTOTYPE(C_TYPE_PTR,NAME);
#include <config_keys.h> // must be included before to have types

/********************************************************************
*                        CONFIG FUNCTIONS
********************************************************************/
#undef ARDRONE_CONFIG_KEY_IMM
#undef ARDRONE_CONFIG_KEY_REF
#undef ARDRONE_CONFIG_KEY_STR
#define ARDRONE_CONFIG_KEY_IMM(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#define ARDRONE_CONFIG_KEY_REF(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#define ARDRONE_CONFIG_KEY_STR(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#include <config_keys.h> // must be included before to have types

#undef ARDRONE_CONFIG_KEY_IMM
#undef ARDRONE_CONFIG_KEY_REF
#undef ARDRONE_CONFIG_KEY_STR
#define ARDRONE_CONFIG_KEY_IMM(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) \
  C_TYPE get_##NAME(void) API_WEAK; \
  C_RESULT set_##NAME(C_TYPE val) API_WEAK;
#define ARDRONE_CONFIG_KEY_REF(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) \
  C_TYPE_PTR get_##NAME(void) API_WEAK; \
  C_RESULT set_##NAME(C_TYPE_PTR val) API_WEAK;
#define ARDRONE_CONFIG_KEY_STR(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) \
  C_TYPE_PTR get_##NAME(void) API_WEAK; \
  C_RESULT set_##NAME(C_TYPE_PTR val) API_WEAK;
// Generate all accessors functions prototypes
#include <config_keys.h>


#undef ARDRONE_CONFIG_KEY_IMM
#undef ARDRONE_CONFIG_KEY_REF
#undef ARDRONE_CONFIG_KEY_STR
#define ARDRONE_CONFIG_KEY_IMM(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#define ARDRONE_CONFIG_KEY_REF(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#define ARDRONE_CONFIG_KEY_STR(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#include <config_keys.h> // must be included before to have types

#undef ARDRONE_CONFIG_KEY_IMM
#undef ARDRONE_CONFIG_KEY_REF
#undef ARDRONE_CONFIG_KEY_STR
#define ARDRONE_CONFIG_KEY_IMM(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) C_TYPE NAME;
#define ARDRONE_CONFIG_KEY_REF(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) C_TYPE NAME;
#define ARDRONE_CONFIG_KEY_STR(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) C_TYPE NAME;

// Fill structure fields types
typedef struct _ardrone_config_t
{
#include <config_keys.h>
}
ardrone_config_t;


#endif // _ARDRONE_API_H_

