file warnings off

: immediate
   lastname c@ 64 or lastname c! ;

: \ 
   source >in ! drop ; immediate

: \g postpone \ ; immediate
: core  postpone \ ; immediate
: internal postpone \ ; immediate

: compile-only
   lastname c@ 32 or lastname c! ;

: char
   parse-word drop c@ ;

: [char] 
   char postpone literal ; immediate compile-only

: ( 
   [char] ) parse 2drop ; immediate


\ Misc


: chars  ( n1 -- n2 ) \ core
\g convert characters to address units
   ; immediate

: 1chars/  ( n1 -- n2 ) \ internal
\g convert address units to chars
   ; immediate

: [']  ( "  xxx"" --  runtime: -- xt ) \ core
\g parse name and return its xt
   ' postpone literal ; immediate compile-only

: dos"  ( -- c-addr u ) \ internal
\g runtime for s" 
   r> count 2dup + aligned >r ; compile-only

: sliteral ( c-addr u -- )
   postpone dos" s, align ; immediate compile-only 

: s" ( "ccc<">" --  R: -- c-addr u )
   [char] " parse   postpone sliteral ; immediate compile-only

: ." ( "ccc<">" --  R: -- )
   [char] " parse   postpone sliteral 
   postpone type ; immediate compile-only


\ Flow control

: ahead ( C: -- orig )
   postpone branch   fwmark ;  immediate compile-only

: if ( compilation: C: -- orig   runtime: x -- )
   postpone 0branch   fwmark ; immediate compile-only

: then ( compilation: C: orig --   runtime: -- )
   fwresolve ; immediate compile-only

: else ( compilation: C: orig1 -- orig2   runtime:  -- )
   postpone ahead  2swap  postpone then ; immediate compile-only

: begin ( C: -- dest )
   bwmark ; immediate compile-only

: while ( C: dest -- orig dest )
   postpone if   2swap ; immediate compile-only 

: again ( C: dest -- )
   postpone branch  bwresolve ; immediate compile-only

: repeat ( C: orig dest -- )
   postpone again  postpone then ; immediate compile-only

: until ( C: dest -- )
   postpone 0branch  bwresolve ; immediate compile-only

\ Exceptions
: ?throw ['] do?throw here 2 cells - ! ; immediate compile-only

\ Definers
: nesting? ( -- )
   bal @ -29 ?throw ;

: constant ( x "<spaces>name" --  R: -- x )
   nesting?  head,  xtof doconst xt, drop  , linklast ;

: value ( x "name" -- R: -- x )
   nesting?  head,  xtof dovalue xt, drop  , linklast ;

: variable ( "<spaces>name" -- ) 
   nesting?  head, xtof dovar xt, drop  -559038737 , linklast ; 


variable leaves

: link ( item list -- , enlaza un item a la lista)
    2dup @ swap ! ! ;

: linked ( list -- , enlaza here a la lista)
    here 0 , swap link ;

: for ( C: -- for-sys )
   postpone dofor  bwmark  leaves off ; immediate compile-only

: do ( C: -- do-sys )
   postpone dodo  bwmark   leaves off ; immediate compile-only

: ?do
   postpone do?do leaves off leaves linked bwmark ; immediate compile-only

: leave ( --  R: loop-sys -- )
   postpone branch  leaves linked ;  immediate compile-only

: foreach ( xt list -- , recorre una lista aplicando xt a cada elemento)
   swap >r @ >r 
   begin r@ while r> r@ over @ >r execute repeat
   rdrop rdrop ;

: forall ( "list" "word" -- , azucar para foreach )
   @r+ execute @r+ swap foreach ; compile-only

: resolvleave ( a-addr -- )
   here over - swap ! ;

: loop ( C: do-sys -- )
   postpone doloop  bwresolve  
   forall leaves resolvleave
   postpone unloop ; immediate compile-only

: next ( C: do-sys -- )
   postpone donext  bwresolve
   forall leaves resolvleave
   postpone unloop ; immediate compile-only

: +loop ( C: do-sys -- )
   postpone do+loop  bwresolve
   forall leaves resolvleave
   postpone unloop ; immediate compile-only

: recurse ( -- )
   bal @ 1- 2* pick -1 <> throw 
   bal @ 1- 2* 1+ pick compile, ; immediate compile-only

: abort ( i*x -- ) ( R: j*x -- )
   -1 throw ;

: (abort")
   if r> count abort"msg 2! -2 throw else r> count + aligned >r then ;

: abort"
   postpone (abort") [char] " parse s, align ; immediate compile-only



\ Definers
: to
   ' ?dodefine xtof dovalue <> -32 ?throw
   state @ if postpone doto cell- , else ! then ;  immediate

: create ( "<spaces>name" --  R: -- a-addr  )
   nesting?  head, xtof docreate xt, drop  0 , linklast ;

: >body ( xt -- a-addr )
   ?dodefine xtof docreate <> -31 ?throw
   cell+ ; 

: pipe ( --  R: xt -- )
   lastname name>xt >body cell- r> swap ! ;

: does> ( C: colon-sys1 -- colon-sys2 )
   nip -1 <> bal @ 1 <> or -22 ?throw
   (compile) pipe (xt,) dolist -1 ; immediate compile-only

\ 

: c,  core ( c -- ) reserve one char in data space and store x in it
   here c! 1 allot ;


: decimal 10 base ! ;

: u. core ( u -- ) display unsigned number followed by space
   0 d. ;

: evaluate  core ( i*x a u -- j*x ) evaluate string
   source >r >r >in @ >r source-id >r 
   -1 to source-id  sourcevar 2!  >in off 
   interpret
   r> to source-id  r> >in !  r> r> sourcevar 2! ;

: word  core ( c "cccxxxc" -- a ) skip leading delimiters and parse a word
   here >r skipparse s, r@ to here r> ;

: find  ( a -- a 0 | xt 1 | xt -1 ) \ XXX
   count nfa if fxt fimmed else parsed cell+ @ -1 chars + 0 then ; 

: sm/rem  core ( d n1 -- n2 n3 ) n2=rem n3=quot of symmetric division
   2dup xor >r over >r >r dup 0< if dnegate then 
   r> abs um/mod
   r> 0< if swap negate swap then
   r> 0< if negate 0 over < -11 ?throw exit then
   dup 0< -11 ?throw ;

: */mod  core ( n1 n2 n3 -- n4 n5 ) n4=rem n5=quot of (n1*n2)/n3
   >r m* r> fm/mod ;

: */  core ( n1 n2 n3 -- n4 ) n4 = (n1*n2)/n3
   */mod nip ;

\ Debugging stuff
: hex 16 base ! ;

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

: dump
    base @ >r hex
    16 /mod swap >r
    0 ?do 16 dumprow loop
    R> dumprow drop r> base ! ;
