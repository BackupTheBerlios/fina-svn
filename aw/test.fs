require aw.fs
create buffer 256 allot
: 0term buffer place 0 buffer count + c! buffer 1+ ;

hex
00004000 constant GL_COLOR_BUFFER_BIT
00000100 constant GL_DEPTH_BUFFER_BIT
decimal
library GL libGL.so
GL glViewport int int int int  (void) glViewport
GL glClear int  (void) glClear

awInit .
awOpen .
char " parse Mi titulo" 0term awSetTitle
100 100 awResize
100 100 awMove
awShow

: unknown ." unknown" cr drop ;
: resize ." resize" 2@ . . cr ;
: close drop awClose ." Goodbye" cr bye ;
: down ." down " @ . ;
: up ." up " @ . ;
: motion ." motion " 2@ . . cr ;

create handlers ' unknown , ' resize , ' close , ' down , ' up , ' motion ,

: evloop
   begin 
      awNextEvent ?dup if 
         dup cell+ swap @ cells handlers + @execute 
      else
         GL_COLOR_BUFFER_BIT GL_DEPTH_BUFFER_BIT or glClear awSwapBuffers
      then 
      pause
   again ;

64 64 64 task: evtask
evtask build
:noname evtask activate evloop ; execute
