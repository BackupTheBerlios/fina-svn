CC = `cat compiler`
HOST = `cat hostforth`
CPPFLAGS = `cat flags`
CFLAGS = -g -O2 -Wall 

COMMON_OBJECTS = main.o sys.o

ALL_HELP = ${ALL_FORTH:%.fs=help/%.help}


FINA_SRC0 = opt.fs tconfig.fs
FINA_SRC1 = meta.fs fina.fs
HOST_FINA0 = core.fs defer.fs throwmsg.fs search.fs coreext.fs
HOST_FINA1 = host-fina.fs
HOST_GFORTH = host-gforth.fs
FINA_TEST0 = core.fs defer.fs throwmsg.fs tester.fs coretest.fs postponetest.fs bye.fs
FINA_TEST = tester.fs coretest.fs postponetest.fs filetest.fs dbltest.fs

RUN_FINA = \
   core.fs throwmsg.fs defer.fs signals.fs search.fs coreext.fs searchext.fs \
   file.fs fileext.fs \
   double.fs doubleext.fs \
   optional.fs string.fs require.fs \
   tools.fs toolsext.fs \
   facility.fs facilityext.fs \
   lineedit.fs help.fs multi.fs

SAVE_FINA = ${RUN_FINA} savefina.fs bye.fs

ALL_FORTH = fina.fs ${SAVE_FINA}

ARCH = `uname -m`
KERN = `uname -s`
SYSTEM = $(KERN)-$(ARCH)


all:
	$(MAKE) $(SYSTEM)
	ln -fs bootstrapdict.s.$(ARCH) bootstrapdict.s
	ln -fs tconfig-$(ARCH).fs tconfig.fs
	ln -fs arch-$(ARCH).h arch.h
	touch bootstrapdict.s
	$(MAKE) fina
	$(MAKE) doc

# Systems

Linux-ppc: anew posix fast
	echo -n "/usr/powerpc-unknown-linux-gnu/gcc-bin/2.95/powerpc-unknown-linux-gnu-gcc" > compiler
	echo -n "gforth-0.5.0" > hostforth

NetBSD-i386: anew posix fast
	echo "/usr/pkg/gcc-2.95.3/bin/gcc" > compiler
	echo -n "gforth-0.5.0" > hostforth

# Compiler

fina: kernel ${SAVE_FINA}
	cat ${SAVE_FINA} | ./$<
	chmod 755 $@

kernel: ${COMMON_OBJECTS}

kernel0: ${COMMON_OBJECTS}

bootstrap: ${COMMON_OBJECTS} 

run: fina
	cat ${RUN_FINA} - | ./kernel

test: fina
	diff kernel kernel0
	./$< ${FINA_TEST} -e bye

bootstraptest: bootstrap
	cat ${FINA_TEST0} ${FINA_TEST} ${FINA_TEST1} | ./$<

kerneldict.s: kernel0 ${HOST_FINA0} ${FINA_SRC0} ${HOST_FINA1} ${FINA_SRC1}
	cat ${HOST_FINA0} ${FINA_SRC0} ${HOST_FINA1} ${FINA_SRC1} | ./$< > $@

kernel0dict.s: bootstrap ${HOST_FINA0} ${FINA_SRC0} ${HOST_FINA1} ${FINA_SRC1}
	cat ${HOST_FINA0} ${FINA_SRC0} ${HOST_FINA1} ${FINA_SRC1} | ./$< > $@

bootstrapdict.s: ${FINA_SRC0} ${HOST_GFORTH} ${FINA_SRC1}
	`cat hostforth` ${FINA_SRC0} ${HOST_GFORTH} ${FINA_SRC1} > $@

main.o : main.c

finac.s: finac.c fina.h arch.h sys.h flags
	${CC} ${CPPFLAGS} ${CFLAGS} -S finac.c -o finac.s

fina.o: fina.s
	${CC} -c ${CFLAGS} $< -o $@

bootstrap.o: bootstrap.s
	${CC} -c ${CFLAGS} $< -o $@

kernel0.o: kernel0.s
	${CC} -c ${CFLAGS} $< -o $@

kernel.o: kernel.s
	${CC} -c ${CFLAGS} $< -o $@

bootstrap.s: bootstrapdict.s finac.s
	cat $^ > $@

kernel0.s: kernel0dict.s finac.s
	cat $^ > $@

kernel.s: kerneldict.s finac.s
	cat $^ > $@

sys.o: sys.c sys.h

anew:
	rm -f arch.h tconfig.fs sys.c opt.fs bootstrapdict.s\
		flags compiler hostforth

clean:
	rm -f *.o *.s fina bootstrap kernel0 kernel *\~ \#*\# \
		help/toc.help ${ALL_HELP}

distclean: anew clean


posix:
	ln -fs sysposix.c sys.c
	$(MAKE) files allocate

fast:
	echo "-1 constant more-prims" >> opt.fs
	echo -n " -DMORE_PRIMS " >> flags

slow:
	echo "0 constant more-prims" >> opt.fs
	echo -n " -UMORE_PRIMS " >> flags

files:
	echo -n " -DHAS_FILES " >> flags
	echo "-1 constant has-files" >> opt.fs

allocate:
	echo -n " -DHAS_ALLOCATE " >> flags
	echo "-1 constant has-allocate" >> opt.fs

# Glossaries

doc: help/toc.help

help/toc.help: fina maketoc.fs ${ALL_HELP}
	./fina maketoc.fs -e "toc{ ${ALL_HELP} }toc bye" > $@

${ALL_HELP}: ${ALL_FORTH} 

help/%.help: %.fs
	./fina glosgen.fs -e "newglos makeglos $< writeglos $@ bye"


