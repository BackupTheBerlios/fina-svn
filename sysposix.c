#include <sys/types.h>
#include <sys/mman.h>

#include <errno.h>
#include <stdio.h>
#include <string.h>
#include <termios.h>


#include "sys.h"

struct termios otio;
int argc;
char ** argv;

void Sys_Init(int argcc, char ** argvv)
{
        struct termios tio;
        tcgetattr(fileno(stdin), &otio);
        tio = otio;
        tio.c_lflag &= ~(ECHO | ICANON);
        tcsetattr(fileno(stdin), TCSANOW, &tio);
        argc = argcc;
        argv = argvv;
}

void Sys_End()
{
        tcsetattr(fileno(stdin), TCSANOW, &otio);
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

void * Sys_OpenFile(const char * name, unsigned mode)
{
        char *modestr[]={"r", "rb", "r+", "r+b", "w", "wb", "BAD"};
        if (mode > 5)
                mode = 6;
        return fopen(name, modestr[mode]);
}

unsigned Sys_FileThrow(void * handle)
{
        unsigned ret = 0;
        switch ((int)handle)
        {
        case -1: // No error
                break;
        case 0: // errno error
                if (errno)
                        ret = -37;
                break;
        default: // FileRead/FileWrite error, check ferror
                if (ferror(((FILE*)handle)))
                        ret = -37;
                clearerr(((FILE*)handle));
        }
        return ret;
}

unsigned Sys_CloseFile(void * handle)
{
        return fclose(handle)? 0 : -1;
}

unsigned Sys_ReadFile(void * handle, char * buf, unsigned len)
{
        return fread(buf, 1, len, handle);
}

unsigned Sys_WriteFile(void * handle, char * buf, unsigned len)
{
        return fwrite(buf, 1, len, handle);
}

void * Sys_MMapFile(void * handle)
{
        void * ret = mmap(0, 0x40000000, PROT_READ, 0, 
                          fileno((FILE*)handle), 0);
        if (ret == (void*)-1)
                perror("mmap");
        return ret;
}
