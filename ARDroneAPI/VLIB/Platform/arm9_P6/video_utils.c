#include <VLIB/video_controller.h>
#include <VLIB/video_picture.h>

#include <VLIB/Platform/video_config.h>
#include <VLIB/Platform/video_utils.h>

#include <stdio.h>

static uint32_t num_references = 0;

C_RESULT video_utils_init( video_controller_t* controller )
{
  if( num_references == 0 )
  {
    video_dct_p6_init();
  }

  num_references ++;

  return C_OK;
}

C_RESULT video_utils_close( video_controller_t* controller )
{
  if( num_references > 0 )
  {
	video_dct_p6_close();
    num_references --;
  }

  return C_OK;
}

uint32_t ramcode_format_shifter_op_imm(uint32_t imm)
{
  uint32_t shifter, imm8;

  shifter = 32/2;
  imm8    = imm;

  while( (imm8 & 0xFF) != imm8 )
  {
    imm8 >>= 2;
    shifter --;
  }

  if( (imm8 << (32-shifter*2)) != imm )
    return 0xFFFFFFFF;

  if( shifter == 16 )
    shifter = 0;

  return (shifter << 8 | imm8);
}

C_RESULT video_utils_set_format( uint32_t width, uint32_t height )
{
  return C_OK;
}
