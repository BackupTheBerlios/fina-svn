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

\ variable #line  -1 #line !
\ variable #lines 0 #lines !
variable (fname) 0 ,

-1 value sourceline#
here ," the terminal" count (fname) 2!

: sourcefilename 
   (fname) 2@ ;

: save-input
   sourcefilename  sourceline#  
   source  >in @  source-id 7 ;

: restore-input
   dup 7 = if
      drop to source-id >in ! sourceVar 2!  
      to sourceline#   (fname) 2! 
      0 exit
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
      1 sourceline# + to sourceline# 
      line 100 2r> over swap 2>r read-line throw
   while ( u )
      line swap r@ execute
   repeat drop rdrop rdrop ;

: intline
   sourcevar 2! >in off interpret ;

: (finclude)
   to source-id  0 to sourceline#
   source-id ['] intline foreachline ;

\g @see ansfile
: include-file
   save-input n>r (finclude) nr> restore-input -37 ?throw ;


\g Hook at start of INCLUDED. The xt must be ( c-addr1 u1 --- c-addr2 u2 )
variable inchook0  ' noop inchook0 !

\g Hook at the end of INCLUDED. The xt must be ( c-addr -- ).
\g Will be called with the value of HERE before file was included.
variable inchook1  ' drop inchook1 !

\g @see ansfile
: included  ( i*x c-addr u -- j*x )
   inchook0 @execute 
   save-input n>r  here >r
   2dup r/o open-file throw >r 
   (fname) 2!
   r@ (finclude)
   r> close-file throw
   r> inchook1 @execute
   nr> restore-input -37 ?throw ;

\g Include file
\g @also included
: include ( i*x "filename" -- j*x )
   here parse-word s, count included ;


