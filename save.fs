\ XXX Better implemented using mmap-file

: d= rot = >r = r> and ;

: h# ( <hexnum> -- u )
    bl parse  base @ >r  hex  s>number  r> base !   
    state @ if dpl @ 0< if postpone literal else postpone 2literal then  then
    ; immediate

: mark ( a1 -- a2 )
   begin cell+ dup 2@ h# feedbabe.deadbeef d= until ;

: cold!
   cell+ dict0 cell+ cell+ ! ;

: save ( a u -- )
   w/o open-file throw >r   0 argv r/o open-file throw
   dup mmap-file throw
   dup dup mark over - dup pad ! r@ write-file throw  \ contents before dictionary
   dict0 memtop over - dup pad +! r@ write-file throw \ dictionary
   dup pad @ + >r ( handle addr r: start)
   over file-size throw drop + ( handle end r: start )
   r@ - r> swap  r@ write-file throw  \ contents after dictionary
   close-file throw  r> close-file throw ;

: save"
   [char] " parse save ;

:noname
   con
   rp0 @ rp! 
   sp0 @ sp!
   argc 1 ?do
      i argv s" -e" compare 0= if 
         i 1+ argv evaluate 2
      else i argv included 1 then
   +loop quit ; cold!

