con
0 constant r/o
2 constant r/w
4 constant w/o

: bin ( u1 -- u2 )
   1 or ;

: open-file ( a u1 u2 -- a ior )
   openf ;

: read-file ( a1 u1 a2 -- u2 ior )
   readf ;

: write-file ( a1 u1 a2 -- u2 ior )
   writef ;

: close-file ( a -- ior )
   closef ;

: mmap-file ( a1 -- a2 )
   mmapf ;

0 value file
: test
   s" /tmp/mierda" r/w open-file throw to file
   pad 80 file read-file throw .
   pad 40 dump
   s" mierda copon" file write-file throw .
   file close-file throw ;


: h# ( <hexnum> -- u )
    0 0 bl parse
    base @ >r  hex  >number  r> base !   
    0= invert -24 and throw 2drop
    state @ if  postpone literal  then
    ; immediate

0 value dptr
0 value file
0 value resetdptr

: save-block ( bytes -- )
   begin
      dup
   while
      dptr @ h# feedbabe = dptr cell+ @ h# deadbeef = and if
         ['] quit cell+ dict0 cell- ! 
         ." Magic found at " dptr . cr 
         dict0 5 cells - to dptr  0 to resetdptr
      then
      dptr memtop > if 256 over - pad + to dptr -1 to resetdptr then
      dptr 1 cells file write-file throw drop
      dptr cell+ to dptr
      1 cells - 
   repeat drop
   dptr . cr resetdptr if pad to dptr then ;

: save-system
   pad to dptr -1 to resetdptr
   s" fina" r/o open-file throw >r
   s" /tmp/mierda" w/o open-file throw to file
   begin  pad 256 r@ read-file throw  dup while  save-block repeat  drop
   r> close-file throw
   file close-file throw ;

: test-mmap
   s" /tmp/mierda" r/o open-file throw to file
   file mmap-file . 
   file close-file throw ;
