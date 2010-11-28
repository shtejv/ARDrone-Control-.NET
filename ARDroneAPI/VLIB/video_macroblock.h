#ifndef _VIDEO_MACROBLOCK_H_
#define _VIDEO_MACROBLOCK_H_

#include <VP_Os/vp_os_types.h>
#include <VLIB/video_picture.h>

// Default zigzag ordering matrix
extern int32_t video_zztable_t81[MCU_BLOCK_SIZE];

typedef struct _video_macroblock_t {
  int32_t   azq;  // All zero coefficients
  int32_t   dquant;
  int32_t   num_coeff_y0; // Number of non-zeros coefficients for block y0
  int32_t   num_coeff_y1; // Number of non-zeros coefficients for block y1
  int32_t   num_coeff_y2; // Number of non-zeros coefficients for block y2
  int32_t   num_coeff_y3; // Number of non-zeros coefficients for block y3
  int32_t   num_coeff_cb; // Number of non-zeros coefficients for block cb
  int32_t   num_coeff_cr; // Number of non-zeros coefficients for block cr
  int16_t*  data;         // macroblock's data
} video_macroblock_t;

#endif // _VIDEO_MACROBLOCK_H_
