CC = `cat compiler`
HOST = `cat hostforth`
CPPFLAGS = `cat flags`
CFLAGS = -g -O2 -Wall 

COMMON_OBJECTS = main.o sys.o

ALL_HELP = ${ALL_FORTH:%.fs=help/%.help}


FINA_SRC0 = opt.fs tconfig.fs
FINA_SRC1 = meta.fs fina.fs
HOST_FINA0 = core.fs coreext.fs throwmsg.fs
HOST_FINA1 = host-fina.fs
HOST_GFORTH = host-gforth.fs
FINA_TEST = core.fs throwmsg.fs tester.fs coretest.fs postponetest.fs filetest bye.fs
RUN_FINA = core.fs coreext.fs throwmsg.fs file.fs \
   double.fs optional.fs string.fs require.fs toolsext.fs search.fs \
   lineedit.fs help.fs multi.fs
SAVE_FINA = ${RUN_FINA} savefina.fs bye.fs

ALL_FORTH = fina.fs ${SAVE_FINA}


fina: fina2 ${SAVE_FINA}
	cat ${SAVE_FINA} | ./fina2 
	chmod 755 fina

fina2: ${COMMON_OBJECTS} fina2.o

fina1: ${COMMON_OBJECTS} fina1.o

fina0: ${COMMON_OBJECTS} 

run: fina
	cat ${RUN_FINA} - | ./fina

test: fina2
	diff fina2 fina1
	cat ${FINA_TEST} | ./$<

test0: fina0
	cat ${FINA_TEST} | ./$<

finas2.s: fina1 ${HOST_FINA0} ${FINA_SRC0} ${HOST_FINA1} ${FINA_SRC1}
	cat ${HOST_FINA0} ${FINA_SRC0} ${HOST_FINA1} ${FINA_SRC1} | ./$< > $@

finas1.s: fina0 ${HOST_FINA0} ${FINA_SRC0} ${HOST_FINA1} ${FINA_SRC1}
	cat ${HOST_FINA0} ${FINA_SRC0} ${HOST_FINA1} ${FINA_SRC1} | ./$< > $@

finas0.s: ${FINA_SRC0} ${HOST_GFORTH} ${FINA_SRC1}
	`cat hostforth` ${FINA_SRC0} ${HOST_GFORTH} ${FINA_SRC1} > $@

# Glossaries


doc: help/toc.help

help/toc.help: fina maketoc.fs ${ALL_HELP}
	./fina maketoc.fs -e "toc{ ${ALL_HELP} }toc bye" > $@

${ALL_HELP}: ${ALL_FORTH} 

help/%.help: %.fs
	./fina glosgen.fs -e "newglos makeglos $< writeglos $@ bye"

main.o : main.c

finac.s: finac.c fina.h arch.h sys.h
	${CC} ${CPPFLAGS} ${CFLAGS} -S finac.c -o finac.s

fina.o: fina.s
	${CC} -c ${CFLAGS} $< -o $@

fina0.o: fina0.s
	${CC} -c ${CFLAGS} $< -o $@

fina1.o: fina1.s
	${CC} -c ${CFLAGS} $< -o $@

fina2.o: fina1.s
	${CC} -c ${CFLAGS} $< -o $@

fina.s: finas.s finac.s
	cat $^ > $@

fina0.s: finas0.s finac.s
	cat $^ > $@

fina1.s: finas1.s finac.s
	cat $^ > $@

fina2.s: finas2.s finac.s
	cat $^ > $@

sys.o: sys.c sys.h

distclean:
	rm -f arch.h tconfig.fs sys.c opt.fs flags compiler hostforth

clean:
	rm -f *.o *.s fina fina0 fina1 fina2 *\~ \#*\# \
		help/toc.help ${ALL_HELP}

powerpc:
	ln -fs tconfig-powerpc.fs tconfig.fs
	ln -fs arch-powerpc.h arch.h
	echo -n " -no-cpp-precomp " >> flags
	echo -n "gcc2" > compiler
	echo -n "gforth-0.5.0" > hostforth

mips:
	ln -fs tconfig-mips.fs tconfig.fs
	ln -fs arch-mips.h arch.h
	echo -n "gcc" > compiler
	echo -n "gforth-0.5.0" > hostforth

x86:
	ln -fs tconfig-x86.fs tconfig.fs
	ln -fs arch-x86.h arch.h
	echo "/usr/pkg/gcc-2.95.3/bin/gcc" > compiler
	echo -n "gforth-0.5.0 -p .:/usr/local/lib/gforth/0.5.0" > hostforth

posix:
	ln -fs sysposix.c sys.c

fast:
	echo "-1 constant fast" >> opt.fs
	echo -n " -DFASTFORTH " >> flags

slow:
	echo "0 constant fast" >> opt.fs
	echo -n " -UFASTFORTH " >> flags

files:
	echo -n " -DHASFILES " >> flags