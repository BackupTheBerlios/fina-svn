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

