/*
	Written by Max Malmgren.
*/

#ifndef VIDEO_DECODER_INTERNAL_H
#define VIDEO_DECODER_INTERNAL_H
#include <VLIB\Stages\vlib_stage_decode.h>
#include <VP_SDK\VP_Stages\vp_stages_yuv2rgb.h>
#include <VP_Api\vp_api_stage.h>
#include "input_data_stage.h"

namespace ARDrone { namespace Video { namespace Decoding { namespace Internal {

		   class VideoDecoder
			{
			public:
				VideoDecoder(int max_height, int max_width);
				~VideoDecoder();
				/*
					Decodes an P264 or P263 encoded picture.
					The result is set into the out parameter.
					The result buffer is valid while the VideoDecoder is not deallocated.
				*/
				size_t Transform(uint8_t *in, size_t in_size, uint8_t **out, int *bytes_per_pixel, int *width, int *height);
			private:
				PIPELINE_HANDLE pipeline_handle;
				vp_api_io_pipeline_t pipeline;
				vp_api_picture_t picture;
				vp_api_io_stage_t stages[3];
				vlib_stage_decoding_config_t vec;
				vp_stages_yuv2rgb_config_t yuv2rgbconf;
				input_data_cfg input_cfg;
			};

			}
		}
	}
}
#endif