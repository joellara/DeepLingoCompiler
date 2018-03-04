 /*
    Esteban Gil Martinez        A01375048
    Javier Esponda Hernández    A01374645
    Joel Lara Quintana          A01374649

  Copyright (C) 2018, Error 404 NullPointeException, ITESM CEM
  
  Deep Lingo compiler - Token categories for the scanner.
  
  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.
  
  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  
  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace DeepLingo {

    enum TokenCategory {
        AND,            // &&
        ASSIGN,         // =
        BREAK,          // break
        CLOSEDCURLY,     // }
        CLOSEDPAR,       // )
        CLOSEDBRACKET,    // ]
        CHAR,      //      'AAA'
        COMA,           // ,
        DECREMENT,      // --
        NOTEQUALS,      // !=
        DIVIDE,         // /
        EQUALS,         // ==
        ELSE,           // else
        ELSEIF,         // elseif
        EOF,            // End of Line
        GREATER,        // >
        GREATEREQUAL,   // >=
        IF,             // if
        IDENTIFIER,     // Common Identifier
        INCREMENT,      // ++
        INTLITERAL,     // Int Literal
        LESS,           // <
        LESSEQUAL,      // <=
        LOOP,           // loop
        MINUS,          // -
        MODULO,         // %
        MULTIPLICATION,   // *
        NEGATION,       // -x
        NOT,            // !
        OPENEDPAR,        // (
        OPENEDCURLY,      // {
        OPENEDBRACKET,     // [
        OR,             // ||
        PLUS,           // +
        RETURN,         // return
        SAME,           // +x
        SEMICOLON,      // ;
        STRING,         // Any string
        VAR,            // var
        ILLEGAL_CHAR    // 
    }
}