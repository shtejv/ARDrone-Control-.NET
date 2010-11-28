#ifndef _VIDEO_STAGE_RECORDER_H_
#define _VIDEO_STAGE_RECORDER_H_

#include <stdio.h>
#include <VP_Api/vp_api.h>

#define VIDEO_FILENAME_LENGTH 256

typedef enum
{
	VIDEO_RECORD_HOLD, // Video recording is on hold, waiting for the start command. This is the default state.
	VIDEO_RECORD_START, // Video recording has started.
	VIDEO_RECORD_STOP		// Video recording has been stopped. Stage will end and restart.
} video_record_state;

typedef struct _video_stage_recorder_config_t
{
  FILE* fp;
	video_record_state startRec;
} video_stage_recorder_config_t;

C_RESULT video_stage_recorder_handle (video_stage_recorder_config_t * cfg, PIPELINE_MSG msg_id, void *callback, void *param);
C_RESULT video_stage_recorder_open(video_stage_recorder_config_t *cfg);
C_RESULT video_stage_recorder_transform(video_stage_recorder_config_t *cfg, vp_api_io_data_t *in, vp_api_io_data_t *out);
C_RESULT video_stage_recorder_close(video_stage_recorder_config_t *cfg);

extern const vp_api_stage_funcs_t video_recorder_funcs;

#endif // _VIDEO_STAGE_RECORDER_H_
