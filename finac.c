#include "fina.h"
#include "arch.h"
#include "sys.h"

// #define PROFILE_FORTH 1

#define FLAG(x) (x)? -1 : 0;

#define POPLL  ll  = (((long long)tos) << 32) | *(unsigned*)dsp++; tos = *dsp++
#define POPLL2 ll2 = (((long long)tos) << 32) | *(unsigned*)dsp++; tos = *dsp++
#define POPULL ull = (((unsigned long long)tos) << 32) \
                   | *(unsigned*)dsp++; tos = *dsp++
#define PUSHLL *--dsp = tos; *--dsp = ll; tos = ll>>32
#define PUSHULL *--dsp = tos; *--dsp = ull; tos = ull>>32

#define PRIM(x, n)  case n: asm(" .globl XT_" #x "\nXT_" #x ":"); { int unused
#define NEXTT goto **fpc++
#define NEXT (void) unused; } NEXTT; break
#define PUSH *--dsp = tos
#define RPUSH(reg) *--rsp = (CELL)(reg)
#define RPOP(reg) (CELL)(reg) = *rsp++;
#define POP tos = *dsp++

struct state {
        CELL * fpc;
        CELL * dsp;
        CELL * rsp;
        CELL   tos;
} _saved;
struct state volatile * volatile saved = &_saved;

#define BOOTSTRAP_STACK 16

static CELL bootstrap_dsp[BOOTSTRAP_STACK];
static CELL bootstrap_rsp[BOOTSTRAP_STACK];

static inline CELL *userP()
{
        extern CELL Forth_UserP;
        return (CELL*)(Forth_UserP + arch_callsize());
}

static inline unsigned upCase(unsigned uiVal)
{
        if (uiVal >= 'a' && uiVal <= 'z')
                uiVal += 'A' - 'a';
        return uiVal;
}

static inline CELL nCaseCompare(CELL p1, CELL p2, CELL len)
{
        CELL c1 = 0, c2 = 0;
        while (len-- && c1 == c2)
        {
                c1 = upCase(*(char*)p1++);
                c2 = upCase(*(char*)p2++);
        }
        return c1 == c2? 0 : c1 < c2? -1 : 1;
}

static inline CELL * searchWordlist(CELL wordList, CELL uiLen, CELL pAddr)
{
        static CELL ret[3];
        ret[0] = 0;
        wordList = *(CELL*)wordList;
        while (wordList)
        {
                unsigned uiLex = *(unsigned char *)wordList;
                unsigned uiLen2 = 31 & uiLex;
                if (uiLen == uiLen2 &&
                    0 == nCaseCompare(pAddr, wordList + 1, uiLen))
                {
                        ret[0] = 0x40 & uiLex? 1 : -1;
                        ret[1] = 0x20 & uiLex? 0 : -1;
                        ret[2] = ((CELL *)wordList)[-2];
                        break;
                }
                wordList = ((CELL*)wordList)[-1];
        }
        return ret;
}

#if defined(HAS_FILES)
static char * zstr(const char * str, unsigned len)
{
        static char res[512];
        len &= 511;
        Sys_MemMove(res, str, len);
        res[len] = 0;
        return res;
}

static char * zstr2(const char * str, unsigned len)
{
        static char res[512];
        len &= 511;
        Sys_MemMove(res, str, len);
        res[len] = 0;
        return res;
}
#endif  // HAS_FILES

static unsigned strLen(const char * str)
{
    const char * ostr = str;
    while (*str++) ;
    return str - ostr - 1;
}

static inline unsigned UMSlashMod(unsigned long long u, unsigned v, 
                                  unsigned * pmod)
{
        int i = 8*sizeof(int), c = 0;
        unsigned q = 0, h = u >> (8*sizeof(int)), l = u;
#define HIGHBIT(x) (((unsigned)(x))>>(8*sizeof(int)-1))
        for (;;)
        {
                if (c || h >= v)
                {
                        q++;
                        h -= v;
                }
                if (--i < 0)
                        break;
                c = HIGHBIT (h);
                h <<= 1;
                h += HIGHBIT (l);
                l <<= 1;
                q <<= 1;
        }
        *pmod = h;
        return q;
}

int FINA_Init(int argc, char ** argv)
{
        extern CELL Forth_Entry;
        Sys_Init(argc, argv);
        
        saved->fpc = (CELL*)(Forth_Entry + arch_callsize());
        saved->dsp = bootstrap_dsp+BOOTSTRAP_STACK;
        saved->rsp = bootstrap_rsp+BOOTSTRAP_STACK;
        saved->tos = 0;
        return 0;
}

void FINA_End()
{
        Sys_End();
}


static int prims() PRIMSATTR;


volatile CELL foo;

int FINA_InternalTick(int throw)
{
        // This crap is a hack to force saving previous register 
        // values in the stack
        CELL * savedfpc = fpc;
        CELL * savedrsp = rsp;
        CELL * saveddsp = dsp;
        CELL   savedtos = tos;
        int ret;
        
        if (throw)
        {
                fpc = (CELL*)(arch_callsize() + **(CELL**)userP());
                rsp = bootstrap_rsp + BOOTSTRAP_STACK;
                dsp = bootstrap_dsp + BOOTSTRAP_STACK;
                tos = throw;
        }
        else
        {
                fpc = saved->fpc;
                rsp = saved->rsp;
                dsp = saved->dsp;
                tos = saved->tos;
        }
        ret = prims();
        fpc = savedfpc;
        rsp = savedrsp;
        dsp = saveddsp;
        tos = savedtos;
        return ret;
}

int FINA_Tick()
{
        return Sys_Tick();
}

static int prims()
{
        extern CELL Forth_Here;
        register CELL t0;
#if defined(MORE_PRIMS)
        long long ll, ll2;
#endif
#if defined(MORE_PRIMS) || defined(HAS_FILES)
        unsigned long long ull;
#endif 
        LNKREG;
        foo = -1;
        while (1) { switch (foo) {
                // DON'T MOVE THIS
		PRIM(NOOP,-1);
		NEXT;

                PRIM(DOLIT,0);
                PUSH;
                tos = *fpc++;
                NEXT;
                
                PRIM(DOCONST,1);
                PUSH;
                tos = *lnk;
                NEXT;
                
                PRIM(DOVALUE,2);
                PUSH;
                tos = *lnk;
                NEXT;
                
                PRIM(DOVAR,3);
                PUSH; 
                tos = (CELL)lnk;
                NEXT;
                
                PRIM(DOCREATE,4);
                PUSH;
                tos = sizeof(CELL) + (CELL)lnk;
                goto **(CELL**)lnk;
                NEXT;
                
                PRIM(DOUSER,6);
                PUSH;
                tos = *userP() + *lnk;
                NEXT;
                
                PRIM(DOLIST,7);
                RPUSH(fpc);
                fpc = (CELL*)lnk;
#if defined(PROFILE_FORTH)
                fpc[-2]++; // XXX No ira en pc
#endif    
                NEXT;
                
                PRIM(RPFETCH,8);
                PUSH;
                tos = (CELL)rsp;
                NEXT;
                
                PRIM(RPSTORE,9);
                rsp = (CELL*)tos;
                POP;
                NEXT;
                
                PRIM(SPFETCH,10);
                PUSH;
                tos = (CELL)dsp;
                NEXT;
                
                PRIM(SPSTORE,11);
                dsp = (CELL*)tos;
                POP;
                NEXT;
                
                PRIM(UMPLUS,12);
                *dsp += tos;
                tos = (unsigned)*dsp < (unsigned)tos;
                NEXT;
                
                PRIM(STORE,15);
                *(CELL*)tos = *dsp++;
                POP;
                NEXT;
                
                PRIM(ZEROLT,16);
                tos = FLAG(tos < 0);
                NEXT;
                
                PRIM(ZEROEQUALS,17);
                tos = FLAG(tos == 0);
                NEXT;
                
                PRIM(TWOSTAR,18);
                tos += tos;
                NEXT;
                
                PRIM(TWOSLASH,19);
                tos >>= 1;
                NEXT;
                
                PRIM(GTR,20);
                RPUSH(tos);
                POP;
                NEXT;
                
                PRIM(FETCH,21);
                tos = *(CELL*)tos;
                NEXT;
                
                PRIM(AND,22);
                tos &= *dsp++;
                NEXT;
                
                PRIM(CSTORE,23);
                *(unsigned char *)tos = *dsp++;
                POP;
                NEXT;
                
                PRIM(CFETCH,24);
                tos = *(unsigned char *)tos;
                NEXT;
                
                PRIM(DROP,25);
                POP;
                NEXT;
                
                PRIM(DUP,26);
                PUSH;
                NEXT;
                
                PRIM(EXECUTE,27);
                t0 = tos;
                POP;
                goto *t0;
                NEXT;
                
                PRIM(EXIT,28);
                RPOP(fpc);
                NEXT;
                
                PRIM(MOVE,29);
                SAVESP;
                Sys_MemMove((char*)dsp[0], (char*)dsp[1], tos);
                RESTORESP;
                dsp += 2;
                POP;
                NEXT;
                
                PRIM(OR,30);
                tos |= *dsp++;
                NEXT;
                
                PRIM(OVER,31);
                PUSH;
                tos = dsp[1];
                NEXT;
                
                PRIM(RGT,32);
                PUSH;
                RPOP(tos);
                NEXT;
                
                PRIM(I, 92);
                PUSH;
                tos = *rsp;
                NEXT;
                
                PRIM(SWAP,34);
                t0 = tos;
                tos = dsp[0];
                dsp[0] = t0;
                NEXT;
                
                PRIM(XOR,35);
                tos ^= *dsp++;
                NEXT;
                
                PRIM(BRANCH,36);
                (char*)fpc += *fpc;
                NEXT;
                
                PRIM(DONEXT, 116);
                if (rsp[0]--)
                        (char*)fpc += *fpc;
                else
                        fpc++;
                NEXT;
                
                PRIM(DOLOOP,37);
                if (++rsp[0] < rsp[1])
                        (char*)fpc += *fpc;
                else
                        fpc++;
                NEXT;
                
                PRIM(DOPLUSLOOP,38);
                t0 = rsp[0] - rsp[1];
                rsp[0] += tos;
                if ((t0 ^ (t0 + tos)) >= 0 || (t0 ^ tos) >= 0)
                        (char*)fpc += *fpc;
                else
                        fpc++;
                POP;
                NEXT;
                
                PRIM(RXQ,66);
                PUSH;
                SAVESP;
                tos = Sys_HasChar();
                RESTORESP;
                NEXT;
                
                PRIM(RXFETCH,39);
                PUSH;
                SAVESP;
                tos = Sys_GetChar(); 
                RESTORESP;
                NEXT;
                
                PRIM(TXQ,40);
                PUSH;
                tos = -1;
                NEXT;
                
                PRIM(TXSTORE,41);
                SAVESP;
                Sys_PutChar(tos);
                RESTORESP;
                POP;
                NEXT;
                
                PRIM(ZEROBRANCH,42);
                if (tos == 0)
                        (char*)fpc += *fpc;
                else
                        fpc++;
                POP;
                NEXT;
                
                PRIM(QDODEFINE,44);
                PUSH;
                if (arch_iscall(tos))
                {
                        *dsp += arch_callsize();
                        tos = arch_calledby(tos);
                }
                else
                        tos = 0;
                NEXT;
                
                PRIM(XTCOMMA,45);
                t0 = *(CELL*)(Forth_Here + arch_callsize());
                t0 += sizeof(CELL)-1;
                t0 &= -sizeof(CELL);
                *(CELL*)(Forth_Here + arch_callsize()) = t0 + arch_callsize();
                arch_xtstore(tos, t0);
                tos = t0;
                NEXT;
                
                PRIM(ENDTICK,55);
                saved->tos = tos;
                saved->rsp = rsp;
                saved->dsp = dsp;
                saved->fpc = fpc;
                return 0;
                NEXT;
                
                PRIM(BYE,56);
                return 1;
                NEXT;
                
                PRIM(CALL0,57);
//                typedef unsigned (*CALL0)();
                SAVESP;
                tos = ((unsigned(*)())tos)();
                RESTORESP;
                NEXT;
                
                PRIM(CALL1,58);
//                typedef unsigned (*CALL1)(unsigned);
                SAVESP;
                tos = ((unsigned(*)())tos)(dsp[0]);
                RESTORESP;
                dsp++;
                NEXT;
                
                PRIM(CALL2,59);
//                typedef unsigned (*CALL2)(unsigned, unsigned);
                SAVESP;
                tos = ((unsigned(*)())tos)(dsp[0], dsp[1]);
                RESTORESP;
                dsp += 2;
                NEXT;
                
                PRIM(CALL3,60);
//                typedef unsigned (*CALL3)(unsigned, unsigned, 
//                                          unsigned);
                SAVESP;
                tos = ((unsigned(*)())tos)(dsp[0], dsp[1], dsp[2]);
                RESTORESP;
                dsp += 3;
                NEXT;
                
                PRIM(CALL4,61);
//                typedef unsigned (*CALL4)(unsigned, unsigned, 
//                                          unsigned, unsigned);
                SAVESP;
                tos = ((unsigned(*)())tos)(dsp[0], dsp[1], dsp[2], dsp[3]);
                RESTORESP;
                dsp += 4;
                NEXT;
                
                PRIM(SAMEQ,62);
                SAVESP;
                tos = nCaseCompare(dsp[0], dsp[1], tos);
                RESTORESP;
                dsp += 2;
                NEXT;

                PRIM(RFETCH,33);
                PUSH;
                tos = *rsp;
                NEXT;

#if defined(MORE_PRIMS)
                PRIM(DOTO,5);
                ((CELL*)*fpc++)[arch_callsize() / sizeof(CELL)] = tos;
                POP;
                NEXT;
                
                
                PRIM(PARENSEARCH_WORDLIST,65);
                CELL * ret;
                SAVESP;
                ret = searchWordlist(tos, dsp[0], dsp[1]);
                RESTORESP;
                dsp += 2;
                tos = ret[0];
                if (tos)
                {
                        tos = ret[2];
                        PUSH;
                        tos = ret[1];
                        PUSH;
                        tos = ret[0];
                }
                NEXT;
                
                PRIM(UMSLASHMOD,67);
#if 1
                t0 = tos;
                POP;
                POPULL;
                PUSH;
                PUSH;
                SAVESP;
                tos = UMSlashMod(ull, t0, dsp);
                RESTORESP;
#else
                t0 = tos;
                POP;
                POPLL;
                PUSH;
                tos = ((unsigned long long)ll) % t0;
                PUSH;
                tos = ((unsigned long long)ll) / t0;
#endif
                NEXT;
                
                PRIM(ALIGNED, 68);
                tos += sizeof(CELL) - 1;
                tos &= -sizeof(CELL);
                NEXT;
                
                PRIM(ULT, 69);
                tos = FLAG(((unsigned)*dsp++) < (unsigned) tos);
                NEXT;
                
                PRIM(PLUS, 71);
                tos += *dsp++;
                NEXT;
                
                PRIM(CHARPLUS, 72);
                tos++;
                NEXT;
                
                PRIM(CELLS, 73);
                tos *= sizeof(CELL);
                NEXT;
                
                PRIM(COUNT, 74);
                *--dsp = tos + 1;
                tos = *(unsigned char*)tos;
                NEXT;
                
                PRIM(QDUP, 75);
                if (tos)
                        PUSH;
                NEXT;
                
                PRIM(NIP, 76);
                dsp++;
                NEXT;
                
                PRIM(DNEGATE, 78);
                POPLL;
                ll = -ll;
                PUSHLL;
                NEXT;
                
                PRIM(ROT, 79);
                t0 = tos;
                tos = dsp[1];
                dsp[1] = dsp[0];
                dsp[0] = t0;
                NEXT;
                
                PRIM(ONEPLUS, 80);
                tos++;
                NEXT;
                
                PRIM(ONEMINUS, 81);
                tos--;
                NEXT;
                
                PRIM(CELLMINUS, 82);
                tos -= sizeof(CELL);
                NEXT;
                
                PRIM(TWODUP, 83); 
                t0 = *dsp;
                *--dsp = tos;
                *--dsp = t0;
                NEXT;
                
                PRIM(DODO, 85);
                RPUSH(*dsp++);
                RPUSH(tos);
                POP;
                NEXT;
                
                PRIM(MINUS, 86);
                tos = *dsp++ - tos;
                NEXT;
                
                PRIM(GT, 87);
                tos = FLAG(*dsp++ > tos);
                NEXT;
                
                PRIM(TWOSTORE, 88);
                ((CELL*)tos)[0] = *dsp++;
                ((CELL*)tos)[1] = *dsp++;
                POP;
                NEXT;
                
                PRIM(EQUALS, 89);
                tos = FLAG(tos == *dsp++);
                NEXT;
                
                PRIM(TWODROP, 90);
                dsp++;
                POP;
                NEXT;
                
                PRIM(TWOSWAP, 91);
                t0 = tos;
                tos = dsp[1];
                dsp[1] = t0;
                t0 = dsp[0];
                dsp[0] = dsp[2];
                dsp[2] = t0;
                NEXT;
                
                PRIM(UNLOOP, 93);
                rsp += 2;
                NEXT;
                
                PRIM(LT, 94);
                tos = FLAG(*dsp++ < tos);
                NEXT;
                
                PRIM(PLUSSTORE, 95);
                *(CELL*)tos += *dsp++;
                POP;
                NEXT;
                
                PRIM(SGTD, 96);
                PUSH;
                tos >>= 31;
                NEXT;
                
                PRIM(TWOFETCH, 97);
                *--dsp=((CELL*)tos)[1];
                tos = ((CELL*)tos)[0];
                NEXT;
                
                PRIM(WITHIN, 98);
                tos = FLAG(((unsigned)dsp[1] - (unsigned)dsp[0]) 
                           < ((unsigned)tos - (unsigned)dsp[0]));
                dsp += 2;
                NEXT;
                
                PRIM(UMSTAR, 99);
                unsigned long long t0 = (unsigned)tos;
                unsigned long long t1;
                POP;
                t1 = (unsigned)tos;
                ull = t1 * t0;
                POP;
                PUSHULL;
                NEXT;
                
                PRIM(DPLUS, 100);
                POPLL;
                POPLL2;
                ll += ll2;
                PUSHLL;
                NEXT;
                
                PRIM(ABS, 101);
                tos = tos > 0? tos : -tos;
                NEXT;
                
                PRIM(NEGATE, 102);
                tos = -tos;
                NEXT;
                
                PRIM(INVERT, 103);
                tos = ~tos;
                NEXT;
                
                PRIM(PICK, 104);
                tos = dsp[tos];
                NEXT;
                
                PRIM(RSHIFT, 105);
                tos = ((unsigned)*dsp++) >> tos;
                NEXT;
                
                PRIM(MAX, 106);
                tos = tos > *dsp? tos : *dsp;
                dsp++;
                NEXT;
                
                PRIM(MIN, 107);
                tos = tos < *dsp? tos : *dsp;
                dsp++;
                NEXT;
                
                PRIM(MSTAR, 108);
                ll = tos;
                POP;
                ll *= tos;
                POP;
                PUSHLL;
                NEXT;
                
                PRIM(FILL, 109);
                SAVESP;
                Sys_MemSet((char*)dsp[1], tos, dsp[0]);
                RESTORESP;
                dsp += 2;
                POP;
                NEXT;
                
                PRIM(STAR, 110);
                tos *= *dsp++;
                NEXT;
                
                PRIM(J, 111);
                PUSH;
                tos = rsp[2];
                NEXT;
                
                PRIM(CELLPLUS, 112);
                tos += sizeof(CELL);
                NEXT;
                
                PRIM(LSHIFT, 113);
                tos = *dsp++ << tos;
                NEXT;
                
                PRIM(TWOOVER, 115);
                PUSH;
                t0 = dsp[3];
                tos = dsp[2];
                *--dsp = t0;
                NEXT;
                
                PRIM(RDROP, 120);
                rsp++;
                NEXT;
                
		PRIM(ZEROLTGT, 121);
		tos = FLAG(tos);
		NEXT;

		PRIM(LTGT, 122);
		tos = FLAG(tos != *dsp++);
		NEXT;
                
//                PRIM(SLASH, 123);
//                tos = *dsp++ / tos;
//                NEXT;
#endif  // MORE_PRIMS
                
#if defined(HAS_FILES)
                PRIM(OPENF, 200);
                dsp[1] = (CELL)Sys_FileOpen(zstr((char*)dsp[1], dsp[0]), tos);
                dsp++;
                tos = Sys_Throw();
                NEXT;

                PRIM(CLOSEF, 201);
                Sys_FileClose((void*)tos);
                tos = Sys_Throw();
                NEXT;

                PRIM(READF, 202);
                dsp[1] = Sys_FileRead((void*)tos, (char*)dsp[1], dsp[0]);
                dsp++;
                tos = Sys_Throw();
                NEXT;
                
                PRIM(WRITEF, 203);
                Sys_FileWrite((void*)tos, (char*)dsp[1], dsp[0]);
                tos = Sys_Throw();
                dsp += 2;
                NEXT;

                PRIM(MMAPF, 204);
                tos = (CELL)Sys_FileMMap((void*)tos);
                PUSH;
                tos = Sys_Throw();
                NEXT;

                PRIM(SEEKF, 205);
                t0 = tos;
                POP;
                POPULL;
                Sys_FileSeek((void*)t0, ull);
                PUSH;
                tos = Sys_Throw();
                NEXT;
                
                PRIM(SIZEF, 206);
                t0 = tos;
                POP;
                ull = Sys_FileSize((void*)t0);
                PUSHULL;
                PUSH;
                tos = Sys_Throw();
                NEXT;

                PRIM(TELLF, 207);
                ull = Sys_FileTell((void*)tos);
                POP;
                PUSHULL;
                PUSH;
                tos = Sys_Throw();
                NEXT;
                        
                PRIM(LINEF, 208);
                dsp[1] = Sys_FileLine((void*)tos, (char*)dsp[1], dsp[0]);
                tos = Sys_Throw();
                dsp[0] = FLAG(tos != -39);
                tos = tos == -39? 0 : tos;
                NEXT;

                PRIM(DELETEF, 209);
                Sys_FileDelete(zstr((char*)*dsp++, tos));
                tos = Sys_Throw();
                NEXT;

                PRIM(STATF, 210);
                dsp[0] = Sys_FileStat(zstr((char*)dsp[0], tos));
                tos = Sys_Throw();
                NEXT;

                PRIM(RENF, 211);
                tos = (CELL)zstr2((char*)dsp[0], tos);
                Sys_FileRen(zstr((char*)dsp[2], dsp[1]), (char*)tos);
                tos = Sys_Throw();
                dsp += 3;
                NEXT;

                PRIM(TRUNCF, 212);
                t0 = tos;
                POP;
                POPULL;
                PUSH;
                Sys_FileTrunc((void*)t0, ull);
                tos = Sys_Throw();
                NEXT;

                PRIM(FLUSHF, 213);
                Sys_FileFlush((void*)tos);
                tos = Sys_Throw();
                NEXT;
                
#endif  // HASFILES

#if defined(HAS_ALLOCATE)
                PRIM(ALLOCATE, 250);
                tos = (CELL)Sys_MemAllocate(tos);
                PUSH;
                tos = Sys_Throw();
                NEXT;

                PRIM(FREE, 251);
                Sys_MemFree((void*)tos);
                POP;
                NEXT;

                PRIM(RESIZE, 252);
                dsp[0] = (CELL)Sys_MemResize((void*)dsp[0], tos);
                tos = Sys_Throw();
                NEXT;
#endif  // HAS_ALLOCATE

                PRIM(MS, 298);
                Sys_Sleep(tos);
                POP;
                NEXT;
                
                PRIM(TIMEANDDATE, 299);
                PUSH;
                tos = (CELL)Sys_Time();
                dsp -= 5;
                dsp[4] = Sys_Second((void*)tos);
                dsp[3] = Sys_Minute((void*)tos);
                dsp[2] = Sys_Hour((void*)tos);
                dsp[1] = Sys_Day((void*)tos);
                dsp[0] = Sys_Month((void*)tos);
                tos = Sys_Year((void*)tos);
                NEXT;

                PRIM(ARGC,300);
                PUSH;
                tos = Sys_Argc();
                NEXT;

                // DON'T MOVE THIS
                PRIM(ARGV,301);
                PUSH;
                *dsp = (CELL)Sys_Argv(tos);
                tos = strLen((char*)*dsp);
                NEXT;

        }NEXTT;}
}
