Import('env')
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

if ARGUMENTS.get('test', 0):
	fenv.Command('testfina', [f] + Split("""
		tester.fs finatest.fs coretest.fs postponetest.fs
		filetest.fs dbltest.fs dbltest2.fs
		"""), '$SOURCE ${SOURCES[1:]}')

awenv = env.Copy()
awenv.Append(LIBS=['X11', 'GL', 'Xxf86vm', 'Xext'])
awenv.Append(CCFLAGS=' -g ')
awenv.SharedLibrary('aw', Split('aw/aw.c aw/awx.c'))
