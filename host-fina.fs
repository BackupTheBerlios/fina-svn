file warnings off

\ glossary stuff
create glosname 32 chars allot
: gtype 2drop ; 
: gemit drop ;
: guppertype 2drop ;
: gnl 10 gemit ;

: lastxt ( -- xt)
   lastname name>xt ;
: lasthead ( -- addr)
   lastname ;
: .special ( xt -- xt n, n=-1 if not special, otherwise # of lits to move )
   ['] dolit   over = if ." XT_DOLIT,"      1 exit then
   ['] do?do   over = if ." XT_DOQDO,"      1 exit then
   ['] doloop  over = if ." XT_DOLOOP,"     1 exit then
   ['] do+loop over = if ." XT_DOPLUSLOOP," 1 exit then
   ['] donext  over = if ." XT_DONEXT,"     1 exit then
   ['] 0branch over = if ." XT_ZEROBRANCH," 1 exit then
   ['] branch  over = if ." XT_BRANCH,"     1 exit then
   ['] do?throw over = if ." XT_DOQTHROW,"  1 exit then
   -1 ;
: type? 
   postpone name>xt postpone ?dodefine postpone nip
   ' postpone literal postpone = ; immediate compile-only
: colon? ( nfa -- flag)
   type? dolist ;
: var? ( nfa -- flag)
   type? dovar ;
: con? ( nfa -- flag)
   type? doconst ;
: user? ( nfa -- flag)
   type? douser ;
: val? ( nfa -- flag)
   type? dovalue ;
: lastbody ( -- addr)
   lasthead var?
   lasthead user? or 
   lasthead con? or 
   lasthead var? or 
   lasthead colon? or
   lasthead val? or 
   if lastxt ?dodefine drop else lastxt >body then ;
: marker ( "name" -- , simplified marker word, enough for our purposes)
   here get-current @ create , , 
does> 
   dup @ get-current ! cell+ @ to here ;
: user variable ;

\ Undefined
create dummy2

\ We create these as normal words so that .special doesn't translate
\ them. This is to avoid translating things like "(compile),doloop" 
create dolit create do?do create doloop create do+loop create donext 
create 0branch create branch

