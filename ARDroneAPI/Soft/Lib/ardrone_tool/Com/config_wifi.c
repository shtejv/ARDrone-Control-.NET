#include <VP_Os/vp_os_malloc.h>

#include <config.h>
#include <ardrone_tool/Com/config_com.h>

vp_com_t* wifi_com(void)
{
  static vp_com_t com = {
    VP_COM_WIFI,
    FALSE,
    0,
#ifdef _WIN32
    { 0 },
#else // ! USE_MINGW32
    PTHREAD_MUTEX_INITIALIZER,
#endif // ! USE_MINGW32
    NULL,
    NULL,
    0,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL
  };

  return &com;
}

vp_com_config_t* wifi_config(void)
{
  static vp_com_wifi_config_t config = {
    { 0 },
    WIFI_MOBILE_IP,
    WIFI_NETMASK,
    WIFI_BROADCAST,
    WIFI_GATEWAY,
    WIFI_SERVER,
    WIFI_INFRASTRUCTURE,
    WIFI_SECURE,
    WIFI_PASSKEY
  };

  return (vp_com_config_t*) &config;
}

vp_com_connection_t* wifi_connection(void)
{
  static vp_com_wifi_connection_t connection = {
    0,
    WIFI_NETWORK_NAME
  };

  return (vp_com_connection_t*) &connection;
}

void wifi_config_socket(vp_com_socket_t* socket, VP_COM_SOCKET_TYPE type, int32_t port, const char* serverhost)
{
  vp_os_memset(socket, 0, sizeof(vp_com_socket_t));

  socket->type           = type;
  socket->protocol       = VP_COM_TCP;
  socket->port           = port;

  if(serverhost && socket->type == VP_COM_CLIENT)
    strcpy(socket->serverHost, serverhost);
}
