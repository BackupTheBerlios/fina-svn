\ XXX Better implemented using mmap-file

: d= rot = >r = r> and ;

: h# ( <hexnum> -- u )
    bl parse  base @ >r  hex  s>number  r> base !   
    state @ if dpl @ 0< if postpone literal else postpone 2literal then  then
    ; immediate

: mark ( a1 -- a2 )
   begin cell+ dup 2@ h# feedbabe.deadbeef d= until ;

: save ( a u -- )
   ['] quit cell+ dict0 cell+ cell+ !
   w/o open-file throw >r   0 argv r/o open-file throw
   dup mmap-file throw
   dup dup mark over - r@ write-file throw   \ contents before dictionary
   dict0 memtop over - r@ write-file throw + \ dictionary
   over + >r ( handle addr r: start)
   over file-size drop throw +
   r@ - r> swap  r@ write-file throw drop    \ contents after dictionary
   close-file throw  r> close-file throw ;

: save"
   [char] " parse save ;
\ save" /tmp/mierda"

