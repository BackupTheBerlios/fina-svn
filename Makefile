
ARCH     := $(shell ./arch)
KERN     := $(shell uname -s)
HOST     := gforth-0.5.0
SYSTEM   := $(KERN)-$(ARCH)
ifeq ($(origin CC), undefined)
CC       := $(shell $(MAKE) -s -f Makefile.systems $(SYSTEM)-gcc)
endif
CPPFLAGS := $(shell $(MAKE) -s -f Makefile.systems $(SYSTEM)-cppflags)
OS       := $(shell $(MAKE) -s -f Makefile.systems $(SYSTEM)-os)
FFLAGS   := $(shell $(MAKE) -s -f Makefile.systems $(SYSTEM)-forthflags)

CFLAGS = -g -O2 -Wall

COMMON_OBJECTS = main.o sys.o

ALL_HELP = ${ALL_FORTH:%.fs=help/%.help}


FINA_SRC0 = opt.fs tconfig-$(ARCH).fs
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
   lineedit.fs help.fs multi.fs osnice.fs

SAVE_FINA = ${RUN_FINA} savefina.fs bye.fs

ALL_FORTH = fina.fs ${SAVE_FINA}


all: fina doc

opt.fs:
	echo $(FFLAGS) > opt.fs

arch.h: arch-$(ARCH).h
	ln -fs arch-$(ARCH).h arch.h

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

bootstrapdict-$(ARCH).s: ${FINA_SRC0} ${HOST_GFORTH} ${FINA_SRC1}
	$(HOST) ${FINA_SRC0} ${HOST_GFORTH} ${FINA_SRC1} > $@

main.o : main.c

finac.s: finac.c fina.h arch.h sys.h
	${CC} ${CPPFLAGS} ${CFLAGS} -S finac.c -o finac.s

fina.o: fina.s
	${CC} -c ${CFLAGS} $< -o $@

bootstrap.o: bootstrap.s
	${CC} -c ${CFLAGS} $< -o $@

kernel0.o: kernel0.s
	${CC} -c ${CFLAGS} $< -o $@

kernel.o: kernel.s
	${CC} -c ${CFLAGS} $< -o $@

sys.o: sys$(OS).c sys.h
	${CC} -c ${CFLAGS} $< -o $@

bootstrap.s: finac.s bootstrapdict-$(ARCH).s
	cat $^ > $@

kernel0.s: kernel0dict.s finac.s
	cat $^ > $@

kernel.s: kerneldict.s finac.s
	cat $^ > $@


clean:
	rm -f *.o kernel*.s bootstrap.s fina*.s arch.h opt.fs fina bootstrap kernel0 kernel *\~ \#*\# \
		help/toc.help ${ALL_HELP}

distclean: clean


# Glossaries

doc: help/toc.help

help/toc.help: fina maketoc.fs ${ALL_HELP}
	./fina maketoc.fs -e "toc{ ${ALL_HELP} }toc bye" > $@

${ALL_HELP}: ${ALL_FORTH} 

help/%.help: %.fs
	./fina glosgen.fs -e "newglos makeglos $< writeglos $@ bye"


