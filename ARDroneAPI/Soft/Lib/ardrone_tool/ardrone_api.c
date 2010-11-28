
#include <VP_Os/vp_os_malloc.h>

#include <config.h>
#include <at_msgs_ids.h>
#include <ardrone_api.h>
#include <Maths/maths.h>
#include <ardrone_tool/ardrone_tool.h>

#undef ARDRONE_CONFIG_KEY_IMM
#undef ARDRONE_CONFIG_KEY_REF
#undef ARDRONE_CONFIG_KEY_STR
#define ARDRONE_CONFIG_KEY_IMM(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#define ARDRONE_CONFIG_KEY_REF(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#define ARDRONE_CONFIG_KEY_STR(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK)
#include <config_keys.h> // must be included before to have types

#undef ARDRONE_CONFIG_KEY_IMM
#undef ARDRONE_CONFIG_KEY_REF
#undef ARDRONE_CONFIG_KEY_STR
#define ARDRONE_CONFIG_KEY_IMM(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) DEFAULT,
#define ARDRONE_CONFIG_KEY_REF(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) DEFAULT,
#define ARDRONE_CONFIG_KEY_STR(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) DEFAULT,
static ardrone_config_t ardrone_config = {
#include <config_keys.h>
};

#undef ARDRONE_CONFIG_KEY_IMM
#undef ARDRONE_CONFIG_KEY_REF
#undef ARDRONE_CONFIG_KEY_STR
#define ARDRONE_CONFIG_KEY_IMM(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) \
C_TYPE get_##NAME(void) \
{ \
  return ardrone_config.NAME; \
} \
C_RESULT set_##NAME(C_TYPE val) \
{ \
  C_RESULT res = C_OK; \
  ardrone_config.NAME = val; \
  return res; \
}
#define ARDRONE_CONFIG_KEY_REF(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) \
C_TYPE_PTR get_##NAME(void) \
{ \
  return &ardrone_config.NAME; \
} \
C_RESULT set_##NAME(C_TYPE_PTR val) \
{ \
  C_RESULT res = C_FAIL; \
  if(val) \
    { \
      res = C_OK; \
      vp_os_memcpy(&ardrone_config.NAME, val, sizeof(C_TYPE)); \
    } \
  return res; \
}
#define ARDRONE_CONFIG_KEY_STR(KEY, NAME, INI_TYPE, C_TYPE, C_TYPE_PTR, RW, DEFAULT, CALLBACK) \
C_TYPE_PTR get_##NAME(void) \
{ \
  return &ardrone_config.NAME[0]; \
} \
C_RESULT set_##NAME(C_TYPE_PTR val) \
{ \
  C_RESULT res = C_FAIL; \
  if(val) \
    { \
      res = C_OK; \
      vp_os_memcpy(&ardrone_config.NAME[0], val, sizeof(C_TYPE)); \
    } \
  return res; \
}

// Generate all accessors functions
#include <config_keys.h>

