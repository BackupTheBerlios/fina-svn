#define CELL int

#define PRIMSATTR

register CELL * rsp asm("%r18");
register CELL * fpc asm("%r17");
register CELL * dsp asm("%r16");
register CELL   tos asm("%r15");
#define SAVESP
#define RESTORESP

static inline CELL * getlnk()
{
	register volatile CELL * lnk asm("%lr");
	return lnk;
}


static inline CELL arch_iscall(CELL xt)
{
        return (*(CELL*)xt & 0xfc000003) == 0x48000001;
}

static inline CELL arch_callsize()
{
        return 4;
}

static inline CELL arch_calledby(CELL xt)
{
        CELL t = *(CELL*)xt;
        t &= 0x3ffffffc;
        t <<= 6;
        t >>= 6;
        t += xt;
        return t;
}

static inline void arch_xtstore(CELL xt, CELL pdict)
{
        xt -= pdict;
        xt &= 0x03fffffc;
        xt |= 0x48000001;
        *(CELL*)pdict = xt;
        asm volatile("dcbst 0,%0\n sync\n icbi 0,%0\n sync\n isync\n"
                     ::"r"(pdict));
}
