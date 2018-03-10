/*
    Esteban Gil Martinez        A01375048
    Javier Esponda Hernández    A01374645
    Joel Lara Quintana          A01374649

  Copyright (C) 2018, Error 404 NullPointeException, ITESM CEM
    

  DeepLingo compiler - This class performs the lexical analysis, 
  (a.k.a. scanning).
  
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DeepLingo {

    class Scanner {

        readonly string input;
        
        static readonly Regex regex = new Regex(
            @"                             
                  (?<AND>               [&]{2}                                                       )
                | (?<CHAR>              '([^\n\\']|\\n|\\r|\\t|\\'|\\\\|\\""|\\u[0-9a-fA-F]{6})'     )
                | (?<MULTICOMMENT>      (\/[*][^*]*[*]+([^*/][^*]*[*]+)*\/)|(\/\/.*)                 )
                | (?<COMMENT>           ([/]{2}.*)                                                   )
                | (?<DECREMENT>         [-]{2}                                                       )
                | (?<EQUALS>            [=]{2}                                                       )
                | (?<GREATEREQUAL>      [>][=]                                                       )
                | (?<INCREMENT>         [+]{2}                                                       )
                | (?<LESSEQUAL>         [<][=]                                                       )
                | (?<NOTEQUALS>         [!][=]                                                       )
                | (?<OR>                [|]{2}                                                       )
                | (?<STRING>            ""([^\n\\""]|\\n|\\r|\\t|\\'|\\\\|\\""|\\u[0-9a-fA-F]{6})*"" )
                | (?<ASSIGN>            [=]                                                          )
                | (?<CLOSEDCURLY>       [}]                                                          )
                | (?<CLOSEDPAR>         [)]                                                          )
                | (?<CLOSEDBRACKET>     []]                                                          )
                | (?<COMMA>             [,]                                                          )
                | (?<DIVIDE>            [/]                                                          )
                | (?<GREATER>           [>]                                                          )
                | (?<LESS>              [<]                                                          )
                | (?<MINUS>             [-]                                                          )
                | (?<MODULO>            [%]                                                          )
                | (?<MULTIPLICATION>    [*]                                                          )
                | (?<NOT>               [!]                                                          )
                | (?<PLUS>              [+]                                                          )
                | (?<OPENEDPAR>         [(]                                                          )
                | (?<OPENEDCURLY>       [{]                                                          )
                | (?<OPENEDBRACKET>     [[]                                                          )
                | (?<SEMICOLON>         [;]                                                          )
                | (?<IDENTIFIER>        [A-Za-z][0-9A-Za-z_]*                                        )
                | (?<INTLITERAL>        [-]?\d+                                                      )
                | (?<NEWLINE>           \n                                                           )
                | (?<CARRIAGERETURN>    \r                                                           )
                | (?<TAB>               \t                                                           )
                | (?<WHITESPACE>        \s                                                           ) # Must go anywhere after Newline.
                | (?<OTHER>             .                                                            ) # Must be last: match any other character.", 
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Multiline);
            

        static readonly IDictionary<string, TokenCategory> keywords =
            new Dictionary<string, TokenCategory>() {
                {"break",   TokenCategory.BREAK     },
                {"else",    TokenCategory.ELSE      },
                {"elseif",  TokenCategory.ELSEIF    },
                {"if",      TokenCategory.IF        },
                {"loop",    TokenCategory.LOOP      },
                {"return",  TokenCategory.RETURN    },
                {"var",     TokenCategory.VAR       }
            };

        static readonly IDictionary<string, TokenCategory> nonKeywords =
            new Dictionary<string, TokenCategory>() {
                {"AND",             TokenCategory.AND           },
                {"ASSIGN",          TokenCategory.ASSIGN        },
                {"CLOSEDCURLY",     TokenCategory.CLOSEDCURLY   },
                {"CLOSEDPAR",       TokenCategory.CLOSEDPAR     },
                {"CLOSEDBRACKET",   TokenCategory.CLOSEDBRACKET },
                {"CHAR",            TokenCategory.CHAR          },
                {"COMMA",           TokenCategory.COMMA         },
                {"DECREMENT",       TokenCategory.DECREMENT     },
                {"DIVIDE",          TokenCategory.DIVIDE        },
                {"EQUALS",          TokenCategory.EQUALS        },
                {"GREATER",         TokenCategory.GREATER       },
                {"GREATEREQUAL",    TokenCategory.GREATEREQUAL  },
                {"INCREMENT",       TokenCategory.INCREMENT     },
                {"INTLITERAL",      TokenCategory.INTLITERAL    },
                {"LESS",            TokenCategory.LESS          },
                {"LESSEQUAL",       TokenCategory.LESSEQUAL     },
                {"MINUS",           TokenCategory.MINUS         },
                {"MODULO",          TokenCategory.MODULO        },
                {"MULTIPLICATION",  TokenCategory.MULTIPLICATION},
                {"NEGATION",        TokenCategory.NEGATION      },
                {"NOT",             TokenCategory.NOT           },
                {"NOTEQUALS",       TokenCategory.NOTEQUALS     },
                {"OPENEDPAR",       TokenCategory.OPENEDPAR     },
                {"OPENEDCURLY",     TokenCategory.OPENEDCURLY   },
                {"OPENEDBRACKET",   TokenCategory.OPENEDBRACKET },
                {"OR",              TokenCategory.OR            },
                {"PLUS",            TokenCategory.PLUS          },
                {"SAME",            TokenCategory.SAME          },
                {"SEMICOLON",       TokenCategory.SEMICOLON     },
                {"STRING",          TokenCategory.STRING        },
            };

        public Scanner(string input) {
            this.input = input;
        }

        public IEnumerable<Token> Start() {
            var row = 1;
            var columnStart = 0;

            Func<Match, TokenCategory, Token> newTok = (m, tc) =>
                new Token(m.Value, tc, row, m.Index - columnStart + 1);

            foreach (Match m in regex.Matches(input)) {

                if (m.Groups["NEWLINE"].Success) { // Found a new line.                  
                    row++;
                    columnStart = m.Index + m.Length;
                } 
                else if (m.Groups["WHITESPACE"].Success) {
                    // Skip white space and comments.
                } 
                else if(m.Groups["COMMENT"].Success){
                    row++;
                }
                else if(m.Groups["MULTICOMMENT"].Success){
                    row += m.Value.Split('\n').Length - 1;
                }
                else if (m.Groups["IDENTIFIER"].Success) {
                    if (keywords.ContainsKey(m.Value)) { // Matched string is a Buttercup keyword.
                        yield return newTok(m, keywords[m.Value]);                                               
                    } 
                    else{ // Otherwise it's just a plain identifier.
                        yield return newTok(m, TokenCategory.IDENTIFIER);
                    }

                } else if (m.Groups["OTHER"].Success) { // Found an illegal character.
                    yield return newTok(m, TokenCategory.ILLEGAL_CHAR);
                } 
                else { // Match must be one of the non keywords.
                    foreach (var name in nonKeywords.Keys) {
                        if (m.Groups[name].Success) {
                            yield return newTok(m, nonKeywords[name]);
                            break;
                        }
                    }
                }
            }

            yield return new Token(null, TokenCategory.EOF, row, input.Length - columnStart + 1);
        }
    }
}
