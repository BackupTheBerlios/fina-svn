\g Verbose includes

variable (incs)  (incs) off

:noname 
   (incs) linked 2dup here 2 cells allot 2! ; inchook0 !

: (.inc) 
   cell+ 2@ type cr ;

: .included
   forall (incs) (.inc) ;

: (inc?)
   cell+ 2@ 2over compare 0= found or to found ;

: included? 
   0 to found forall (incs) (inc?) 2drop found ;

: required
   2dup included? if 2drop else included then ;

: require
   parse-word required ;
