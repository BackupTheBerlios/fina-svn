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

