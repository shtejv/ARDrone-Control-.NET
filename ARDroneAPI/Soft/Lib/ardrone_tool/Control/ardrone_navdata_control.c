#include <ardrone_tool/Control/ardrone_navdata_control.h>
#include <ardrone_tool/Control/ardrone_control.h>

C_RESULT ardrone_navdata_control_init( void* data )
{
  return C_OK;
}

C_RESULT ardrone_navdata_control_process( const navdata_unpacked_t* const navdata )
{
  return ardrone_control_resume_on_navdata_received(navdata->ardrone_state);
}

C_RESULT ardrone_navdata_control_release( void )
{
  return C_OK;
}
