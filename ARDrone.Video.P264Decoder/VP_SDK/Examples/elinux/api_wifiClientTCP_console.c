#include <stdlib.h>
#include <ctype.h>

#include <VP_Api/vp_api.h>
#include <VP_Api/vp_api_error.h>
#include <VP_Stages/vp_stages_configs.h>
#include <VP_Stages/vp_stages_io_console.h>
#include <VP_Stages/vp_stages_io_com.h>
#include <VP_Os/vp_os_print.h>
#include <VP_Os/vp_os_malloc.h>


#define NB_STAGES 2


static PIPELINE_HANDLE pipeline_handle;


int
main(int argc, char **argv)
{
  vp_api_io_pipeline_t    pipeline;
  vp_api_io_data_t        out;
  vp_api_io_stage_t       stages[NB_STAGES];

  vp_stages_input_com_config_t     icc;

  vp_com_t                      com;
  vp_com_wifi_config_t          config;
  vp_com_wifi_connection_t      connection;

  vp_os_memset( &icc,         0, sizeof(vp_stages_input_com_config_t));
  vp_os_memset( &connection,  0, sizeof(vp_com_wifi_connection_t) );
  strcpy(connection.networkName,"linksys");
  vp_stages_fill_default_config(WIFI_COM_CONFIG, &config, sizeof(config));
  vp_os_memset(&com, 0, sizeof(vp_com_t));

  com.type                          = VP_COM_WIFI;
  icc.com                           = &com;
  icc.config                        = (vp_com_config_t*)&config;
  icc.connection                    = (vp_com_connection_t*)&connection;
  icc.socket.type                   = VP_COM_CLIENT;
  icc.socket.protocol               = VP_COM_TCP;
  icc.socket.port                   = 5555;
  icc.buffer_size                   = 6400;

  strcpy(icc.socket.serverHost,"192.168.1.23");

  stages[0].type = VP_API_INPUT_SOCKET;
  stages[0].cfg = (void *)&icc;
  stages[0].funcs = vp_stages_input_com_funcs;

  stages[1].type = VP_API_OUTPUT_CONSOLE;
  stages[1].cfg = NULL;
  stages[1].funcs = vp_stages_output_console_funcs;

  pipeline.nb_stages = NB_STAGES;
  pipeline.stages = &stages[0];

  vp_api_open(&pipeline, &pipeline_handle);
  out.status = VP_API_STATUS_PROCESSING;
  while(SUCCEED(vp_api_run(&pipeline, &out)) && (out.status == VP_API_STATUS_PROCESSING || out.status == VP_API_STATUS_STILL_RUNNING));
  vp_api_close(&pipeline, &pipeline_handle);

  return EXIT_SUCCESS;
}
