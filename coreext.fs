
\g @see anscore
: hex ( -- )
   16 base ! ;

\g @see anscore 
: unused ( -- u )
   pad here - ;

\g @see anscore
: 2>r  ( d --  r: -- d )
   swap r> swap >r swap >r >r ;

\g @see anscore
: 2r>  ( -- d  r: d -- )
   r> r> swap r> swap >r swap ;

\g Runtime for C"
: doc" ( -- c-addr )
   r> count over + aligned >r [ -1 chars ] literal + ; compile-only

\g Compile a counted string literal
: csliteral ( c-addr u -- )
   postpone doc" s, align ; immediate compile-only 

\g #see anscore    
: c"  ( <string>" --  rt: -- c-addr )
   [char] " parse   postpone csliteral ; immediate compile-only

\g @see anscore
: query ( -- )
   source-id 0 to source-id refill drop to source-id ;

\g @see anscore
: #tib ( -- a-addr )
   source nip ;
