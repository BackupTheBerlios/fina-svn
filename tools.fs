\g @see anstools
: ?  ( a-addr -- )
   @ . ;

\g Type name of word at NFA
: .name ( nfa -- )
   namecount type space ;

: (.name) ( nfa -- nfa )
   dup .name 
   1 here +! ;

\g @see anstools
: words ( -- )
   here off
   ['] (.name) forwords 
   cr here @ . ." words" cr ;

: dumprow ( addr u -- addr' )
  swap cr dup 0 <#  # # # # # # # #  #> type space
  16 0 do
     over i 1+ < if 2 spaces else dup c@ 0 <# # # #> type then space char+ 
  loop 2 spaces 16 chars - 
  16 0 do 
     over i 1+ < if  bl  else 
        dup c@ 127 and dup 0 bl within over 127 = or if 
           drop [char] .
        then
     then emit char+
  loop nip ;

\g @see anstools
: dump
   base @ >r hex
   16 /mod swap >r
   0 ?do 16 dumprow loop
   R> dumprow drop r> base ! ;

: (next?)
   2dup cell- @ = over and to found ;

: nextnfa ( nfa1 -- nfa2 )
   ['] (next?) forwords drop found ;

: /xt ( xt -- a-addr )
   xt>name nextnfa cell- ;

: type? ( addr <inline-doer> -- flag
   ?dodefine nip @r+ = ;

: doersee ( xt -- )
   dup type? dolist if ." : " then
   dup type? dovar if ." variable " then
   dup type? doconst if ." constant " then
   dup type? douser if ." user " then
   dup type? dovalue if ." value " then
   drop ;

: xt? ( val -- flag )
   dup aligned over <> if drop 0 exit then
   dup dict? 0= if primxt? else ?dodefine nip 0<> then ;

: safext>name ( xt -- name|0 )
   dup xt? if xt>name else drop c" (null)" then ;

: cellsee ( a-addr1 -- a-addr2 )
   dup @ safext>name .name 
   cell+ ;

: xtsee ( xt -- )
\   dup doersee 
\   dup xt>name .name
   dup /xt swap \ ?dodefine drop 
   begin  2dup >  while  cellsee  repeat ;

\g @see anstools
: see
   ' /fcompo fimmed rot xtsee 
   0 > if ." immediate "  then    0= if ." compile-only" then cr ;
