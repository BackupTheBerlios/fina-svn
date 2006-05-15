Import('env prefix helpdir')
fenv = env.Copy()
fenv.Append(CPPPATH=[Dir('.'), '/usr/include/libffi'])
fenv.Append(LIBPATH='/usr/lib/libffi/')
fenv.Tab('primstab.it', 'prims.i')
fenv.Tab('moreprimstab.it', 'moreprims.i')
fenv.Tab('filestab.it', 'files.i')
fenv.Tab('allocatetab.it', 'allocate.i')
fenv.Tab('fixedtab.it', 'fixed.i')
fenv.Tab('ffitab.it', 'ffi.i')
fenv.Command('arch.h', '$ARCH-arch.h', 'ln -sf ${SOURCE.abspath} $TARGET')
fenv.Asm('finac.s', 'finac.c')
fenv.Append(LIBS=['dl', 'ffi'])

for phase in range(3):
        ks = fenv.Command('kernel' + str(phase) + '.s', 
                ['finac.s', '$ARCH-dict' + str(phase) + '.s'],
                'cat $SOURCES > $TARGET')
        k = fenv.Program('kernel' + str(phase), [ks, 'sysposix.c', 'main.c'])
        if ARGUMENTS.get('test', 0):
                fenv.Command('dummy' + str(phase), [k] + Split("""
                core.fs defer.fs throwmsg.fs tester.fs coretest.fs 
                postponetest.fs bye.fs
                """), 'cat ${SOURCES[1:]} | $SOURCE')           
        fenv.Command('$ARCH-dict' + str(phase+1) + '.s', [k] + Split("""
                core.fs defer.fs throwmsg.fs search.fs coreext.fs
                opt.fs $ARCH-tconfig.fs
                host-fina.fs
                meta.fs fina.fs
                """), 'cat ${SOURCES[1:]} | $SOURCE > $TARGET')

if helpdir[0] == '#':
	helpdir2 = helpdir[1:]
else:
	helpdir2 = helpdir

fenv.Command('help.fs', 'help.tmpl.fs', 
        'sed "s^@HLPDIR@^' + helpdir2 + '^" $SOURCE > $TARGET')

f = fenv.Command('fina', Split("""kernel2
           core.fs throwmsg.fs defer.fs signals.fs search.fs
           coreext.fs searchext.fs
           file.fs fileext.fs
           double.fs doubleext.fs
           optional.fs string.fs require.fs
           tools.fs toolsext.fs
           facility.fs facilityext.fs 
           lineedit.fs multi.fs osnice.fs
           help.fs savefina.fs
        """),
        ['echo "`cat ${SOURCES[1:]} ` save\\" $TARGET" bye"  | $SOURCE',
        'chmod 777 $TARGET'])
env.Default(env.Install(prefix + 'bin', f))

if ARGUMENTS.get('test', 0):
        fenv.Command('testfina', [f] + Split("""
                tester.fs finatest.fs coretest.fs postponetest.fs
                filetest.fs dbltest.fs dbltest2.fs
                """), '$SOURCE ${SOURCES[1:]}')


awenv = env.Copy()
awenv.Append(LIBS=['X11', 'GL', 'Xxf86vm', 'Xext'])
awenv.Append(CCFLAGS=' -g ')
awenv.SharedLibrary(prefix + 'lib/fina/aw', Split('aw.c awx.c'))

env.Default(env.Install(prefix + 'share/fina', Split("""
        coretest.fs gtk.fs sh.fs ans-report.fs dbltest.fs module.fs
        answords.fs dbltest2.fs assert.fs aw.fs awtest.fs
        tester.fs backtrace.fs postponetest.fs bnf.fs ffi.fs
        hype.fs cce.fs filetest.fs verboseinc.fs checkans.fs
        finatest.fs wordsets.fs
        """)))

allforth = """
ans-report.fs  coretest.fs     fina.fs		meta.fs		    searchext.fs
answords.fs    dbltest.fs      finatest.fs	module.fs	    sh.fs
assert.fs      dbltest2.fs     glosgen.fs	multi.fs	    signals.fs
aw.fs	       defer.fs        gtk.fs		opt.fs		    string.fs
awtest.fs      double.fs       help.fs	        optional.fs	    tconfig-mips.fs
backtrace.fs   doubleext.fs    host-fina.fs	osnice.fs	    tester.fs
bnf.fs	       facility.fs     host-gforth.fs	postponetest.fs     throwmsg.fs
bye.fs	       facilityext.fs  hype.fs		powerpc-tconfig.fs  tools.fs
cce.fs	       ffi.fs	       i386-tconfig.fs	require.fs	    toolsext.fs
checkans.fs    file.fs	       lineedit.fs	save.fs		    verboseinc.fs
core.fs        fileext.fs      maketoc.fs	savefina.fs	    wordsets.fs
coreext.fs     filetest.fs     memory.fs	search.fs
"""

anshelp = Split("""
	help/ansblock.help     help/ansexception.help  
	help/ansfloating.help  help/anssearch.help
	help/anscore.help      help/ansfacility.help   
	help/anslocals.help    help/ansstring.help
	help/ansdouble.help    help/ansfile.help          
	help/ansmemory.help    help/anstools.help
""")

allhelp = [env.Hlp(i.replace('.fs', '.help'), i) for i in Split(allforth)]
toc = env.Command('toc.help', [f] + allhelp, 
        '$SOURCE maketoc.fs -e "toc{ ${SOURCES[1:]} }toc bye" > $TARGET')

env.Default(env.Install(helpdir, [toc] + allhelp + anshelp))
