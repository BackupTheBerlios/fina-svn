:noname ( u1 c a -- u1 c | u2 0 )
   over 127 = if 2drop dup if 
      8 emit bl emit 8 emit 1- 0 
   then else drop then ; 'khan !
