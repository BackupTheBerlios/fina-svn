
: C+!  ( n addr -- )  dup >R  C@ +  R> C! ;

: APPEND                    ( str len addr -- )
    2dup 2>R  COUNT chars +  SWAP chars MOVE ( ) 2R> C+! ;


: beginswith? ( a1 u1 a2 u2 -- flag, does str1 begin with str2?)
   2>r r@ min 2r> compare 0= ;

variable requested 0 ,

: nextword ( a1 u1 -- a2 u2 )
   2dup bl scan nip - ;

: matches? ( a u -- flag, did we find a matching line?)
   2dup nextword requested 2@ compare 0= >r
   bl scan bl skip nextword requested 2@ compare 0= r> or ;

0 value helpstatus 
0 value '@see
0 value level

\ XXX
: 0> 0 > ;

: ?type level 0> and type ;
: ?cr level 0> if cr then ;

: .line? ( a u -- )
   2dup s" ====" beginswith? if  0 to helpstatus  then
   helpstatus 1 = if  2dup matches?  if  -1000 to helpstatus  then  then
   helpstatus 0< if
      2dup s" @see" beginswith? if  
         2dup bl scan  bl skip helpstatus >r '@see execute r> to helpstatus
      else 2dup s" @also" beginswith? if
         ." See also: " 2dup bl scan  bl skip ?type ?cr
      else
         2dup ?type ?cr
      then then
   then
   helpstatus 1+ to helpstatus
   2drop ;

: @see ( a u -- )
   level 1+ to level
   s" <<" ?type 2dup ?type s" >>" ?type ?cr
   s" help/" pad place
   pad append
   s" .help" pad append 
   pad count r/o open-file throw >r
   2 to helpstatus
   r@ ['] .line? foreachline
   r> close-file throw ;  ' @see to '@see

\g Display word documentation
: help ( "word" -- )
   -1 to level
   parse-word requested 2! s" toc" cr @see ;
