
32        constant tcellbits
256       constant /pad
256       constant /tib
256       constant /hld
256       constant /user
1024      constant /ds
1024      constant /rs
64 1024 * constant /tdict
4         constant /tcall

: .call"
   s"  bl XT_" postpone sliteral postpone type 
   [char] " parse postpone sliteral postpone type  
   postpone cr ; immediate
: .init 
   ."  .globl _Forth_Entry " cr 
   ."  .globl _Forth_UserP" cr 
   ."  .globl _Forth_Here" cr 
   ."  .data" cr
   ."  .align 2" cr 
   ."  .long 0xfeedbabe, 0xdeadbeef" cr
   ." _Forth_Entry: .long XT_COLD" cr
   ." _Forth_UserP: .long XT_USERP + 4" cr
   ." _Forth_Here: .long XT_HERE + 4" cr
;
