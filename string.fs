
: compare
    rot 2dup swap - >r
    min same? ?dup if rdrop exit then
    r> dup if 0< 2* 1+ then ;

: -trailing ( a u1 -- a u2 )
   begin 2dup + 1- c@ bl = over and while 1- repeat ;
