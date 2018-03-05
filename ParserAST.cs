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
            //var n = ExprList();
            ExprList();
            Expect(TokenCategory.CLOSEDPAR);
            //return n;
        }
        
        public void Program(){ 
            // <def>*
            //var n = new Program();
            //var temp = n;
            while(CurrentToken != TokenCategory.EOF){
                //var n1 = Def();
                Def();
                //temp.add(n1);
                //temp = n1;
            }
            Expect(TokenCategory.EOF);
            //return n;
        }
        
        public void Def(){
            //var ‹id-list› ; | ‹fun-def›
            //var n = new Def();
            if(CurrentToken == TokenCategory.VAR){
                //n.Add(VarDef());
                VarDef();
            }
            else{
                //n.Add(FunDef());
                FunDef(); 
            }
            //return n;
        }
        
        public void VarDef(){
            Expect(TokenCategory.VAR);
            //var n = new VarDef(){ //Esto no está bien, o no tengo idea
                IdList();
            //}
            Expect(TokenCategory.SEMICOLON);
            //return n;
        }
        
        public void IdList(){
            //‹id› (,‹id›)*
            //var n = new IdList();
            //n.Add(Expect(TokenCategory.IDENTIFIER));
            Expect(TokenCategory.IDENTIFIER);
            while(CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                //n.Add(Expect(TokenCategory.IDENTIFIER));
                Expect(TokenCategory.IDENTIFIER);
            }
            //return n;
        }
        
        public void FunDef(){
            //‹id›(‹id-list›?){‹var-def›* ‹stmt-list›*}
            //var n = new FunDef(){AnchorToken = Expect(TokenCategory.IDENTIFIER);}
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.OPENEDPAR);
            //if(Current != TokenCategory.CLOSEDPAR){
            //    n.Add(IdList());   
            //}
            if (CurrentToken == TokenCategory.IDENTIFIER){
                IdList();
            }
            Expect(TokenCategory.CLOSEDPAR);
            Expect(TokenCategory.OPENEDCURLY);
            while(CurrentToken == TokenCategory.VAR){
                //n.Add(VarDef());
                VarDef();
            }
            while (firstOfStmt.Contains(CurrentToken)){
                //n.Add(Stmt());
                Stmt();
            }
            Expect(TokenCategory.CLOSEDCURLY);
        }
        
        public void ElseIfList(){
            //elseif(‹expr›){‹stmt-list›} *
            //var n = new ElseIfList();
            while (CurrentToken == TokenCategory.ELSEIF){
                Expect(TokenCategory.ELSEIF);
                Expect(TokenCategory.OPENEDPAR);
                //n.Add(Expr());
                Expr();
                Expect(TokenCategory.CLOSEDPAR);
                Expect(TokenCategory.OPENEDCURLY);
                //n.Add(StmtList());
                while (firstOfStmt.Contains(CurrentToken)){
                    //n.Add(Stmt());
                    Stmt();
                }
                Expect(TokenCategory.CLOSEDCURLY);
            }
            //return n;
        }
        
        public void Else(){
            //var n = new Else();
            if (CurrentToken == TokenCategory.ELSE){
                Expect(TokenCategory.ELSE);
                Expect(TokenCategory.OPENEDCURLY);
                //n.Add(StmtList());
                StmtList();
                Expect(TokenCategory.CLOSEDCURLY);
            }
            //return n;
        }
        
        public void Stmt(){
            switch (CurrentToken){
                case TokenCategory.IDENTIFIER:
                    Expect(TokenCategory.IDENTIFIER);
                    switch (CurrentToken){
                        case TokenCategory.ASSIGN:
                            Expect(TokenCategory.ASSIGN);
                            //var n = Expr();
                            Expr();
                            Expect(TokenCategory.SEMICOLON);
                            //return n;
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
                            //var n = FunCall();
                            ExprList();
                            Expect(TokenCategory.CLOSEDPAR);
                            Expect(TokenCategory.SEMICOLON);
                            break;
                    }
                    break;
                case TokenCategory.IF:
                    Expect(TokenCategory.IF);
                    Expect(TokenCategory.OPENEDPAR);
                    //var n = Expr();
                    Expr();
                    Expect(TokenCategory.CLOSEDPAR);
                    Expect(TokenCategory.OPENEDCURLY);
                    //n.Add(StmtList());
                    StmtList();
                    Expect(TokenCategory.CLOSEDCURLY);
                    
                    //n.Add(ElseIfList());
                    ElseIfList();
                    //n.Add(Else());
                    Else();
                    //return n;
                    break;

                case TokenCategory.LOOP:
                    Expect(TokenCategory.LOOP);
                    Expect(TokenCategory.OPENEDCURLY);
                    //var n = StmtList();
                    StmtList();
                    Expect(TokenCategory.CLOSEDCURLY);
                    //return n;
                    break;

                case TokenCategory.BREAK:
                    Expect(TokenCategory.BREAK);
                    Expect(TokenCategory.SEMICOLON);
                    break;

                case TokenCategory.RETURN:
                    Expect(TokenCategory.RETURN);
                    //var n = Expr();
                    Expr();
                    Expect(TokenCategory.SEMICOLON);
                    //return n;
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
            //var n1 = ExprAnd();
            ExprAnd();
            while(CurrentToken == TokenCategory.OR){
                //n2 = new Expr();
                //n2.AnchorToken = Expect(TokenCategory.OR)
                Expect(TokenCategory.OR);
                //n2.Add(n1);
                //n2.Add(ExprAnd());
                ExprAnd();
                //n1 = n2;
            }
            //return n1;
        }
        
        public void ExprAnd(){
            //var n1 = ExprComp();
            ExprComp();
            while(CurrentToken == TokenCategory.AND){
                //n2 = new ExprAnd();
                //n2.AnchorToken = Expect(TokenCategory.AND)
                Expect(TokenCategory.AND);
                //n2.Add(n1);
                //n2.Add(ExprComp());
                ExprComp();
                //n1 = n2;
            }
            //return n1;
        }
        
        public void ExprComp(){
            //‹expr-rel›  <‹op-comp› ‹expr-rel›>*
            //var n1 = ExprRel();
            ExprRel();
            while(CurrentToken == TokenCategory.EQUALS || CurrentToken == TokenCategory.NOTEQUALS){
                //n2 = new ExprComp();
                if(CurrentToken == TokenCategory.EQUALS){
                    //n2.AnchorToken = Expect(TokenCategory.EQUALS);
                    Expect(TokenCategory.EQUALS);
                }
                else{
                    //n2.AnchorToken = Expect(TokenCategory.LESSEQUAL);
                    Expect(TokenCategory.NOTEQUALS);
                }
                        
                //n2.Add(n1);
                //n2.Add(ExprRel());
                ExprRel();
                //n1 = n2
            }
            //return n1;
        }
        
        public void ExprRel(){
            //‹expr-add› <‹op-rel› ‹expr-add›>*
            //var n1 = ExprAdd();
            ExprAdd();
            while (firstOfExprRel.Contains(CurrentToken)){
                //n2 = new ExprRel();
                switch (CurrentToken){
                    case TokenCategory.LESS:
                        //n2.AnchorToken = Expect(TokenCategory.LESS);
                        Expect(TokenCategory.LESS);
                        break;

                    case TokenCategory.LESSEQUAL:
                        //n2.AnchorToken = Expect(TokenCategory.LESSEQUAL);
                        Expect(TokenCategory.LESSEQUAL);
                        break;

                    case TokenCategory.GREATER:
                        //n2.AnchorToken = Expect(TokenCategory.GREATER);
                        Expect(TokenCategory.GREATER);
                        break;

                    case TokenCategory.GREATEREQUAL:
                        //n2.AnchorToken = Expect(TokenCategory.GREATEREQUAL);
                        Expect(TokenCategory.GREATEREQUAL);
                        break;

                    default:
                        throw new SyntaxError(firstOfExprRel,tokenStream.Current);
                }
                //n2.Add(n1);
                //n2.Add(ExprAdd());
                ExprAdd();
                //n1 = n2
                
            }
            //return n1;
        }
        
        public void ExprAdd(){
            //‹expr-mul› < ‹op-add› ‹expr-mul› >*
            //var n1 = ExprMul();
            ExprMul();
            while (CurrentToken == TokenCategory.PLUS || CurrentToken == TokenCategory.MINUS){
                //n2 = new ExprMul();
                if(CurrentToken == TokenCategory.MINUS){
                    Expect(TokenCategory.MINUS);    
                }
                else{
                    Expect(TokenCategory.PLUS);
                }
                //n2.Add(n1);
                //n2.Add(ExprMul());
                ExprMul();
                //n1 = n2
            }
            //return n1;
        }
        
        public void ExprMul(){
            //var n1 = ExprUnary();
            ExprUnary();
            while (firstOfExprMul.Contains(CurrentToken)){
                //n2 = new ExprMul();
                switch (CurrentToken){
                    case TokenCategory.MULTIPLICATION:
                        //n2.AnchorToken = Expect(TokenCategory.MULTIPLICATION);
                        Expect(TokenCategory.MULTIPLICATION);
                        break;

                    case TokenCategory.MODULO:
                        //n2.AnchorToken = Expect(TokenCategory.MODULO);
                        Expect(TokenCategory.MODULO);
                        break;

                    case TokenCategory.DIVIDE:
                        //n2.AnchorToken = Expect(TokenCategory.DIVIDE);
                        Expect(TokenCategory.DIVIDE);
                        break;

                    default:
                        throw new SyntaxError(firstOfExprMul,tokenStream.Current);
                }
                //n2.Add(n1);
                //n2.Add(ExprUnary());
                ExprUnary();
                //n1 = n2
                
            }
            //return n1;
            
        }
        
        public void ExprUnary(){
            //< ‹op-unary› >* ‹expr-primary›
            //var top = new ExprUnary();
            //var temp = top; //reference _magic_ 
             if (firstOfExprUnary.Contains(CurrentToken)){
                while (firstOfExprUnary.Contains(CurrentToken)){
                    switch (CurrentToken){
                        case TokenCategory.PLUS:
                            //temp.AnchorToken = Expect(TokenCategory.PLUS);
                            Expect(TokenCategory.PLUS);
                            break;

                        case TokenCategory.MINUS:
                            //temp.AnchorToken = Expect(TokenCategory.MINUS);
                            Expect(TokenCategory.MINUS);
                            break;

                        case TokenCategory.NOT:
                            //temp.AnchorToken = Expect(TokenCategory.NOT);
                            Expect(TokenCategory.NOT);
                            break;

                        default:
                            throw new SyntaxError(firstOfExprUnary,tokenStream.Current);
                    }
                    //var newNode = new ExprUnary(); //ToBeReplacedPrimary or next Unary
                    ExprUnary();
                    //temp.Add(newNode);
                    //temp = newNode;
                }
            }
            else{
                //temp = ExprPrimary(); //replace the invisible current node for the real Primary
                ExprPrimary();
            }
            
            //return top; //Always return the head
        }
        
        public void ExprPrimary(){
            //‹id› | ‹fun-call› | ‹array› | ‹lit› | (‹expr›)
            
            switch (CurrentToken){
                case TokenCategory.IDENTIFIER:
                    //var n = new ExprPrimary(){
                    //AnchorToken = Expect(TokenCategory.IDENTIFIER)
                    Expect(TokenCategory.IDENTIFIER);
                    //}
                    if (CurrentToken == TokenCategory.OPENEDPAR){
                        Expect(TokenCategory.OPENEDPAR);
                        //n.Add(FunCall());
                        ExprList();
                        Expect(TokenCategory.CLOSEDPAR);
                    }
                    //return n;
                    break;
                case TokenCategory.OPENEDBRACKET:
                    Expect(TokenCategory.OPENEDBRACKET);
                    //var n = new ExprPrimary(){
                        ExprList();
                    //}
                    Expect(TokenCategory.CLOSEDBRACKET);
                    //return n;
                    break;

                case TokenCategory.OPENEDPAR:
                    Expect(TokenCategory.OPENEDPAR);
                    //var n = new ExprPrimary(){
                        Expr();
                    //}
                    Expect(TokenCategory.CLOSEDPAR);
                    //return n;
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
                    //n.Add(Expr());
                    Expr();
                }
            }
            
            //Borre ExprListCont //(‹expr› (,‹expr›)*)?
            /*
            var n = new ExprListCont()
            while(Current == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                n.Add(Expr());
            }
            return n;
            */
        }
    }
}