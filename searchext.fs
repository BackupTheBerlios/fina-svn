
\g @see anssearch
: also ( -- )
   get-order 1+ over swap set-order ;

\g @see anssearch
: forth ( -- )
   get-order nip forth-wordlist set-order ;

\g @see anssearch
: only ( -- )
   -1 set-order ;

\g @see anssearch.fs
: previous  ( -- )
   get-order nip 1- set-order ;

env: seach-order-ext true ;env
