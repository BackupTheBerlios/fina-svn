\ CHECKANS.STR ANS Forth wordset checker                01may93jaw

\ 1-3MAY93 Jens A. Wilke
\ This program is public domain

DECIMAL

VARIABLE CharCount
30 CONSTANT MaxChars
VARIABLE Flag

CREATE Names 125 CELLS ALLOT
VARIABLE PNT Names PNT !

: INIT -1 Flag ! 0 CharCount ! ;

: ^     PNT @ DUP @ 1+ SWAP !
        BL WORD FIND
        0= IF PNT @ CELL+ DUP @ 1+ SWAP !
              Flag @ IF CR ." Missing: " 0 Flag ! THEN
              COUNT DUP CharCount +! TYPE SPACE
              CharCount @ MaxChars U< 0= IF CR 9 SPACES 0 CharCount ! THEN
           ELSE DROP THEN ;

: PLACE ( adr cnt adr -- ) 2DUP C! 1+ SWAP MOVE ;

: WS    INIT
        PNT @ 2 CELLS + PNT !
        BL WORD
        CR CR ." Checking " DUP COUNT TYPE ."  wordset..."
        DUP COUNT PNT @ PLACE COUNT SWAP DROP 1+
        PNT @ + ALIGNED DUP PNT !
        DUP 0 SWAP ! CELL+ 0 SWAP ! ;



WS CORE

^ ! ^ # ^ #> ^ #S ^ ' ^ ( ^ * ^ */ ^ */MOD ^ + ^ +! ^ +LOOP
^ , ^ - ^ . ^ ." ^ / ^ /MOD ^ 0< ^ 0= ^ 1+ ^ 1- ^ 2! ^ 2* ^ 2/
^ 2@ ^ 2DROP ^ 2DUP ^ 2OVER ^ 2SWAP ^ : ^ ; ^ < ^ <# ^ = ^ >
^ >BODY ^ >IN ^ >NUMBER ^ >R ^ ?DUP ^ @ ^ ABORT ^ ABORT"
^ ABS ^ ACCEPT ^ ALIGN ^ ALIGNED ^ ALLOT ^ AND ^ BASE ^ BEGIN
^ BL ^ C! ^ C, ^ C@ ^ CELL+ ^ CELLS ^ CHAR ^ CHAR+ ^ CHARS
^ CONSTANT ^ COUNT ^ CR ^ CREATE ^ DECIMAL ^ DEPTH ^ DO ^ DOES>
^ DROP ^ DUP ^ ELSE ^ EMIT ^ ENVIRONMENT? ^ EVALUATE ^ EXECUTE
^ EXIT ^ FILL ^ FIND ^ FM/MOD ^ HERE ^ HOLD ^ I ^ IF ^ IMMEDIATE
^ INVERT ^ J ^ KEY ^ LEAVE ^ LITERAL ^ LOOP ^ LSHIFT ^ M* ^ MAX
^ MIN ^ MOD ^ MOVE ^ NEGATE ^ OR ^ OVER ^ POSTPONE ^ QUIT
^ R> ^ R@ ^ RECURSE ^ REPEAT ^ ROT ^ RSHIFT ^ S" ^ S>D
^ SIGN ^ SM/REM ^ SOURCE ^ SPACE ^ SPACES ^ STATE ^ SWAP
^ THEN ^ TYPE ^ U. ^ U< ^ UM* ^ UM/MOD ^ UNLOOP ^ UNTIL ^ VARIABLE
^ WHILE ^ WORD ^ XOR ^ [ ^ ['] ^ [CHAR] ^ ]

WS CORE-EXT

^ #TIB ^ .( ^ .R ^ 0<> ^ 0> ^ 2>R ^ 2R> ^ 2R@ ^ :NONAME ^ <>
^ ?DO ^ AGAIN ^ C" ^ CASE ^ COMPILE, ^ CONVERT ^ ENDCASE
^ ENDOF ^ ERASE ^ EXPECT ^ FALSE ^ HEX ^ MARKER ^ NIP ^ OF
^ PAD ^ PARSE ^ PICK ^ QUERY ^ REFILL ^ RESTORE-INPUT ^ ROLL
^ SAVE-INPUT ^ SOURCE-ID ^ SPAN ^ TIB ^ TO ^ TRUE ^ TUCK ^ U.R
^ U> ^ UNUSED ^ VALUE ^ WITHIN ^ [COMPILE] ^ \

WS BLOCK

^ BLK ^ BLOCK ^ BUFFER ^ EVALUATE ^ FLUSH ^ LOAD ^ SAVE-BUFFERS
^ UPDATE

WS BLOCK-EXT

^ EMPTY-BUFFERS ^ LIST ^ REFILL ^ SCR ^ THRU ^ \

WS DOUBLE

^ 2CONSTANT ^ 2LITERAL ^ 2VARIABLE ^ D+ ^ D- ^ D. ^ D.R ^ D0<
^ D0= ^ D2* ^ D2/ ^ D< ^ D= ^ D>S ^ DABS ^ DMAX ^ DMIN ^ DNEGATE
^ M*/ ^ M+

WS DOUBLE-EXT

^ 2ROT ^ DU<

WS EXCEPTION

^ CATCH ^ THROW

WS EXCEPTION-EXT

^ ABORT ^ ABORT"

WS FACILITY

^ AT-XY ^ KEY? ^ PAGE

WS FACILITY-EXT

^ EKEY ^ EKEY>CHAR ^ EKEY? ^ EMIT? ^ MS ^ TIME&DATE

WS FILE

^ (  ^ BIN ^ CLOSE-FILE ^ CREATE-FILE ^ DELETE-FILE
^ FILE-POSITION ^ FILE-SIZE ^ INCLUDE-FILE ^ INCLUDED
^ OPEN-FILE ^ R/O ^ R/W ^ READ-FILE ^ READ-LINE ^ REPOSITION-FILE
^ RESIZE-FILE ^ S" ^ SOURCE-ID ^ W/O ^ WRITE-FILE ^ WRITE-LINE

WS FILE-EXT

^ FILE-STATUS ^ FLUSH-FILE ^ REFILL ^ RENAME-FILE

WS FLOAT

^ >FLOAT ^ D>F ^ F! ^ F* ^ F+ ^ F- ^ F/ ^ F0< ^ F0= ^ F< ^ F>D
^ F@ ^ FALIGN ^ FALIGNED ^ FCONSTANT ^ FDEPTH ^ FDROP ^ FDUP
^ FLITERAL ^ FLOAT+ ^ FLOATS ^ FLOOR ^ FMAX ^ FMIN ^ FNEGATE
^ FOVER ^ FROT ^ FROUND ^ FSWAP ^ FVARIABLE ^ REPRESENT

WS FLOAT-EXT

^ DF! ^ DF@ ^ DFALIGN ^ DFALIGNED ^ DFLOAT+ ^ DFLOATS ^ F**
^ F. ^ FABS ^ FACOS ^ FACOSH ^ FALOG ^ FASIN ^ FASINH ^ FATAN
^ FATAN2 ^ FATANH ^ FCOS ^ FCOSH ^ FE. ^ FEXP ^ FEXPM1
^ FLN ^ FLNP1 ^ FLOG ^ FS. ^ FSIN ^ FSINCOS ^ FSINH
^ FSQRT ^ FTAN ^ FTANH ^ F~ ^ PRECISION ^ SET-PRECISION
^ SF! ^ SF@ ^ SFALIGN ^ SFALIGNED ^ SFLOAT+ ^ SFLOATS

WS LOCAL

^ (LOCAL) ^ TO

WS LOCAL-EXT

^ LOCALS|

WS MEMORY

^ ALLOCATE ^ FREE ^ RESIZE

WS TOOLKIT

^ .S ^ ? ^ DUMP ^ SEE ^ WORDS

WS TOOLKIT-EXT

^ ;CODE ^ AHEAD ^ ASSEMBLER ^ BYE ^ CODE ^ CS-PICK ^ CS-ROLL
^ EDITOR ^ FORGET ^ STATE ^ [ELSE] ^ [IF] ^ [THEN]

WS SEARCH

^ DEFINITIONS ^ FIND ^ FORTH-WORDLIST ^ GET-CURRENT ^ GET-ORDER
^ SEARCH-WORDLIST ^ SET-CURRENT ^ SET-ORDER ^ WORDLIST

WS SEARCH-EXT

^ ALSO ^ FORTH ^ ONLY ^ ORDER ^ PREVIOUS

WS STRING

^ -TRAILING ^ /STRING ^ BLANK ^ CMOVE ^ CMOVE> ^ COMPARE ^ SEARCH
^ SLITERAL



: END
        CR CR ." Wordset:            Status:  Words:" CR

        Names 2 CELLS +
        BEGIN
                DUP COUNT TYPE
                DUP COUNT SWAP DROP 20 SWAP - SPACES
                COUNT + ALIGNED
                DUP @ OVER CELL+ @
                2DUP 0=
                IF ." complete " . DROP DROP
                ELSE OVER =
                 IF ." missing  " . DROP
                 ELSE ." partial  " OVER SWAP - . ." / " .
                 THEN
                THEN CR
                2 CELLS +
                DUP PNT @ U< 0=
        UNTIL DROP ;

END
