\g @see anssearch 
: wordlist ( -- wid )
   forth-wordlist begin cell+ dup @ while @ repeat
   here swap !  here 0 , 0 , 0 , ;

\g @see anssearch
: get-order  ( -- widn ... wid1 n )
   #order cell+ 
   >r #order @ begin dup while @r+ swap 1- repeat drop rdrop
   #order @ ;

\g @see anssearch
: set-order  ( widn ... wid1 n -- )
   dup -1 = if drop forth-wordlist 1 recurse exit then
   #order !
   #order cell+ 
   >r #order @ begin dup while swap !r+ 1- repeat drop rdrop ;

\g @see anssearch
: definitions ( -- )
   #order cell+ @ set-current ;

env: search-order -1 ;env
