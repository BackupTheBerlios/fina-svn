typedef int CELL;

#define PRIMSATTR  

CELL * rsp;
register CELL * fpc asm("%esi");
register CELL * dsp asm("%edi");
register CELL   tos asm("%ebx");
static inline CELL * getlnk()
{
	CELL * res;
        asm volatile (" popl %0 " : "=r" (res));
        return res;
}

static inline CELL getsp()
{
        register CELL   sp asm("%esp");
        CELL   osp;
        asm volatile (" mov %1,%0 " : "=r" (osp) : "r" (sp) );
        return osp;
}

static inline void setsp(CELL osp)
{
        register CELL   sp asm("%esp");
        asm volatile (" mov %1,%0 " : "=r" (sp) : "r" (osp) );
}
#define SAVESP { CELL savedsp = getsp()
#define RESTORESP setsp(savedsp);}

static inline CELL arch_iscall(CELL xt)
{
        return (*(CELL*)xt) == 0xe8909090;
}

static inline CELL arch_callsize()
{
        return 8;
}

static inline CELL arch_calledby(CELL xt)
{
        return xt + 3 + 5 + ((CELL*)xt)[1];
}

static inline void arch_xtstore(CELL xt, CELL pdict)
{
        ((CELL*)pdict)[0] = 0xe8909090;
        ((CELL*)pdict)[1] = xt - (pdict + 3 + 5);
}

