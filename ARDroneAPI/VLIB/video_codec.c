#include <VLIB/video_codec.h>
#include <VLIB/video_quantizer.h>
#include <VLIB/video_packetizer.h>
#include <VLIB/Platform/video_utils.h>
#include <VLIB/Platform/video_config.h>

#include <VP_Os/vp_os_malloc.h>

#ifdef _ELINUX
#include "dma_malloc.h"
#define vp_os_aligned_malloc(A,B) dma_malloc(A)
#define vp_os_aligned_free(A) dma_free(A)
#endif

extern C_RESULT video_utils_init( video_controller_t* controller );
extern C_RESULT video_utils_close( video_controller_t* controller );

extern void uvlc_codec_alloc( video_controller_t* controller );
extern void uvlc_codec_free( video_controller_t* controller );

extern void p263_codec_alloc( video_controller_t* controller );
extern void p263_codec_free( video_controller_t* controller );

C_RESULT video_codec_open( video_controller_t* controller, codec_type_t codec_type )
{
  C_RESULT res;
  // Data used to initilize macroblock's cache
  int32_t i;
  int16_t* cache;
  video_macroblock_t* mb;

  // Close any previously allocated codec for this controller
  video_codec_close( controller );

  video_utils_init( controller );

  controller->mode            = 0;
  controller->use_me          = FALSE;
  controller->do_azq          = FALSE;
  controller->aq              = 0;
  controller->bq              = 0;
  controller->target_bitrate  = VLIB_DEFAULT_BITRATE;
  controller->num_frames      = 0;
  controller->picture_type    = 0;
  controller->width           = 0;
  controller->height          = 0;
  controller->num_blockline   = 0;
  controller->mb_blockline    = 0;
  controller->blockline       = 0;
  controller->picture_complete= 0;
#ifdef USE_TABLE_QUANTIZATION
  controller->quant           = TABLE_QUANTIZATION;
#else
  controller->quant           = DEFAULT_QUANTIZATION;
#endif
  controller->dquant          = 0;
  controller->Qp              = 0;
  controller->invQp           = 1;
  controller->gobs            = NULL;
  controller->cache           = NULL;
  controller->codec_type      = 0;
  controller->video_codec     = NULL;

  if( controller->blockline_cache == NULL )
  {
    // We alloc two buffers to be compatible with an asynchronous DCT
    // When a DCT will be performed on one buffer, we will be able to use the other for caching or computing purpose
    // DCT_BUFFER_SIZE = MAX_NUM_MACRO_BLOCKS_PER_CALL * 6 * MCU_BLOCK_SIZE
    controller->blockline_cache = (int16_t*)vp_os_aligned_malloc( 2*DCT_BUFFER_SIZE*sizeof(int16_t), VLIB_ALLOC_ALIGN );
  }

  controller->cache_mbs = vp_os_malloc( 2 * MAX_NUM_MACRO_BLOCKS_PER_CALL * sizeof(video_macroblock_t) );
  mb = &controller->cache_mbs[0];
  cache = controller->blockline_cache;
  for(i = 2*MAX_NUM_MACRO_BLOCKS_PER_CALL; i > 0; i-- )
  {
    mb->data = cache;
    cache   += MCU_BLOCK_SIZE*6;
    mb ++;
  }

  video_packetizer_init( controller );
  video_quantizer_init( controller );

  switch( codec_type )
  {
    case UVLC_CODEC:
      uvlc_codec_alloc( controller );
      break;

    case P263_CODEC:
      p263_codec_alloc( controller );
      break;

    default:
      controller->video_codec = NULL;
      break;
  }

  if( controller->video_codec != NULL )
  {
    controller->codec_type = codec_type;
    res = C_OK;
  }
  else
  {
    res = C_FAIL;
  }

  return res;
}

C_RESULT video_codec_close( video_controller_t* controller )
{
  video_utils_close( controller );

  if( controller->blockline_cache != NULL )
    vp_os_aligned_free( controller->blockline_cache );

  if( controller->in_stream.bytes != NULL )
    video_packetizer_close( controller );

  switch( controller->codec_type )
  {
    case UVLC_CODEC:
      uvlc_codec_free( controller );
      break;

    case P263_CODEC:
      p263_codec_free( controller );
      break;

    default:
      break;
  }

  // Cleanup caches
  video_controller_cleanup( controller );

  return C_OK;
}

C_RESULT video_encode_picture( video_controller_t* controller, const vp_api_picture_t* picture, bool_t* got_image )
{
  vp_api_picture_t blockline = { 0 };

  controller->mode  = VIDEO_ENCODE;

  video_controller_set_format( controller, picture->width, picture->height );

  blockline                   = *picture;
  blockline.height            = MB_HEIGHT_Y;
  blockline.complete          = 1;
  blockline.vision_complete   = 0;

  // Reset internal stream for new blockline/picture
  controller->in_stream.used  = 0;
  controller->in_stream.index = 0;

  while( !controller->picture_complete )
  {
    video_encode_blockline( controller, &blockline, blockline.blockline == (controller->num_blockline-1) );

    blockline.y_buf  += MB_HEIGHT_Y * picture->y_line_size;
    blockline.cb_buf += MB_HEIGHT_C * picture->cb_line_size;
    blockline.cr_buf += MB_HEIGHT_C * picture->cr_line_size;

    blockline.blockline++;
  }

  if( picture->complete )
  {
    video_write_data( &controller->in_stream, 0, controller->in_stream.length+1 );
    controller->in_stream.length = 32;
    controller->picture_complete = 0;
    *got_image = TRUE;
  }

  return C_OK;
}

C_RESULT video_decode_picture( video_controller_t* controller, vp_api_picture_t* picture, video_stream_t* ex_stream, bool_t* got_image )
{
  vp_api_picture_t blockline = { 0 };

  controller->mode  = VIDEO_DECODE; // mandatory because of video_cache_stream

  blockline                   = *picture;
  blockline.height            = MB_HEIGHT_Y;
  blockline.complete          = 1;
  blockline.vision_complete   = 0;

  while( VP_SUCCEEDED(video_cache_stream( controller, ex_stream )) )
  {
    video_decode_blockline( controller, &blockline, got_image );
  }

  return C_OK;
}
