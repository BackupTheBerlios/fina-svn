typedef int CELL;

#define PRIMSATTR __attribute__ ((section (".data")))

register CELL * rsp asm("$16");
register CELL * fpc asm("$17");
register CELL * dsp asm("$18");
register CELL   tos asm("$19");
#define LNKREG register volatile CELL * lnk asm("$31")
#define SAVESP
#define RESTORESP

#if defined(__linux__)
#include <sys/cachectl.h>
static inline void flush_cache(char * pdict, unsigned bytes)
{
	cacheflush((char*)pdict, bytes, BCACHE);
}
#else
#error Define flush_cache
#endif

static inline CELL arch_iscall(CELL xt)
{
        return (*(CELL*)xt & 0xffff0000) == 0x04110000;
}

static inline CELL arch_callsize()
{
        return 2*sizeof(CELL);
}

static inline CELL arch_calledby(CELL xt)
{
        CELL t = *(CELL*)xt;
        t &= 0xffff;
        t <<= 16;
        t >>= 14;
        t += xt+4;
        return t;
}

static inline void arch_xtstore(CELL xt, CELL pdict)
{
        xt -= pdict+4;
	xt >>= 2;
        xt &= 0xffff;
        xt |= 0x04110000;
        ((CELL*)pdict)[0] = xt;
        ((CELL*)pdict)[1] = 0;
	flush_cache(pdict, 8);
}
