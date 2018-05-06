DeepLingo compiler, version 0.3

Esteban Gil Martinez        A01375048  
Javier Esponda Hernández    A01374645  
Joel Lara Quintana          A01374649  

Copyright (C) 2018, Error 404 NullPointeException, ITESM CEM
===============================

This program is free software. You may redistribute it under the terms of
the GNU General Public License version 3 or later. See license.txt for 
details.    

Included in this release:

    * Lexical analysis
    * Syntactic analysis
    * AST construction
    * Semantic analysis
    * Intermidiate code generation
    
## Building Deep Lingo
### To build the program, at the terminal type:  
    
    make

### To build the library

    make deeplingolib.dll
    
### To build a program

    python deepc test_programs/<file_name>
    
## To run a program, type:

    mono <file_name>.exe
    
Where <file_name> is the name of a DeepLingo source file. You can try with
these files:

   * ultimate
   * literals