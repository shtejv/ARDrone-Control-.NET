/********************************************************************
 *                    COPYRIGHT PARROT 2010
 ********************************************************************
 *       PARROT - A.R.Drone SDK Windows Client Example
 *-----------------------------------------------------------------*/

#ifndef _VIDEO_STAGE_H
#define _VIDEO_STAGE_H

#include <config.h>
#include <VP_Api/vp_api_thread_helper.h>

/* Declares the thread in charge of reading
the video socket and decoding the video stream. */
PROTO_THREAD_ROUTINE(video_stage, data);

#endif
