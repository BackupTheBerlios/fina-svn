\g @see ansdouble 
: d=  ( d1 d2 -- flag )
    rot xor >r xor r> or 0= ;

: d<>  ( d1 d2 -- flag )
   d= 0= ;

\g @see ansdouble
: 2constant ( xd "name" -- )
   create , , does> 2@ ;

\g @see ansdouble
: 2variable ( xd "name" -- )
   create 0 , 0 , does> ;

\g @see ansdouble
: d- ( d1 d2 -- d3 )
   dnegate d+ ;

\g @see ansdouble
: d< ( d1 d2 -- flag )
   rot - ?dup if  0 > nip nip  else  u<  then ;

\g @see ansdouble
: d0< ( d -- flag )
   nip 0< ;

\g @see ansdouble
: d0=  ( d -- flag )
   or 0= ;

: d> ( d1 d2 -- flag )
   2swap d- d0< ;

\g @see ansdouble
: d2*  ( d1 -- d2 )
   2dup d+ ;

\g @see ansdouble
: d2/  ( d1 -- d2 )
   swap 2/ over 1 and 31 lshift or swap 2/ ;

\g @see ansdouble
: d>s  ( d -- u )
   drop ; 

\g Discard second pair
: 2nip  ( x1 x2 x3 x4 -- x3 x4 )
   >r nip nip r> ;

\g @see ansdouble
: dmax  ( d1 d2 -- d3 )
   2over 2over d< if 2nip else 2drop then ;

\g @see ansdouble
: dmin  ( d1 d2 -- d3 )
   2over 2over d< if 2drop else 2nip then ;

\g @see ansdouble
: dabs  ( d1 -- d2 )
   2dup dnegate dmin ;

\g @see ansdouble
: m+  ( d1 n1 -- d2 )
   s>d d+ ;

: d<=
   d- 2dup d0< >r d0= r> or ;

