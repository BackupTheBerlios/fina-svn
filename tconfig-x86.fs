32 constant tcellbits
256 constant /pad
64 1024 * constant /tdict
: .call" 
   s"  nop" postpone sliteral postpone type postpone cr
   s"  nop" postpone sliteral postpone type postpone cr
   s"  nop" postpone sliteral postpone type postpone cr
   s"  call xt_" postpone sliteral postpone type 
   [char] " parse postpone sliteral postpone type  
   postpone cr ; immediate
: .init 
   ."  .data" cr
   ."  .globl _Forth_Entry " cr 
   ." _Forth_Entry: .long xt_cold + 8" cr ;


