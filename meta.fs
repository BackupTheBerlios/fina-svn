\ Glossary stuff
create glosname 32 chars allot
: glos
   glosname count guppertype bl gemit
   [char] ) parse gtype s" )" gtype
   bl gemit gtype
   10 parse gtype gnl ;
: core
   s" CORE" glos ; immediate
: coreext
   s" CORE EXT" glos ; immediate
: double
   s" DOUBLE" glos ; immediate
: exception
   s" EXCEPTION" glos ; immediate
: facilityext
   s" FACILITY EXT" glos ; immediate
: internal
   s" INTERNAL" glos ; immediate
: search
   s" SEARCH" glos ; immediate
: string
   s" STRING" glos ; immediate
: toolsext
   s" TOOLS EXT" glos ; immediate



\ Helpers
: literal postpone literal ; immediate compile-only
: under+ rot + swap ;
: 2>r postpone swap postpone >r postpone >r ; immediate compile-only
: 2r> postpone r> postpone r> postpone swap ; immediate compile-only
: [char] bl parse drop c@ postpone literal ; immediate compile-only
: h# ( <hexnum> -- u )
    0 0 bl parse
    base @ >r  hex  >number  r> base !   
    0= invert -24 and throw 2drop
    state @ if  postpone literal  then
    ; immediate
: log2 
   -1 swap begin dup while 2/ 1 under+ repeat drop ;
: c+!  ( n addr -- )  
   >r r@  c@ +  r> c! ;
: append ( str len addr -- )
   2dup 2>r  count chars +  swap chars move ( ) 2r> c+! ;
: len c@ 31 and ;
: lex c@ 127 and ;
: namecount count 31 and ;
variable options
: case options off ; immediate compile-only
: of  
   1 options +! 
   postpone over postpone = 
   postpone if postpone drop ; immediate compile-only
: endof
   postpone else ; immediate compile-only
: endcase 
   postpone drop options @ 0 ?do postpone then loop ; immediate compile-only
: ,"
   [char] " parse here over 1 chars + allot place align ;
: asmtype
   bounds do 
      i c@ toupper case
        [char] @ of ." FETCH"    endof
        [char] ! of ." STORE"    endof
        [char] > of ." GT"       endof
        [char] < of ." LT"       endof
        [char] ? of ." Q"        endof
        [char] + of ." PLUS"     endof
        [char] - of ." MINUS"    endof
        [char] * of ." STAR"     endof
        [char] / of ." SLASH"    endof
        [char] \ of ." BSLASH"   endof
        [char] = of ." EQUALS"   endof
        [char] # of ." NUMBER"   endof
        [char] [ of ." LBRACKET" endof
        [char] ] of ." RBRACKET" endof
        [char] ( of ." LPAREN"   endof
        [char] ) of ." RPAREN"   endof
        [char] . of ." DOT"      endof 
        [char] , of ." COMMA"    endof
        [char] " of ." QUOTE"    endof
        [char] 0 of ." ZERO"     endof
        [char] 1 of ." ONE"      endof
        [char] 2 of ." TWO"      endof
        [char] 4 of ." FOUR"     endof
        [char] 6 of ." SIX"      endof
        [char] 8 of ." EIGHT"    endof
        [char] ' of ." TICK"     endof
        [char] $ of ." DOLLAR"   endof
        [char] : of ." COLON"    endof
        [char] ; of ." SEMI"     endof
        dup emit
      endcase
   loop ;
: xttype ." XT_" asmtype ;
: asm. ." 0x" base @ >r hex 0 <# #s #> type r> base ! ;

\ Metacompiler constants
32 constant compo
64 constant immed

\ Target dictionary
: tcells 
   tcellbits 8 / * ;
0 value tlastname

variable size  
5 tcells size ! \ For the header in tconfig.fs

: +body 
   here lastbody -  1 cells / tcells size +! ;
: +bytes
   align here lastbody - size +! ;

: .name
   namecount type ;
0 value link
: .label ." link" link 0 <# # # # # #> type ;
: .link 
   ."  .long " link if .label else 0 . then cr 
   link 1+ to link .label ." :" cr ;
variable underscore  underscore off
: taligned
   1 tcells 1- + 1 tcells negate and ;
: name>t ( -- )
   ."   .align 2" cr
   .link 
   ."   .byte " lastname lex asm.   
   lastname namecount    
   bounds do ." ," i c@ asm. loop cr
   ."   .align 2" cr 
   underscore @ if [char] _ emit then 
   lastname namecount xttype ." :" cr 
   lastname namecount nip 1+  taligned 1 tcells + size +! ;
: body>t
   ."  .long "
   lastbody begin 
      dup here <> 
   while 
      dup @ asm. 
      cell+ 
      dup here <> if ." ," then 
   repeat drop 
   +body ;
: bytes>t
   align
   lastbody begin 
      dup here <> 
   while 
      dup c@ asm.   
      1+ 
      dup here <> if ." ," then 
   repeat drop cr 
   +bytes ;
: create>t ( -- )
   name>t .call" DOCREATE" ."  .long 0 " cr body>t 
   /tcall 1 tcells + size +! ;
: var>t ( -- ) 
   name>t .call" DOVAR" body>t 
   /tcall size +! ;
: bytevar>t ( -- )
   name>t .call" DOVAR" ."   .byte " bytes>t 
   /tcall size +! ;
: val>t ( -- )
   name>t .call" DOVALUE" body>t
   /tcall size +! ;

-1 tcells value useroffset
: nextuser useroffset dup -1 tcells + to useroffset ;
: user>t ( -- )
   name>t
   .call" DOUSER"
   ."  .long " nextuser asm. 
   /tcall 1 tcells + size +! ;
: lit>t ( addr -- addr' )
   cell+  dup @ asm. ;
: cell>t ( addr -- addr' )
\  ." moving " dup @ xt>name if dup @ xt>name .name space else dup @ hex. then 
   dup @ .special dup 0< if 
      drop xt>name namecount xttype
   else
      nip 0 ?do lit>t loop
   then cell+ ;
: list>t ( -- )
   lastbody begin 
      dup here <> 
   while
      cell>t
      dup here <> if ." ," then
   repeat drop
   +body ;
: col>t ( -- )
   name>t
   .call" DOLIST"
   ."  .long " list>t 
   /tcall size +! ;
: prim>t ( -- )
   underscore on name>t underscore off
   ."  .long 0xdeadbeef," lastname namecount xttype cr 
   2 tcells size +! ;

:noname ; value type>t


: forgetit
   s" forget-previous marker forget-previous" evaluate cr cr ;
: >t
   type>t execute forgetit ;

: glosname! lastname namecount glosname place ;

: ?stack
   depth abort" stack error" ;

: constant' constant ;
: :' : ;
: to' postpone to ; 
: value' value ;

: prim
   >t ?stack ['] prim>t to type>t create glosname! ;
:' bcreate
   >t ?stack ['] bytevar>t to type>t create glosname! ;
:' create     
   >t ?stack ['] create>t to type>t create glosname! ;
:' user 
   >t ?stack ['] user>t to type>t user glosname! ;
:' ivariable
   >t ['] var>t to type>t variable glosname! lastxt execute ! ;
:' constant   
   >t ['] val>t to type>t constant glosname! ;
:' value      
   >t ['] val>t to type>t value glosname! ;
:' :
   >t ?stack ['] col>t to type>t :' glosname! ;
:' p: 
   >t ?stack fast if ['] prim>t else ['] col>t then to type>t :' glosname! ;

:' to postpone doto ; immediate compile-only

:' ?throw ['] do?throw here 2 cells - ! ; immediate compile-only

:' bye
   ."  .fill " /tdict size @ - .  ." ,1,0" cr
   ."  .long 0xcacacaca" cr 
   bye ; 

marker forget-previous
.init

