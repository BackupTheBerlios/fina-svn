

: .line type cr ;

0 value tocstatus
variable fname 0 ,

: .entry
   2dup s" ====" beginswith? if
      0 to tocstatus
   then
   tocstatus 1 = if
      ." ====" cr 
      2dup type cr
      ." @see " fname 2@ type cr
   then
   tocstatus 1+ to tocstatus 2drop ;

: toc
   2dup [char] / scan 1 /string 2dup [char] . scan nip - fname 2! 
   2 to tocstatus
   r/o open-file throw >r
   r@ ['] .entry foreachline 
   r> close-file throw ;

: toc{
   begin
      parse-word 2dup s" }toc" compare
   while
      toc
   repeat 2drop ;
