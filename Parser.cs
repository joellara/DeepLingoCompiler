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
        
        public void FunCall(){
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.OPENEDPAR);
            ExprList();
            Expect(TokenCategory.CLOSEDPAR);
        }
        
        public void Program(){ 
            // <def>*
            while(CurrentToken != TokenCategory.EOF){
                Def();
            }
            Expect(TokenCategory.EOF);
        }
        
        public void Def(){
            //var ‹id-list› ; | ‹fun-def›
            if(CurrentToken == TokenCategory.VAR){
                VarDef();
            }else{
                FunDef(); 
            }
        }
        
        public void VarDef(){
            Expect(TokenCategory.VAR);
            IdList();
            Expect(TokenCategory.SEMICOLON);
        }
        
        public void IdList(){
            //‹id› (,‹id›)*
            Expect(TokenCategory.IDENTIFIER);
            while(CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                Expect(TokenCategory.IDENTIFIER);
            }
        }
        
        public void FunDef(){
            //‹id›(‹id-list›?){‹var-def›* ‹stmt-list›*}
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.OPENEDPAR);
            if (CurrentToken != TokenCategory.CLOSEDPAR){
                IdList();
            }
            Expect(TokenCategory.CLOSEDPAR);
            Expect(TokenCategory.OPENEDCURLY);
            while(CurrentToken == TokenCategory.VAR){
                VarDef();
            }
            StmtList();
            Expect(TokenCategory.CLOSEDCURLY);
        }
        
        public void ElseIfList(){
            //elseif(‹expr›){‹stmt-list›} *
            while (CurrentToken == TokenCategory.ELSEIF){
                Expect(TokenCategory.ELSEIF);
                Expect(TokenCategory.OPENEDPAR);
                Expr();
                Expect(TokenCategory.CLOSEDPAR);
                Expect(TokenCategory.OPENEDCURLY);
                StmtList();
                Expect(TokenCategory.CLOSEDCURLY);
            }
        }
        
        public void Else(){
            //var n = new Else();
            if (CurrentToken == TokenCategory.ELSE){
                Expect(TokenCategory.ELSE);
                Expect(TokenCategory.OPENEDCURLY);
                StmtList();
                Expect(TokenCategory.CLOSEDCURLY);
            }
        }
        
        public void Stmt(){
            switch (CurrentToken){
                case TokenCategory.IDENTIFIER:
                    Expect(TokenCategory.IDENTIFIER);
                    switch (CurrentToken){
                        case TokenCategory.ASSIGN:
                            Expect(TokenCategory.ASSIGN);
                            Expr();
                            Expect(TokenCategory.SEMICOLON);
                            break;
                        case TokenCategory.INCREMENT:
                            Expect(TokenCategory.INCREMENT);
                            Expect(TokenCategory.SEMICOLON);
                            break;
                        case TokenCategory.DECREMENT:
                            Expect(TokenCategory.DECREMENT);
                            Expect(TokenCategory.SEMICOLON);
                            break;
                        case TokenCategory.OPENEDPAR:
                            Expect(TokenCategory.OPENEDPAR);
                            ExprList();
                            Expect(TokenCategory.CLOSEDPAR);
                            Expect(TokenCategory.SEMICOLON);
                            break;
                    }
                    break;
                case TokenCategory.IF:
                    Expect(TokenCategory.IF);
                    Expect(TokenCategory.OPENEDPAR);
                    Expr();
                    Expect(TokenCategory.CLOSEDPAR);
                    Expect(TokenCategory.OPENEDCURLY);
                    StmtList();
                    Expect(TokenCategory.CLOSEDCURLY);
                    ElseIfList();
                    Else();
                    break;
                case TokenCategory.LOOP:
                    Expect(TokenCategory.LOOP);
                    Expect(TokenCategory.OPENEDCURLY);
                    StmtList();
                    Expect(TokenCategory.CLOSEDCURLY);
                    break;
                case TokenCategory.BREAK:
                    Expect(TokenCategory.BREAK);
                    Expect(TokenCategory.SEMICOLON);
                    break;
                case TokenCategory.RETURN:
                    Expect(TokenCategory.RETURN);
                    Expr();
                    Expect(TokenCategory.SEMICOLON);
                    break;
                case TokenCategory.SEMICOLON:
                    Expect(TokenCategory.SEMICOLON);
                    break;
                default:
                    throw new SyntaxError(firstOfStmt,tokenStream.Current);
            }
        }
        
        public void Expr(){
            //ExprOr  --> ‹expr-and› < || ‹expr-and› >*
            ExprAnd();
            while(CurrentToken == TokenCategory.OR){
                Expect(TokenCategory.OR);
                ExprAnd();
            }
        }
        
        public void ExprAnd(){
            ExprComp();
            while(CurrentToken == TokenCategory.AND){
                Expect(TokenCategory.AND);
                ExprComp();
            }
        }
        
        public void ExprComp(){
            //‹expr-rel›  <‹op-comp› ‹expr-rel›>*
            ExprRel();
            while(CurrentToken == TokenCategory.EQUALS || CurrentToken == TokenCategory.NOTEQUALS){
                if(CurrentToken == TokenCategory.EQUALS){
                    Expect(TokenCategory.EQUALS);
                }else{
                    Expect(TokenCategory.NOTEQUALS);
                }
                ExprRel();
            }
        }
        
        public void ExprRel(){
            //‹expr-add› <‹op-rel› ‹expr-add›>*
            ExprAdd();
            while (firstOfExprRel.Contains(CurrentToken)){
                switch (CurrentToken){
                    case TokenCategory.LESS:
                        Expect(TokenCategory.LESS);
                        break;

                    case TokenCategory.LESSEQUAL:
                        Expect(TokenCategory.LESSEQUAL);
                        break;

                    case TokenCategory.GREATER:
                        Expect(TokenCategory.GREATER);
                        break;

                    case TokenCategory.GREATEREQUAL:
                        Expect(TokenCategory.GREATEREQUAL);
                        break;

                    default:
                        throw new SyntaxError(firstOfExprRel,tokenStream.Current);
                }
                ExprAdd();
            }
        }
        
        public void ExprAdd(){
            //‹expr-mul› < ‹op-add› ‹expr-mul› >*
            ExprMul();
            while (CurrentToken == TokenCategory.PLUS || CurrentToken == TokenCategory.MINUS){
                if(CurrentToken == TokenCategory.MINUS){
                    Expect(TokenCategory.MINUS);    
                }
                else{
                    Expect(TokenCategory.PLUS);
                }
                ExprMul();
            }
        }
        
        public void ExprMul(){
            ExprUnary();
            while (firstOfExprMul.Contains(CurrentToken)){
                switch (CurrentToken){
                    case TokenCategory.MULTIPLICATION:
                        Expect(TokenCategory.MULTIPLICATION);
                        break;

                    case TokenCategory.MODULO:
                        Expect(TokenCategory.MODULO);
                        break;

                    case TokenCategory.DIVIDE:
                        Expect(TokenCategory.DIVIDE);
                        break;

                    default:
                        throw new SyntaxError(firstOfExprMul,tokenStream.Current);
                }
                ExprUnary();
            }
        }
        
        public void ExprUnary(){
            //< ‹op-unary› >* ‹expr-primary›
             if (firstOfExprUnary.Contains(CurrentToken)){
                while (firstOfExprUnary.Contains(CurrentToken)){
                    switch (CurrentToken){
                        case TokenCategory.PLUS:
                            Expect(TokenCategory.PLUS);
                            break;

                        case TokenCategory.MINUS:
                            Expect(TokenCategory.MINUS);
                            break;

                        case TokenCategory.NOT:
                            Expect(TokenCategory.NOT);
                            break;

                        default:
                            throw new SyntaxError(firstOfExprUnary,tokenStream.Current);
                    }
                    ExprUnary();
                }
            }
            else{
                ExprPrimary();
            }
        }
        
        public void ExprPrimary(){
            //‹id› | ‹fun-call› | ‹array› | ‹lit› | (‹expr›)
            switch (CurrentToken){
                case TokenCategory.IDENTIFIER:

                    Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.OPENEDPAR){
                        Expect(TokenCategory.OPENEDPAR);
                        ExprList();
                        Expect(TokenCategory.CLOSEDPAR);
                    }
                    break;
                case TokenCategory.OPENEDBRACKET:
                    Expect(TokenCategory.OPENEDBRACKET);
                    ExprList();
                    Expect(TokenCategory.CLOSEDBRACKET);
                    break;

                case TokenCategory.OPENEDPAR:
                    Expect(TokenCategory.OPENEDPAR);
                    Expr();
                    Expect(TokenCategory.CLOSEDPAR);
                    break;
                case TokenCategory.STRING:
                    Expect(TokenCategory.STRING);
                    break;
                case TokenCategory.CHAR:
                    Expect(TokenCategory.CHAR);
                    break;
                case TokenCategory.INTLITERAL:
                    Expect(TokenCategory.INTLITERAL);
                    break;
                default:
                    throw new SyntaxError(firstOfExprPrimary,tokenStream.Current);
            }
        }
        
        public void ExprList(){
             if (firstOfExpr.Contains(CurrentToken)){
                Expr();
                while (CurrentToken == TokenCategory.COMMA){
                    Expect(TokenCategory.COMMA);
                    Expr();
                }
            }
        }
        
        public void StmtList(){
            //‹stmt›*
            while (firstOfStmt.Contains(CurrentToken)){
                Stmt();
            }
        }
        
    }
}