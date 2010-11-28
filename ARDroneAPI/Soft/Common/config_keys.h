#ifndef CONFIG_KEYS_STRING_TYPE_DEFINED
#define CONFIG_KEYS_STRING_TYPE_DEFINED
  typedef char string_t[64];
#endif // ! CONFIG_KEYS_STRING_TYPE_DEFINED
#include <Maths/maths.h>

#ifndef CONFIG_KEYS_RW_ENUM_DEFINED
#define CONFIG_KEYS_RW_ENUM_DEFINED
  enum {
    K_READ  = 1,
    K_WRITE = 1<<1,
    K_NOBIND = 1<<2  // Data will no be read from config.ini file. Only the program can change it.
  };
#endif // ! CONFIG_KEYS_RW_ENUM_DEFINED

#ifndef CONFIG_KEYS_DEFINES_DEFINED

# ifdef INSIDE_FLIGHT
#   define MAX_EULER_ANGLES_REF (12000.0f * MDEG_TO_RAD)     /* EA control, maximum reference [rad] */
#   define MAX_OUTDOOR_EULER_ANGLES_REF (20000.0f * MDEG_TO_RAD)     /* EA control, maximum reference [rad] */
# else
#   define MAX_EULER_ANGLES_REF (12000.0f * MDEG_TO_RAD)    /* EA control, maximum reference [rad] */
#   define MAX_OUTDOOR_EULER_ANGLES_REF (20000.0f * MDEG_TO_RAD)     /* EA control, maximum reference [rad] */
# endif

# define CONFIG_KEYS_DEFINES_DEFINED
//Calibration renvoye par le PIC dans le cas ou il n'en a pas recut
#define DEFAULT_PWM_REF_GYRO                500
#define DEFAULT_GYRO_OFFSET_THR_X           4.0
#define DEFAULT_GYRO_OFFSET_THR_Y           4.0
#define DEFAULT_GYRO_OFFSET_THR_Z           0.5
#define default_accs_offset          {{{ -2048.0f, 2048.0f, 2048.0f}}}
#define default_accs_gain            {1.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f, -1.0f }
#define default_gyros_offset         {{{ 1662.5f, 1662.5f, 1662.5f}}}
#define default_gyros_gains          {{{ 395.0f * MDEG_TO_RAD, -395.0f * MDEG_TO_RAD, -207.5f * MDEG_TO_RAD }}}
#define default_gyros110_offset      {{ 1662.5f, 1662.5f}}
#define default_gyros110_gains       {{ 87.5f * MDEG_TO_RAD, -87.5f * MDEG_TO_RAD }}
#define default_motor_version         "0.0"
#define NULL_MAC											"00:00:00:00:00:00"

static const vector21_t DEFAULT_GYROS110_OFFSET = default_gyros110_offset;
static const vector21_t DEFAULT_GYROS110_GAIN   = default_gyros110_gains;

#define default_pwm_ref_gyro          DEFAULT_PWM_REF_GYRO
#define default_gyro_offset_thr_x     DEFAULT_GYRO_OFFSET_THR_X
#define default_gyro_offset_thr_y     DEFAULT_GYRO_OFFSET_THR_Y
#define default_gyro_offset_thr_z     DEFAULT_GYRO_OFFSET_THR_Z

#define default_euler_angle_ref_max    		MAX_EULER_ANGLES_REF
#define default_outdoor_euler_angle_ref_max	MAX_OUTDOOR_EULER_ANGLES_REF
# define default_altitude_max  			(3000)
# define default_altitude_min  			(50)
# define default_control_trim_z  		(0.0f * MDEG_TO_RAD)
# define default_control_iphone_tilt 	(20000.0f * MDEG_TO_RAD)
# define default_control_vz_max			(700.0f)
# define default_outdoor_control_vz_max	(1000.0f)
# define default_control_yaw			(100000.0f * MDEG_TO_RAD)
# define default_outdoor_control_yaw	(200000.0f * MDEG_TO_RAD)

#define default_enemy_colors			ORANGE_GREEN
#define default_detect_type				CAD_TYPE_NONE

#ifndef CARD_VERSION
#define CARD_VERSION 0x00
#endif

#endif // ! CONFIG_KEYS_DEFINES_DEFINED

ARDRONE_CONFIG_KEY_IMM("GENERAL", num_version_config,  	 INI_INT,      int32_t,    int32_t*,    K_READ|K_NOBIND, CURRENT_NUM_VERSION_CONFIG,         default_config_callback)
ARDRONE_CONFIG_KEY_IMM("GENERAL", num_version_mb,  		 INI_INT,      int32_t,    int32_t*,    K_READ|K_NOBIND, CARD_VERSION,        								default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", num_version_soft,    	 INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, CURRENT_NUM_VERSION_SOFT,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", soft_build_date,  	 INI_STRING,   string_t,   char*,       K_READ|K_NOBIND, 				CURRENT_BUILD_DATE,                 default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", motor1_soft,    		 INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, default_motor_version,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", motor1_hard,    		 INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, default_motor_version,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", motor1_supplier, 		 INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, default_motor_version,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", motor2_soft,    		 INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, default_motor_version,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", motor2_hard,    		 INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, default_motor_version,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", motor2_supplier, 		 INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, default_motor_version,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", motor3_soft,    		 INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, default_motor_version,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", motor3_hard,    		 INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, default_motor_version,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", motor3_supplier, 		 INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, default_motor_version,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", motor4_soft,    		 INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, default_motor_version,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", motor4_hard,    		 INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, default_motor_version,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", motor4_supplier, 	   INI_STRING,   string_t,   char*,    		K_READ|K_NOBIND, default_motor_version,           default_config_callback)
ARDRONE_CONFIG_KEY_STR("GENERAL", ardrone_name,        INI_STRING,   string_t,   char*,       K_READ|K_WRITE, "My ARDrone",                      default_config_callback)
ARDRONE_CONFIG_KEY_IMM("GENERAL", flying_time,         INI_INT,   uint32_t,   uint32_t*,      K_READ, 0,                      default_config_callback)
ARDRONE_CONFIG_KEY_IMM("GENERAL", navdata_demo,        INI_BOOLEAN,  bool_t,     bool_t*,     K_READ|K_WRITE, FALSE,                             navdata_demo_config_callback)
ARDRONE_CONFIG_KEY_IMM("GENERAL", com_watchdog,        INI_INT,      int32_t,    int32_t*,    K_READ|K_WRITE, COM_INPUT_LANDING_TIME,            default_config_callback)
ARDRONE_CONFIG_KEY_IMM("GENERAL", video_enable,        INI_BOOLEAN,  bool_t,     bool_t*,     K_READ|K_WRITE, TRUE,                              default_config_callback)
ARDRONE_CONFIG_KEY_IMM("GENERAL", vision_enable,       INI_BOOLEAN,  bool_t,     bool_t*,     K_READ|K_WRITE, TRUE,                              default_config_callback)
ARDRONE_CONFIG_KEY_IMM("GENERAL", vbat_min,            INI_INT,      int32_t,    int32_t*,    K_READ|K_NOBIND, VBAT_POWERING_OFF,                 default_config_callback)

ARDRONE_CONFIG_KEY_REF("CONTROL", accs_offset,         INI_VECTOR,   vector31_t, vector31_t*, K_READ|K_NOBIND, default_accs_offset,               default_config_callback)
ARDRONE_CONFIG_KEY_REF("CONTROL", accs_gains,          INI_MATRIX,   matrix33_t, matrix33_t*, K_READ|K_NOBIND, default_accs_gain,                 default_config_callback)
ARDRONE_CONFIG_KEY_REF("CONTROL", gyros_offset,        INI_VECTOR,   vector31_t, vector31_t*, K_READ|K_NOBIND, default_gyros_offset,              default_config_callback)
ARDRONE_CONFIG_KEY_REF("CONTROL", gyros_gains,         INI_VECTOR,   vector31_t, vector31_t*, K_READ|K_NOBIND, default_gyros_gains,               default_config_callback)
ARDRONE_CONFIG_KEY_REF("CONTROL", gyros110_offset,     INI_VECTOR21,   vector21_t, vector21_t*, K_READ|K_NOBIND, default_gyros110_offset,              default_config_callback)
ARDRONE_CONFIG_KEY_REF("CONTROL", gyros110_gains,      INI_VECTOR21,   vector21_t, vector21_t*, K_READ|K_NOBIND, default_gyros110_gains,               default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", gyro_offset_thr_x,   INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_NOBIND, default_gyro_offset_thr_x,         default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", gyro_offset_thr_y,   INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_NOBIND, default_gyro_offset_thr_y,         default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", gyro_offset_thr_z,   INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_NOBIND, default_gyro_offset_thr_z,         default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", pwm_ref_gyros,       INI_INT,      int32_t,    int32_t*,    K_READ|K_NOBIND, default_pwm_ref_gyro,              default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", control_level,       INI_INT,      int32_t,    int32_t*,    K_READ|K_WRITE, 0,								 default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", shield_enable,       INI_INT,      int32_t,    int32_t*,    K_READ|K_WRITE, 1,                                 default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", euler_angle_max,     INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_WRITE, default_euler_angle_ref_max,       control_changed_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", altitude_max,        INI_INT,    	 int32_t,    int32_t*,    K_READ|K_WRITE, default_altitude_max,              default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", altitude_min,        INI_INT,    	 int32_t,    int32_t*,    K_READ|K_WRITE, default_altitude_min,              default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", control_trim_z,      INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_WRITE, default_control_trim_z,            default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", control_iphone_tilt, INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_WRITE, default_control_iphone_tilt,       default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", control_vz_max,      INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_WRITE, default_control_vz_max,        	 control_changed_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", control_yaw,         INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_WRITE, default_control_yaw,               control_changed_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", outdoor,	       	   INI_BOOLEAN,  bool_t,     bool_t*,     K_READ|K_WRITE, FALSE,				        	 control_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", flight_without_shell,INI_BOOLEAN,  bool_t,     bool_t*,     K_READ|K_WRITE, FALSE,				             default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", brushless,	       INI_BOOLEAN,  bool_t,     bool_t*,     K_READ|K_WRITE, TRUE,				             	 default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", autonomous_flight,   INI_BOOLEAN,  bool_t,     bool_t*,     K_READ|K_WRITE, FALSE,				             default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", manual_trim,		   INI_BOOLEAN,  bool_t,     bool_t*,     K_READ|K_WRITE, FALSE,				             default_config_callback)

ARDRONE_CONFIG_KEY_IMM("CONTROL", indoor_euler_angle_max,     INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_WRITE, default_euler_angle_ref_max,default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", indoor_control_vz_max,      INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_WRITE, default_control_vz_max,     default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", indoor_control_yaw,         INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_WRITE, default_control_yaw,        default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", outdoor_euler_angle_max,    INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_WRITE, default_outdoor_euler_angle_ref_max, default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", outdoor_control_vz_max,     INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_WRITE, default_outdoor_control_vz_max,      default_config_callback)
ARDRONE_CONFIG_KEY_IMM("CONTROL", outdoor_control_yaw,        INI_FLOAT,    float32_t,  float32_t*,  K_READ|K_WRITE, default_outdoor_control_yaw,         default_config_callback)

ARDRONE_CONFIG_KEY_STR("NETWORK", ssid_single_player,  INI_STRING,   string_t,   char*,       K_READ|K_WRITE, WIFI_NETWORK_NAME,                 default_config_callback)
ARDRONE_CONFIG_KEY_STR("NETWORK", ssid_multi_player,   INI_STRING,   string_t,   char*,       K_READ|K_WRITE, WIFI_NETWORK_NAME,             	 default_config_callback)
ARDRONE_CONFIG_KEY_IMM("NETWORK", infrastructure,      INI_BOOLEAN,  bool_t,     bool_t*,     K_READ|K_WRITE, 1,                                 default_config_callback)
ARDRONE_CONFIG_KEY_IMM("NETWORK", secure,              INI_BOOLEAN,  bool_t,     bool_t*,     K_READ|K_WRITE, 0,                                 default_config_callback)
ARDRONE_CONFIG_KEY_STR("NETWORK", passkey,             INI_STRING,   string_t,   char*,       K_READ|K_WRITE, "",                                default_config_callback)
ARDRONE_CONFIG_KEY_IMM("NETWORK", navdata_port,        INI_INT,      int32_t,    int32_t*,    K_READ|K_NOBIND, NAVDATA_PORT,                      default_config_callback)
ARDRONE_CONFIG_KEY_IMM("NETWORK", video_port,          INI_INT,      int32_t,    int32_t*,    K_READ|K_NOBIND, VIDEO_PORT,                        default_config_callback)
ARDRONE_CONFIG_KEY_IMM("NETWORK", at_port,             INI_INT,      int32_t,    int32_t*,    K_READ|K_NOBIND, AT_PORT,                           default_config_callback)
ARDRONE_CONFIG_KEY_IMM("NETWORK", cmd_port,            INI_INT,      int32_t,    int32_t*,    K_READ|K_NOBIND, 0,                                 default_config_callback)
ARDRONE_CONFIG_KEY_STR("NETWORK", owner_mac,           INI_STRING,   string_t,   char*,       K_READ|K_WRITE, NULL_MAC, 						              owner_mac_callback)
ARDRONE_CONFIG_KEY_IMM("NETWORK", owner_ip_address,    INI_INT,      uint32_t,   uint32_t*,   K_READ|K_WRITE, 0,                                 default_config_callback)
ARDRONE_CONFIG_KEY_IMM("NETWORK", local_ip_address,    INI_INT,      uint32_t,   uint32_t*,   K_READ|K_WRITE, 0,                                 default_config_callback)
ARDRONE_CONFIG_KEY_IMM("NETWORK", broadcast_address,   INI_INT,      uint32_t,   uint32_t*,   K_READ|K_WRITE, 0,                                 default_config_callback)

ARDRONE_CONFIG_KEY_IMM("PIC",     ultrasound_freq,     INI_INT,      int32_t,    int32_t*,    K_READ|K_WRITE, ADC_CMD_SELECT_ULTRASOUND_25Hz,    ultrasound_freq_callback)
ARDRONE_CONFIG_KEY_IMM("PIC",     ultrasound_watchdog, INI_INT,      int32_t,    int32_t*,    K_READ|K_WRITE, 3,                                 default_config_callback)
ARDRONE_CONFIG_KEY_IMM("PIC",     pic_version        , INI_INT,      int32_t,    int32_t*,    K_READ|K_NOBIND, 0x00040030,              default_config_callback)

ARDRONE_CONFIG_KEY_IMM("VIDEO",   camif_fps,           INI_INT,      int32_t,    int32_t*,    K_READ|K_NOBIND, 15,                                default_config_callback)
ARDRONE_CONFIG_KEY_IMM("VIDEO",   camif_buffers,       INI_INT,      int32_t,    int32_t*,    K_READ|K_NOBIND, CAMIF_NUM_BUFFERS,                 default_config_callback)
ARDRONE_CONFIG_KEY_IMM("VIDEO",   num_trackers,        INI_INT,      int32_t,    int32_t*,    K_READ|K_NOBIND, 12,                                default_config_callback)

ARDRONE_CONFIG_KEY_IMM("DETECT",  enemy_colors, 	   INI_INT,      int32_t,    int32_t*,    K_READ|K_WRITE, default_enemy_colors,      		 default_config_callback)
ARDRONE_CONFIG_KEY_IMM("DETECT",  enemy_without_shell, INI_INT,      int32_t,    int32_t*,    K_READ|K_WRITE, 0,      		 					 default_config_callback)
ARDRONE_CONFIG_KEY_IMM("DETECT",  detect_type, 		   INI_INT,      int32_t,    int32_t*,    K_READ|K_WRITE|K_NOBIND, default_detect_type,      		 detect_type_callback)

ARDRONE_CONFIG_KEY_IMM("SYSLOG",  output,              INI_INT,      int32_t,    int32_t*,    K_READ|K_WRITE, UART_PRINT|WIFI_PRINT|FLASH_PRINT, default_config_callback)
ARDRONE_CONFIG_KEY_IMM("SYSLOG",  max_size,            INI_INT,      int32_t,    int32_t*,    K_READ|K_WRITE, 100*1024,                          default_config_callback)
ARDRONE_CONFIG_KEY_IMM("SYSLOG",  nb_files,            INI_INT,      int32_t,    int32_t*,    K_READ|K_WRITE, 5,                                 default_config_callback)
