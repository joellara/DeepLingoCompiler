/*
  DeepLingo compiler - This class performs the syntactic analysis,
  (a.k.a. parsing).
  
    Esteban Gil Martinez        A01375048
    Javier Esponda Hernández    A01374645
    Joel Lara Quintana          A01374649

  Copyright (C) 2018, Error 404 NullPointeException, ITESM CEM
  
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
using System.Text.RegularExpressions;
namespace DeepLingo {
 
     class Parser{
        static readonly ISet<TokenCategory> firstOfStmt = new HashSet<TokenCategory>(){
            TokenCategory.IDENTIFIER,
            TokenCategory.IF,
            TokenCategory.LOOP,
            TokenCategory.BREAK,
            TokenCategory.RETURN,
            TokenCategory.SEMICOLON
        };
        static readonly ISet<TokenCategory> firstOfExprRel = new HashSet<TokenCategory>(){
            TokenCategory.LESS,
            TokenCategory.LESSEQUAL,
            TokenCategory.GREATER,
            TokenCategory.GREATEREQUAL
        };
        static readonly ISet<TokenCategory> firstOfExprMul = new HashSet<TokenCategory>(){
            TokenCategory.MULTIPLICATION,
            TokenCategory.MODULO,
            TokenCategory.DIVIDE
        };
        static readonly ISet<TokenCategory> firstOfExprUnary = new HashSet<TokenCategory>(){
            TokenCategory.PLUS,
            TokenCategory.MINUS,
            TokenCategory.NOT
        };
        static readonly ISet<TokenCategory> firstOfExprPrimary = new HashSet<TokenCategory>(){
            TokenCategory.IDENTIFIER,
            TokenCategory.OPENEDBRACKET,
            TokenCategory.STRING,
            TokenCategory.CHAR,
            TokenCategory.INTLITERAL,
            TokenCategory.OPENEDPAR
        };
        /*
        static readonly ISet<TokenCategory> firstOfExpr = new HashSet<TokenCategory>(){ 
            TokenCategory.IDENTIFIER,
            TokenCategory.OPENEDBRACKET,
            TokenCategory.STRING,
            TokenCategory.CHAR,
            TokenCategory.INTLITERAL,
            TokenCategory.OPENEDPAR,
            TokenCategory.PLUS,
            TokenCategory.MINUS,
            TokenCategory.NOT,
            TokenCategory.MULTIPLICATION,
            TokenCategory.MODULO,
            TokenCategory.DIVIDE,
            TokenCategory.LESS,
            TokenCategory.LESSEQUAL,
            TokenCategory.GREATER,
            TokenCategory.GREATEREQUAL,
            TokenCategory.NOTEQUALS,
            TokenCategory.EQUALS
        };
        */
        IEnumerator<Token> tokenStream;
        public Parser(IEnumerator<Token> tokenStream) {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }
        public TokenCategory CurrentToken {
            get { return tokenStream.Current.Category; }
        }
        public Token Expect(TokenCategory category) {
            if (CurrentToken == category) {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            } 
            else {
                throw new SyntaxError(category, tokenStream.Current);                
            }
        }

        public Node Program(){ 
            // <def>*
            var n = new Program();
            while(CurrentToken != TokenCategory.EOF){
                if(CurrentToken == TokenCategory.VAR){
                    n.Add(VarDef());
                }
                else{
                    n.Add(FunDef());
                }
            }
            Expect(TokenCategory.EOF);
            return n;
        }
        
        
        public Node VarDef(){
            var n = new VarDef(){
                AnchorToken = Expect(TokenCategory.VAR)
            };
            n.Add(IdList());
            Expect(TokenCategory.SEMICOLON);
            return n;
        }
        
        public Node IdList(){
            //‹id› (,‹id›)*
            var n = new IdList();
            if(CurrentToken == TokenCategory.CLOSEDPAR){
                return n;
            }
            n.Add(new Identifier(){
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            });
            while(CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                n.Add(new Identifier(){
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
                });
            }
            return n;
        }
        
        public Node FunDef(){
            //‹id›(‹id-list›?){‹var-def›* ‹stmt-list›*}
            var n = new FunDef(){
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            };
            Expect(TokenCategory.OPENEDPAR);
            //if(CurrentToken != TokenCategory.CLOSEDPAR){
            n.Add(IdList());   
            //}
            Expect(TokenCategory.CLOSEDPAR);
            Expect(TokenCategory.OPENEDCURLY);
            
            //while(CurrentToken == TokenCategory.VAR){
                //n.Add(VarDef());
            //}
            n.Add(VarDefList());
            //if(firstOfStmt.Contains(CurrentToken)){
                n.Add(StmtList());
            //}
            Expect(TokenCategory.CLOSEDCURLY);
            return n;
        }
        
        public Node VarDefList(){
            var n = new VarDefList();
            while(CurrentToken == TokenCategory.VAR){
                n.Add(VarDef());
            }
            return n;
        }
        
        public Node ElseIf(){
            //elseif(‹expr›){‹stmt-list›} *
            var n = new ElseIfList();
            while (CurrentToken == TokenCategory.ELSEIF){
                var n1 = new ElseIf();
                Expect(TokenCategory.ELSEIF);
                Expect(TokenCategory.OPENEDPAR);
                n1.Add(Expr());
                Expect(TokenCategory.CLOSEDPAR);
                Expect(TokenCategory.OPENEDCURLY);
                n1.Add(StmtList());
                Expect(TokenCategory.CLOSEDCURLY);
                n.Add(n1);
            }
            return n;
        }
        public Node Else(){
            var n = new Else();
            if (CurrentToken == TokenCategory.ELSE){
                Expect(TokenCategory.ELSE);
                Expect(TokenCategory.OPENEDCURLY);
                if(CurrentToken != TokenCategory.CLOSEDCURLY){
                    n.Add(StmtList());    
                }
                Expect(TokenCategory.CLOSEDCURLY);
            }
            return n;
        }
        public Node StmtList(){
            //<stmt>*
            var n = new StmtList();
            if(CurrentToken == TokenCategory.CLOSEDCURLY){
                return n;
            }
            while (firstOfStmt.Contains(CurrentToken)){
                n.Add(Stmt());
            }
            return n;
        }
        public Node If(){
            var n = new If();
            Expect(TokenCategory.IF);
            Expect(TokenCategory.OPENEDPAR);
            n.Add(Expr());
            Expect(TokenCategory.CLOSEDPAR);
            Expect(TokenCategory.OPENEDCURLY);
            //if(CurrentToken != TokenCategory.CLOSEDCURLY){
                n.Add(StmtList());
            //}
            Expect(TokenCategory.CLOSEDCURLY);
            //if(CurrentToken == TokenCategory.ELSEIF){
                n.Add(ElseIf());    
            //}
            //if(CurrentToken == TokenCategory.ELSE){
                n.Add(Else());
            //}
            return n;
        }
        public Node Loop(){
            var n  = new Loop(){
                AnchorToken = Expect(TokenCategory.LOOP)
            };
            Expect(TokenCategory.OPENEDCURLY);
            if(CurrentToken != TokenCategory.CLOSEDCURLY){
                n.Add(StmtList());   
            }
            Expect(TokenCategory.CLOSEDCURLY);
            return n;
        }
        
        public Node Return(){
            var n = new Return() {
                AnchorToken = Expect(TokenCategory.RETURN)
            };
            n.Add(Expr());
            Expect(TokenCategory.SEMICOLON);
            return n;
        }
        
        public Node Stmt(){
            switch (CurrentToken){
                case TokenCategory.IDENTIFIER:
                    var idToken = Expect(TokenCategory.IDENTIFIER);
                    switch (CurrentToken){
                        case TokenCategory.ASSIGN:
                            Expect(TokenCategory.ASSIGN);
                            var ass = new Assignment(){
                                AnchorToken = idToken
                            };
                            ass.Add(Expr());
                            Expect(TokenCategory.SEMICOLON);
                            return ass;

                        case TokenCategory.INCREMENT:
                            Expect(TokenCategory.INCREMENT);
                            //Expect(TokenCategory.INCREMENT);
                            var inc = new Increment(){
                                AnchorToken = idToken
                            };
                            Expect(TokenCategory.SEMICOLON);
                            return inc;

                        case TokenCategory.DECREMENT:
                            Expect(TokenCategory.DECREMENT);
                            var dec = new Decrement(){
                                AnchorToken = idToken
                            };
                            Expect(TokenCategory.SEMICOLON);
                            return dec;
                            
                        case TokenCategory.OPENEDPAR:
                            Expect(TokenCategory.OPENEDPAR);
                            var fun = new FunCall(){
                                AnchorToken = idToken
                            };
                            //if(CurrentToken != TokenCategory.CLOSEDPAR){
                            fun.Add(ExprList());   
                            //}
                            Expect(TokenCategory.CLOSEDPAR);
                            Expect(TokenCategory.SEMICOLON);
                            return fun;
                    }
                    break;
                case TokenCategory.IF:
                    return If();

                case TokenCategory.LOOP:
                    return Loop();

                case TokenCategory.BREAK:
                    var bre = new Stmt(){
                        AnchorToken = Expect(TokenCategory.BREAK)
                    };
                    Expect(TokenCategory.SEMICOLON);
                    return bre;

                case TokenCategory.RETURN:
                    return Return();

                case TokenCategory.SEMICOLON:
                    return new Stmt(){
                        AnchorToken = Expect(TokenCategory.SEMICOLON)
                    };

                default:
                    throw new SyntaxError(firstOfStmt,tokenStream.Current);
            }
            throw new SyntaxError(firstOfStmt,tokenStream.Current);
        }
        
        public Node ExprList(){
            var n = new ExprList();
            if(CurrentToken == TokenCategory.CLOSEDPAR || CurrentToken == TokenCategory.CLOSEDBRACKET){
                return n;
            }
            n.Add(Expr());
            while (CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                n.Add(Expr());
            }
            return n;
        }
        
        public Node Expr(){
            //ExprOr  --> ‹expr-and› < || ‹expr-and› >*
            var n = ExprAnd();
            while(CurrentToken == TokenCategory.OR){
                var n1 = new ExprOr(){
                    AnchorToken = Expect(TokenCategory.OR)
                };
                n1.Add(n);
                n1.Add(ExprAnd());
                n = n1;
            }
            return n;
        }
        
        public Node ExprAnd(){
            //‹expr-comp› (&& ‹expr-comp›)*
            var n1 = ExprComp();
            while(CurrentToken == TokenCategory.AND){
                var n2 = new ExprAnd(){
                    AnchorToken = Expect(TokenCategory.AND)
                };
                n2.Add(n1);
                n2.Add(ExprComp());
                n1 = n2;
            }
            return n1;
        }
        
        public Node ExprComp(){
            //‹expr-rel›  (‹op-comp› ‹expr-rel›)*
            var n1 = ExprRel();
            while(CurrentToken == TokenCategory.EQUALS || CurrentToken == TokenCategory.NOTEQUALS){
                var n2 = new ExprComp();
                if(CurrentToken == TokenCategory.EQUALS){
                    n2.AnchorToken = Expect(TokenCategory.EQUALS);
                }else{
                    n2.AnchorToken = Expect(TokenCategory.NOTEQUALS);
                }
                n2.Add(n1);
                n2.Add(ExprRel());
                n1 = n2;
            }
            return n1;
        }
        
        public Node ExprRel(){
            //‹expr-add› (‹op-rel› ‹expr-add›)*
            var n1 = ExprAdd();
            while (firstOfExprRel.Contains(CurrentToken)){
                var n2 = new ExprRel();
                switch (CurrentToken){
                    case TokenCategory.LESS:
                        n2.AnchorToken = Expect(TokenCategory.LESS);
                        break;

                    case TokenCategory.LESSEQUAL:
                        n2.AnchorToken = Expect(TokenCategory.LESSEQUAL);
                        break;

                    case TokenCategory.GREATER:
                        n2.AnchorToken = Expect(TokenCategory.GREATER);
                        break;

                    case TokenCategory.GREATEREQUAL:
                        n2.AnchorToken = Expect(TokenCategory.GREATEREQUAL);
                        break;

                    default:
                        throw new SyntaxError(firstOfExprRel,tokenStream.Current);
                }
                n2.Add(n1);
                n2.Add(ExprAdd());
                n1 = n2;
            }
            return n1;
        }
        
        public Node ExprAdd(){
            //‹expr-mul› < ‹op-add› ‹expr-mul› >*
            var n1 = ExprMul();
            while (CurrentToken == TokenCategory.PLUS || CurrentToken == TokenCategory.MINUS){
                var n2 = new ExprAdd();
                if(CurrentToken == TokenCategory.MINUS){
                    n2.AnchorToken = Expect(TokenCategory.MINUS);    
                }else{
                    n2.AnchorToken = Expect(TokenCategory.PLUS);
                }
                n2.Add(n1);
                n2.Add(ExprMul());
                n1 = n2;
            }
            return n1;
        }
        
        public Node ExprMul(){
            var n1 = ExprUnary();
            while (firstOfExprMul.Contains(CurrentToken)){
                var n2 = new ExprMul();
                switch (CurrentToken){
                    case TokenCategory.MULTIPLICATION:
                        n2.AnchorToken = Expect(TokenCategory.MULTIPLICATION);
                        break;

                    case TokenCategory.MODULO:
                        n2.AnchorToken = Expect(TokenCategory.MODULO);
                        break;

                    case TokenCategory.DIVIDE:
                        n2.AnchorToken = Expect(TokenCategory.DIVIDE);
                        break;

                    default:
                        throw new SyntaxError(firstOfExprMul,tokenStream.Current);
                }
                n2.Add(n1);
                n2.Add(ExprUnary());
                n1 = n2;
            }
            return n1;
        }
        
        public Node ExprUnary(){
            //< ‹op-unary› >* ‹expr-primary›
            var top = new ExprUnary();
            var temp = top; //reference _magic_ 
             if (firstOfExprUnary.Contains(CurrentToken)){
                while (firstOfExprUnary.Contains(CurrentToken)){
                    switch (CurrentToken){
                        case TokenCategory.PLUS:
                            temp.AnchorToken = Expect(TokenCategory.PLUS);
                            break;

                        case TokenCategory.MINUS:
                            temp.AnchorToken = Expect(TokenCategory.MINUS);
                            break;

                        case TokenCategory.NOT:
                            temp.AnchorToken = Expect(TokenCategory.NOT);
                            break;

                        default:
                            throw new SyntaxError(firstOfExprUnary,tokenStream.Current);
                    }
                    if (!firstOfExprUnary.Contains(CurrentToken)){
                        temp.Add(ExprPrimary());
                    }else{
                        var newNode = new ExprUnary(); //ToBeReplacedPrimary or next Unary
                        temp.Add(newNode);
                        temp = newNode;
                    }
                    
                }
                
            }
            else{
                return ExprPrimary(); //replace the invisible current node for the real Primary
            }
            return top; //Always return the head
        }
        
        public Node ExprPrimary(){
            //‹id› | ‹fun-call› | ‹array› | ‹lit› | (‹expr›)
            switch (CurrentToken){
                case TokenCategory.IDENTIFIER: //var or fun-call
                    var idToken = Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.OPENEDPAR){
                        Expect(TokenCategory.OPENEDPAR);
                        var fun = new FunCall(){
                            AnchorToken = idToken
                        };
                        fun.Add(ExprList());
                        Expect(TokenCategory.CLOSEDPAR);
                        return fun;
                    }
                    else{
                        var id =  new Identifier(){
                            AnchorToken = idToken
                        };
                        return id;
                    }
                case TokenCategory.OPENEDBRACKET: //array
                    return Arr();

                case TokenCategory.OPENEDPAR: //(expr)
                    Expect(TokenCategory.OPENEDPAR);
                    var n = Expr();
                    Expect(TokenCategory.CLOSEDPAR);
                    return n;
                    
                case TokenCategory.STRING:
                    return new Str(){
                        AnchorToken = Expect(TokenCategory.STRING)
                    };
    
                case TokenCategory.CHAR:
                    return new Char(){
                        AnchorToken = Expect(TokenCategory.CHAR)
                    };
    
                case TokenCategory.INTLITERAL:
                    return new IntLiteral(){
                        AnchorToken = Expect(TokenCategory.INTLITERAL)
                    };
                    
                default:
                    throw new SyntaxError(firstOfExprPrimary,tokenStream.Current);
            }
        }
        
        public Node Arr(){
            var n = new Arr();
            Expect(TokenCategory.OPENEDBRACKET);
            n.Add(ExprList());
            Expect(TokenCategory.CLOSEDBRACKET);
            return n;
        }
    
    }
}