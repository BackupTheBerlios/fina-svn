Import('env')
env.Append(CPPPATH=[Dir('.'), '/usr/include/libffi'])
env.Append(LIBPATH='/usr/lib/libffi/')
env.Tab('primstab.it', 'prims.i')
env.Tab('moreprimstab.it', 'moreprims.i')
env.Tab('filestab.it', 'files.i')
env.Tab('allocatetab.it', 'allocate.i')
env.Tab('fixedtab.it', 'fixed.i')
env.Tab('ffitab.it', 'ffi.i')
env.Command('arch.h', '$ARCH-arch.h', 'ln -sf ${SOURCE.abspath} $TARGET')
env.Asm('finac.s', 'finac.c')
env.Append(LIBS=['dl', 'ffi'])

for phase in range(3):
	ks = env.Command('kernel' + str(phase) + '.s', 
		['finac.s', '$ARCH-dict' + str(phase) + '.s'],
		'cat $SOURCES > $TARGET')
	k = env.Program('kernel' + str(phase), [ks, 'sysposix.c', 'main.c'])
	if ARGUMENTS.get('test', 0):
		env.Command('dummy' + str(phase), [k] + Split("""
		core.fs defer.fs throwmsg.fs tester.fs coretest.fs 
		postponetest.fs bye.fs
		"""), 'cat ${SOURCES[1:]} | $SOURCE')		
	env.Command('$ARCH-dict' + str(phase+1) + '.s', [k] + Split("""
		core.fs defer.fs throwmsg.fs search.fs coreext.fs
		opt.fs $ARCH-tconfig.fs
		host-fina.fs
		meta.fs fina.fs
		"""), 'cat ${SOURCES[1:]} | $SOURCE > $TARGET')


f = env.Command('fina', Split("""kernel2
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
	env.Command('testfina', [f] + Split("""
		tester.fs finatest.fs coretest.fs postponetest.fs
		filetest.fs dbltest.fs dbltest2.fs
		"""), '$SOURCE ${SOURCES[1:]}')
