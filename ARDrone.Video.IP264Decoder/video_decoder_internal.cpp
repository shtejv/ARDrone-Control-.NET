/*
	Written by Max Malmgren.
*/

#include "decoder.h"
#include <VP_Os\vp_os_malloc.h>
#include <VLIB\Stages\vlib_stage_decode.h>
#include <VP_SDK\VP_Stages\vp_stages_yuv2rgb.h>

namespace ARDrone { namespace Video { namespace Decoding { namespace Internal {

				VideoDecoder::VideoDecoder(int maxWidth, int maxHeight) : pipeline(), picture(), vec(), stages(), yuv2rgbconf()
				{
						/* Picture configuration */
					picture.format        = PIX_FMT_YUV420P;
					picture.width         = maxWidth ;
					picture.height        = maxHeight;
					picture.framerate     = 15;

					picture.y_buf   = new uint8_t[maxWidth  * maxHeight];
					picture.cr_buf  = new uint8_t[maxWidth * maxHeight / 4];
					picture.cb_buf  = new uint8_t[maxWidth * maxHeight / 4];

					picture.y_line_size   = maxWidth ;
					picture.cb_line_size  = maxWidth  / 2;
					picture.cr_line_size  = maxWidth  / 2;

					/* Video decoder configuration */
						/* Size of the buffers used for decoding 
							This must be set to the maximum possible video resolution used by the drone 
							The actual video resolution will be stored by the decoder in vec.controller 
							(see vlib_stage_decode.h) */
					vec.width               = maxWidth;
					vec.height              = maxHeight;
					vec.picture             = &picture;
					vec.block_mode_enable   = TRUE;
					vec.luma_only           = FALSE;

					yuv2rgbconf.rgb_format = VP_STAGES_RGB_FORMAT_RGB24;

					/* Data input from byte array. At each transform it is sent in input_cfg. */
					stages[pipeline.nb_stages].type    = VP_API_FILTER_DECODER; // NOT USED
					stages[pipeline.nb_stages].cfg     = (void*)&input_cfg;
					stages[pipeline.nb_stages].funcs   = input_stage_funcs;

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

					pipeline.stages = stages;

					int res = vp_api_open(&pipeline, &pipeline_handle);

					if(!VP_SUCCEEDED(res))
						throw gcnew DecodingException("Failed to initialize decoding pipeline.");
				}

				VideoDecoder::~VideoDecoder()
				{
					vp_api_close(&pipeline, &pipeline_handle);
					delete[] picture.y_buf;
					delete[] picture.cr_buf;
					delete[] picture.cb_buf;
				}

				size_t VideoDecoder::Transform(uint8_t *in, size_t in_size, uint8_t **out, int *bytes_per_pixel, int *width, int *height)
				{
					 input_cfg.input = (int8_t*)in;
					 input_cfg.input_size = in_size;
					 vp_api_io_data_t api_out;
					 if(!VP_SUCCEEDED(vp_api_run(&pipeline, &api_out)))
						 throw gcnew DecodingException("Failed to transform data.");
					 *bytes_per_pixel = 3;
					 *width = vec.controller.width;
					 *height = vec.controller.height;
					 *out = (uint8_t*)api_out.buffers[0];
					 return api_out.size;
				}
			}
		}
	}
}