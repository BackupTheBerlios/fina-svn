
: unused pad here - ;

: 2>r
   swap r> swap >r swap >r >r ;

: 2r>   
   r> r> swap r> swap >r swap ;

: doc"
   r> count over + aligned >r [ -1 chars ] literal + ; compile-only

: csliteral
   postpone doc" s, align ; immediate compile-only 
   
: c"
   [char] " parse   postpone csliteral ; immediate compile-only
