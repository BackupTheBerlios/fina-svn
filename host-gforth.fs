warnings off
: xt>name ( xt -- name )
   dup >name if >name name>string drop 1- else drop 0 then ;
: lastname 
   last @ cell+ ;
create do?throw
: .special ( xt -- xt n, n=-1 if not special, otherwise # of lits to move )
   ['] ;s      over = if ." XT_EXIT"        0 exit then
   ['] (for)   over = if ." XT_DOFOR"       0 exit then
   ['] (do)    over = if ." XT_DODO"        0 exit then
   ['] (?do)   over = if ." XT_DOQDO,"      1 exit then
   ['] lit     over = if ." XT_DOLIT,"      1 exit then
   ['] (loop)  over = if ." XT_DOLOOP,"     1 exit then
   ['] (+loop) over = if ." XT_DOPLUSLOOP," 1 exit then
   ['] (next)  over = if ." XT_DONEXT,"     1 exit then
   ['] ?branch over = if ." XT_ZEROBRANCH," 1 exit then
   ['] branch  over = if ." XT_BRANCH,"     1 exit then
   ['] do?throw over = if ." XT_DOQTHROW,"  1 exit then
   -1 ;
: lastbody
   lastxt >body ;
s" glos.txt" w/o open-file throw constant glos 
: gemit
   glos emit-file throw ;
: gtype 
   bounds do i c@ gemit loop ;
: guppertype 
   bounds do i c@ toupper gemit loop ;
: gnl 
   newline gtype ;
\ Undefined in host
create rx? create rx@ create tx? create tx! create ?dodefine
create xt, create um+ create dolit create douser
create dovar create dofor create donext create doloop create do?do
create do+loop create 0branch create dodo create docreate
create doconst create dos" create s, create (abort") create dovalue
create primxt? create dict0 create /prims create prims create dict?
create cold

create exit create pause create sourceid create .hi 
create stacktop create follower create throwframe create memtop
create sourcevar create skipparse create histr create ?dict
create name>xt create #order create parse-word create search-word
create errword create redefstr create okstr create hld create bal
create throwmsgtable create .prompt create abort"msg 
create throwmsg create exstr create (d.) create (doublealso)
create dolit create pack" create dp create choose create litchoose
create doublealso, create doublealso create doword create userp
create 'ekey? create 'ekey create 'emit? create 'emit
create 'interpret create @execute create $skip create same?
create (nfa-search-wordlist) create remaining create digit
create accum create dolist create hasname? create nesting? 
create linklast create (head,) create head, create 0branch
create branch create fw create bw create resolve create mark
create link create foreach create forall create @r+ create !r+
create leaves create resolvleave create (xt,) create xtof create pipe
create .ok create con create echo create 'echo create '.prompt
create file create doto create ?throw

: ahead postpone ahead ; immediate compile-only
: if postpone if ; immediate compile-only
: then postpone then ; immediate compile-only
: again postpone again ; immediate compile-only
create sliteral immediate compile-only

create 4arshift create 8arshift create 12arshift create 16arshift
create 20arshift create 24arshift create 28arshift create .dig
create .val create dumprow create arshift

create type2 create rdepth create rpick

create head>xt create xt>head create forwords create findname create naddr create nlen create nomore create forwords create co create caller create found create cellcount create match? create forth-wordlist' create #order' create wid 
create forwordsin create findnamein
create fxt create fimmed create /fcompo
create 'throwmsg create s>unumber
create parsed create nfa create nfain
create 'khan
create dummy2
create bwmark create bwresolve create fwmark create fwresolve
create .err
create '.error create .rs
create do2lit

: char+ postpone 1+ ; immediate compile-only
: 1chars/ ; immediate
: cell- 1 cells - ;
