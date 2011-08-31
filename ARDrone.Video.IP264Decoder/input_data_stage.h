/*
	Written by Max Malmgren.
*/

#ifndef INPUT_DATA_STAGE
#define INPUT_DATA_STAGE
#include <VP_Api\vp_api_stage.h>
#include <VP_Api\vp_api.h>

namespace ARDrone { namespace Video { namespace Decoding {
			struct input_data_cfg
			{
				int8_t *input;
				size_t input_size;
			};

			C_RESULT input_data_stage_open(input_data_cfg *cfg);
			C_RESULT input_data_stage_transform(input_data_cfg *cfg, vp_api_io_data_t *in, vp_api_io_data_t *out);
			C_RESULT input_data_stage_close(input_data_cfg *cfg);

			static vp_api_stage_funcs_t input_stage_funcs = 
			{
					NULL,
					(vp_api_stage_open_t)input_data_stage_open,
					(vp_api_stage_transform_t)input_data_stage_transform,
					(vp_api_stage_close_t)input_data_stage_close
			};


		}
	}
}

#endif