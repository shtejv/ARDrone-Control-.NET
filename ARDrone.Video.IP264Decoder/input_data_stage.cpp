/*
	Written by Max Malmgren.
*/

#include "input_data_stage.h"

namespace ARDrone { namespace Video { namespace Decoding {

			C_RESULT input_data_stage_open(input_data_cfg *cfg)
			{
				return C_OK;
			}
			C_RESULT input_data_stage_transform(input_data_cfg *cfg, vp_api_io_data_t *in, vp_api_io_data_t *out)
			{
				out->buffers = &cfg->input;
				out->size = cfg->input_size;
				out->numBuffers = 1;
				return C_OK;
			}

			C_RESULT input_data_stage_close(input_data_cfg *cfg)
			{
				return C_OK;
			}
		}
	}
}