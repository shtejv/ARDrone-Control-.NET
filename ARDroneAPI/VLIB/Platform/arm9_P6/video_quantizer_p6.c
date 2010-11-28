#include "video_quantizer_p6.h"
#include "video_utils_p6.h"
#include "video_config.h"

#ifdef HAS_DO_QUANTIZE_INTRA_MB
#ifdef USE_TABLE_QUANTIZATION
// when quantization is done by P6 hardware, do_quantize_intra_mb just count the number of coefficient and find the last one
int16_t* do_quantize_intra_mb(int16_t* ptr, int32_t invQuant, int32_t* last_ptr)
{
	int32_t i, num_coeff;
	int32_t last;
	uint32_t* ptr32 = (int32_t*)ptr;

	  for( i = 6; i > 0; i-- )
	  {
		last=0;
		ptr = (int16_t*)ptr32;
		if( *ptr == 0 )
		{
	      *ptr = 1;
		}

		num_coeff = MCU_BLOCK_SIZE/4;

	    while( num_coeff > 0 )
	    {
	    	// we load 2 coeff at a time
	    	uint32_t coeff2 = *ptr32++;
	    	num_coeff--;
			if (coeff2 > 0x0000FFFF)
				last++;
			if ((coeff2<<16) > 0x0000FFFF)
				last++;
			// next 2 coeffs (gcc will unroll the loop)
			coeff2 = *ptr32++;
			if (coeff2 > 0x0000FFFF)
				last++;
			if ((coeff2<<16) > 0x0000FFFF)
				last++;
	    }

	    *last_ptr++ = last;
	  }


	  return (int16_t*)ptr32;
}
#else
int16_t* do_quantize_intra_mb(int16_t* ptr, int32_t invQuant, int32_t* last_ptr)
{
	int32_t i, num_coeff;
	int32_t coeff, sign, last;

	  for( i = 6; i > 0; i-- )
	  {
	    // LEVEL = (COF + 4)/(2*4) see III.3.2.3
	    coeff = (((*ptr)<<1) + 4) >> 3;
	    if( coeff == 0 )
	      coeff = 1;
	    *ptr++ = coeff;

	    last = 1;
	    num_coeff = MCU_BLOCK_SIZE-1;

	    while( num_coeff > 0 )
	    {
          if( *ptr++ != 0 )
	      {
	        last++;
	      }
	      num_coeff--;
	    }

	    *last_ptr++ = last;
	  }

	  return ptr;
}
#endif // USE_TABLE_QUANTIZATION
#endif // HAS_DO_QUANTIZE_INTRA_MB
