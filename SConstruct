env = Environment(ARCH='powerpc', CC='gcc-4.0.2')
env.Append(CPPFLAGS='-O2')
env.Append(CPPDEFINES=['HAS_FILES', 'HAS_ALLOCATE', 'HAS_FIXED', 'HAS_FFI', 
			'MORE_PRIMS'])
tab = Builder(
  action='cat $SOURCE | grep "^ *PRIM(" | sed "s/PRIM(\(.*\),.*/\&\&\\1,/g" > $TARGET')

asm = Builder(
  action='$CC $CCFLAGS $CPPFLAGS $_CPPDEFFLAGS $_CPPINCFLAGS -S -o $TARGET $SOURCE',
  source_scanner = CScan)

env.Append(BUILDERS = {'Tab' : tab})
env.Append(BUILDERS = {'Asm' : asm})
env.SConscript('SConscript', 
		build_dir = 'obj', 
		src_dir = '.', 
		exports='env',
		duplicate = 0)
