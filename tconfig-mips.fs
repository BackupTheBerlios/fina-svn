32 constant tcellbits
256 constant /pad
65536 constant /tdict
: .call"
   s"  bal " postpone sliteral postpone type
   [char] " parse postpone sliteral postpone type 
   postpone cr 
   s"  nop " postpone sliteral postpone type 
   postpone cr ; immediate
: .init
   ." .set noreorder" cr
   ." .set noat" cr
   ." .set nomacro" cr ;
   