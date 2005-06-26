PREFIX ?= .
TMPDIR ?= tmp
BINDIR ?= $(PREFIX)/bin
HLPDIR ?= $(PREFIX)/share/fina/help
ARCH     := $(shell ./arch)
KERN     := $(shell uname -s)
HOST     := gforth-0.5.0
SYSTEM   := $(KERN)-$(ARCH)
CC       := $(shell $(MAKE) -s -f Makefile.systems $(SYSTEM)-gcc)
CPPFLAGS := $(shell $(MAKE) -s -f Makefile.systems $(SYSTEM)-cppflags)
OS       := $(shell $(MAKE) -s -f Makefile.systems $(SYSTEM)-os)
FFLAGS   := $(shell $(MAKE) -s -f Makefile.systems $(SYSTEM)-forthflags)

CFLAGS = -g -O2 -Wall
CPPFLAGS += -I$(TMPDIR)

SRCDIR := $(shell pwd)

COMMON_OBJECTS = $(TMPDIR)/main.o $(TMPDIR)/sys.o

ALL_HELP = ${ALL_FORTH:%.fs=$(HLPDIR)/%.help}

FINA_SRC0 = $(TMPDIR)/opt.fs tconfig-$(ARCH).fs
FINA_SRC1 = meta.fs fina.fs
HOST_FINA0 = core.fs defer.fs throwmsg.fs search.fs coreext.fs
HOST_FINA1 = host-fina.fs
HOST_GFORTH = host-gforth.fs
FINA_TEST0 = core.fs defer.fs throwmsg.fs tester.fs coretest.fs \
	     postponetest.fs bye.fs
FINA_TEST = tester.fs finatest.fs coretest.fs postponetest.fs \
	    filetest.fs dbltest.fs

RUN_FINA0 = \
   core.fs throwmsg.fs defer.fs signals.fs search.fs \
   coreext.fs searchext.fs \
   file.fs fileext.fs \
   double.fs doubleext.fs \
   optional.fs string.fs require.fs \
   tools.fs toolsext.fs \
   facility.fs facilityext.fs \
   lineedit.fs multi.fs osnice.fs

RUN_FINA = $(RUN_FINA0) help.fs

SAVE_FINA = $(RUN_FINA0) $(TMPDIR)/help.fs savefina.fs

ALL_FORTH = $(RUN_FINA0) help.fs savefina.fs fina.fs

bootstrap-from-assembler: mkdirs $(TMPDIR)/opt.fs
	$(MAKE) all

all: mkdirs $(BINDIR)/fina doc gen-bootstrap

gen-bootstrap: bootstrapdict-$(ARCH).s

mkdirs:
	[ -d $(TMPDIR) ] || install -d $(TMPDIR)
	[ -d $(BINDIR) ] || install -d $(BINDIR)
	[ -d $(HLPDIR) ] || install -d $(HLPDIR)

$(TMPDIR)/opt.fs:
	echo $(FFLAGS) > $@

$(TMPDIR)/arch.h: $(SRCDIR)/arch-$(ARCH).h
	[ -f $@ ] || ln -fs $< $@

# Compiler

$(BINDIR)/fina: $(TMPDIR)/kernel $(SAVE_FINA)
	echo "`cat $(SAVE_FINA)` save\" $@\" bye"  | $<
	chmod 755 $@

$(TMPDIR)/kernel: $(COMMON_OBJECTS)

$(TMPDIR)/kernel0: $(COMMON_OBJECTS)

$(TMPDIR)/bootstrap: $(COMMON_OBJECTS) 

run: $(BINDIR)/fina
	cat $(RUN_FINA) - | $(TMPDIR)/kernel

test: $(BINDIR)/fina
	diff $(TMPDIR)/kernel $(TMPDIR)/kernel0
	$< $(FINA_TEST) -e bye

$(TMPDIR)/bootstraptest: $(TMPDIR)/bootstrap
	cat $(FINA_TEST0) $(FINA_TEST) $(FINA_TEST1) | $<

$(TMPDIR)/kerneldict.s: $(TMPDIR)/kernel0 $(HOST_FINA0) $(FINA_SRC0) $(HOST_FINA1) $(FINA_SRC1)
	cat $(HOST_FINA0) $(FINA_SRC0) $(HOST_FINA1) $(FINA_SRC1) | $< > $@

$(TMPDIR)/kernel0dict.s: $(TMPDIR)/bootstrap $(HOST_FINA0) $(FINA_SRC0) $(HOST_FINA1) $(FINA_SRC1)
	cat $(HOST_FINA0) $(FINA_SRC0) $(HOST_FINA1) $(FINA_SRC1) | $< > $@

$(TMPDIR)/bootstrapdict-$(ARCH).s: $(FINA_SRC0) $(HOST_GFORTH) $(FINA_SRC1)
	$(HOST) $(FINA_SRC0) $(HOST_GFORTH) $(FINA_SRC1) > $@.tmp
	mv $@.tmp $@

$(TMPDIR)/main.o : main.c
	$(CC) $(CPPFLAGS) $(CFLAGS) $^ -c -o $@	

$(TMPDIR)/finac.s: finac.c fina.h $(TMPDIR)/arch.h sys.h
	$(CC) $(CPPFLAGS) $(CFLAGS) -S finac.c -o $@

$(TMPDIR)/bootstrap.o: $(TMPDIR)/bootstrap.s
	$(CC) -c $(CPPFLAGS) $(CFLAGS) $< -o $@

$(TMPDIR)/kernel0.o: $(TMPDIR)/kernel0.s
	$(CC) -c $(CPPFLAGS) $(CFLAGS) $< -o $@

$(TMPDIR)/kernel.o: $(TMPDIR)/kernel.s
	$(CC) -c $(CPPFLAGS) $(CFLAGS) $< -o $@

$(TMPDIR)/sys.o: sys$(OS).c sys.h
	$(CC) -c $(CPPFLAGS) $(CFLAGS) $< -o $@

$(TMPDIR)/bootstrap.s: $(TMPDIR)/finac.s $(TMPDIR)/bootstrapdict-$(ARCH).s
	cat $^ > $@

bootstrapdict-$(ARCH).s: $(TMPDIR)/bootstrapdict-$(ARCH).s
	cp $< $@

$(TMPDIR)/kernel0.s: $(TMPDIR)/kernel0dict.s $(TMPDIR)/finac.s
	cat $^ > $@

$(TMPDIR)/kernel.s: $(TMPDIR)/kerneldict.s $(TMPDIR)/finac.s
	cat $^ > $@


clean:
	rm -f   $(TMPDIR)/*.o $(TMPDIR)/kernel*.s $(TMPDIR)/bootstrap.s \
		$(TMPDIR)/fina*.s $(TMPDIR)/arch.h $(TMPDIR)/opt.fs \
		$(TMPDIR)/bootstrapdict-$(ARCH).s \
		$(TMPDIR)/bootstrap $(TMPDIR)/kernel0 \
		$(TMPDIR)/kernel $(TMPDIR)/glos.txt $(TMPDIR)/help.fs \
		*\~ \#*\#

uninstall:
	rm -f $(BINDIR)/fina

distclean: clean


# Glossaries

doc: $(HLPDIR)/toc.help

$(HLPDIR)/toc.help: $(BINDIR)/fina maketoc.fs $(ALL_HELP)
	cp -a help/*.help $(HLPDIR)
	$< maketoc.fs -e "toc{ $(ALL_HELP) }toc bye" > $@

$(ALL_HELP): $(ALL_FORTH) 

$(HLPDIR)/%.help: %.fs
	$(BINDIR)/fina glosgen.fs -e "newglos makeglos $< writeglos $@ bye"


$(TMPDIR)/help.fs : help.fs
	sed "s^@HLPDIR@^$(HLPDIR)^" $< > $@
