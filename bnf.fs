\ BNF Parser                                (c) 1988 B. J. Rodriguez
\ Adapted to FINA by Jorge Acereda

: dp! to here ;
0 value success
: <bnf ( enter rule )
   r> success if  >in @ here 2>r  >r  else  drop  then ;
: bnf> ( leave rule )
   r> 2r> success if  2drop  else  dp! >in !  then >r ;
: | ( prepare to evaluate alternate rule )
   r> 2r> success if  2drop drop  else  2dup dp! >in ! 2>r >r then 
   true to success ;
: bnf: ( start rule definition)
   : reveal postpone <bnf ;
: ;bnf ( end rule definition)
   postpone bnf> postpone ; ; immediate
: @token ( - n , get current token)  
   source >in @ /string drop c@ ;
: +token ( -- , consume token)
   success negate >in +! ;
: =token ( n -- , compare against current token and set success)
   @token =  success and to success ;
: token ( n "name" -- , create a token)
   create c, does> c@ =token +token ;
: 0bnf ( -- , start bnf definition)
   0 source + c!
   true to success ;
: /bnf ( -- , end bnf definition)
   source nip >in ! ;


1 [if]


\ BNF Parser Example #1 - pattern recog.
\ from Aho & Ullman, Principles of Compiler Design, p.137
\ this grammar recognizes strings having balanced parentheses
hex    28 token '('      29 token ')'      0 token <eol>
bnf: <char>
   @token  dup 2a 7f within  swap 1 27 within or to success  +token ;bnf
bnf: <s>   '(' <s> ')' <s>  |  <char> <s>  |  ;bnf
: parse1
   0bnf <s> <eol> /bnf
   cr success if ." successful " else ." failed " then ;


\  BNF Parser Example    #2  - infix notation        18 9 88 bjr 14:54
hex    2b token   '+'    2d  token '-'     2a  token  '*'     2f token '/'
       28 token   '('    29  token ')'     5e  token  '^'
       30 token   '0'    31  token '1'     32  token  '2'     33 token '3'
       34 token   '4'    35  token '5'     36  token  '6'     37 token '7'
       38 token   '8'    39  token '9'       0 token  <eol>
bnf: <digit>      
   '0'  | '1' | '2' |  '3' | '4' | '5' | '6' | '7' |  '8' | '9' ;bnf
bnf: <number>    <digit> <number>    |     <digit> ;bnf

\ from Aho & Ullman,     Principles of Compiler Design, pp.135,178
0 value (expression)
bnf: <element>     '(' (expression) execute ')'  |   <number> ;bnf
bnf: <primary>     '-' <primary>    |   <element> ;bnf
bnf: <factor>    <primary> '^' <factor> | <primary> ;bnf
bnf: <t'>     '*' <factor> <t'> | '/' <factor> <t'> | ;bnf
bnf: <term>  <factor> <t'> ;bnf
bnf: <e'>    '+' <term> <e'>  | '-' <term> <e'>  | ;bnf
bnf: <expression>     <term> <e'> ;bnf
' <expression> to (expression)
: parse2     
   0bnf  <expression> <eol> /bnf
   cr success if  ." successful " else ." failed " then ;



\  BNF Example #3       code generation               18 9 88 bjr 21:57

: (s,)
   tuck here swap move allot ;
: ," 
  [char] " parse postpone sliteral postpone (s,) ; immediate compile-only


hex    2b token   '+'    2d  token '-'     2a  token  '*'     2f token '/'
       28 token   '('    29  token ')'     5e  token  '^'
       30 token   '0'    31  token '1'     32  token  '2'     33 token '3'
       34 token   '4'    35  token '5'     36  token  '6'     37 token '7'
       38 token   '8'    39  token '9'       0 token  <eol>
bnf: {digit}      
   '0'  | '1' | '2' |  '3' | '4' | '5' | '6' | '7' |  '8' | '9' ;bnf
bnf: <digit>       @token {digit} c, ;bnf
bnf: <number>      <digit> <number>    |    <digit> ;bnf

0 value (expression)
bnf: <element>     '(' (expression) execute  ')'
                |   <number> bl c, ;bnf
bnf: <primary>      '-'  <primary>  ," minus "
                |    <element> ;bnf
bnf: <factor>      <primary> '^' <factor>      ," power "
                |  <primary> ;bnf
bnf: <t'>      '*' <factor>     ," * "    <t'>
            |  '/' <factor>     ," / "    <t'>
            |  ;bnf
bnf: <term>     <factor> <t'>       ;bnf
bnf: <e'>      '+' <term>    ," + "   <e'>
            |  '-' <term>    ," - "   <e'>
            |  ;bnf
bnf: <expression>       <term> <e'> ;bnf
' <expression> to (expression)
: parse3    
   0bnf  here <expression> <eol>  /bnf
   cr success if here over - dup negate allot type  else ." failed" then ;
[then]