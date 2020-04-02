( Core words in z80. This requires a full Forth interpreter
  to run, but is also necessary for core.fs. This means that
  it needs to be compiled from a prior bootstrapped binary.

  This stage is tricky due to the fact that references in
  Forth are all absolute, except for prev word refs. This
  means that there are severe limitations to the kind of code
  you can put here.

  You shouldn't define any word with reference to other words.
  This means no regular definition. You can, however, execute
  any word from our high level Forth, as long as it doesn't
  spit word references.

  These restrictions are temporary, I'll figure something out
  so that we can end up fully bootstrap Forth from within
  itself.

  Oh, also: KEY and EMIT are not defined here. There're
  expected to be defined in platform-specific code.

  This unit expects the same conf as boot.fs.
)

( dummy entry for dict hook )
(entry) _
H@ 256 /MOD 2 PC! 2 PC!

( a b c -- b c a )
CODE ROT
    HL POPqq,       ( C )
    DE POPqq,       ( B )
    BC POPqq,       ( A )
    chkPS,
    DE PUSHqq,      ( B )
    HL PUSHqq,      ( C )
    BC PUSHqq,      ( A )
;CODE

( a -- a a )
CODE DUP
    HL POPqq,       ( A )
    chkPS,
    HL PUSHqq,      ( A )
    HL PUSHqq,      ( A )
;CODE

( a -- )
CODE DROP
    HL POPqq,
;CODE

( a b -- b a )
CODE SWAP
    HL POPqq,       ( B )
    DE POPqq,       ( A )
    chkPS,
    HL PUSHqq,      ( B )
    DE PUSHqq,      ( A )
;CODE

( a b -- a b a )
CODE OVER
    HL POPqq,       ( B )
    DE POPqq,       ( A )
    chkPS,
    DE PUSHqq,      ( A )
    HL PUSHqq,      ( B )
    DE PUSHqq,      ( A )
;CODE

( a b -- a b a b )
CODE 2DUP
    HL POPqq,       ( B )
    DE POPqq,       ( A )
    chkPS,
    DE PUSHqq,      ( A )
    HL PUSHqq,      ( B )
    DE PUSHqq,      ( A )
    HL PUSHqq,      ( B )
;CODE

( a b -- )
CODE 2DROP
    HL POPqq,
    HL POPqq,
;CODE

( a b c d -- a b c d a b )

CODE 2OVER
    HL POPqq,       ( D )
    DE POPqq,       ( C )
    BC POPqq,       ( B )
    IY POPqq,       ( A )
    chkPS,
    IY PUSHqq,      ( A )
    BC PUSHqq,      ( B )
    DE PUSHqq,      ( C )
    HL PUSHqq,      ( D )
    IY PUSHqq,      ( A )
    BC PUSHqq,      ( B )
;CODE

( a b c d -- c d a b )

CODE 2SWAP
    HL POPqq,       ( D )
    DE POPqq,       ( C )
    BC POPqq,       ( B )
    IY POPqq,       ( A )
    chkPS,
    DE PUSHqq,      ( C )
    HL PUSHqq,      ( D )
    IY PUSHqq,      ( A )
    BC PUSHqq,      ( B )
;CODE

CODE AND
    HL POPqq,
    DE POPqq,
    chkPS,
    A E LDrr,
    L ANDr,
    L A LDrr,
    A D LDrr,
    H ANDr,
    H A LDrr,
    HL PUSHqq,
;CODE

CODE OR
    HL POPqq,
    DE POPqq,
    chkPS,
    A E LDrr,
    L ORr,
    L A LDrr,
    A D LDrr,
    H ORr,
    H A LDrr,
    HL PUSHqq,
;CODE

CODE XOR
    HL POPqq,
    DE POPqq,
    chkPS,
    A E LDrr,
    L XORr,
    L A LDrr,
    A D LDrr,
    H XORr,
    H A LDrr,
    HL PUSHqq,
;CODE

CODE NOT
    HL POPqq,
    chkPS,
    A L LDrr,
    H ORr,
    HL 0 LDddnn,
    JRNZ, L1 FWR ( skip )
    ( false, make 1 )
    HL INCss,
L1 FSET ( skip )
    HL PUSHqq,
;CODE

CODE +
    HL POPqq,
    DE POPqq,
    chkPS,
    DE ADDHLss,
    HL PUSHqq,
;CODE

CODE -
    DE POPqq,
    HL POPqq,
    chkPS,
    A ORr,
    DE SBCHLss,
    HL PUSHqq,
;CODE

CODE *
    DE POPqq,
    BC POPqq,
    chkPS,
	( DE * BC -> DE (high) and HL (low) )
    HL 0 LDddnn,
    A 0x10 LDrn,
( loop )
    HL ADDHLss,
    E RLr,
    D RLr,
    JRNC, 4 A, ( noinc )
    BC ADDHLss,
    JRNC, 1 A, ( noinc )
    DE INCss,
( noinc )
    A DECr,
    JRNZ, -14 A, ( loop )
    HL PUSHqq,
;CODE

( Borrowed from http://wikiti.brandonw.net/ )
( Divides AC by DE and places the quotient in AC and the
  remainder in HL )
CODE /MOD
    DE POPqq,
    BC POPqq,
    chkPS,
    A B LDrr,
    B 16 LDrn,
    HL 0 LDddnn,
L1 BSET ( loop )
    SCF,
    C RLr,
    RLA,
    HL ADCHLss,
    DE SBCHLss,
    JRNC, L2 FWR ( skip )
    DE ADDHLss,
    C DECr,
L2 FSET ( skip )
    DJNZ, L1 BWR ( loop )
    B A LDrr,
    HL PUSHqq,
    BC PUSHqq,
;CODE

CODE !
    HL POPqq,
    DE POPqq,
    chkPS,
    (HL) E LDrr,
    HL INCss,
    (HL) D LDrr,
;CODE

CODE @
    HL POPqq,
    chkPS,
    E (HL) LDrr,
    HL INCss,
    D (HL) LDrr,
    EXDEHL,
    HL PUSHqq,
;CODE

CODE C!
    HL POPqq,
    DE POPqq,
    chkPS,
    (HL) E LDrr,
;CODE

CODE C@
    HL POPqq,
    chkPS,
    L (HL) LDrr,
    H 0 LDrn,
    HL PUSHqq,
;CODE

CODE PC!
    BC POPqq,
    HL POPqq,
    chkPS,
    L OUT(C)r,
;CODE

CODE PC@
    BC POPqq,
    chkPS,
    H 0 LDrn,
    L INr(C),
    HL PUSHqq,
;CODE

CODE I
    L 0 IX+ LDrIXY,
    H 1 IX+ LDrIXY,
    HL PUSHqq,
;CODE

CODE I'
    L 2 IX- LDrIXY,
    H 1 IX- LDrIXY,
    HL PUSHqq,
;CODE

CODE J
    L 4 IX- LDrIXY,
    H 3 IX- LDrIXY,
    HL PUSHqq,
;CODE

CODE >R
    HL POPqq,
    chkPS,
    ( 17 == pushRS )
    17 CALLnn,
;CODE

CODE R>
    ( 20 == popRS )
    20 CALLnn,
    HL PUSHqq,
;CODE

CODE IMMEDIATE
    CURRENT LDHL(nn),
    HL DECss,
    7 (HL) SETbr,
;CODE

CODE IMMED?
    HL POPqq,
    chkPS,
    HL DECss,
    DE 0 LDddnn,
    7 (HL) BITbr,
    JRZ, L1 FWR ( notset )
    DE INCss,
L1 FSET ( notset )
    DE PUSHqq,
;CODE

CODE BYE
    HALT,
;CODE

CODE (resSP)
    ( INITIAL_SP == RAM+0 )
    SP RAMSTART LDdd(nn),
;CODE

CODE (resRS)
    IX RS_ADDR LDddnn,
;CODE

CODE SCMP
    DE  POPqq,
    HL  POPqq,
    chkPS,
L1 BSET ( loop )
    LDA(DE),
    (HL) CPr,
    JRNZ, L2 FWR ( not equal? break early to "end".
                   NZ is set. )
    A ORr,   ( if our char is null, stop )
    HL INCss,
    DE INCss,
    JRNZ, L1 BWR ( loop )
L2 FSET ( end )
    ( 40 == flagsToBC )
    40 CALLnn,
    BC PUSHqq,
;CODE

CODE CMP
    HL  POPqq,
    DE  POPqq,
    chkPS,
    A ORr,      ( clear carry )
    DE SBCHLss,
    ( 40 == flagsToBC )
    40 CALLnn,
    BC PUSHqq,
;CODE

( Takes Word and Latest and spits Address and Flag.
  Latest is the tip of the dictionary to seek. )
( w l -- a f )
CODE (find)
    DE POPqq,   ( l )
    HL POPqq,   ( w )
    chkPS,
    ( 3 == find )
    3 CALLnn,
    JRZ, L1 FWR ( found )
    ( not found )
    HL PUSHqq,
    DE 0 LDddnn,
    DE PUSHqq,
    JPNEXT,
L1 FSET ( found )
    DE PUSHqq,
    DE 1 LDddnn,
    DE PUSHqq,
;CODE

CODE SCPY
    HL POPqq,
    chkPS,
    DE HERE LDdd(nn),
    B 0 LDrn,
L1 BSET ( loop )
    A (HL) LDrr,
    LD(DE)A,
    HL INCss,
    DE INCss,
    B INCr,
    A ORr,
    JRNZ, L1 BWR ( loop )
    DE A LD(dd)r
    HERE DE LD(nn)dd,
;CODE
