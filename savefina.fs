include save.fs

: doargs
   argc 1 ?do
      i argv s" -e" compare 0= if 
         i 1+ argv evaluate 2
      else i argv included 1 then
   +loop ;

:noname
   con
   rp0 @ rp! 
   sp0 @ sp!
   ['] doargs catch '.error @execute
   quit ; 'cold !

save" fina"
