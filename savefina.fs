include save.fs

: .line type cr ;
: license
   s" LICENSE" r/o open-file throw >r
   r@ ['] .line foreachline
   r> close-file throw ;

defer banner

:noname
   ." FINA v0.1. Copyright (c) 2004-2005, Jorge Acereda Macia." cr
   ." FINA comes with EVEN LESS WARRANTY; for details type 'license'." cr
   ." Type 'bye' to exit." cr
; is banner

: doargs
   argc 1 ?do
      0
      i argv drop c@ [char] - = if
         i argv s" -e" compare 0= if  i swap >r 1+ argv evaluate  r> 2 +  then
         i argv s" -s" compare 0= if  ['] noop is banner 1+  then
         dup 0= if i argv type ."  ignored" cr 1+ then
      else i swap >r argv included r> 1+ then
   +loop ;

:noname
   rp0 @ rp! 
   sp0 @ sp!
   ['] doargs catch '.error @execute
   banner
   quit ; 'cold!

:noname
   ." ok" cr ; '.prompt ! 
echo on 

