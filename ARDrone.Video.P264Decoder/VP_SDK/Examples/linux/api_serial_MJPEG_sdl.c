#include <stdlib.h>
#include <ctype.h>

#include <VP_Api/vp_api.h>
#include <VP_Api/vp_api_thread_helper.h>
#include <VP_Api/vp_api_error.h>
#include <VP_Stages/vp_stages_configs.h>
#include <VP_Stages/vp_stages_io_console.h>
#include <VP_Stages/vp_stages_o_sdl.h>
#include <VP_Stages/vp_stages_io_com.h>
#include <VP_Stages/vp_stages_io_file.h>
#include <VP_Os/vp_os_print.h>
#include <VP_Os/vp_os_malloc.h>
#include <VP_Os/vp_os_delay.h>

#include <MJPEG/mjpeg.h>
#include <MJPEG/stream.h>

#define NB_STAGES 3

#define ACQ_WIDTH   320
#define ACQ_HEIGHT  240


static PIPELINE_HANDLE pipeline_handle;

////////////////////////////////////////////////////////////////////////////////
// cpu load information part
//
#define NOMFICH_CPUINFO "/proc/cpuinfo"
double RDTSC(void)
{
  unsigned long long x;
  __asm__ volatile  (".byte 0x0f, 0x31" : "=A"(x));
  return (double)x;
}

int read_cpu_freq (double* freq)
{
  const char* prefix_cpu_mhz = "cpu MHz";
  FILE* F;
  char line[300+1];
  char *pos;
  int ok=0;

  F = fopen(NOMFICH_CPUINFO, "r");
  if (!F) return 0;

  while (!feof(F))
  {
    fgets (line, sizeof(line), F);

    if (!strncmp(line, prefix_cpu_mhz, strlen(prefix_cpu_mhz)))
    {
      pos = strrchr (line, ':') +2;
      if (!pos) break;
      if (pos[strlen(pos)-1] == '\n') pos[strlen(pos)-1] = '\0';
      strcpy (line, pos);
      strcat (line,"e6");
      *freq = atof (line);
      ok = 1;
      break;
    }
  }
  fclose (F);
  return ok;
}
////////////////////////////////////////////////////////////////////////////////

PROTO_THREAD_ROUTINE(escaper,nomParams);
PROTO_THREAD_ROUTINE(app,nomParams);

BEGIN_THREAD_TABLE
  THREAD_TABLE_ENTRY(escaper,20)
  THREAD_TABLE_ENTRY(app,10)
END_THREAD_TABLE


typedef struct _mjpeg_stage_decoding_config_t
{
  stream_t          stream;
  mjpeg_t           mjpeg;
  vp_api_picture_t* picture;

  uint32_t          out_buffer_size;

} mjpeg_stage_decoding_config_t;

double cpufreq;
double t1,t2, t;
C_RESULT mjpeg_stage_decoding_open(mjpeg_stage_decoding_config_t *cfg)
{
  stream_new( &cfg->stream, OUTPUT_STREAM );

  read_cpu_freq(&cpufreq);

  return mjpeg_init( &cfg->mjpeg, MJPEG_DECODE, cfg->picture->width, cfg->picture->height, cfg->picture->format );
}

C_RESULT mjpeg_stage_decoding_transform(mjpeg_stage_decoding_config_t *cfg, vp_api_io_data_t *in, vp_api_io_data_t *out)
{
  uint32_t success;
  bool_t got_image;
  static FILE* fp=NULL;

  vp_os_mutex_lock( &out->lock );

  if(out->status == VP_API_STATUS_INIT)
  {
    out->numBuffers   = 1;
    out->buffers      = (int8_t**)cfg->picture;
    out->indexBuffer  = 0;
    out->lineSize     = 0;

    out->status = VP_API_STATUS_PROCESSING;

	fp = fopen("tmp.mjpg", "wb");
	//mjpeg_init_block_mode( &cfg->mjpeg, 15 );
  }

  if( in->status == VP_API_STATUS_ENDED )
    out->status = in->status;

  // Several cases must be handled in this stage
  // 1st: Input buffer is too small to decode a complete picture
  // 2nd: Input buffer is big enough to decode 1 frame
  // 3rd: Input buffer is so big we can decode more than 1 frame

  if( in->size > 0 )
  {
    if( out->status == VP_API_STATUS_PROCESSING )
    {
	  if( NULL != fp )
		  fwrite( in->buffers[in->indexBuffer] , 1, in->size, fp );

      // Reinit stream with new data
      stream_config( &cfg->stream, in->size, in->buffers[in->indexBuffer] );
    }

    if(out->status == VP_API_STATUS_PROCESSING || out->status == VP_API_STATUS_STILL_RUNNING)
    {
      // If out->size == 1 it means picture is ready
      out->size = 0;
      out->status = VP_API_STATUS_PROCESSING;
	  success = SUCCEED(mjpeg_decode( &cfg->mjpeg, cfg->picture, &cfg->stream , &got_image ));
      if( got_image )
      {
        // we got one picture (handle case 1)
        out->size = 1;
      }
      // handle case 2 & 3
      if( FAILED(stream_is_empty( &cfg->stream )) )
      {
        // Some data are still in stream
        // Next time we run this stage we don't want this data to be lost
        // So flag it!
        out->status = VP_API_STATUS_STILL_RUNNING;
      }
    }
  }

  vp_os_mutex_unlock( &out->lock );

  return C_OK;
}

C_RESULT mjpeg_stage_decoding_close(mjpeg_stage_decoding_config_t *cfg)
{
  stream_delete( &cfg->stream );

  return mjpeg_release( &cfg->mjpeg );
}


const vp_api_stage_funcs_t mjpeg_decoding_funcs = {
  (vp_api_stage_handle_msg_t) NULL,
  (vp_api_stage_open_t) mjpeg_stage_decoding_open,
  (vp_api_stage_transform_t) mjpeg_stage_decoding_transform,
  (vp_api_stage_close_t) mjpeg_stage_decoding_close
};

int
main(int argc, char **argv)
{
  START_THREAD(escaper, NO_PARAM);
  START_THREAD(app, argv);

  JOIN_THREAD(escaper);
  JOIN_THREAD(app);

  return EXIT_SUCCESS;
}

PROTO_THREAD_ROUTINE(app,argv)
{
  int32_t success, curstage=0;
  vp_api_picture_t picture;

  vp_api_io_pipeline_t    pipeline;
  vp_api_io_data_t        out;
  vp_api_io_stage_t       stages[NB_STAGES];

  vp_stages_input_com_config_t    icc;
  mjpeg_stage_decoding_config_t   dec;
  vp_stages_output_sdl_config_t   osc;
  vp_com_serial_config_t          config;

  vp_com_t                        com;

  /// Picture configuration
  picture.format        = PIX_FMT_YUV420P;
  picture.width         = ACQ_WIDTH;
  picture.height        = ACQ_HEIGHT;
  picture.framerate     = 30;
  picture.y_buf         = vp_os_malloc( ACQ_WIDTH*ACQ_HEIGHT );
  picture.cr_buf        = vp_os_malloc( ACQ_WIDTH*ACQ_HEIGHT/4 );
  picture.cb_buf        = vp_os_malloc( ACQ_WIDTH*ACQ_HEIGHT/4 );
  picture.y_line_size   = ACQ_WIDTH;
  picture.cb_line_size  = ACQ_WIDTH / 2;
  picture.cr_line_size  = ACQ_WIDTH / 2;
  picture.y_pad         = 0;
  picture.c_pad         = 0;

  dec.picture         = &picture;
  dec.out_buffer_size = 4096;

  vp_os_memset( &icc,         0, sizeof(vp_stages_input_com_config_t)  );
  vp_os_memset( &osc,         0, sizeof(vp_stages_output_sdl_config_t) );
  vp_os_memset( &com,         0, sizeof(vp_com_t)                      );

  strcpy(config.itfName, "/dev/ttyUSB1");
  
  //
  config.initial_baudrate = VP_COM_BAUDRATE_921600;
  config.baudrate = VP_COM_BAUDRATE_921600;
  //
  //config.initial_baudrate = VP_COM_BAUDRATE_460800;
  //config.baudrate = VP_COM_BAUDRATE_460800;
  //

  config.sync = 0;
  config.blocking = 1;
  com.type = VP_COM_SERIAL;

  icc.com                           = &com;
  icc.config                        = (vp_com_config_t*)&config;
  icc.socket.type                   = VP_COM_CLIENT;
  icc.buffer_size                   = 320*240;

  //osc.width           = 640;
  //osc.height          = 480;
  osc.width           = 320;
  osc.height          = 240;
  osc.bpp             = 16;
  //osc.window_width    = 640;
  //osc.window_height   = 480;
  osc.window_width    = 320;
  osc.window_height   = 240;
  osc.pic_width       = ACQ_WIDTH;
  osc.pic_height      = ACQ_HEIGHT;
  osc.y_size          = ACQ_WIDTH*ACQ_HEIGHT;
  osc.c_size          = (ACQ_WIDTH*ACQ_HEIGHT) >> 2;

  stages[0].type                    = VP_API_INPUT_SOCKET;
  stages[0].cfg                     = (void *)&icc;
  stages[0].funcs                   = vp_stages_input_com_funcs;

  stages[1].type                    = VP_API_FILTER_DECODER;
  stages[1].cfg                     = (void*)&dec;
  stages[1].funcs                   = mjpeg_decoding_funcs;

  stages[2].type                    = VP_API_OUTPUT_SDL;
  stages[2].cfg                     = (void *)&osc;
  stages[2].funcs                   = vp_stages_output_sdl_funcs;

  pipeline.nb_stages                = NB_STAGES;
  pipeline.stages                   = &stages[0];

  vp_api_open(&pipeline, &pipeline_handle);
  out.status = VP_API_STATUS_PROCESSING;
  while(SUCCEED(vp_api_run(&pipeline, &out)) && (out.status == VP_API_STATUS_PROCESSING || out.status == VP_API_STATUS_STILL_RUNNING));
  /*
  do {
	  t1 = RDTSC();
	  success = SUCCEED(vp_api_run(&pipeline, &out));
	  t2 = RDTSC();
	  
	  t = (t2-t1)*1000 / cpufreq ;
	  if( t>4 )printf("%d\t%f\n", curstage, t );
	  if( ++curstage == 3 ) curstage = 0;
  } while( success && (out.status == VP_API_STATUS_PROCESSING || out.status == VP_API_STATUS_STILL_RUNNING) );
  */

  vp_api_close(&pipeline, &pipeline_handle);

  return EXIT_SUCCESS;
}

///*******************************************************************************************************************///


// static THREAD_HANDLE dct_thread_handle;

static dct_io_buffer_t* current_io_buffer;
static dct_io_buffer_t* result_io_buffer;

static void fdct(const unsigned short* in, short* out);
static void idct(const short* in, unsigned short* out);


//-----------------------------------------------------------------------------
// DCT API
//-----------------------------------------------------------------------------


bool_t dct_init(void)
{
  current_io_buffer = NULL;
  result_io_buffer  = NULL;

  return TRUE;
}

bool_t dct_compute( dct_io_buffer_t* io_buffer )
{
  bool_t res = FALSE;

  assert(io_buffer != NULL);

  if( current_io_buffer == NULL && result_io_buffer == NULL )
  {
    current_io_buffer = io_buffer;
    res = TRUE;

  }

  return res;
}

dct_io_buffer_t* dct_result( void )
{
  uint32_t i;
  dct_io_buffer_t* io_buffer;

  io_buffer = NULL;

  if( current_io_buffer != NULL)
  {
    if( current_io_buffer->dct_mode == DCT_MODE_FDCT )
    {
      for( i = 0; i < current_io_buffer->num_total_blocks; i++ )
      {
        fdct(current_io_buffer->input[i], current_io_buffer->output[i]);
      }
    }
    else if( current_io_buffer->dct_mode == DCT_MODE_IDCT )
    {
      for( i = 0; i < current_io_buffer->num_total_blocks; i++ )
      {
        idct(current_io_buffer->input[i], current_io_buffer->output[i]);
      }
    }

    io_buffer = current_io_buffer;
    current_io_buffer = NULL;
  }

  return io_buffer;
}


//-----------------------------------------------------------------------------
// DCT Computation
//-----------------------------------------------------------------------------


#define FIX_0_298631336  ((INT32)  2446)	/* FIX(0.298631336) */
#define FIX_0_390180644  ((INT32)  3196)	/* FIX(0.390180644) */
#define FIX_0_541196100  ((INT32)  4433)	/* FIX(0.541196100) */
#define FIX_0_765366865  ((INT32)  6270)	/* FIX(0.765366865) */
#define FIX_0_899976223  ((INT32)  7373)	/* FIX(0.899976223) */
#define FIX_1_175875602  ((INT32)  9633)	/* FIX(1.175875602) */
#define FIX_1_501321110  ((INT32)  12299)	/* FIX(1.501321110) */
#define FIX_1_847759065  ((INT32)  15137)	/* FIX(1.847759065) */
#define FIX_1_961570560  ((INT32)  16069)	/* FIX(1.961570560) */
#define FIX_2_053119869  ((INT32)  16819)	/* FIX(2.053119869) */
#define FIX_2_562915447  ((INT32)  20995)	/* FIX(2.562915447) */
#define FIX_3_072711026  ((INT32)  25172)	/* FIX(3.072711026) */

#define INT32       int
#define DCTELEM     int
#define DCTSIZE     8
#define DCTSIZE2    64
#define CONST_BITS  13
#define PASS1_BITS  1
#define ONE	((INT32) 1)
#define MULTIPLY(var,const)  ((var) * (const))
#define DESCALE(x,n)  RIGHT_SHIFT((x) + (ONE << ((n)-1)), n)
#define RIGHT_SHIFT(x,shft)	((x) >> (shft))

static void fdct(const unsigned short* in, short* out)
{
  INT32 tmp0, tmp1, tmp2, tmp3, tmp4, tmp5, tmp6, tmp7;
  INT32 tmp10, tmp11, tmp12, tmp13;
  INT32 z1, z2, z3, z4, z5;
  int ctr;
  // SHIFT_TEMPS

  int data[DCTSIZE * DCTSIZE];
  int i, j;
  int* dataptr = data;

  for( i = 0; i < DCTSIZE; i++ )
  {
    for( j = 0; j < DCTSIZE; j++ )
    {
      int temp;

      temp = in[i*DCTSIZE + j];
      dataptr[i*DCTSIZE + j] = temp;
    }
  }

  /* Pass 1: process rows. */
  /* Note results are scaled up by sqrt(8) compared to a true DCT; */
  /* furthermore, we scale the results by 2**PASS1_BITS. */

  dataptr = data;
  for (ctr = DCTSIZE-1; ctr >= 0; ctr--) {
    tmp0 = dataptr[0] + dataptr[7];
    tmp7 = dataptr[0] - dataptr[7];
    tmp1 = dataptr[1] + dataptr[6];
    tmp6 = dataptr[1] - dataptr[6];
    tmp2 = dataptr[2] + dataptr[5];
    tmp5 = dataptr[2] - dataptr[5];
    tmp3 = dataptr[3] + dataptr[4];
    tmp4 = dataptr[3] - dataptr[4];

    /* Even part per LL&M figure 1 --- note that published figure is faulty;
     * rotator "sqrt(2)*c1" should be "sqrt(2)*c6".
     */

    tmp10 = tmp0 + tmp3;
    tmp13 = tmp0 - tmp3;
    tmp11 = tmp1 + tmp2;
    tmp12 = tmp1 - tmp2;

    dataptr[0] = (DCTELEM) ((tmp10 + tmp11) << PASS1_BITS);
    dataptr[4] = (DCTELEM) ((tmp10 - tmp11) << PASS1_BITS);

    z1 = MULTIPLY(tmp12 + tmp13, FIX_0_541196100);
    dataptr[2] = (DCTELEM) DESCALE(z1 + MULTIPLY(tmp13, FIX_0_765366865), CONST_BITS-PASS1_BITS);
    dataptr[6] = (DCTELEM) DESCALE(z1 + MULTIPLY(tmp12, - FIX_1_847759065), CONST_BITS-PASS1_BITS);

    /* Odd part per figure 8 --- note paper omits factor of sqrt(2).
     * cK represents cos(K*pi/16).
     * i0..i3 in the paper are tmp4..tmp7 here.
     */

    z1 = tmp4 + tmp7;
    z2 = tmp5 + tmp6;
    z3 = tmp4 + tmp6;
    z4 = tmp5 + tmp7;
    z5 = MULTIPLY(z3 + z4, FIX_1_175875602); /* sqrt(2) * c3 */

    tmp4 = MULTIPLY(tmp4, FIX_0_298631336); /* sqrt(2) * (-c1+c3+c5-c7) */
    tmp5 = MULTIPLY(tmp5, FIX_2_053119869); /* sqrt(2) * ( c1+c3-c5+c7) */
    tmp6 = MULTIPLY(tmp6, FIX_3_072711026); /* sqrt(2) * ( c1+c3+c5-c7) */
    tmp7 = MULTIPLY(tmp7, FIX_1_501321110); /* sqrt(2) * ( c1+c3-c5-c7) */
    z1 = MULTIPLY(z1, - FIX_0_899976223); /* sqrt(2) * (c7-c3) */
    z2 = MULTIPLY(z2, - FIX_2_562915447); /* sqrt(2) * (-c1-c3) */
    z3 = MULTIPLY(z3, - FIX_1_961570560); /* sqrt(2) * (-c3-c5) */
    z4 = MULTIPLY(z4, - FIX_0_390180644); /* sqrt(2) * (c5-c3) */

    z3 += z5;
    z4 += z5;

    dataptr[7] = (DCTELEM) DESCALE(tmp4 + z1 + z3, CONST_BITS-PASS1_BITS);
    dataptr[5] = (DCTELEM) DESCALE(tmp5 + z2 + z4, CONST_BITS-PASS1_BITS);
    dataptr[3] = (DCTELEM) DESCALE(tmp6 + z2 + z3, CONST_BITS-PASS1_BITS);
    dataptr[1] = (DCTELEM) DESCALE(tmp7 + z1 + z4, CONST_BITS-PASS1_BITS);

    dataptr += DCTSIZE;		/* advance pointer to next row */
  }

  /* Pass 2: process columns.
   * We remove the PASS1_BITS scaling, but leave the results scaled up
   * by an overall factor of 8.
   */

  dataptr = data;
  for (ctr = DCTSIZE-1; ctr >= 0; ctr--) {
    tmp0 = dataptr[DCTSIZE*0] + dataptr[DCTSIZE*7];
    tmp7 = dataptr[DCTSIZE*0] - dataptr[DCTSIZE*7];
    tmp1 = dataptr[DCTSIZE*1] + dataptr[DCTSIZE*6];
    tmp6 = dataptr[DCTSIZE*1] - dataptr[DCTSIZE*6];
    tmp2 = dataptr[DCTSIZE*2] + dataptr[DCTSIZE*5];
    tmp5 = dataptr[DCTSIZE*2] - dataptr[DCTSIZE*5];
    tmp3 = dataptr[DCTSIZE*3] + dataptr[DCTSIZE*4];
    tmp4 = dataptr[DCTSIZE*3] - dataptr[DCTSIZE*4];

    /* Even part per LL&M figure 1 --- note that published figure is faulty;
     * rotator "sqrt(2)*c1" should be "sqrt(2)*c6".
     */

    tmp10 = tmp0 + tmp3;
    tmp13 = tmp0 - tmp3;
    tmp11 = tmp1 + tmp2;
    tmp12 = tmp1 - tmp2;

    dataptr[DCTSIZE*0] = (DCTELEM) DESCALE(tmp10 + tmp11, PASS1_BITS);
    dataptr[DCTSIZE*4] = (DCTELEM) DESCALE(tmp10 - tmp11, PASS1_BITS);

    z1 = MULTIPLY(tmp12 + tmp13, FIX_0_541196100);
    dataptr[DCTSIZE*2] = (DCTELEM) DESCALE(z1 + MULTIPLY(tmp13, FIX_0_765366865), CONST_BITS+PASS1_BITS);
    dataptr[DCTSIZE*6] = (DCTELEM) DESCALE(z1 + MULTIPLY(tmp12, - FIX_1_847759065), CONST_BITS+PASS1_BITS);

    /* Odd part per figure 8 --- note paper omits factor of sqrt(2).
     * cK represents cos(K*pi/16).
     * i0..i3 in the paper are tmp4..tmp7 here.
     */

    z1 = tmp4 + tmp7;
    z2 = tmp5 + tmp6;
    z3 = tmp4 + tmp6;
    z4 = tmp5 + tmp7;
    z5 = MULTIPLY(z3 + z4, FIX_1_175875602); /* sqrt(2) * c3 */

    tmp4 = MULTIPLY(tmp4, FIX_0_298631336); /* sqrt(2) * (-c1+c3+c5-c7) */
    tmp5 = MULTIPLY(tmp5, FIX_2_053119869); /* sqrt(2) * ( c1+c3-c5+c7) */
    tmp6 = MULTIPLY(tmp6, FIX_3_072711026); /* sqrt(2) * ( c1+c3+c5-c7) */
    tmp7 = MULTIPLY(tmp7, FIX_1_501321110); /* sqrt(2) * ( c1+c3-c5-c7) */
    z1 = MULTIPLY(z1, - FIX_0_899976223); /* sqrt(2) * (c7-c3) */
    z2 = MULTIPLY(z2, - FIX_2_562915447); /* sqrt(2) * (-c1-c3) */
    z3 = MULTIPLY(z3, - FIX_1_961570560); /* sqrt(2) * (-c3-c5) */
    z4 = MULTIPLY(z4, - FIX_0_390180644); /* sqrt(2) * (c5-c3) */

    z3 += z5;
    z4 += z5;

    dataptr[DCTSIZE*7] = (DCTELEM) DESCALE(tmp4 + z1 + z3, CONST_BITS+PASS1_BITS);
    dataptr[DCTSIZE*5] = (DCTELEM) DESCALE(tmp5 + z2 + z4, CONST_BITS+PASS1_BITS);
    dataptr[DCTSIZE*3] = (DCTELEM) DESCALE(tmp6 + z2 + z3, CONST_BITS+PASS1_BITS);
    dataptr[DCTSIZE*1] = (DCTELEM) DESCALE(tmp7 + z1 + z4, CONST_BITS+PASS1_BITS);

    dataptr++;  /* advance pointer to next column */
  }

  for( i = 0; i < DCTSIZE; i++ )
    for( j = 0; j < DCTSIZE; j++ )
      out[i*DCTSIZE + j] = data[i*DCTSIZE + j] >> 3;
}

static void idct(const short* in, unsigned short* out)
{
  INT32 tmp0, tmp1, tmp2, tmp3;
  INT32 tmp10, tmp11, tmp12, tmp13;
  INT32 z1, z2, z3, z4, z5;
  int* wsptr;
  int* outptr;
  const short* inptr;
  int ctr;
  int workspace[DCTSIZE2];	/* buffers data between passes */
  int data[DCTSIZE2];
  // SHIFT_TEMPS

  /* Pass 1: process columns from input, store into work array. */
  /* Note results are scaled up by sqrt(8) compared to a true IDCT; */
  /* furthermore, we scale the results by 2**PASS1_BITS. */

  inptr = in;
  wsptr = workspace;
  for (ctr = DCTSIZE; ctr > 0; ctr--) {
    /* Due to quantization, we will usually find that many of the input
     * coefficients are zero, especially the AC terms.  We can exploit this
     * by short-circuiting the IDCT calculation for any column in which all
     * the AC terms are zero.  In that case each output is equal to the
     * DC coefficient (with scale factor as needed).
     * With typical images and quantization tables, half or more of the
     * column DCT calculations can be simplified this way.
     */

    if( inptr[DCTSIZE*1] == 0 && inptr[DCTSIZE*2] == 0 &&
        inptr[DCTSIZE*3] == 0 && inptr[DCTSIZE*4] == 0 &&
        inptr[DCTSIZE*5] == 0 && inptr[DCTSIZE*6] == 0 &&
        inptr[DCTSIZE*7] == 0 ) {
      /* AC terms all zero */
      int dcval = inptr[DCTSIZE*0] << PASS1_BITS;

      wsptr[DCTSIZE*0] = dcval;
      wsptr[DCTSIZE*1] = dcval;
      wsptr[DCTSIZE*2] = dcval;
      wsptr[DCTSIZE*3] = dcval;
      wsptr[DCTSIZE*4] = dcval;
      wsptr[DCTSIZE*5] = dcval;
      wsptr[DCTSIZE*6] = dcval;
      wsptr[DCTSIZE*7] = dcval;

      inptr++;  /* advance pointers to next column */
      wsptr++;
      continue;
    }

    /* Even part: reverse the even part of the forward DCT. */
    /* The rotator is sqrt(2)*c(-6). */

    z2 = inptr[DCTSIZE*2];
    z3 = inptr[DCTSIZE*6];

    z1 = MULTIPLY(z2 + z3, FIX_0_541196100);
    tmp2 = z1 + MULTIPLY(z3, - FIX_1_847759065);
    tmp3 = z1 + MULTIPLY(z2, FIX_0_765366865);

    z2 = inptr[DCTSIZE*0];
    z3 = inptr[DCTSIZE*4];

    tmp0 = (z2 + z3) << CONST_BITS;
    tmp1 = (z2 - z3) << CONST_BITS;

    tmp10 = tmp0 + tmp3;
    tmp13 = tmp0 - tmp3;
    tmp11 = tmp1 + tmp2;
    tmp12 = tmp1 - tmp2;

    /* Odd part per figure 8; the matrix is unitary and hence its
     * transpose is its inverse.  i0..i3 are y7,y5,y3,y1 respectively.
     */

    tmp0 = inptr[DCTSIZE*7];
    tmp1 = inptr[DCTSIZE*5];
    tmp2 = inptr[DCTSIZE*3];
    tmp3 = inptr[DCTSIZE*1];

    z1 = tmp0 + tmp3;
    z2 = tmp1 + tmp2;
    z3 = tmp0 + tmp2;
    z4 = tmp1 + tmp3;
    z5 = MULTIPLY(z3 + z4, FIX_1_175875602); /* sqrt(2) * c3 */

    tmp0 = MULTIPLY(tmp0, FIX_0_298631336); /* sqrt(2) * (-c1+c3+c5-c7) */
    tmp1 = MULTIPLY(tmp1, FIX_2_053119869); /* sqrt(2) * ( c1+c3-c5+c7) */
    tmp2 = MULTIPLY(tmp2, FIX_3_072711026); /* sqrt(2) * ( c1+c3+c5-c7) */
    tmp3 = MULTIPLY(tmp3, FIX_1_501321110); /* sqrt(2) * ( c1+c3-c5-c7) */
    z1 = MULTIPLY(z1, - FIX_0_899976223); /* sqrt(2) * (c7-c3) */
    z2 = MULTIPLY(z2, - FIX_2_562915447); /* sqrt(2) * (-c1-c3) */
    z3 = MULTIPLY(z3, - FIX_1_961570560); /* sqrt(2) * (-c3-c5) */
    z4 = MULTIPLY(z4, - FIX_0_390180644); /* sqrt(2) * (c5-c3) */

    z3 += z5;
    z4 += z5;

    tmp0 += z1 + z3;
    tmp1 += z2 + z4;
    tmp2 += z2 + z3;
    tmp3 += z1 + z4;

    /* Final output stage: inputs are tmp10..tmp13, tmp0..tmp3 */

    wsptr[DCTSIZE*0] = (int) DESCALE(tmp10 + tmp3, CONST_BITS-PASS1_BITS);
    wsptr[DCTSIZE*7] = (int) DESCALE(tmp10 - tmp3, CONST_BITS-PASS1_BITS);
    wsptr[DCTSIZE*1] = (int) DESCALE(tmp11 + tmp2, CONST_BITS-PASS1_BITS);
    wsptr[DCTSIZE*6] = (int) DESCALE(tmp11 - tmp2, CONST_BITS-PASS1_BITS);
    wsptr[DCTSIZE*2] = (int) DESCALE(tmp12 + tmp1, CONST_BITS-PASS1_BITS);
    wsptr[DCTSIZE*5] = (int) DESCALE(tmp12 - tmp1, CONST_BITS-PASS1_BITS);
    wsptr[DCTSIZE*3] = (int) DESCALE(tmp13 + tmp0, CONST_BITS-PASS1_BITS);
    wsptr[DCTSIZE*4] = (int) DESCALE(tmp13 - tmp0, CONST_BITS-PASS1_BITS);

    inptr++;  /* advance pointers to next column */
    wsptr++;
  }

  /* Pass 2: process rows from work array, store into output array. */
  /* Note that we must descale the results by a factor of 8 == 2**3, */
  /* and also undo the PASS1_BITS scaling. */

  wsptr = workspace;
  outptr = data;
  for (ctr = 0; ctr < DCTSIZE; ctr++) {
    /* Even part: reverse the even part of the forward DCT. */
    /* The rotator is sqrt(2)*c(-6). */

    z2 = (INT32) wsptr[2];
    z3 = (INT32) wsptr[6];

    z1 = MULTIPLY(z2 + z3, FIX_0_541196100);
    tmp2 = z1 + MULTIPLY(z3, - FIX_1_847759065);
    tmp3 = z1 + MULTIPLY(z2, FIX_0_765366865);

    tmp0 = ((INT32) wsptr[0] + (INT32) wsptr[4]) << CONST_BITS;
    tmp1 = ((INT32) wsptr[0] - (INT32) wsptr[4]) << CONST_BITS;

    tmp10 = tmp0 + tmp3;
    tmp13 = tmp0 - tmp3;
    tmp11 = tmp1 + tmp2;
    tmp12 = tmp1 - tmp2;

    /* Odd part per figure 8; the matrix is unitary and hence its
     * transpose is its inverse.  i0..i3 are y7,y5,y3,y1 respectively.
     */

    tmp0 = (INT32) wsptr[7];
    tmp1 = (INT32) wsptr[5];
    tmp2 = (INT32) wsptr[3];
    tmp3 = (INT32) wsptr[1];

    z1 = tmp0 + tmp3;
    z2 = tmp1 + tmp2;
    z3 = tmp0 + tmp2;
    z4 = tmp1 + tmp3;
    z5 = MULTIPLY(z3 + z4, FIX_1_175875602); /* sqrt(2) * c3 */

    tmp0 = MULTIPLY(tmp0, FIX_0_298631336); /* sqrt(2) * (-c1+c3+c5-c7) */
    tmp1 = MULTIPLY(tmp1, FIX_2_053119869); /* sqrt(2) * ( c1+c3-c5+c7) */
    tmp2 = MULTIPLY(tmp2, FIX_3_072711026); /* sqrt(2) * ( c1+c3+c5-c7) */
    tmp3 = MULTIPLY(tmp3, FIX_1_501321110); /* sqrt(2) * ( c1+c3-c5-c7) */
    z1 = MULTIPLY(z1, - FIX_0_899976223); /* sqrt(2) * (c7-c3) */
    z2 = MULTIPLY(z2, - FIX_2_562915447); /* sqrt(2) * (-c1-c3) */
    z3 = MULTIPLY(z3, - FIX_1_961570560); /* sqrt(2) * (-c3-c5) */
    z4 = MULTIPLY(z4, - FIX_0_390180644); /* sqrt(2) * (c5-c3) */

    z3 += z5;
    z4 += z5;

    tmp0 += z1 + z3;
    tmp1 += z2 + z4;
    tmp2 += z2 + z3;
    tmp3 += z1 + z4;

    /* Final output stage: inputs are tmp10..tmp13, tmp0..tmp3 */

    outptr[0] = (tmp10 + tmp3) >> ( CONST_BITS+PASS1_BITS+3 );
    outptr[7] = (tmp10 - tmp3) >> ( CONST_BITS+PASS1_BITS+3 );
    outptr[1] = (tmp11 + tmp2) >> ( CONST_BITS+PASS1_BITS+3 );
    outptr[6] = (tmp11 - tmp2) >> ( CONST_BITS+PASS1_BITS+3 );
    outptr[2] = (tmp12 + tmp1) >> ( CONST_BITS+PASS1_BITS+3 );
    outptr[5] = (tmp12 - tmp1) >> ( CONST_BITS+PASS1_BITS+3 );
    outptr[3] = (tmp13 + tmp0) >> ( CONST_BITS+PASS1_BITS+3 );
    outptr[4] = (tmp13 - tmp0) >> ( CONST_BITS+PASS1_BITS+3 );

    wsptr += DCTSIZE; /* advance pointer to next row */
    outptr += DCTSIZE;
  }

  for(ctr = 0; ctr < DCTSIZE2; ctr++)
    out[ctr] = data[ctr];
}
