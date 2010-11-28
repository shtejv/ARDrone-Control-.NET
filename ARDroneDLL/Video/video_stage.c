/********************************************************************
 *                    COPYRIGHT PARROT 2010
 ********************************************************************
 *       PARROT - A.R.Drone SDK Windows Client Example
 *-----------------------------------------------------------------*/
/**
 * @file video_stage.c 
 * @brief Video stream reception code
 *
 * @author marc-olivier.dzeukou@parrot.com
 * @date 2007/07/27
 *
 * @author Stephane Piskorski <stephane.piskorski.ext@parrot.fr>
 * @date   Sept, 8. 2010
 *
 *******************************************************************/


#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>


#include <time.h>

/* A.R.Drone OS dependant includes */
	#include <config.h>
	#include <VP_Os/vp_os_print.h>
	#include <VP_Os/vp_os_malloc.h>
	#include <VP_Os/vp_os_delay.h>

/* A.R.Drone Video API includes */
	#include <VP_Api/vp_api.h>
	#include <VP_Api/vp_api_error.h>
	#include <VP_Api/vp_api_stage.h>
	#include <VP_Api/vp_api_picture.h>
	#include <VP_Stages/vp_stages_io_file.h>
	#include <VP_Stages/vp_stages_i_camif.h>
	#include <VLIB/Stages/vlib_stage_decode.h>
	#include <VP_Stages/vp_stages_yuv2rgb.h>
	#include <VP_Stages/vp_stages_buffer_to_picture.h>

/* A.R.Drone Tool includes */
	#include <ardrone_tool/ardrone_tool.h>
	#include <ardrone_tool/Com/config_com.h>
	#include <ardrone_tool/UI/ardrone_input.h>
	#include <ardrone_tool/Video/video_com_stage.h>

/* Configuration file */
	#include <win32_custom.h>

/* Our local pipeline */
	#include "Video/video_stage.h"
	
#include <UI/directx_rendering.h>

/* Global variables to build our video pipeline*/
	#define NB_STAGES 10
	PIPELINE_HANDLE pipeline_handle;
	static uint8_t*  pixbuf_data       = NULL;
	static vp_os_mutex_t  video_update_lock;





uint8_t* _stdcall GetCurrentImage() {
	//printf("Sent pixbuf data \n");
	return pixbuf_data;
}



/*****************************************************************************/
/*
\brief Initialization of the video rendering stage.
*/
C_RESULT output_rendering_device_stage_open( void *cfg, vp_api_io_data_t *in, vp_api_io_data_t *out)
{
	vp_os_mutex_init(&video_update_lock);
	return (VP_SUCCESS);
}

extern uint8_t * FrameBuffer;

/*****************************************************************************/
/*
\brief Video rendering function (called for each received frame from the drone).
*/

 
C_RESULT output_rendering_device_stage_transform( void *cfg, vp_api_io_data_t *in, vp_api_io_data_t *out)
{
  vlib_stage_decoding_config_t* vec = (vlib_stage_decoding_config_t*)cfg;

  vp_os_mutex_lock(&video_update_lock);
 
  /* Get a reference to the last decoded picture */
  pixbuf_data      = (uint8_t*)in->buffers[0];

  vp_os_mutex_unlock(&video_update_lock);
  return (VP_SUCCESS);
}




/*****************************************************************************/
/*
\brief Video rendering function (called for each received frame from the drone).
*/
C_RESULT output_rendering_device_stage_close( void *cfg, vp_api_io_data_t *in, vp_api_io_data_t *out)
{
  return (VP_SUCCESS);
}


/*****************************************************************************/
/*
	List of the functions that define the rendering stage.
*/
const vp_api_stage_funcs_t vp_stages_output_rendering_device_funcs =
{
  NULL,
  (vp_api_stage_open_t)output_rendering_device_stage_open,
  (vp_api_stage_transform_t)output_rendering_device_stage_transform,
  (vp_api_stage_close_t)output_rendering_device_stage_close
};




/*****************************************************************************/
/*
	The video processing thread.
	This function can be kept as it is by most users.
	It automatically receives the video stream in a loop, decode it, and then 
		call the 'output_rendering_device_stage_transform' function for each decoded frame.
*/
DEFINE_THREAD_ROUTINE(video_stage, data)
{
  C_RESULT res;

  vp_api_io_pipeline_t    pipeline;
  vp_api_io_data_t        out;
  vp_api_io_stage_t       stages[NB_STAGES];

  vp_api_picture_t picture;

  video_com_config_t              icc;
  vlib_stage_decoding_config_t    vec;
  vp_stages_yuv2rgb_config_t      yuv2rgbconf;
  

  /* Picture configuration */
	  picture.format        = PIX_FMT_YUV420P;

	  picture.width         = DRONE_VIDEO_MAX_WIDTH;
	  picture.height        = DRONE_VIDEO_MAX_HEIGHT;
	  picture.framerate     = 15;

	  picture.y_buf   = vp_os_malloc( DRONE_VIDEO_MAX_WIDTH * DRONE_VIDEO_MAX_HEIGHT     );
	  picture.cr_buf  = vp_os_malloc( DRONE_VIDEO_MAX_WIDTH * DRONE_VIDEO_MAX_HEIGHT / 4 );
	  picture.cb_buf  = vp_os_malloc( DRONE_VIDEO_MAX_WIDTH * DRONE_VIDEO_MAX_HEIGHT / 4 );

	  picture.y_line_size   = DRONE_VIDEO_MAX_WIDTH;
	  picture.cb_line_size  = DRONE_VIDEO_MAX_WIDTH / 2;
	  picture.cr_line_size  = DRONE_VIDEO_MAX_WIDTH / 2;

	  vp_os_memset(&icc,          0, sizeof( icc ));
	  vp_os_memset(&vec,          0, sizeof( vec ));
	  vp_os_memset(&yuv2rgbconf,  0, sizeof( yuv2rgbconf ));

   /* Video socket configuration */
	  icc.com                 = COM_VIDEO();
	  icc.buffer_size         = 100000;
	  icc.protocol            = VP_COM_UDP;
  
	  COM_CONFIG_SOCKET_VIDEO(&icc.socket, VP_COM_CLIENT, VIDEO_PORT, wifi_ardrone_ip);

  /* Video decoder configuration */
	  /* Size of the buffers used for decoding 
         This must be set to the maximum possible video resolution used by the drone 
         The actual video resolution will be stored by the decoder in vec.controller 
		 (see vlib_stage_decode.h) */
	  vec.width               = DRONE_VIDEO_MAX_WIDTH;
	  vec.height              = DRONE_VIDEO_MAX_HEIGHT;
	  vec.picture             = &picture;
	  vec.block_mode_enable   = TRUE;
	  vec.luma_only           = FALSE;

  yuv2rgbconf.rgb_format = VP_STAGES_RGB_FORMAT_RGB24;

   /* Video pipeline building */
  
		 pipeline.nb_stages = 0;

		/* Video stream reception */
		stages[pipeline.nb_stages].type    = VP_API_INPUT_SOCKET;
		stages[pipeline.nb_stages].cfg     = (void *)&icc;
		stages[pipeline.nb_stages].funcs   = video_com_funcs;

		pipeline.nb_stages++;

		/* Video stream decoding */
		stages[pipeline.nb_stages].type    = VP_API_FILTER_DECODER;
		stages[pipeline.nb_stages].cfg     = (void*)&vec;
		stages[pipeline.nb_stages].funcs   = vlib_decoding_funcs;

		pipeline.nb_stages++;

		/* YUV to RGB conversion 
		YUV format is used by the video stream protocol
		Remove this stage if your rendering device can handle 
		YUV data directly
		*/
		stages[pipeline.nb_stages].type    = VP_API_FILTER_YUV2RGB;
		stages[pipeline.nb_stages].cfg     = (void*)&yuv2rgbconf;
		stages[pipeline.nb_stages].funcs   = vp_stages_yuv2rgb_funcs;

		pipeline.nb_stages++;

		/* User code */  
		stages[pipeline.nb_stages].type    = VP_API_OUTPUT_SDL;  /* Set to VP_API_OUTPUT_SDL even if SDL is not used */
		stages[pipeline.nb_stages].cfg     = (void*)&vec;   /* give the decoder information to the renderer */
		stages[pipeline.nb_stages].funcs   = vp_stages_output_rendering_device_funcs;

		  pipeline.nb_stages++;
		  pipeline.stages = &stages[0];
 
 
		  /* Processing of a pipeline */
			  if( !ardrone_tool_exit() )
			  {
				PRINT("\n   Video stage thread initialisation\n\n");

				res = vp_api_open(&pipeline, &pipeline_handle);

				if( VP_SUCCEEDED(res) )
				{
				  int loop = VP_SUCCESS;
				  out.status = VP_API_STATUS_PROCESSING;

				  while( !ardrone_tool_exit() && (loop == VP_SUCCESS) )
				  {
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

  PRINT("   Video stage thread ended\n\n");

  return (THREAD_RET)0;
}

