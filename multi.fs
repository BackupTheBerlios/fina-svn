\ FINA multitasker
\
\ Originally written by Bill Muench.
\ Adapted to hForth by Wonyong Koh
\ Adapted to FINA by Jorge Acereda

require assert.fs

: hex. base @ >r hex . r> base ! ;

: user ( "<spaces>name" -- ) 
   nesting?  head, xtof douser xt, drop  
   lastuser ,  lastuser -1 cells + to lastuser
   linklast ;

\g User variable holding the stack pointer for each sleeping task
user stacktop  ( -- a-addr )




\ Structure of a task created by HAT
\ userP points follower.
\ //userP//return_stack//data_stack
\ //user_area/user1/taskName/throwFrame/stackTop/status/follower/sp0/rp0

\ //user_area/user1/taskName/throwFrame/stackTop/status/follower/sp0/rp0

\   's          ( tid "uservar" -- a-addr )
\               Index another task's USER variable
: 's-offset ' execute userP @ - assert0( DUP -1024 1 WITHIN ) ; IMMEDIATE
: 's 
   postpone 's-offset state @ if
      postpone literal postpone +
   else + then ; immediate

: findprevtask ( tid -- tid')
    's status userp @ begin
        2dup 's follower @ <>
        over 's follower @ status <> and
    while 
        's follower @ cell+
    repeat swap over 's follower @ = and ;


\g Stop current task and transfer control to the task of which
\g 'status' USER variable is stored in 'follower' USER variable
\g of current task.
: pause  ( -- ) 
   rp@ sp@ stacktop !  follower @ >r ; compile-only  


\g Wake current task.
: wake  ( -- )
   r> userp !          \ userp points 'follower' of current task
   stacktop @ sp!      \ set data stack
   rp! ; compile-only  \ set return stack

\g Sleep current task.
: stop  ( -- )
   ['] branch status ! PAUSE ;

\g Sleep another task.
: sleep ( tid -- )
    assert( dup findprevtask )
    's status   ['] branch  SWAP ! ;

\g Awake another task.
: AWAKE  ( tid -- )
    assert( dup findprevtask )
    's status   ['] wake  SWAP ! ;

\g Kill another task.
: kill ( tid -- )
    >r
    assert( r@ userp @ <> )      \ no es un suicidio
    assert( r@ findprevtask )    \ esta en la lista
    r@ findprevtask r@ 's follower @ swap 's follower ! 
    assert( r@ findprevtask 0= ) \ no esta en la lista
    rdrop ;

: ,allot ( size fill -- )
   >r here over allot swap r> fill align ;

\g Create a new task.
: hat ( user_size ds_size rs_size "<spaces>name" --  rt: -- tid )
   create 
   2 pick aligned 2 pick aligned 2 pick aligned + + here + >r
   r@ ,  
   [ hex ]
   0AA ,allot here cell- r@ 's rp0 ! 
   055 ,allot here cell- r@ 's sp0 ! 
   r@ 's sp0 @ r@ 's rp0 @ 2>r 
   033 ,allot
   [ decimal ]
   2r> r@ 's rp0 ! r@ 's sp0 !
   lastname r> 's taskname ! 
   does> @ ;

\g Initialize and link new task into PAUSE chain.
: build ( tid -- )
    assert( dup findprevtask 0= )
    follower @                \ current task's 'follower'
    over 's follower !        \ store it in new task's 'follower'
    dup 's status follower !  \ store new 'status' in current 'follower'
    sleep ;               \ sleep new task

\g Activate the task identified by tid. ACTIVATE must be used
\g only in definition. The code following ACTIVATE must not
\g EXIT. In other words it must be infinite loop like QUIT.
: activate ( tid -- )
   assert( dup findprevtask )
   dup 's sp0 @ over 's rp0 @  
   R> over !                   \ save entry at rp
   over !                      \ save rp at sp
   over 's stackTop !          \ save sp in stackTop
   awake ; compile-only

\g Display tasks list
: .tasks ( tid -- )
   cr follower                    \ current task's tid
   begin
      dup 's taskname @ .name
      ."  at " dup hex.
      dup 's status @  ['] wake = if
         dup follower = if s" running " else s" awaked " then
      else
         s" sleeping "
      then type cr
      @ [ follower status - ] literal +           \ next task's follower
      dup follower =
    until drop ;


\ Initializations for system task
' wake status !
status follower !
follower constant system
lastname taskname !


\ Pause when no key ready
:noname
   ekey? 0= if begin pause ekey? until then [ 'ekey @ compile, ] ;  'ekey !




88 88 88 hat mierda  mierda build
: test mierda activate 0 begin 1+ dup . pause again ;
test
:noname 10 0 do pause loop cr ; execute
