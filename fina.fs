
\ SYSTEM VARIABLES
0 ivariable echo

0 ivariable parsed 0 ,
internal ( -- a ) dvariable holding last found word

0 ivariable bal
internal ( -- a ) holds depth of control-flow stack

0 ivariable sourcevar 0 ,
internal ( -- a ) dvariable holding input buffer string

0 ivariable abort"msg  0 ,
internal ( -- a ) dvariable holding abort message

0 ivariable hld
internal ( -- a ) pointer to numeric output string

0 ivariable userp
internal ( -- a ) user variables pointer

-1 ivariable warnings
internal ( -- a ) holds flag to control printing of warnings

0 ivariable 'ekey?  
internal ( -- a ) execution vertor of EKEY?

0 ivariable 'ekey  
internal ( -- a ) execution vector of EKEY

0 ivariable 'emit? 
internal ( -- a ) execution vector of EMIT?

0 ivariable 'emit  
internal ( -- a ) execution vector of EMIT

0 ivariable '.prompt  
internal ( -- a ) execution vector of .PROMPT

0 ivariable 'throwmsg
internal ( -- a ) execution vector for getting throw error message

0 ivariable 'interpret
internal ( -- a ) execution vector for interpreter

0 ivariable forth-wordlist 0 ,
core ( -- a ) wid of forth wordlist

10 ivariable base
core ( -- a ) radix base for numeric i/o

0 ivariable >in
core ( -- a ) holds the offset from start of input buffer to parse area

0 ivariable state
core ( -- a ) holds compilation state flag

0 ivariable hasname?

\ SYSTEM VALUES
0 value get-current 
core ( -- wid ) identifier of the compilation wordlist

h# abadcafe value memtop  
internal ( -- a ) holds upper dictionary limit

0 value lastname  
internal ( -- a ) pointer to name of last word

h# abadcafe value here  
core ( -- a ) data space pointer

h# abadcafe value source-id  
coreext ( -- 0 | -1) source identifier

h# abadcafe value pad  
coreext ( -- a ) address of transient region

h# abadcafe value tib  
coreext ( -- a ) address of the terminal input buffer

0 value found 
internal ( -- a ) result of last nfa search

\ SYSTEM CONSTANTS
32 constant bl  
core ( -- c) character value for a space

\ USER VARIABLES

\ XXX could this be a normal var? 
\ probably, under the restriction that no task will check it between PAUSEs
user dpl 
internal ( -- a ) decimal point location

user stacktop  
internal ( -- a) uvar holding the stack pointer for each sleeping task

user follower
internal ( -- a) uvar holding a pointer to the next task

user sp0  
internal ( -- a) uvar holding a pointer to the stack origin

user rp0  
internal ( -- a) uvar holding a pointer to the return stack origin

user throwframe  
internal ( -- a) uvar holding the throw frame pointer for each task

\ PRIMITIVES

\ arithmetic
prim um+
internal ( u1 u2 -- u3 1|0) returns sum and carry of two unsigned numbers

prim and
core ( x1 x2 -- x3) bitwise and

prim 2*
core ( x1 -- x2) shift left 1 bit

prim 2/
core ( x1 -- x2) arithmetic shift right 1 bit

prim xor
core ( x1 -- x2) bitwise exclusive or

prim or
core ( x1 -- x2) bitwise inclusive or

\ comparisons
prim 0<
core ( n -- f) returns true if n is negative

prim 0=
core ( x -- f) returns true if x is zero


\ i/o
prim rx?
internal ( -- f) returns true if any key is pressed

prim rx@
internal ( -- u) receive keyboard event

prim tx?
internal ( -- f) returns true if output device is ready

prim tx! 
internal ( u --) send char to output device

\ stack
prim drop
core ( x -- ) discard topmost stack item

prim dup
core ( x -- x x) duplicate topmost stack item

prim over
core ( x1 x2 -- x1 x2 x1) duplicate second stack item

prim swap
core ( x1 x2 -- x2 x1) exchange top two stack items

prim rp@
internal ( -- a) return stack pointer

prim rp!
internal ( a -- ) set return stack pointer

prim sp@ 
internal ( -- a ) data stack pointer

prim sp!
internal ( a -- ) set data stack pointer


\ memory
prim !
core ( x a --) store value at address

prim @
core ( a -- x) fetch value at address

prim c!
core ( c a -- ) store char at address

prim c@
core ( a -- c) fetch char at address

prim move
core ( a1 a2 u -- ) copy u address units from a1 to a2

prim same?
internal ( a1 a2 u -- -1|0|1 ) alphabetically compare strings

\ misc
prim execute
core ( i*x xt -- j*x ) call execution token

prim endtick 
internal ( -- ) return control to caller program

prim bye
toolsext ( -- ) return to OS

prim ?dodefine
internal ( xt1 -- xt1 0 | a xt2) if xt1 calls a runtime, return the address after the call and the xt of the runtime

prim xt,
internal ( xt1 -- xt2 ) compile call to xt1 and return xt of current word



\ compile-only primitives
prim docreate compile-only
internal ( -- a ) runtime for CREATE

prim dolit compile-only
internal ( -- x ) runtime for LITERAL

prim douser compile-only
internal ( -- x ) runtime for USER

prim dovar compile-only
internal ( -- a ) runtime for VARIABLE

prim doconst  compile-only
internal ( -- x ) runtime for CONSTANT

prim dovalue  compile-only
internal ( -- x ) runtime for VALUE

prim dolist compile-only
internal ( -- ) runtime for processing colon lists

prim doloop compile-only
internal ( R: n1 n2 -- n1 n3 ) runtime for LOOP

prim do+loop compile-only
internal ( R: n1 n2 -- n1 n3 ) runtime for +LOOP

prim donext compile-only
internal ( R: n1 -- n2 ) runtime for NEXT

prim 0branch compile-only
internal ( f -- ) branch to inline address if flag is zero

prim branch compile-only
internal ( -- ) branch unconditionally to inline address

prim exit compile-only
core ( -- ) return control to caller

prim i compile-only
core ( -- x ) loop index

prim >r compile-only
core ( x --  R: -- x ) move item to return stack

prim r> compile-only
core ( -- x  R: x -- ) move item from return stack

prim r@ compile-only
core ( -- x  R: x -- x ) copy top of return stack

\ Files
prim openf 
internal ( a1 u1 u2 -- a2 ior ) open file

prim closef
internal ( a -- ior ) open file

prim readf
internal ( a1 u1 a2 -- u2 ior ) read from file

prim writef
internal ( a1 u1 a2 -- u2 ior ) write to file

prim mmapf
internal ( a1 -- a2 ) mmap file

prim argc
internal ( -- u ) number of arguments command line 

prim argv
internal ( u1 -- a u2 ) get command line argument

\ COLON DEFINITIONS
\ Multitasking
: pause  internal ( -- ) transfer control to next task
   rp@ sp@ stacktop !   follower @ >r ; compile-only  

\ I/O

: @execute  internal ( a -- i*x ) execute content of a
  \ @ ?dup if execute then ; 
   @ execute ;

: ekey?  facilityext ( -- f ) returns true if a keyboard event is available
   'ekey? @execute ;  

: ekey  facilityext ( -- u ) receive keyboard event
   ekey? 0= if begin pause ekey? until then 'ekey @execute ;  

: emit?  facilityext ( -- f ) returns true if output device is ready
   'emit? @execute ;  

: emit  core ( c -- ) send char to output device
   'emit @execute ;  

: .prompt  internal ( -- ) display prompt, vectored
   '.prompt @execute ;  

: cr  core ( -- ) display carriage return
   10 emit ;  

bcreate okstr ," ok"
: .ok  internal ( -- ) display prompt
   okstr count type ;  

: space  core ( -- ) send space to output device
   32 emit ;   

: spaces  core ( n -- ) send n spaces to output device
   0 max 0 ?do space loop ;  

: noop  internal ( -- ) do nothing
   ;  

: file  internal ( -- ) setup vectors for file input
   xtof noop '.prompt ! 
   echo off ;  

: con  internal ( -- ) setup vectors for console input
   xtof .ok  '.prompt ! 
   echo on ;  

\ Stack
p: ?dup  core ( x -- x x | 0 ) duplicate item if it is not zero
   dup if dup then ;  

p: nip  core ( x1 x2 -- x2 ) discard second stack item
   swap drop ;  

p: rot  core ( x1 x2 x3 -- x2 x3 x1 ) rotate top three stack items
   >r swap r> swap ;  

p: 2drop  core ( x1 x2 -- ) drop pair
   drop drop ;  

p: 2dup  core ( x1 x2 -- x1 x2 x1 x2 ) duplicate top pair
   over over ;  

p: 2swap  core ( x1 x2 x3 x4 -- x3 x4 x1 x2 ) exchange two pairs
   rot >r rot r> ;  

p: 2over  core ( x1 x2 x3 x4 -- x1 x2 x3 x4 x1 x2 ) duplicate second cell pair
   >r >r 2dup r> r> 2swap ;  

p: unloop  core ( --  R: x1 x2 -- ) discard loop control parameters
   r> r> r> 2drop >r ; compile-only  

p: rdrop  internal ( --  R: x -- ) drop top of return stack
   r> r> drop >r ; compile-only  

: @r+  internal ( -- x  R: a1 -- a2 ) fetch from post-incremented RTOS
   r> r@ @ r> cell+ >r swap >r ; compile-only  

: !r+  internal ( x --  R: a1 -- a2 ) store at post-incremented RTOS
   r> swap r@ ! r> cell+ >r >r ; compile-only  

\ More stack
: depth ( -- +n )
   sp@ negate sp0 @ + [ 1 tcells log2 ] literal arshift ;  
: rdepth ( -- +n )
   rp@ negate rp0 @ + [ 1 tcells log2 ] literal arshift ;  
p: pick coreext ( xu ... x1 x0 u -- xu ... x1 x0 xu ) copy uth item
   depth dup 2 < -4 ?throw
   2 - over u< -4 ?throw
   1+ cells sp@ + @ ;  
: rpick ( x_u ... x1 x0 u -- x_u ... x1 x0 x_u )
   rdepth dup 2 < -4 ?throw
   2 - over u< -4 ?throw 
   1+ cells rp@ + @ ;  
: .s
   depth 0 <# [char] > hold #s [char] < hold #> type space
   depth 0 > if depth 1- for i pick . next then cr ; 
: .rs
   rdepth . 4 spaces
   rdepth 0 ?do i rpick . loop cr ;  

\ Exception handling
: throw  exception ( i*x n -- i*x | j*x n ) raise exception if n is non-zero
   ?dup if
      throwframe @ rp!
      r> throwframe !
      r> swap >r sp!
      drop r>
   then ;  

: do?throw 
   0<> @r+ and throw ; compile-only  

: catch exception ( i*x xt -- j*x 0 | i*x n ) setup exception handling and execute xt 
   sp@ >r throwframe @ >r
   rp@ throwframe !  execute
   r> throwframe !
   rdrop 0 ;  

\ Arithmetic
p: +  core ( n1 n2 -- n3 ) add top two items
   um+ drop ;  

p: 1+  core ( n1 -- n2 ) increment by one
   1 + ;  

p: 1-  core ( n1 -- n2 ) decrement by one
   -1 + ;  

p: invert  core ( x1 -- x2 ) invert all bits
   -1 xor ;  
p: negate  core ( n1 -- n2 ) negate value
   invert 1+ ;  
p: -  core ( n1 n2 -- n3 ) substract n2 from n1
  negate + ;  


\ Looping
p: j  core ( -- x ) outer loop index
   rp@ [ 3 tcells ] literal + @ ; compile-only 

\ Comparisons
: 0<>  coreext ( x -- f ) returns true if x is not zero
   0= 0= ;  

p: <  core ( n1 n2 -- f ) signed comparison n1 < n2
   2dup xor 0< if  drop 0< exit  then - 0< ;  

p: u<  core ( u1 u2 -- f ) unsigned comparison u1 < u2
   2dup xor 0< if  nip 0< exit  then - 0< ;  

p: >  core ( n1 n2 -- f ) signed comparison n1 > n2
   swap < ;  

: >=  internal ( n1 n2 -- f ) returns true if n1 >= n2
   < 0= ; 

: u>  coreext ( u1 u2 -- f ) returns true if u1 > u2
   swap u< ;  

: u>=  internal ( u1 u2 -- f ) returns true if u1 >= u2
   u< 0= ;  

p: =  core ( x1 x2 -- f ) true if x1 = x2
   xor 0= ; 

: <>  coreext ( x1 x2 -- f ) returns true if x1 <> x2
   = 0= ; 

p: cell+  core ( n1 -- n2 ) increment by the size of a cell
   [ 1 tcells ] literal + ;  

p: dodo  internal ( x1 x2 --  R: x1 x2 ) runtime for DO
   r> rot >r swap >r >r ; compile-only 

: dofor ( n1 -- ) ( R: -- n1 n1 )
   r> swap dup >r >r >r ; compile-only  

: do?do ( n1 n2 -- ) ( R: -- n2 n1 )
   2dup = if 2drop r> dup @ + cell+ >r exit then
   r> rot >r swap >r cell+ >r ; compile-only 


\ Arithmetic
p: abs  core ( n -- u ) absolute value
   dup 0< if  negate  then ;  

: lshift  core ( x1 u -- x2 ) shift x1 u bits left
   0 ?do 2* loop ;  

: arshift  internal ( n1 u -- n2 ) shift n1 u bits right arithmetically
   0 ?do 2/ loop ;  

p: rshift  core ( x1 u -- x2 ) logical shift right of x1 by u bits
   0 ?do 2/ [ -1 1 rshift ] literal and loop ;  

p: max  core ( n1 n2 -- n3 ) greater of two items
   2dup < if swap then drop ;  

p: min  core ( n1 n2 -- n3 ) smaller of two items
   2dup > if swap then drop ;  

p: um*  core ( u1 u2 -- ud ) unsigned multiply giving double result
   0 swap [ tcellbits 1- ] literal for
      dup um+ >r >r dup um+ r> + 
      r> if >r over um+ r> + then
   next rot drop ;  

p: dnegate  double ( d1 -- d2 ) double negation
   invert >r invert 1 um+ r> + ;  

p: m*  core ( n1 n2 -- d ) signed multiply giving double result
   2dup xor 0< >r abs swap abs um* r> if dnegate then ;  

p: *  core ( x1 x2 -- x3 ) multiply
   um* drop ;  

p: um/mod core ( ud1 u1 -- u2 u3 ) u2=remainder u3=quotient of unsigned division
   dup 0= -10 ?throw 
   2dup u< 0= -11 ?throw 
   negate [ tcellbits 1- ] literal for
      >r dup um+ >r >r dup um+ r> + dup
      r> r@ swap >r um+ r> or 
      if >r drop 1+ r> else drop then r> 
   next drop swap ;  

: fm/mod  core ( d n1 -- n2 n3 ) n2=rem n3=quot of signed floored division
   >r r@ 2dup xor >r >r dup 0< if dnegate then
   r@ abs um/mod
   r> 0< if swap negate swap then 
   r> 0< if 
      negate over if r@ rot - swap 1- then 
      rdrop 
      0 over < -11 ?throw exit 
   then rdrop 
   dup 0< -11 ?throw ;  

p: s>d  core ( n1 -- d1 ) convert single number to double number
   dup 0< ;  

: /mod  core ( n1 n2 -- n3 n4 ) n3=rem n3=quot of n1/n2
   >r s>d r> fm/mod ;  

: /  core ( n1 n2 -- n3 ) divide n1 by n2
   /mod nip ;  

: mod  core ( n1 n2 -- n3 ) remainder of n1/n2
   /mod drop ;  

p: within  coreext ( x1 x2 x3 -- f ) circular inclusion test of x1 in [x2 x3)
   over - >r - r> u< ;  

p: d+  double ( d1 d2 -- d3 ) double numbers addition
   >r swap >r um+ r> r> + + ;  

\ Memory
p: char+  core ( n1 -- n2 ) increment by the size of a char
   1+ ;  

: ?dict  internal ( -- ) check for dictionary overflow
   here pad u>= -8 ?throw ; 

p: cells  core ( n1 -- n2 ) multiply by the size of a cell
   [ 1 tcells log2 ] literal lshift ;  

p: cell-  internal ( n1 -- n2 ) decrement by the size of a cell
   [ -1 tcells ] literal + ;  

p: 2!  core ( x1 x2 a -- ) store pair
   swap over ! cell+ ! ;  

p: 2@  core ( a -- x1 x2 ) fetch pair
   dup cell+ @ swap @ ;  

p: aligned  core ( a1 -- a2 ) align address to cell boundary
   [ 1 tcells 1- ] literal + [ -1 tcells ] literal and ;  

: align  core ( -- ) align HERE to cell boundary
   here aligned to here ?dict ;  

p: fill  core ( a u c -- ) fill u characters at address with c
   rot rot 0 ?do 2dup c! char+ loop 2drop ;  

p: count  core ( a1 -- a2 u) convert counted string to string
   dup char+ swap c@ ;  

: namecount  internal ( a1 -- a2 u ) convert word name to counted string
   count 31 and ;  

: place  internal ( a1 u a2 -- ) place string a1 u at a2
   over 255 u>= -18 ?throw
   >r r@ c!   r@ 1+ r> c@ move ; 

p: +!  core ( x a -- ) increment location by x
   swap over @ + swap ! ;  

: allot  core ( n -- ) allocate space in dictionary
   here + to here ?dict ;  

: ,  core ( x -- ) reserve one cell in data space and store x in it
   here !  [ 1 tcells ] literal allot ;  

: off  internal ( a -- ) store 0 at address
   0 swap ! ;  

: on  internal ( a -- ) store -1 at address
   -1 swap ! ;  

\ Parsing
: source  core ( -- a u ) return input buffer string
   sourcevar 2@ ;  

: /string  string ( a1 u1 n -- a2 u2 ) consume n chars from string
   >r r@ - swap r> chars + swap ;  

: remaining  internal ( -- a u ) returns unparsed input
   source >in @ /string ;  

: scan  internal ( a1 u1 c -- a2 u2 ) scan string looking for char
   >r begin  dup while over c@ i xor while 1 /string repeat then 
   rdrop ;  

: skip  internal ( a1 u1 c -- a2 u2 ) skip leading characters
   >r begin  dup while over c@ i = while 1 /string repeat then 
   rdrop ;  

: parse  coreext ( c "xxxc" -- a u ) parse input stream with delimiter
   >r remaining over swap r> scan >r
   over - dup r> if 1+ then >in +! ;  

: skipparse internal skipparse ( c "cccxxxc" -- a u ) parse input skipping leading delimiters
   >r remaining r@ skip drop remaining drop - >in +!
   r> parse ;  

: parse-word  internal ( "  xxx " -- a u ) skip leading spaces and parse word
   bl skipparse ; 

\ Dictionary
create #order 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 ,  
internal ( -- a ) variable holding the search order stack depth

: dict?  internal ( a -- f ) is address within dictionary space?
   dict0 here within ;  

: name>xt  internal ( a -- xt ) go from name to execution token
   namecount + aligned 
   dup @ h# deadbeef = if cell+ @ then ;  

: primxt? internal ( xt -- f ) is xt a primitive xt?
   dict? 0= ;  

: xt>name  internal ( xt -- c ) go from execution token to name
   dup xtof dict0 = if 8 - exit then
   >r r@ primxt? if
      xtof cold recurse
      begin cell- @ dup name>xt r@ = until
   else
      r@ begin cell- dup name>xt r@ = over cell- @ dict? and  until
   then  rdrop ;  

: bounds over + swap ;  

: forwordsin ( wid xt -- )
   0 to found
   >r cell+ begin 
      cell- @ dup found 0= and 
   while 
      r@ execute 
   repeat drop rdrop ;  

: forwords ( xt -- )
   >r #order cell+ #order @ cells bounds ?do
      i @ j forwordsin found if leave then
   1 cells +loop rdrop ;  

: match? ( a -- a )
   dup namecount parsed @ = if 
      parsed 2@ same? 0= if dup to found then
   else drop then ;  

: nfain  internal ( a u wid -- nfa ) find word in wordlist, sets PARSED, result also stored in FOUND
   >r parsed 2! r> xtof match? forwordsin found ;  
   
: nfa  internal ( a u -- nfa ) find word, sets PARSED, result also stored in FOUND
   parsed 2! xtof match? forwords found ;  

: fxt found name>xt ;  
: fimmed found c@ [ immed ] literal and 0= 2* 1+ ;  
: /fcompo found c@ [ compo ] literal and 0= ;  

: '  core ( "  xxx" -- xt ) parse name and return its xt
   parse-word nfa 0= -13 ?throw fxt ;  

: type  core ( a u -- ) send string to output device
   bounds ?do i c@ emit loop ; 

bcreate redefstr ," redefined "
: (head,)  internal ( a u -- ) compile a header
   ?dict
   dup 0= -16 ?throw
   dup 31 > -19 ?throw 
   2dup get-current nfain
   if  warnings @ if redefstr count type 2dup type space then  then
   align  get-current @ ,
   here to lastname
   s, align ;  

: head,  internal ( " xxx" -- ) parse input and compile header
   parse-word (head,) ;  

\ Numeric output
: hold  core ( c -- ) add char to beginning of pictured numeric output
    -1 hld +!   hld @ c! ;  

: <#  core ( -- ) initiate numeric output conversion process
   here [ /hld ] literal + hld ! ;   

: #  core ( ud1 -- ud2 ) extract one digit and append to pictured numeric output
   0 base @ um/mod >r base @ um/mod swap 
   9 over < [ char A char 9 1 + - ] literal and + 
   [ char 0 ] literal + hold r> ;  

: #s  core ( ud -- 0 0 ) add all digits in ud to pictured numeric output
   begin # 2dup or 0= until ;  

: #>  core ( xd -- a u ) prepare pictured numeric output to be TYPEd
   2drop hld @ here [ /hld ] literal + over - 1chars/ ;  

: sign  core ( n -- ) if n is negative append "-" to pictured numeric output
   0< if [char] - hold then ;  

: (d.)  internal ( d -- a u) convert double number to counted string
   swap over  dup 0< if  dnegate  then   <# #s rot sign #> ;  

: d.  double ( d -- ) display double signed number followed by space
   (d.) type space ;  

: .  core ( n -- ) display signed number followed by space
   s>d d. ;  

\ Interpreter

: [  core ( -- ) enter interpretation state
   state off ; immediate compile-only 

: ]  core ( -- ) enter compilation state
   state on ; 

0 ivariable 'khan \ ( c a -- c | 0 )
: accept
   0 rot rot bounds ?do  
      ekey 255 and 
      dup 10 = if drop leave then
      i 'khan @execute 
      dup if
         echo @ if  dup emit  then  
         i c! 1+
      else drop then
   loop ;
         
: refill  coreext ( -- f ) refill the input buffer
   source-id if 0 exit then
   tib dup [ /tib ] literal accept sourcevar 2! >in off -1 ;  

: compile,  coreext ( xt -- ) compile xt into current colon definition
   , ; compile-only  

: (compile)  internal ( -- ) compile inline xt into current colon def
   @r+ compile, ;  

: xtof  internal ( -- xt ) extract inline xt
   @r+ ; compile-only 

: (xt,)  internal ( -- xt ) compile call to inline xt into current colon def
   @r+ xt, ;  

: toupper  internal ( c1 -- c2) convert character to upper case
   dup [char] a [ char z 1+ ] literal within h# 20 and invert and ;  

: digit  internal ( c -- u ) convert character to digit
   toupper
   dup [ char 9 1+ ] literal [char] A within 128 and + 
   dup [char] A u>= [ char A char 9 - 1- ] literal and - 
   [char] 0 - ;  

: digit?  internal ( c -- f ) is character a valid digit for current base?
   digit base @ u< ;  

: accum  internal ( ud1 u1 -- ud2 ) accumulate u1 into ud1
   swap base @ um* drop rot base @ um* d+ ;  

: >number  core ( ud1 a1 u1 -- ud2 a2 u2 ) add number string's value to ud1, returns unconverted chars
   begin
      over c@ digit? over and 
   while
      >r >r r@ c@ digit accum r> r> 1 /string
   repeat ;  

: s>unumber ( a u -- u | ud )
   -1 dpl !
   0 0 2swap begin 
      >number dup 
   while 
      over c@ [char] . <> -13 ?throw dup 1- dpl ! 1 /string 
   repeat 2drop 
   dpl @ 0< if drop then ;  

: s>number ( c-addr u -- x | x x )
   over c@ [char] - = >r  
   r@ negate /string
   s>unumber 
   r> if 
      dpl @ 0<  if negate else dnegate then  
   then ;  
: literal ( C: x -- ) ( R: -- x )
   postpone dolit , ; immediate compile-only  
: 2literal 
   swap postpone literal postpone literal ;  immediate compile-only 
: interpret
   begin
      depth 0< -4 ?throw
      parse-word dup
   while
      nfa if 
         state @ /fcompo or 0= -14 ?throw 
         fxt state @ if 
            fimmed 0< if compile, else execute then 
         else 
            execute 
         then
      else 
         parsed 2@ s>number state @ if 
            dpl @ 0< if postpone literal else postpone 2literal then
         then
      then
   repeat 2drop ;
bcreate exstr ," exception # "
: quit
   begin
      sp0 @ sp!  rp0 @ rp!  0 to source-id  bal off  postpone [  begin
         refill drop space
         'interpret @ catch ?dup 0=
      while
         state @ 0= if .prompt cr then
      repeat
      dup -1 <> if
         space
         dup -2 = if  abort"msg 2@ type then
         dup -2 <> if
            parsed 2@ type space [char] ? emit space
            dup -1 -58 within 'throwmsg @ 0= or if 
               exstr count type . 
            else 'throwmsg @execute type then
         then cr
      then
   again ;  
: :noname ( -- xt colon-sys )
   bal @ -29 ?throw
   hasname? off  1 bal !  ]
   xtof dolist xt, dup -1 ;  
: : ( "<spaces>name" -- colon-sys )
   head, :noname rot drop   hasname? on ;  
0 ivariable dummy1
: linklast ( -- )
   lastname get-current ! ;  
: ; ( colon-sys -- )
   bal @ 1- -22 ?throw 
   nip 1+ -22 ?throw
   hasname? @ if  linklast  then
   hasname? off  bal off
   postpone exit  postpone [ ; immediate compile-only 

\ Jumps
: bwmark
   here 1 bal +! -1 ;

: fwmark
   here h# deadbeef , 1 bal +! 1 ;

: bwresolve
   -1 <> -22 ?throw  -1 bal +!
   here - , ;

: fwresolve
   1 <> -22 ?throw  -1 bal +!
   here over - swap ! ;

\ Definers
p: doto  internal ( x -- ) runtime for TO, store x at inline address
\ XXX will fail on x86
   @r+ cell+ ! ; compile-only 

: postpone ( "<spaces>name" -- )
   ' fimmed 0< if postpone (compile) , exit then
   compile, ; immediate compile-only  

\ Strings
: s, ( c-addr u -- )
   here over char+ allot place align ; 

\ Initialization
: cold ( -- )
   xtof dict0 xt>name cell- xtof dict0 cell+ !
   xtof dummy2 xt>name to here
   dict0
   [ /tdict ] literal + dup to memtop
   dup userp ! [ /user ]  literal - \ must be initialized before rp0 and sp0
   dup rp0 !   [ /rs ]    literal - 
   dup sp0 !   [ /ds ]    literal - 
   [ /tib ]   literal - dup to tib
   [ /pad ]   literal - dup to pad
   drop
   xtof rx? 'ekey? !
   xtof rx@ 'ekey !
   xtof tx? 'emit? !
   xtof tx! 'emit !
   xtof drop 'khan !
   xtof interpret 'interpret !
   xtof cold xt>name forth-wordlist !
   forth-wordlist to get-current
   get-current 1 #order 2!
   file
   quit ;
0 ivariable dummy2
0 ivariable dummy3
:noname 
   ."  .long " 
   /tdict size @ - 1 tcells / 
   0 do ." 0," loop ." 0" cr 
   ."  .long 0xcacacaca" cr 
; execute

bye
