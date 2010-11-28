#include <time.h>
#ifndef _WIN32
	#include <sys/time.h>
#else
 
 #include <sys/timeb.h>
 #include <Winsock2.h>  // for timeval structure

 int gettimeofday (struct timeval *tp, void *tz)
 {
	 struct _timeb timebuffer;
	 _ftime (&timebuffer);
	 tp->tv_sec = (long)timebuffer.time;
	 tp->tv_usec = (long)timebuffer.millitm * 1000;
	 return 0;
 }
#endif

#include <VP_Os/vp_os_malloc.h>
#include <VP_Api/vp_api_picture.h>

#include <config.h>
#include <ardrone_tool/Video/video_stage_recorder.h>

#ifdef USE_VIDEO_YUV
#define VIDEO_FILE_EXTENSION "yuv"
#else
#define VIDEO_FILE_EXTENSION "y"
#endif

#ifndef VIDEO_FILE_DEFAULT_PATH
#define VIDEO_FILE_DEFAULT_PATH "/data/video"
#endif

#if defined (NAVDATA_VISION_INCLUDED) && defined (USE_ELINUX)
static int32_t picture_captured = 0;
extern void navdata_set_raw_picture(int32_t new_raw_picture);
#endif

const vp_api_stage_funcs_t video_recorder_funcs = {
  (vp_api_stage_handle_msg_t) video_stage_recorder_handle,
  (vp_api_stage_open_t) video_stage_recorder_open,
  (vp_api_stage_transform_t) video_stage_recorder_transform,
  (vp_api_stage_close_t) video_stage_recorder_close
};

char video_filename[VIDEO_FILENAME_LENGTH];

C_RESULT
video_stage_recorder_handle (video_stage_recorder_config_t * cfg, PIPELINE_MSG msg_id, void *callback, void *param)
{
	switch (msg_id)
	{
		case PIPELINE_MSG_START:
			{
				if(cfg->startRec==VIDEO_RECORD_STOP)
					cfg->startRec=VIDEO_RECORD_HOLD;
				else
					cfg->startRec=VIDEO_RECORD_STOP;
			}
			break;
		default:
			break;
	}
	return (VP_SUCCESS);
}

C_RESULT video_stage_recorder_open(video_stage_recorder_config_t *cfg)
{
	cfg->startRec=VIDEO_RECORD_STOP;
  return C_OK;
}

C_RESULT video_stage_recorder_transform(video_stage_recorder_config_t *cfg, vp_api_io_data_t *in, vp_api_io_data_t *out)
{
	 time_t temptime;

  vp_os_mutex_lock( &out->lock );

  if( out->status == VP_API_STATUS_INIT )
  {
    out->numBuffers   = 1;
    out->indexBuffer  = 0;
    out->lineSize     = NULL;
    //out->buffers      = (int8_t **) vp_os_malloc( sizeof(int8_t *) );
  }

  out->size     = in->size;
  out->status   = in->status;
  out->buffers  = in->buffers;

  if( in->status == VP_API_STATUS_ENDED ) {
    out->status = in->status;
  }
  else if(in->status == VP_API_STATUS_STILL_RUNNING) {
    out->status = VP_API_STATUS_PROCESSING;
  }
  else {
    out->status = in->status;
  }

	if(cfg->startRec==VIDEO_RECORD_HOLD)
	{
		struct timeval tv;
		struct tm *atm;

		gettimeofday(&tv,NULL);

		 temptime = (time_t)tv.tv_sec;
		 atm = localtime(&temptime);  //atm = localtime(&tv.tv_sec);

		sprintf(video_filename, "%s/video_%04d%02d%02d_%02d%02d%02d.%s",
				VIDEO_FILE_DEFAULT_PATH,
				atm->tm_year+1900, atm->tm_mon+1, atm->tm_mday,
				atm->tm_hour, atm->tm_min, atm->tm_sec, VIDEO_FILE_EXTENSION);

		cfg->fp = fopen(video_filename, "wb");
                if (cfg->fp == NULL)
                   printf ("error open file %s\n", video_filename);
		cfg->startRec=VIDEO_RECORD_START;
	}

  if( cfg->fp != NULL && out->size > 0 && out->status == VP_API_STATUS_PROCESSING && cfg->startRec==VIDEO_RECORD_START)
  {
    vp_api_picture_t* picture = (vp_api_picture_t *) in->buffers;

#if defined (NAVDATA_VISION_INCLUDED) && defined (USE_ELINUX)
		navdata_set_raw_picture(picture_captured++);
#endif

    fwrite(picture->y_buf, picture->width * picture->height, 1, cfg->fp);
#ifdef USE_VIDEO_YUV
    fwrite(picture->cb_buf, picture->width * picture->height >> 2, 1, cfg->fp);
    fwrite(picture->cr_buf, picture->width * picture->height >> 2, 1, cfg->fp);
#endif
  }
	else
	{
		if(cfg->startRec==VIDEO_RECORD_STOP && cfg->fp !=NULL)
		{
			fclose(cfg->fp);
			cfg->fp=NULL;
		}
	}

  vp_os_mutex_unlock( &out->lock );

  return C_OK;
}

C_RESULT video_stage_recorder_close(video_stage_recorder_config_t *cfg)
{
  if( cfg->fp != NULL )
    fclose( cfg->fp );

  return C_OK;
}
