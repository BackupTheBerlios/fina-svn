include save.fs

:noname
   con
   rp0 @ rp! 
   sp0 @ sp!
   argc 1 ?do
      i argv s" -e" compare 0= if 
         i 1+ argv evaluate 2
      else i argv included 1 then
   +loop quit ; cold!

save" fina"
