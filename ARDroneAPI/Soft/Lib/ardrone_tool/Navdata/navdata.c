
#include <VP_Os/vp_os_types.h>
#include <VP_Os/vp_os_malloc.h>
#include <VP_Os/vp_os_print.h>

#include <ardrone_api.h>

uint32_t ardrone_navdata_compute_cks( uint8_t* nv, int32_t size )
{
  int32_t i;
  uint32_t cks;
  uint32_t temp;

  cks = 0;

  for( i = 0; i < size; i++ )
  {
    temp = nv[i];
    cks += temp;
  }

  return cks;
}

navdata_option_t* ardrone_navdata_search_option( navdata_option_t* navdata_options_ptr, uint32_t tag )
{
  uint8_t* ptr;

  while( navdata_options_ptr->tag != tag )
  {
    ptr  = (uint8_t*) navdata_options_ptr;
    ptr += navdata_options_ptr->size;

    navdata_options_ptr = (navdata_option_t*) ptr;
  }

  return navdata_options_ptr;
}

C_RESULT ardrone_navdata_unpack_all(navdata_unpacked_t* navdata_unpacked, navdata_t* navdata, uint32_t* cks)
{
  C_RESULT res;
  navdata_cks_t navdata_cks = { 0 };
  navdata_option_t* navdata_option_ptr;

  navdata_option_ptr = (navdata_option_t*) &navdata->options[0];

  vp_os_memset( navdata_unpacked, 0, sizeof(*navdata_unpacked) );

  navdata_unpacked->ardrone_state   = navdata->ardrone_state;
  navdata_unpacked->vision_defined  = navdata->vision_defined;

  res = C_OK;

  while( navdata_option_ptr != NULL )
  {
    // Check if we have a valid option
    if( navdata_option_ptr->size == 0 )
    {
      PRINT("One option is not a valid because its size is zero\n");
      navdata_option_ptr = NULL;
      res = C_FAIL;
    }
    else
    {
      switch( navdata_option_ptr->tag )
      {
        case NAVDATA_DEMO_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_demo );
          break;

        case NAVDATA_TIME_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_time );
          break;

        case NAVDATA_RAW_MEASURES_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_raw_measures );
          break;

        case NAVDATA_PHYS_MEASURES_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_phys_measures );
          break;

        case NAVDATA_GYROS_OFFSETS_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_gyros_offsets );
          break;

        case NAVDATA_EULER_ANGLES_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_euler_angles );
          break;

        case NAVDATA_REFERENCES_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_references );
          break;

        case NAVDATA_TRIMS_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_trims );
          break;

        case NAVDATA_RC_REFERENCES_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_rc_references );
          break;

        case NAVDATA_PWM_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_pwm );
          break;

        case NAVDATA_ALTITUDE_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_altitude );
          break;

        case NAVDATA_VISION_RAW_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_vision_raw );
          break;

        case NAVDATA_VISION_OF_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_vision_of );
          break;

        case NAVDATA_VISION_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_vision );
          break;

        case NAVDATA_VISION_PERF_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_vision_perf );
          break;

        case NAVDATA_TRACKERS_SEND_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_trackers_send );
          break;

        case NAVDATA_VISION_DETECT_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_vision_detect );
          break;

        case NAVDATA_WATCHDOG_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_watchdog );
          break;

        case NAVDATA_ADC_DATA_FRAME_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_unpacked->navdata_adc_data_frame );
          break;

        case NAVDATA_CKS_TAG:
          navdata_option_ptr = ardrone_navdata_unpack( navdata_option_ptr, navdata_cks );
          *cks = navdata_cks.cks;
          navdata_option_ptr = NULL; // End of structure
          break;

        default:
          PRINT("Tag %d is not a valid navdata option tag\n", (int) navdata_option_ptr->tag);
          navdata_option_ptr = NULL;
          res = C_FAIL;
          break;
      }
    }
  }

  return res;
}
