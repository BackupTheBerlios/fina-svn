#include "arch.h"
#include <sys/types.h>
#include <sys/mman.h>
#include <sys/stat.h>

#include <errno.h>
#include <setjmp.h>
#include <signal.h>
#include <stdio.h>
#include <string.h>
#include <time.h>
#include <termios.h>
#include <unistd.h>


#include "fina.h"
#include "sys.h"

static struct termios otio;
static int argc;
static char ** argv;
static int throw;
jmp_buf jmpbuf;

static void errnoThrow(int error)
{
        if (error) switch (errno)
        {
        case ENOENT:
                throw = -38;
                break;
        default:
                throw = -37;
        }
        if (!error)
                throw = 0;
        errno = 0;
}

static void ferrorThrow(int error, FILE * handle)
{
        if (error && feof(handle))
                throw = -39;
        if (error && ferror(handle))
                throw = -37;
        if (!error)
                throw = 0;
        clearerr(handle);
}

static void sighandler(int sig)
{
        switch (sig)
        {
        case SIGBUS:
        case SIGSEGV:
        case SIGILL:
                throw = -9;
                break;
        default:
                throw = -59;
        }
        signal(sig, sighandler);
//        printf("Got signal %d, throw %d\n", sig, throw);
        longjmp(jmpbuf, throw);
}

int Sys_Tick()
{
        static int set = 0;
        int throw;
        if (!set)
                throw = setjmp(jmpbuf);
        set = !throw;
        return FINA_InternalTick(throw);
}

static void initSignals()
{
        signal(SIGBUS, sighandler);
        signal(SIGSEGV, sighandler);
        signal(SIGILL, sighandler);
}

static void initTerm()
{
        struct termios tio;
        tcgetattr(fileno(stdin), &otio);
        tio = otio;
        tio.c_lflag &= ~(ECHO | ICANON);
        tcsetattr(fileno(stdin), TCSANOW, &tio);
}

static void endTerm()
{
        tcsetattr(fileno(stdin), TCSANOW, &otio);
}

void Sys_Init(int argcc, char ** argvv)
{
        argc = argcc;
        argv = argvv;
        initTerm();
        initSignals();
}

void Sys_End()
{
        endTerm();
}

unsigned Sys_HasChar()
{
        return -1;
}

unsigned Sys_GetChar() 
{
        return getchar();
}

void Sys_PutChar(unsigned c)
{
        putchar(c);
        fflush(stdout);
}

void Sys_MemMove(char * to, const char * from, unsigned bytes)
{
        memmove(to, from, bytes);
}

void Sys_MemSet(char * dst, unsigned val, unsigned bytes)
{
        memset(dst, val, bytes);
}

void * Sys_FileOpen(const char * name, unsigned mode)
{
        char *modestr[]={"r", "rb", "r+", "r+b", "w", "wb", "BAD"};
        void * handle;
        if (mode > 5)
                mode = 6;
        handle = fopen(name, modestr[mode]);
        errnoThrow(handle == 0);
        return handle;
}

unsigned Sys_FileThrow()
{
        return throw;
}

void Sys_FileClose(void * handle)
{
        errnoThrow(handle == 0);
        if (!throw) errnoThrow(0 != fclose(handle));
}

unsigned Sys_FileRead(void * handle, char * buf, unsigned len)
{
        unsigned res = 0;
        errnoThrow(handle == 0);
        if (!throw) res = fread(buf, 1, len, handle);
        if (!throw) ferrorThrow(1, handle);
        return res;
}

void Sys_FileWrite(void * handle, char * buf, unsigned len)
{
        unsigned res = 0;
        errnoThrow(handle == 0);
        if (!throw) res = fwrite(buf, 1, len, handle);
        if (!throw) ferrorThrow(res != len, handle);
}

void * Sys_FileMMap(void * handle)
{
        void * res = 0;
        errnoThrow(handle == 0);
        if (!throw) res = mmap(0, 0x40000000, PROT_READ, MAP_SHARED, 
                               fileno((FILE*)handle), 0);
        if (!throw) errnoThrow(res == (void*)-1);
        return res;
}

unsigned Sys_Argc()
{
        return argc;
}

char * Sys_Argv(unsigned i)
{
        return argv[i];
}

unsigned long long Sys_FileSize(void * handle)
{
        off_t prev = -1, res = -1;
        errnoThrow(handle == 0);
        if (!throw) errnoThrow(-1 == (prev = ftello(handle)));
        if (!throw) errnoThrow(-1 == fseeko(handle, 0, SEEK_END));
        if (!throw) errnoThrow(-1 == (res = ftello(handle)));
        if (!throw) errnoThrow(-1 == fseeko(handle, prev, SEEK_SET));
        return res;
}

void Sys_FileSeek(void * handle, unsigned long long pos)
{
        errnoThrow(handle == 0);
        if (!throw) errnoThrow(-1 == fseeko(handle, pos, SEEK_SET));
}

unsigned long long Sys_FileTell(void * handle)
{
        unsigned long long res = -1;
        errnoThrow(handle == 0);
        if (!throw) res = ftello(handle);
        if (!throw) errnoThrow(-1 == res);
        return res;
}

unsigned Sys_FileLine(void * handle, char * buf, unsigned size)
{
        unsigned res = 0;
        buf[size] = 0;
        errnoThrow(handle == 0);
        if (!throw) ferrorThrow(0 == fgets(buf, size+1, handle), handle);
        if (!throw) res = strlen(buf);
        if (!throw) res -= buf[res-1] == '\n';
        return res;
}

void * Sys_Time()
{
        time_t t = time(0);
        return localtime(&t);
}

unsigned Sys_Second(void * t)
{
        return ((struct tm *)t)->tm_sec;
}

unsigned Sys_Minute(void * t)
{
        return ((struct tm *)t)->tm_min;
}

unsigned Sys_Hour(void * t)
{
        return ((struct tm *)t)->tm_hour;
}

unsigned Sys_Day(void * t)
{
        return ((struct tm *)t)->tm_mday;
}

unsigned Sys_Month(void * t)
{
        return ((struct tm *)t)->tm_mon + 1;
}

unsigned Sys_Year(void * t)
{
        return ((struct tm *)t)->tm_year + 1900;
}

void Sys_Sleep(unsigned ms)
{
        usleep(ms * 1000);
}

void Sys_FileDelete(const char * name)
{
        errnoThrow(-1 == unlink(name));
}

unsigned Sys_FileStat(const char * name)
{
        struct stat s;
        errnoThrow(stat(name, &s));
        return s.st_mode;
}

void Sys_FileRen(const char * old, const char * new)
{
        printf("renaming %s to %s\n", old, new);
        errnoThrow(rename(old, new));
}

void Sys_FileTrunc(void * handle, unsigned long long size)
{
        errnoThrow(ftruncate(fileno((FILE*)handle), size));
}
