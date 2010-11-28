ifdef ARDRONE_CUSTOM_CONFIG 
   ifdef ARDRONE_BUILD_CONFIG
       include $(ARDRONE_CUSTOM_CONFIG)
       include $(ARDRONE_BUILD_CONFIG)
   else
       include ../../Build/custom.makefile
       include ../../Build/config.makefile
   endif
else
 include ../../Build/custom.makefile
 include ../../Build/config.makefile
endif

LIB_ID=ardrone
ifeq ($(USE_ARDRONE_TOOL),no)
LIB_ID:=$(LIB_ID)_notool
endif
GENERIC_TARGET_LIBRARY:=lib$(TARGET)$(LIB_ID).a

GENERIC_CFLAGS+=-DAT_MESSAGES_HEADER="\"$(COMMON_DIR)/at_msgs.h\""
GENERIC_CFLAGS+=-DUSE_NEW_ATCODEC

ifeq ($(USE_ARDRONE_MAINLOOP),no)
GENERIC_CFLAGS+=-DNO_ARDRONE_MAINLOOP
endif
  
ifeq ("$(USE_CHECK_WIFI_CONFIG)","yes")
   GENERIC_CFLAGS+=-DCHECK_WIFI_CONFIG
endif

SDK_FLAGS+="USE_APP=no"
SDK_FLAGS+="USE_LIB=yes"
SDK_FLAGS+="LIB_ID=ardrone_lib"

MATHS_DIR=Maths
CONTROL_DIR=Control
ARDRONE_TOOL_DIR=ardrone_tool
INIPARSER_DIR=iniparser3.0b/src
LIBCALIBRATION_DIR=libCalibration

GENERIC_LIBRARY_SOURCE_FILES+=			\
	$(INIPARSER_DIR)/iniparser.c			\
	$(INIPARSER_DIR)/dictionary.c			\
	$(MATHS_DIR)/filter.c							\
	$(MATHS_DIR)/maths.c							\
	$(MATHS_DIR)/matrices.c						\
	$(MATHS_DIR)/matrix3d.c						\
	$(MATHS_DIR)/time.c							  \
	$(MATHS_DIR)/vision_math.c		    \
	$(ARDRONE_TOOL_DIR)/Navdata/navdata.c	    

ifneq ($(CONTROL_DLL),yes)
   ifeq ($(USE_ARDRONE_TOOL),yes)
      ifeq ($(PC_USE_POLARIS),yes)
         GENERIC_CFLAGS+=-DPC_USE_POLARIS
      endif
GENERIC_LIBRARY_SOURCE_FILES+=					       \
	$(ARDRONE_TOOL_DIR)/AT/ardrone_at.c			       \
	$(ARDRONE_TOOL_DIR)/Control/ardrone_control_soft_update.c       \
	$(ARDRONE_TOOL_DIR)/Control/ardrone_control_configuration.c     \
	$(ARDRONE_TOOL_DIR)/Control/ardrone_control_ack.c     	       \
	$(ARDRONE_TOOL_DIR)/Navdata/ardrone_navdata_file.c	       \
	$(ARDRONE_TOOL_DIR)/UI/ardrone_input.c			       \
	$(ARDRONE_TOOL_DIR)/UI/ardrone_tool_ui.c			       \
	$(ARDRONE_TOOL_DIR)/ardrone_time.c			       \
	$(ARDRONE_TOOL_DIR)/ardrone_api.c			       \
	$(ARDRONE_TOOL_DIR)/ardrone_tool.c			       \
	$(ARDRONE_TOOL_DIR)/Com/config_wifi.c

     ifneq ($(USE_MINGW32),yes)
       GENERIC_LIBRARY_SOURCE_FILES+=				\
	      $(ARDRONE_TOOL_DIR)/Video/video_com_stage.c		\
	      $(ARDRONE_TOOL_DIR)/Control/ardrone_control.c		\
	      $(ARDRONE_TOOL_DIR)/Control/ardrone_navdata_control.c	\
	      $(ARDRONE_TOOL_DIR)/Navdata/ardrone_navdata_client.c	

         ifeq ($(USE_IPHONE),no)
            GENERIC_LIBRARY_SOURCE_FILES+=				\
               $(ARDRONE_TOOL_DIR)/Com/config_serial.c			
          endif
     endif
endif

ifneq ($(USE_MINGW32),yes)
  GENERIC_LIBRARY_SOURCE_FILES+=				\
	$(ARDRONE_TOOL_DIR)/Video/video_stage_recorder.c		
endif

ifeq ($(USE_ELINUX),yes)

HARDWARE_ARDRONE:=$(SRC_PATH)/Soft/Toy/Sources/$(CARD_HARDWARE_DIR)

   ifeq ($(USE_MINGW32),yes)
      HARDWARE_ARDRONE_CALIBRATION:=c:/$(CARD_HARDWARE_DIR)
   else
      HARDWARE_ARDRONE_CALIBRATION:=$(HARDWARE_ARDRONE)
   endif

GENERIC_CFLAGS+=-DHARDWARE_ARDRONE=\"$(HARDWARE_ARDRONE)\" \
		-DHARDWARE_ARDRONE_CALIBRATION=\"$(HARDWARE_ARDRONE_CALIBRATION)\"
endif

endif

BASE_RELATIVE_PATH_FROM_SDK=../../

GENERIC_INCLUDES+=					\
       -I$(BASE_RELATIVE_PATH_FROM_SDK)/Soft/Common   \
       -I$(BASE_RELATIVE_PATH_FROM_SDK)/Soft/Lib

export GENERIC_INCLUDES
export GENERIC_LIBRARY_SOURCE_DIR=$(BASE_RELATIVE_PATH_FROM_SDK)/Soft/Lib
export GENERIC_BINARIES_SOURCE_DIR=$(BASE_RELATIVE_PATH_FROM_SDK)/Soft/Lib

export GENERIC_CFLAGS
export GENERIC_LIBRARY_SOURCE_FILES
export GENERIC_TARGET_LIBRARY

export GENERIC_BINARIES_COMMON_SOURCE_FILES=
export GENERIC_BINARIES_SOURCE_ENTRYPOINTS=

all $(MAKECMDGOALS):
	@$(MAKE) -C ../../../VP_SDK/Build $(TMP_SDK_FLAGS) $(SDK_FLAGS) $(MAKECMDGOALS)

