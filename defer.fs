:noname -1 abort" deferred word not initialized" ; pad !

: defer ( "<spaces>name" -- )
   create [ pad @ ] literal , does> @ execute ;

: is ( xt "<spaces>name" -- )
   ' >body ! ;

: deferred ( "<spaces>name" -- )
   ' >body @ compile, ; immediate compile-only 
