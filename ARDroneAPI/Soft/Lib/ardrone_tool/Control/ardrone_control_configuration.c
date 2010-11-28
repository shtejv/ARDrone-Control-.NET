#include <VP_Os/vp_os_malloc.h>
#include <VP_Os/vp_os_print.h>

#include <ardrone_api.h>

#include <ardrone_tool/Control/ardrone_control_configuration.h>

static int8_t ini_buffer[ARDRONE_CONTROL_CONFIGURATION_INI_BUFFER_SIZE];
static uint32_t ini_buffer_index = 0;

static void set_state(ardrone_control_configuration_event_t* event, ardrone_config_state_t s)
{
  DEBUG_PRINT_SDK("[CONTROL CONFIGURATION] Switching to state %d\n", s);

  event->config_state = s;
}

C_RESULT ardrone_control_configuration_run( uint32_t ardrone_state, ardrone_control_configuration_event_t* event )
{
	int32_t buffer_size;
	char *start_buffer, *buffer;
	char *value, *param, *c;
	bool_t ini_buffer_end, ini_buffer_more;
	C_RESULT res = C_OK;
 
	switch( event->config_state )
	{
		case CONFIG_REQUEST_INI:
			if( ardrone_state & ARDRONE_COMMAND_MASK )
			{
				ardrone_at_update_control_mode(ACK_CONTROL_MODE, 0);
			}
			else
			{
				ini_buffer_index = 0;
				ardrone_at_configuration_get_ctrl_mode();
				set_state(event, CONFIG_RECEIVE_INI);
			}
			break;

		case CONFIG_RECEIVE_INI:
			// Read data coming from ARDrone
			buffer_size = ARDRONE_CONTROL_CONFIGURATION_INI_BUFFER_SIZE - ini_buffer_index;
			res = ardrone_control_read( &ini_buffer[ini_buffer_index], &buffer_size );
			if(VP_SUCCEEDED(res))
			{      
				buffer_size += ini_buffer_index;

				// Parse received data
				if( buffer_size > 0 )
				{
					ini_buffer_end  = ini_buffer[buffer_size-1] == '\0'; // End of configuration data
					ini_buffer_more = ini_buffer[buffer_size-1] != '\n'; // Need more configuration data to end parsing current line

					start_buffer = (char*)&ini_buffer[0];
					buffer = strchr(start_buffer, '\n');

					while( buffer != NULL )
					{
						value = start_buffer;
						param = strchr(value, '=');

						*buffer = '\0';
						*param  = '\0';

						// Remove spaces at end of strings
						c = param - 1;
						while( *c == ' ' )
						{
							*c = '\0';
							c  = c-1;
						}

						c = buffer-1;
						while( *c == ' ' )
						{
							*c = '\0';
							c  = c-1;
						}

						// Remove spaces at begining of strings
						while( *value == ' ' )
						{
							value = value + 1;
						}

						param = param + 1;
						while( *param == ' ' )
						{
							param = param + 1;
						}
						iniparser_setstring( event->ini_dict, value, param );

						start_buffer = buffer + 1;
						buffer = strchr(start_buffer, '\n');
					}

					if( ini_buffer_end )
					{
						event->status = ARDRONE_CONTROL_EVENT_FINISH_SUCCESS;
					}
					else if( ini_buffer_more )
					{
						// Compute number of bytes to copy
						int32_t size = (int32_t)&ini_buffer[buffer_size] - (int32_t)start_buffer;
						vp_os_memcpy( &ini_buffer[0], start_buffer, size );
						ini_buffer_index = size;
					}
				}
			}
         else
         {
            if(!(ardrone_state & ARDRONE_COMMAND_MASK))
				   set_state(event, CONFIG_REQUEST_INI);
         }
			break;

		default:
         res = C_FAIL;
			break;
	}

	return res;
}
