require ffi.fs
library libc libc.so.6
libc system ptr (int) system

create buffer 256 allot
: 0term buffer place 0 buffer count + c! buffer 1+ ;
: (sh) 0term system ;
: sh" 
   [char] " parse postpone sliteral postpone (sh) ; immediate compile-only
: sh 0 parse (sh) ;
