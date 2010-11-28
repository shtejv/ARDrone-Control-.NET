#ifndef _VIDEO_QUANTIZER_H_
#define _VIDEO_QUANTIZER_H_

#include <VLIB/video_controller.h>

// (quant > 0)&&(quant < 31) => use constant quantization
// quant == 31 				 => use quantization table
#define TABLE_QUANTIZATION 31

// P6 DCT quantization table = 2^15/iquant_tab
/*static const int16_t quant_tab[64]  ={	8192, 4681, 3277, 2521, 2048, 1725, 1489, 1311,
										4681, 3277, 2521, 2048, 1725, 1489, 1311, 1170,
										3277, 2521, 2048, 1725, 1489, 1311, 1170, 1057,
										2521, 2048, 1725, 1489, 1311, 1170, 1057,  964,
										2048, 1725, 1489, 1311, 1170, 1057,  964,  886,
										1725, 1489, 1311, 1170, 1057,  964,  886,  819,
										1489, 1311, 1170, 1057,  964,  886,  819,  762,
										1311, 1170, 1057,  964,  886,  819,  762,  712
									  };*/

static const int16_t quant_tab[64]  ={  10923, 6554, 4681, 3641, 2979, 2521, 2185, 1928,
										 6554, 4681, 3641, 2979, 2521, 2185, 1928, 1725,
										 4681, 3641, 2979, 2521, 2185, 1928, 1725, 1560,
										 3641, 2979, 2521, 2185, 1928, 1725, 1560, 1425,
										 2979, 2521, 2185, 1928, 1725, 1560, 1425, 1311,
										 2521, 2185, 1928, 1725, 1560, 1425, 1311, 1214,
										 2185, 1928, 1725, 1560, 1425, 1311, 1214, 1130,
										 1928, 1725, 1560, 1425, 1311, 1214, 1130, 1057
									 };
// inverse quantization table
/*static const int16_t iquant_tab[64]  ={	 4,  7, 10, 13, 16, 19, 22, 25,
										 7, 10, 13, 16, 19, 22, 25, 28,
										10, 13, 16, 19, 22, 25, 28, 31,
										13, 16, 19, 22, 25, 28, 31, 34,
										16, 19, 22, 25, 28, 31, 34, 37,
										19, 22, 25, 28, 31, 34, 37, 40,
										22, 25, 28, 31, 34, 37, 40, 43,
										25, 28, 31, 34, 37, 40, 43, 46
									  };*/

static const int16_t iquant_tab[64]  ={  3,  5,  7,  9, 11, 13, 15, 17,
										 5,  7,  9, 11, 13, 15, 17, 19,
										 7,  9, 11, 13, 15, 17, 19, 21,
										 9, 11, 13, 15, 17, 19, 21, 23,
										11, 13, 15, 17, 19, 21, 23, 25,
										13, 15, 17, 19, 21, 23, 25, 27,
										15, 17, 19, 21, 23, 25, 27, 29,
										17, 19, 21, 23, 25, 27, 29, 31
									  };

// Utility functions
int16_t* do_quantize_intra_mb(int16_t* ptr, int32_t invQuant, int32_t* last_ptr);
int16_t* do_quantize_inter_mb(int16_t* ptr, int32_t quant, int32_t invQuant, int32_t* last_ptr);
C_RESULT do_unquantize(int16_t* ptr, int32_t picture_type, int32_t quant, int32_t num_coeff);

// Default quantization scheme
C_RESULT video_quantizer_init( video_controller_t* controller );
C_RESULT video_quantizer_update( video_controller_t* controller );
C_RESULT video_quantize( video_controller_t* controller, video_macroblock_t* macroblock, int32_t num_macro_blocks );
C_RESULT video_unquantize( video_controller_t* controller, video_macroblock_t* macroblock, int32_t num_macro_blocks );

#endif // _VIDEO_QUANTIZER_H_
