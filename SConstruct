import os
import sys

def shelloutput(cmd):
        pipe = os.popen(cmd, 'r')
        ret = pipe.read()
        pipe.close()
        return ret.strip()

def arch():
	arch = shelloutput('uname -m')
	if arch == 'ppc':
		arch = 'powerpc'
	return arch

prefix = ARGUMENTS.get('prefix', '#')
helpdir = prefix + 'share/fina/help'

env = Environment(ARCH=arch(), CC='gcc')
env.Append(CPPFLAGS='-O2')
env.Append(LINKFLAGS='-g')
env.Append(CPPDEFINES=['HAS_FILES', 'HAS_ALLOCATE', 'HAS_FIXED', 'HAS_FFI', 
			'MORE_PRIMS'])


tab = Builder(action=
	'cat $SOURCE | grep "^ *PRIM(" | sed "s/PRIM(\(.*\),.*/\&\&\\1,/g" > $TARGET',
	source_scanner = CScan)

asm = Builder(action=
	'$CC $CCFLAGS $CPPFLAGS $_CPPDEFFLAGS $_CPPINCFLAGS -S -o $TARGET $SOURCE',
	source_scanner = CScan)
hlp = Builder(action='obj/fina glosgen.fs -e "newglos makeglos $SOURCE writeglos $TARGET bye"')


env.Append(BUILDERS = {'Tab' : tab})
env.Append(BUILDERS = {'Asm' : asm})
env.Append(BUILDERS = {'Hlp' : hlp})

env['INCFFI'] = {
	'darwin' : [
		'/sw/lib/gcc4/lib/gcc/i386-apple-darwin8/4.2.0/include/libffi', 
		'/sw/lib/gcc4/include/'
	],
	'linux2' : [	'/usr/include/libffi' ]
}[sys.platform]
env['LIBFFI'] = {
	'darwin' : '/sw/lib/gcc4/lib',
	'linux2' : '/usr/lib/libffi/'
}[sys.platform]



env.SConscript('SConscript', 
		build_dir = 'obj', 
		src_dir = '.', 
		exports=['env', 'prefix', 'helpdir'],
		duplicate = 0)
