/**
 * @file thread.c
 * @author aurelien.morelle@parrot.fr
 * @date 2006/12/15
 */

#include <stdarg.h>
#include <signal.h>
#include <sched.h>

#include <VP_Os/vp_os_types.h>
#include <VP_Os/vp_os_thread.h>
#include <VP_Os/vp_os_signal.h>
#include <VP_Os/vp_os_malloc.h>

#define SIGRESUME  SIGUSR1
#define SIGSUSPEND SIGUSR2

typedef struct _pthread_data_t
{
  THREAD_HANDLE   handle;
  pthread_attr_t  attr;
  uint32_t        isSleeping;
} pthread_data_t;

static const THREAD_HANDLE NULL_THREAD_HANDLE = 0;

static uint32_t threadTabSize    = 128;
//static uint32_t numCreatedThread = 0;
static pthread_data_t* threadTab = NULL;

static pthread_once_t once;
static vp_os_mutex_t thread_mutex;


static void suspendSignalHandler(int sig)
{
  sigset_t signal_set;

  /* Block all signals except PTHREAD_RESUME while suspended. */
  sigfillset(&signal_set);
  sigdelset(&signal_set, SIGRESUME);
  sigsuspend(&signal_set);
}

static void resumeSignalHandler(int sig)
{
}

static void init_thread(void)
{
  struct sigaction sigsuspend, sigresume;

  vp_os_mutex_init(&thread_mutex);

  sigresume.sa_flags = 0;
  sigsuspend.sa_flags = 0;

  sigemptyset(&sigresume.sa_mask);
  sigemptyset(&sigsuspend.sa_mask);
  sigaddset(&sigsuspend.sa_mask, SIGSUSPEND);
  sigaddset(&sigresume.sa_mask, SIGRESUME);

  sigresume.sa_handler  = resumeSignalHandler;
  sigsuspend.sa_handler = suspendSignalHandler;

  sigaction(SIGRESUME,&sigresume,NULL);
  sigaction(SIGSUSPEND,&sigsuspend,NULL);
}

static pthread_data_t* findThread(THREAD_HANDLE handle)
{
  uint32_t i = 0;

  if(threadTab == NULL)
    return NULL;

  for(i = 0;i < threadTabSize;i++)
  {
    if( pthread_equal(handle,threadTab[i].handle) )
      return &threadTab[i];
  }

  return NULL;
}

void
vp_os_thread_create(THREAD_ROUTINE f, THREAD_PARAMS parameters, THREAD_HANDLE *handle, ...)
{
  pthread_data_t* freeSlot = NULL;

  pthread_once(&once,&init_thread);

  vp_os_mutex_lock(&thread_mutex);

  freeSlot = findThread( NULL_THREAD_HANDLE );
  while(freeSlot == NULL)
  {
    /* BUG Fix on 2010/07/19 : first half of the array was never initialized. */

	  if (threadTab!=NULL){
		/* Doubles the size of the array */
			threadTab = ( pthread_data_t* )vp_os_realloc( threadTab, 2 * threadTabSize * sizeof( pthread_data_t ) );
		/* Initializes the newly created second half */
			vp_os_memset( threadTab + threadTabSize, 0, threadTabSize * sizeof( pthread_data_t ) );
			threadTabSize *= 2;

	}else{
			threadTab = ( pthread_data_t* )vp_os_malloc( threadTabSize * sizeof( pthread_data_t ) );
			vp_os_memset( threadTab , 0, threadTabSize * sizeof( pthread_data_t ) );
	}


    freeSlot = findThread( NULL_THREAD_HANDLE );
  }

  pthread_attr_init( &freeSlot->attr );
  pthread_create( &freeSlot->handle, &freeSlot->attr, f, parameters);

  *handle = freeSlot->handle;

  vp_os_mutex_unlock(&thread_mutex);
}

void
vp_os_thread_join(THREAD_HANDLE handle)
{
  vp_os_mutex_lock(&thread_mutex);

  pthread_data_t* freeSlot = findThread( handle );

  if( freeSlot != NULL )
  {
    void *res;

    vp_os_memset(freeSlot, 0, sizeof(pthread_data_t));
    pthread_join( handle, &res);
  }

  vp_os_mutex_unlock(&thread_mutex);
}

THREAD_HANDLE
vp_os_thread_self(void)
{
  return pthread_self();
}

void
vp_os_thread_suspend(THREAD_HANDLE handle)
{
  vp_os_mutex_lock(&thread_mutex);

  pthread_data_t* freeSlot = findThread( handle );

  if( freeSlot != NULL )
  {
    if(!freeSlot->isSleeping)
    {
      freeSlot->isSleeping = 1;
      pthread_kill(handle,SIGSUSPEND);
    }
  }

  vp_os_mutex_unlock(&thread_mutex);
}

void
vp_os_thread_resume(THREAD_HANDLE handle)
{
  vp_os_mutex_lock(&thread_mutex);

  pthread_data_t* freeSlot = findThread( handle );

  if( freeSlot != NULL )
  {
    if(freeSlot->isSleeping)
    {
      pthread_kill(handle,SIGRESUME);
      freeSlot->isSleeping = 0;
    }
  }

  vp_os_mutex_unlock(&thread_mutex);
}

void
vp_os_thread_yield(void)
{
  sched_yield();
}

void
vp_os_thread_priority(THREAD_HANDLE handle, int32_t priority)
{
  vp_os_mutex_lock(&thread_mutex);

  pthread_data_t* freeSlot = findThread( handle );

  if( freeSlot != NULL )
  {
    int rc, policy = SCHED_OTHER;
    struct sched_param param;

    vp_os_memset(&param, 0, sizeof(param));

    rc = pthread_getschedparam(handle, &policy, &param);

    if( policy == SCHED_OTHER )
    {
      policy = SCHED_FIFO;
    }
    param.sched_priority = priority;
   
    rc = pthread_setschedparam(handle, policy, &param);
  }

  vp_os_mutex_unlock(&thread_mutex);
}
