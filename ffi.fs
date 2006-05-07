: lastbody ( -- addr )
   lastname name>xt xt>body ;
: fn ( libhandle "name" -- )
   lastbody 9 cells + \ args
   here cell- \ ret
   here cell-  lastbody 9 cells +  - 1 cells / \ nargs
   lastbody cell+ \ cif  
   ffprep abort" Unable to prepare function call" 
   0 parse rot dlsym dup 0= abort" Unable to lookup symbol" lastbody ! ;
: int ( -- ) ffint , ;
: float ( -- ) fffloat , ;
: ptr ( -- ) ffptr , ;
: (int) ( -- ) ffint , fn ;
: (float) ( -- ) fffloat , fn ;
: (ptr) ( -- ) ffptr , fn ;
: (void) ( -- ) ffvoid , fn ;
: newfun ( lib -- )
   create 0 , 8 cells allot \ func|cif, will be filled by "ret"
   does> dup @ swap cell+ ffcall ;
: library ( "forthname" "libname" ) 
   create 0 parse dlopen , 
   does> @ newfun ;

0 [if]
library libc libc.so.6

libc sleep int (int) sleep
libc cwrite int ptr int (int) write
1 lastname count cwrite
3 sleep
[then]
