file warnings off

: immediate
   lastname c@ 64 or lastname c! ;

: \ 
   source >in ! drop ; immediate

: \g postpone \ ; immediate
: internal postpone \ ; immediate
: core postpone \ ; immediate

: compile-only
   lastname c@ 32 or lastname c! ;

: char
   parse-word drop c@ ;

: [char] 
   char postpone literal ; immediate compile-only

: ( 
   [char] ) parse 2drop ; immediate


\ Misc

\g Make last definition a compile-only word
\ compile-only ( -- ) \ internal

\g @see anscore
: chars  ( n1 -- n2 ) \ core
   ; immediate

\g Convert address units to chars
: 1chars/  ( n1 -- n2 ) \ internal
   ; immediate

\g @see anscore
: [']  ( "  xxx"" --  rt: -- xt )
   ' postpone literal ; immediate compile-only

\g Runtime for s" 
: dos"  ( -- c-addr u ) \ internal
   r> count 2dup + aligned >r ; compile-only

\g @see ansstring
: sliteral ( c-addr u -- )
   postpone dos" s, align ; immediate compile-only 

\g @see anscore
: s"  ( "ccc<quote>" --  rt: -- c-addr u )
   [char] " parse   postpone sliteral ; immediate compile-only


\g @see anscore
: ."  ( "ccc<quote>" --  rt: -- )
   [char] " parse   postpone sliteral 
   postpone type ; immediate compile-only


\ Flow control

\g @see anscore
: ahead  ( -- orig 1 )
   postpone branch   fwmark ;  immediate compile-only

\g @see anscore
: if  ( -- orig 1 )
   postpone 0branch   fwmark ; immediate compile-only

\g @see anscore
: then ( orig 1 -- )
   fwresolve ; immediate compile-only

\g @see anscore
: else ( orig1 1 -- orig2 1 )
   postpone ahead  2swap  postpone then ; immediate compile-only

\g @see anscore
: begin  ( -- dest -1 )
   bwmark ; immediate compile-only

\g @see anscore
: while ( dest -1 -- orig 1 dest -1 )
   postpone if   2swap ; immediate compile-only 

\g @see anscore
: again ( dest -1 -- )
   postpone branch  bwresolve ; immediate compile-only

\g @see anscore
: repeat ( orig 1 dest -1 -- )
   postpone again  postpone then ; immediate compile-only

\g @see anscore
: until ( dest -1 -- )
   postpone 0branch  bwresolve ; immediate compile-only

\ Exceptions
\g Throw code if flag is true
: ?throw ( compilation: --  rt: flag code -- ) \ internal
   ['] do?throw here 2 cells - ! ; immediate compile-only

\ Definers

\g Check for compiler nesting
: nesting? ( -- ) \ internal
   bal @ -29 ?throw ;

\g @see anscore
: constant ( x "<spaces>name" --  rt: -- x )
   nesting?  head,  xtof doconst xt, drop  , linklast ;

\g @see anscore
: value ( x "name" -- rt: -- x )
   nesting?  head,  xtof dovalue xt, drop  , linklast ;

\g @see anscore
: variable ( "<spaces>name" -- ) 
   nesting?  head, xtof dovar xt, drop  -559038737 , linklast ; 


variable leaves

\g Link item to list
: link ( item list -- ) \ internal
    2dup @ swap ! ! ;

\g Allocate space for a link field and link it to list
: linked ( list -- , enlaza here a la lista)
    here 0 , swap link ;

\g Start for-next loop, will iterate count+1 times
: for ( ct: -- dest -1  rt: count -- r: u1 u1 )
   postpone dofor  bwmark  leaves off ; immediate compile-only

\g @see anscore
: do ( ct: -- dest -1  rt: end start -- r: -- start end )
   postpone dodo  bwmark   leaves off ; immediate compile-only

\g @see anscore
: ?do ( ct: -- dest -1  rt: end start -- r: -- start end )
   postpone do?do leaves off leaves linked bwmark ; immediate compile-only

\g @see anscore
: leave ( r: limit index -- )
   postpone branch  leaves linked ;  immediate compile-only

\g Apply xt to each element in list
: foreach ( xt list -- )
   swap >r @ >r 
   begin r@ while r> r@ over @ >r execute repeat
   rdrop rdrop ;

\g Syntactic sugar for FOREACH
: forall ( "list" "word" -- , azucar para foreach )
   @r+ execute @r+ swap foreach ; compile-only

: resolvleave ( a-addr -- )
   here over - swap ! ;

\g @see anscore
: loop ( ct: dest -1  rt:  --  r: limit index --  | limit index+1 )
   postpone doloop  bwresolve  
   forall leaves resolvleave
   postpone unloop ; immediate compile-only

\g Terminate for-next loop
: next ( ct: dest -1  rt: initial index --  | initial index-1 )
   postpone donext  bwresolve
   forall leaves resolvleave
   postpone unloop ; immediate compile-only

\g @see anscore
: +loop ( ct: dest -1  rt: n --  r: limit index --  | limit index+n )
   postpone do+loop  bwresolve
   forall leaves resolvleave
   postpone unloop ; immediate compile-only

\g @see anscore
: recurse ( ct: -- )
   bal @ 1- 2* pick -1 <> throw 
   bal @ 1- 2* 1+ pick compile, ; immediate compile-only

\g @see anscore
: abort ( i*x --  r: j*x -- )
   -1 throw ;

\g Runtime for ABORT"
: (abort")
   if r> count abort"msg 2! -2 throw else r> count + aligned >r then ;

\g @see anscore
: abort"
   postpone (abort") [char] " parse s, align ; immediate compile-only


\ Definers
' quit dup ?dodefine drop swap - constant /call

\g @see anscore
: to
   ' ?dodefine xtof dovalue <> -32 ?throw
   state @ if postpone doto /call - , else ! then ;  immediate

\g @see anscore
: create ( "<spaces>name" --  R: -- a-addr  )
   nesting?  head, xtof docreate xt, drop  0 , linklast ;

\g @see anscore
: >body ( xt -- a-addr )
   ?dodefine xtof docreate <> -31 ?throw
   cell+ ; 

\ Connect latest word to the DOES> code
: pipe ( --  R: xt -- )
   lastname name>xt >body cell- r> swap ! ;

\g @see anscore
: does> ( C: colon-sys1 -- colon-sys2 )
   nip -1 <> bal @ 1 <> or -22 ?throw
   (compile) pipe (xt,) dolist -1 ; immediate compile-only

\ 

\g Reserve one char in data space and store x in it
: c,  ( c -- ) \ core
   here c! 1 allot ;


\g Set the numeric conversion radix to ten (decimal).  
: decimal  ( -- )
   10 base ! ;

\g @see anscore
: u.  ( u -- )
   0 d. ;

\g @see anscore
: evaluate  ( i*x c-addr u -- j*x )
   source >r >r >in @ >r source-id >r 
   -1 to source-id  sourcevar 2!  >in off 
   interpret
   r> to source-id  r> >in !  r> r> sourcevar 2! ;

\g @see anscore
: word  ( char "<chars>ccc<char>" -- c-addr )
   here >r skipparse s, r@ to here r> ;

\g @see anscore
: find  ( a -- a 0 | xt 1 | xt -1 )
   count nfa if fxt fimmed else parsed cell+ @ -1 chars + 0 then ; 

\g @see anscore
: sm/rem  ( d n1 -- n2 n3 )
   2dup xor >r over >r >r dup 0< if dnegate then 
   r> abs um/mod
   r> 0< if swap negate swap then
   r> 0< if negate 0 over < -11 ?throw exit then
   dup 0< -11 ?throw ;

\g @see anscore
: */mod  ( n1 n2 n3 -- n4 n5 )
   >r m* r> fm/mod ;


\g @see anscore
: */  ( n1 n2 n3 -- n4 )
   */mod nip ;

\g @see anscore
: key  ( -- char )
   ekey 255 and ;


