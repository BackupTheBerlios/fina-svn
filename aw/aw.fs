require ../ffi.fs
library aw libaw.so

aw awInit (int) awInit
aw awEnd (void) awEnd
aw awOpen (int) awOpen
aw awSelect int (void) awSelect
aw awClose (void) awClose
aw awSwapBuffers (void) awSwapBuffers
aw awMakeCurrent (void) awMakeCurrent
aw awShow (void) awShow
aw awHide (void) awHide
aw awSetTitle ptr (void) awSetTitle
aw awMove int int (void) awMove
aw awResize int int (void) awResize
aw awNextEvent (ptr) awNextEvent

