0 constant r/o
2 constant r/w
4 constant w/o

: bin ( u1 -- u2 )
   1 or ;

: open-file ( a u1 u2 -- a ior )
   >r 2dup parsed 2! r>
   openf ;

: read-file ( a1 u1 a2 -- u2 ior )
   readf ;

: write-file ( a1 u a2 -- ior )
   writef ;

: close-file ( a -- ior )
   closef ;

: mmap-file ( a1 -- a2 ior )
   mmapf ;

: file-size ( a -- ud ior )
   sizef ;

: file-position ( a -- ud ior )
   tellf ;

: reposition-file ( ud a -- ior )
   seekf ;

: read-line ( a1 u1 a2 -- u2 ior )
   linef ;

here 1 c, 10 c, pad !
: newline [ pad @ ] literal count ;

: write-line ( a1 u a2 -- ior )
   >r r@ write-file ?dup 0= if newline r@ write-file then rdrop ;

: create-file ( a1 u1 u2 -- a2 ior )
   open-file ;

variable #line  -1 #line !
variable #lines 0 #lines !

: save-input
   #line @  source  >in @  source-id 5 ;

: restore-input
   dup 5 = if
      drop to source-id >in ! sourceVar 2! #line ! 0 exit
   then 0 ?do drop loop -1 ;

create nrbuf 2 cells allot
: n>r ( n1 .. nn n -- )
    dup 10 > abort" too many items"
    r> over nrbuf 2! 
    cells negate [ here 9 cells + 5 cells + ] literal + >r exit
    >r >r >r >r   >r >r >r >r   >r
    nrbuf 2@ >r >r ; compile-only
: nr> ( -- n1 .. nn n )
    r> r> dup rot nrbuf 2!
    cells negate [ here 9 cells + 5 cells + ] literal + >r exit
    r> r> r> r>   r> r> r> r>   r> 
    nrbuf 2@ >r ; compile-only

create line 102 allot

: foreachline ( file xt -- )
   2>r begin
      1  #line +!
      line 100 2r> over swap 2>r read-line throw
   while ( u )
      line swap r@ execute
   repeat drop rdrop rdrop ;

: intline
   sourcevar 2! >in off interpret ;

: include-file
   save-input n>r  to source-id  #line off  
   source-id ['] intline foreachline
   nr> restore-input -37 and throw ;

variable verbose verbose off

: included 
   here >r 2dup
   r/o open-file throw
   dup >r include-file
   r> close-file throw
   verbose @ if
       type space ." took " here r> - . ." bytes" cr
   else 2drop rdrop then ;

: include 
   parse-word included ;

: require
   ." XXX require not implemented" cr include ;

