
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

0 value status 

0 value '@see

: .line? ( a u -- )
   2dup s" ====" beginswith? if  0 to status  then
   status 1 = if  2dup matches?  if  -1000 to status  then  then
   status 0< if
      2dup s" @see" beginswith? if  
         2dup bl scan  bl skip status >r '@see execute r> to status
      else
         2dup type cr
      then
   then
   status 1+ to status
   2drop ;

: @see ( a u -- )
   ." <<" 2dup type ." >>" cr
   s" help/" pad place
   pad append
   s" .help" pad append 
   pad count r/o open-file throw >r
   2 to status
   r@ ['] .line? foreachline
   r> close-file throw ;  ' @see to '@see

: help
   parse-word requested 2! s" toc" cr @see ;
