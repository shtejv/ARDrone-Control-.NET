/********************************************************************
 *                    COPYRIGHT PARROT 2010
 ********************************************************************
 *       PARROT                MODULES
 *-----------------------------------------------------------------*/
/**
 * @file   ardrone_at.c
 * @brief  .
 *
 * @author K. Leplat <karl.leplat.ext@parrot.com>
 * @date   Mon Feb 1 10:30:50 2010
 *
 *
 *******************************************************************/
#include <VP_Os/vp_os_assert.h>
#include <VP_Os/vp_os_print.h>
#include <ardrone_tool/Com/config_com.h>
#include <ardrone_tool/ardrone_tool.h>

//Common
#include <ardrone_api.h>
#include <at_msgs_ids.h>
#include <config.h>

//SDK
#include <VP_Com/vp_com.h>
#include <VP_Com/vp_com_socket.h>

//GNU STANDARD C LIBRARY
#include <stdio.h>
#include <ctype.h>

/********************************************************************
 * Constants
 *******************************************************************/
#define MAX_BUF_SIZE 256

/********************************************************************
 * Static variables and types
 *******************************************************************/
AT_CODEC_MSG_IDS ids;
static uint32_t at_init = 0;
static uint32_t nb_sequence = 0;
static vp_com_socket_t at_socket;
static Write atcodec_write = NULL;
static Read atcodec_read = NULL;

// Navdata
float32_t nd_iphone_gaz=0;
float32_t nd_iphone_yaw=0;
int32_t nd_iphone_enable=0;
float32_t nd_iphone_phi=0;
float32_t nd_iphone_theta=0;

/********************************************************************
 * Static function declarations
 *******************************************************************/
static void atcodec_init( AT_CODEC_FUNCTIONS_PTRS *funcs );
AT_CODEC_ERROR_CODE atresu_error(ATcodec_Memory_t *mem, ATcodec_Memory_t *output, int *id);
AT_CODEC_ERROR_CODE atresu_ok(ATcodec_Memory_t *mem, ATcodec_Memory_t *output, int *id);
AT_CODEC_ERROR_CODE host_init( void );
AT_CODEC_ERROR_CODE host_shutdown( void );
AT_CODEC_ERROR_CODE host_enable( void );
AT_CODEC_ERROR_CODE host_open( void );
AT_CODEC_ERROR_CODE host_close( void );
AT_CODEC_ERROR_CODE host_write(int8_t *buffer, int32_t *len);
AT_CODEC_ERROR_CODE host_read(int8_t *buffer, int32_t *len);


/********************************************************************
 * Static functions
 *******************************************************************/
AT_CODEC_ERROR_CODE atresu_ok(ATcodec_Memory_t *mem, ATcodec_Memory_t *output, int *id)
{
  return AT_CODEC_GENERAL_OK;
}

AT_CODEC_ERROR_CODE atresu_error(ATcodec_Memory_t *mem, ATcodec_Memory_t *output, int *id)
{
  return AT_CODEC_GENERAL_OK;
}

AT_CODEC_ERROR_CODE host_init( void )
{
# undef ATCODEC_DEFINE_AT_CMD
# define ATCODEC_DEFINE_AT_CMD(ID,Str,From,Cb,Prio) \
    if((ids.ID = ATcodec_Add_Defined_Message(Str)) == -1) \
      { \
        return AT_CODEC_INIT_ERROR; \
      }

# undef ATCODEC_DEFINE_AT_RESU
# define ATCODEC_DEFINE_AT_RESU(ID,Str,From,Cb) \
    if((ids.ID = ATcodec_Add_Hashed_Message(Str,ids.From,Cb,0)) == -1) \
      { \
        return AT_CODEC_INIT_ERROR; \
      }

# include <at_msgs.h>

  return AT_CODEC_INIT_OK;
}

AT_CODEC_ERROR_CODE host_shutdown( void )
{
  ATcodec_Shutdown_Library();

  return AT_CODEC_SHUTDOWN_OK;
}

AT_CODEC_ERROR_CODE host_enable( void )
{
   /* Only used with ARDrone */
   return AT_CODEC_ENABLE_OK;
}

AT_CODEC_ERROR_CODE host_open( void )
{
	static bool_t init_ok = FALSE;

	if( !init_ok )
	{
		COM_CONFIG_SOCKET_AT(&at_socket, VP_COM_CLIENT, AT_PORT, wifi_ardrone_ip);
		at_socket.protocol = VP_COM_UDP;

		if(VP_FAILED(vp_com_init(COM_AT())))
		{
         PRINT ("Failed to init AT\n");
			vp_com_shutdown( COM_AT() );
			return AT_CODEC_OPEN_ERROR;
		}

      if(VP_FAILED(vp_com_open(COM_AT(), &at_socket, &atcodec_read, &atcodec_write)))
		{
         PRINT ("Failed to open AT\n");
			return AT_CODEC_OPEN_ERROR;
		}
       
		init_ok = TRUE;
	}

	return AT_CODEC_OPEN_OK;
}

AT_CODEC_ERROR_CODE host_close( void )
{
  vp_com_close(COM_AT(), &at_socket);

  return AT_CODEC_CLOSE_OK;
}

AT_CODEC_ERROR_CODE host_write(int8_t *buffer, int32_t *len)
{
  if( atcodec_write != NULL )
  {
    return VP_FAILED(atcodec_write(&at_socket, buffer, len)) ? AT_CODEC_WRITE_ERROR : AT_CODEC_WRITE_OK;
  }

  return AT_CODEC_WRITE_OK;
}

AT_CODEC_ERROR_CODE host_read(int8_t *buffer, int32_t *len)
{
  return AT_CODEC_READ_OK;
}

static void atcodec_init( AT_CODEC_FUNCTIONS_PTRS *funcs )
{
   if( funcs != NULL)
   {
      ATcodec_Init_Library( funcs );
   }
   else
	{

#if defined (_MSC_VER)

		AT_CODEC_FUNCTIONS_PTRS ptrs =
		{
			/*init*/host_init,
			/*shutdown*/host_shutdown,
			/*enable*/host_enable,
			/*open*/host_open,
			/*close*/host_close,
			/*write*/host_write,
			/*read*/host_read
		};
#else
		AT_CODEC_FUNCTIONS_PTRS ptrs =
		{
			.init     = host_init,
			.shutdown = host_shutdown,
			.enable   = host_enable,
			.open     = host_open,
			.close    = host_close,
			.read     = host_read,
			.write    = host_write,
		};
#endif
		ATcodec_Init_Library( &ptrs );
	}
}

/********************************************************************
 * Public functions
 *******************************************************************/
void ardrone_at_set_ui_value( uint32_t value )
{
  if (!at_init)
     return;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_RC_REF_EXE,
                                ++nb_sequence,
                                value );
}

void ardrone_at_set_pmode( int32_t pmode )
{
  if (!at_init)
     return;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_PMODE_EXE,
                                ++nb_sequence,
                                pmode );
}

void ardrone_at_set_ui_misc(int32_t m1, int32_t m2, int32_t m3, int32_t m4)
{
  if (!at_init)
     return;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_MISC_EXE, 
                                ++nb_sequence,
                                m1, 
                                m2, 
                                m3, 
                                m4 );
}

void ardrone_at_set_anim( int32_t type, int32_t timeout )
{
  if (!at_init)
     return;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_ANIM_EXE,
                                ++nb_sequence,
                                type,
                                timeout );
}

void ardrone_at_set_flat_trim(void)
{
  if (!at_init)
     return;

	ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_FTRIM_EXE,
                                 ++nb_sequence );
}

void ardrone_at_set_manual_trims(float32_t trim_pitch, float32_t trim_roll, float32_t trim_yaw)
{
  float_or_int_t _trim_pitch, _trim_roll, _trim_yaw;
  if (!at_init)
     return;

  _trim_pitch.f = trim_pitch;
  _trim_roll.f = trim_roll;
  _trim_yaw.f = trim_yaw;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_MTRIM_EXE, 
                                ++nb_sequence,
                                _trim_pitch.i, 
                                _trim_roll.i, 
                                _trim_yaw.i);
}

void ardrone_at_set_control_gains( api_control_gains_t* gains )
{
  if (!at_init)
     return;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_GAIN_EXE,
                                ++nb_sequence,
                                gains->pq_kp, gains->r_kp, gains->r_ki, gains->ea_kp, gains->ea_ki,
                                gains->alt_kp, gains->alt_ki, gains->vz_kp, gains->vz_ki,
                                gains->hovering_kp, gains->hovering_ki );
}

void ardrone_at_set_vision_track_params( api_vision_tracker_params_t* params )
{
  if (!at_init)
     return;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_VISP_EXE,
                                ++nb_sequence,
                                params->coarse_scale,
                                params->nb_pair,
                                params->loss_per,
                                params->nb_tracker_width,
                                params->nb_tracker_height,
                                params->scale,
                                params->trans_max,
                                params->max_pair_dist,
                                params->noise );
}

void ardrone_at_start_raw_capture(void)
{
  if (!at_init)
     return;

  ATcodec_Queue_Message_valist(ids.AT_MSG_ATCMD_RAWC_EXE,
                               ++nb_sequence);
}

void ardrone_at_zap( ZAP_VIDEO_CHANNEL channel )
{
  if (!at_init)
     return;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_ZAP_EXE,
                                ++nb_sequence,
                                channel );
}

void ardrone_at_cad( CAD_TYPE type, float32_t tag_size )
{
  float_or_int_t size;
  size.f = tag_size;

  if (!at_init)
     return;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_CAD_EXE, 
                                ++nb_sequence,
                                type, 
                                size.i);
}

/********************************************************************
 * ardrone_at_set_progress_cmd: 
 *-----------------------------------------------------------------*/
/**
 * @param enable 1,with pitch,roll and 0,without pitch,roll.
 * @param pitch Using floating value between -1 to +1. 
 * @param roll Using floating value between -1 to +1.
 * @param gaz Using floating value between -1 to +1.
 * @param yaw Using floating value between -1 to +1.
 *
 * @brief
 *
 * @DESCRIPTION
 *
 *******************************************************************/
void ardrone_at_set_progress_cmd( int32_t enable, float32_t phi, float32_t theta, float32_t gaz, float32_t yaw )
{
   float_or_int_t _phi, _theta, _gaz, _yaw;

	if (!at_init)
		return;
   
   _phi.f = phi;
   _theta.f = theta;
   _gaz.f = gaz;
   _yaw.f = yaw;

	// Saving values to set them in navdata_file
   nd_iphone_enable=enable;
	nd_iphone_phi=phi;
	nd_iphone_theta=theta;
	nd_iphone_gaz=gaz;
	nd_iphone_yaw=yaw;

	ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_PCMD_EXE, 
                                 ++nb_sequence,
                                 enable, 
                                 _phi.i, 
                                 _theta.i, 
                                 _gaz.i, 
                                 _yaw.i );
}

void ardrone_at_set_led_animation ( LED_ANIMATION_IDS anim_id, float32_t freq, uint32_t duration_sec )
{
	float_or_int_t _freq;

	if (!at_init)
     return;
	
	_freq.f = freq;
	
	ATcodec_Queue_Message_valist(ids.AT_MSG_ATCMD_LED_EXE,
                                ++nb_sequence,
                                anim_id, 
                                _freq.i, 
                                duration_sec);
}

void ardrone_at_set_vision_update_options(int32_t user_vision_option)
{
  if (!at_init)
     return;

  ATcodec_Queue_Message_valist(ids.AT_MSG_ATCMD_VISO_EXE, 
                               ++nb_sequence,
                               user_vision_option);
}

/********************************************************************
 * ardrone_at_set_polaris_pos:
 *-----------------------------------------------------------------*/
void ardrone_at_set_polaris_pos( float32_t fx, float32_t fy, float32_t fpsi, bool_t defined, int32_t time_us )
{
  float_or_int_t x, y, psi;

  if (!at_init)
     return;

  x.f   = fx;
  y.f   = fy;
  psi.f = fpsi;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_POLARIS_EXE,
                                ++nb_sequence,
                                x.i, 
                                y.i, 
                                psi.i, 
                                defined, 
                                time_us );
}

/********************************************************************
 * ardrone_at_set_toy_configuration:
 *-----------------------------------------------------------------*/
/**
 * @param param A key as read from an ini file is given as "section:key".
 *
 * @param value
 *
 * @brief .
 *
 * @DESCRIPTION
 *
 *******************************************************************/
void ardrone_at_set_toy_configuration(const char* param, const char* value)
{
  if (!at_init)
     return;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_CONFIG_EXE,
                                ++nb_sequence,
                                param, 
                                value );
}

/********************************************************************
 * ardrone_at_reset_com_watchdog:
 *-----------------------------------------------------------------*/
/**
 * @brief Re-connect with the ARDrone.
 *
 * @DESCRIPTION
 *
 *******************************************************************/
void ardrone_at_reset_com_watchdog(void)
{
  if (!at_init)
     return;
  
  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_RESET_COM_WATCHDOG,
                                ++nb_sequence );
}

/********************************************************************
 * ardrone_at_update_control_mode:
 *-----------------------------------------------------------------*/
/**
 * @param what_to_do
 *
 * @param filesize
 *
 * @brief .
 *
 * @DESCRIPTION
 *
 *******************************************************************/
void ardrone_at_update_control_mode(uint32_t what_to_do, uint32_t filesize)
{
  if (!at_init)
     return;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_CTRL_EXE,
                                ++nb_sequence,
                                what_to_do, 
                                filesize );
}

/********************************************************************
 * ardrone_at_configuration_get_ctrl_mode:
 *-----------------------------------------------------------------*/
/**
 * @brief Request to receive configuration file.
 *
 * @DESCRIPTION
 *
 *******************************************************************/
void ardrone_at_configuration_get_ctrl_mode(void)
{
  if (!at_init)
     return;
  
 ardrone_at_update_control_mode( CFG_GET_CONTROL_MODE, 0 ); 
}

/********************************************************************
 * ardrone_at_configuration_ack_ctrl_mode:
 *-----------------------------------------------------------------*/
/**
 * @brief Signal to the drone that the file has been received.
 *
 * @DESCRIPTION
 *
 *******************************************************************/
void ardrone_at_configuration_ack_ctrl_mode(void)
{
  if (!at_init)
     return;
 
  ardrone_at_update_control_mode( ACK_CONTROL_MODE, 0 );
}

/********************************************************************
 * ardrone_at_set_pwm: .
 *-----------------------------------------------------------------*/
/**
 * @param p1
 *
 * @param p2
 *
 * @param p3
 *
 * @param p4
 *
 * @brief .
 *
 * @DESCRIPTION
 *
 *******************************************************************/
void ardrone_at_set_pwm(int32_t p1, int32_t p2, int32_t p3, int32_t p4)
{
  if (!at_init)
     return;

  ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_PWM_EXE, 
                                ++nb_sequence,
                                p1,  
                                p2, 
                                p3, 
                                p4 );
}

void ardrone_at_set_autonomous_flight( int32_t isActive )
{
  if (!at_init)
     return;

   ATcodec_Queue_Message_valist( ids.AT_MSG_ATCMD_AUTONOMOUS_FLIGHT_EXE,
                                ++nb_sequence,
                                isActive );
}

/********************************************************************
 * ardrone_at_set_autonomous_flight: Enables / disables the autopilot.
 *-----------------------------------------------------------------*/
/**
 * @param isActive Integer set to 1 to enable the autopilot.
 *
 * @brief Enables / disables the autopilot.
 *
 * @DESCRIPTION
 *
 *******************************************************************/
static inline void strtolower(char *str)
{
	int i;
	for(i = 0 ; str[i] != '\0' ; i++)
		str[i] = (char)tolower((int)str[i]);
}

#undef ARDRONE_CONFIG_KEY_IMM
#undef ARDRONE_CONFIG_KEY_REF
#undef ARDRONE_CONFIG_KEY_STR
#define ARDRONE_CONFIG_KEY_IMM(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) 	\
ARDRONE_CONFIGURATION_PROTOTYPE(C_TYPE,NAME)													\
{																								\
	C_RESULT res = C_FAIL;																		\
	if(((RW) & K_WRITE) == K_WRITE)																\
	{																							\
		char msg[64];																			\
		if(strcmp("" #INI_TYPE, "INI_FLOAT") == 0)												\
		{																						\
			sprintf( &msg[0], "%f", (float)NAME);												\
			res = C_OK;																			\
		}																						\
		else if(strcmp("" #INI_TYPE, "INI_INT") == 0)											\
		{																						\
			sprintf( &msg[0], "%d", (int)NAME);													\
			res = C_OK;																			\
		}																						\
		else if(strcmp("" #INI_TYPE, "INI_BOOLEAN") == 0)										\
		{																						\
			sprintf( &msg[0], (NAME) ? "TRUE" : "FALSE");										\
			res = C_OK;																			\
		}																						\
		if(res == C_OK)																			\
		{																						\
			char tmp[64];																		\
			strcpy(&tmp[0], KEY);																\
			strtolower(&tmp[0]);																\
			strcat(tmp, ":" #NAME);																\
			ardrone_at_set_toy_configuration( &tmp[0], &msg[0] );								\
		}																						\
	}																							\
	return res;																					\
}

#define ARDRONE_CONFIG_KEY_REF(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)

#define ARDRONE_CONFIG_KEY_STR(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)  \
ARDRONE_CONFIGURATION_PROTOTYPE(C_TYPE_PTR,NAME)												\
{																								\
	C_RESULT res = C_FAIL; 																		\
	if((NAME != NULL) && (((RW) & K_WRITE) == K_WRITE))											\
	{																							\
		char tmp[64];																			\
		strcpy(&tmp[0], KEY);																	\
		strtolower(&tmp[0]);																	\
		strcat(&tmp[0], ":" #NAME);																\
		ardrone_at_set_toy_configuration( &tmp[0], &NAME[0] );									\
		res = C_OK;																				\
	}																							\
	return res;																					\
}

#include <config_keys.h> // must be included before to have types

/********************************************************************
 * ardrone_at_init_with_funcs: Init at command with ATCodec funcs
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
void ardrone_at_init_with_funcs ( const char* ip, size_t ip_len, AT_CODEC_FUNCTIONS_PTRS *funcs)
{
   if ( at_init )
      return;

   VP_OS_ASSERT( ip_len < MAX_BUF_SIZE );

   vp_os_memcpy( &wifi_ardrone_ip[0], ip, ip_len);
   wifi_ardrone_ip[ip_len]='\0';

   atcodec_init (funcs);

   at_init = 1;
}

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
void ardrone_at_init ( const char* ip, size_t ip_len)
{
   if ( at_init )
      return;

   VP_OS_ASSERT( ip_len < MAX_BUF_SIZE );

   if(wifi_ardrone_ip!=ip){
	   vp_os_memcpy( &wifi_ardrone_ip[0], ip, ip_len);
   }
   wifi_ardrone_ip[ip_len]='\0';
   
   atcodec_init (NULL);

   at_init = 1;
}

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
ATCODEC_RET ardrone_at_open ( void )
{
   if ( !at_init )
      return ATCODEC_FALSE;

   return host_open()==AT_CODEC_OPEN_OK?ATCODEC_TRUE:ATCODEC_FALSE;
}

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
ATCODEC_RET ardrone_at_send ( void )
{
	C_RESULT res;

    if ( !at_init )
      return ATCODEC_FALSE;

	res = ATcodec_Send_Messages();

   return res;
}

