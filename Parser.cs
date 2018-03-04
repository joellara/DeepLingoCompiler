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
using System.Text;
using System.Text.RegularExpressions;

namespace deepLingo {
    class Parser {      
                
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
            // <DefList>
            var n = new Program() {
                DefList()
            };
            Expect(TokenCategory.EOF);
            return n;
        }
        
        public Node DefList(){
            // <Def> *
            var n1 = Node();
            while (Current != TokenCategory.EOF) {
                var n2 = new Def();
                n2.Add(n1);
                n1 = n2;
            }
            return n1;
        }
        
        public Node Def(){
            //‹var-def› | ‹fun-def›
        }
        
        public Node VarDef(){
            //var‹id-list›;
        }
        public Node VarList(){
            //‹id-list›
        }
        public Node IdList(){
            //‹id› ‹id-list-cont›
        }
        public Node IdListCont(){
            //,‹id›*
        }
        public Node FunDef(){
            //‹id›(‹param-list›){‹var-def-list› ‹stmt-list›}
        }
        public Node ParamList(){
            //<‹id-list› >?
        }
        public Node VarDefList(){
            //‹var-list›
        }
        public Node StmtList(){
            //‹stmt›*
            var n = StmtList();
            while(Current == TokenCategory.IDENTIFIER || Current == TokenCategory.IF || Current == TokenCategory.LOOP || Current == TokenCategory.BREAK || Current == TokenCategory.RETURN || Current == TokenCategory.SEMICOLON){
                n.Add(Stmt());
            }
            return n;
        }
        public Node Stmt(){
            //‹id›=‹expr›; | ‹id›++; | ‹id›−−; | ‹fun-call›; | if(‹expr›){‹stmt-list›}‹else-if-list› ‹else› | loop{‹stmt-list›} | break; | return‹expr›; | ;
        }
        public Node FunCall(){
            // ‹id› ( ‹expr-list› )
        }
        public Node ExprList(){
            //< ‹expr› ‹expr-list-cont› >?
            //steve help
        }
        public Node ExprListCont(){
            //,‹expr› *
            var n = new ExprListCont()
            while(Current == TokenCategory.COMA){
                Expect(TokenCategory.COMA);
                    n.Add(Expr());
            }
            return n;
        }
        public Node Else(){
            //< else{‹stmt-list›} >?
            var n = new Else();
            if(Current == TokenCategory.ELSE){
                Expect(TokenCategory.ELSE);
                Expect(TokenCategory.OPENEDCURLY);
                n.Add(StmtList());
                Expect(TokenCategory.CLOSEDCURLY);
            }
            return n;
        }
        public Node ElseIfList(){
            //elseif(‹expr›){‹stmt-list›} *
            var n = new ElseIfList();
            while(Current == TokenCategory.ELSEIF)
                Expect(TokenCategory.ELSEIF);
                Expect(TokenCategory.OPENEDPAR);
                n.Add(Expr());
                Expect(TokenCategory.CLOSEDPAR);
                Expect(TokenCategory.OPENEDCURLY);
                n.Add(StmtList());
                Expect(TokenCategory.CLOSEDCURLY);
            }
            return n;
        }
        public Node Expr(){ //‹expr-or›
            var n = new Expr(){
                ExprOr();
            }
            return n;
        }
        public Node ExprOr(){ //‹expr-and› < || ‹expr-and› >*
            var n = new ExprOr(){
                ExprAnd();
            }
            while(Current == TokenCategory.OR){
                Expect(TokenCategory.OR);
                n.Add(ExprAnd());
            }
            return n;
        }
        
        public Node ExprAnd(){//‹expr-comp› < &&‹expr-comp› >*
            var n = new ExprAnd(){
                ExprComp()
            }
            while(Current == TokenCategory.AND){
                Expect(TokenCategory.AND);
                n.Add(ExprComp());
            }
            return n;
        }
        public Node ExprComp(){
            //‹expr-rel›  <‹op-comp› ‹expr-rel›>*
            var n = new ExprComp(){
                ExprRel()
            }
            while(Current == TokenCategory.NOTEQUALS || Current == TokenCategory.EQUALS){
                n.Add(OpComp);
                n.Add(ExprRel);
            }
            return n;
        }
        
        public Node OpComp(){//!= | ==
            switch(Current){
                case TokenCategory.NOTEQUALS:
                    var n = OpComp(){
                        Expect(TokenCategory.NOTEQUALS);
                    }
                    return n;
                case TokenCategory.EQUALS:
                    var n = OpComp(){
                        Expect(TokenCategory.EQUALS);
                    }
                    return n;
                default: 
                    throw new SyntaxError(category, tokenStream.Current);
            }
        }
        public Node ExprRel(){
            //‹expr-add› <‹op-rel› ‹expr-add›>*
            var n = new ExprRel(){
                ExprAdd()
            }
            while(Current = TokenCategory.LESS || Current = TokenCategory.LESSEQUAL || Current = TokenCategory.GREATER || Current = TokenCategory.GREATEREQUAL){
                n.Add(OpRel());
                n.Add(ExprAdd());
            }
            return n;
        }
        public Node OpRel(){// < | <= | > | >=
            switch(Current){
                case TokenCategory.LESS:
                    var n = OpRel(){
                        Expect(TokenCategory.LESS);
                    }
                    return n;
                case TokenCategory.LESSEQUAL:
                    var n = OpRel(){
                        Expect(TokenCategory.LESSEQUAL);
                    }
                    return n;
                case TokenCategory.GREATER:
                    var n = OpRel(){
                        Expect(TokenCategory.GREATER);
                    }
                    return n;
                case TokenCategory.GREATEREQUAL:
                    var n = OpRel(){
                        Expect(TokenCategory.GREATEREQUAL);
                    }
                    return n;
                default: 
                    throw new SyntaxError(category, tokenStream.Current);
            }
        }
        
        public Node ExprAdd(){
            //‹expr-mul› < ‹op-add› ‹expr-mul› >*
            var n = new ExprAdd(){
                ExprMul()
            }
            while(Current == TokenCategory.MINUS || Current == TokenCategory.PLUS){
                n.Add(OpAdd());
                n.Add(ExprMul());
            }
            return n;
        }
        
        public Node OpAdd(){//− | +
            switch(Current){
                case TokenCategory.MINUS:
                    var n = OpAdd(){
                        Expect(TokenCategory.MINUS);
                    }
                    return n;
                case TokenCategory.PLUS:
                    var n = OpAdd(){
                        Expect(TokenCategory.PLUS);
                    }
                    return n;
                default: 
                    throw new SyntaxError(category, tokenStream.Current);
            }
        }
        
        public Node ExprMul(){ //‹expr-unary› < ‹op-mul› ‹expr-unary› >*
            var n = new ExprMul(){
                ExprUnary()
            }
            while(Current == TokenCategory.MULTIPLICATION || Current == TokenCategory.DIVIDE || Current == TokenCategory.MODULO){
                n.Add(OpUnary());
                n.Add(ExprUnary());
            }
            return n;
        }
        
        public Node OpMul(){//* | / | %
            switch(Current){
                case TokenCategory.MULTIPLICATION:
                    var n = OpMul(){
                        Expect(TokenCategory.MULTIPLICATION)    
                    }
                    return n;
                case TokenCategory.DIVIDE:
                    var n = OpMul(){
                        Expect(TokenCategory.DIVIDE)    
                    }
                    return n;
                case TokenCategory.MODULO:
                    var n = OpMul(){
                        Expect(TokenCategory.MODULO)    
                    }
                    return n;
                default:
                    throw new SyntaxError(category, tokenStream.Current);
            }
        }
        
        public Node ExprUnary(){
            //< ‹op-unary› >* ‹expr-primary›
            var n = ExprUnary();
            while(Current = TokenCategory.MINUS || Current = TokenCategory.PLUS || Current = TokenCategory.NOT){
                n.Add(OpUnary());
            }
            n.Add(ExprPrimary());
            return n;
        }
        
        public Node OpUnary(){ //− | + | !
            switch(Current){
                case TokenCategory.MINUS:
                    var n = OpUnary(){
                        Expect(TokenCategory.MINUS)    
                    }
                    return n;
                case TokenCategory.PLUS:
                    var n = OpUnary(){
                        Expect(TokenCategory.PLUS)    
                    }
                    return n;
                case TokenCategory.NOT:
                    var n = OpUnary(){
                        Expect(TokenCategory.NOT)    
                    }
                    return n;
                default:
                    throw new SyntaxError(category, tokenStream.Current);
            }
        }
        
        public Node ExprPrimary(){
            //‹id› | ‹fun-call› | ‹array› | ‹lit› | (‹expr›)
            switch(Current){
                case TokenCategory.IDENTIFIER:
                    var n = new ExprPrimary(){
                        Expect(TokenCategory.IDENTIFIER)
                    }
                    if(Current == TokenCategory.OPENEDPAR){
                        Expect(TokenCategory.OPENEDPAR)
                        n.Add(FunCall());
                        Expect(TokenCategory.CLOSEDPAR)
                    }
                    return n;
                case TokenCategory.OPENEDBRACKET:
                    Expect(TokenCategory.OPENEDBRACKET)
                    var n = new ExprPrimary(){
                        ExprList()
                    }
                    Expect(TokenCategory.CLOSEDBRACKET)
                    return n;
                case TokenCategory.OPENEDPAR:
                    Expect(TokenCategory.OPENEDPAR)
                    var n = new ExprPrimary(){
                        Expr()
                    }
                    Expect(TokenCategory.CLOSEDPAR)
                    return n;
                default:
                    return Lit();
            }
        }
        
        public Node Array(){ //[‹expr-list›]
            Expect(TokenCategory.OPENEDBRACKET);
            var n = new Array(){
                ExprList()
            }
            Expect(TokenCategory.CLOSEDBRACKET);
            return n;
        }
        
        public Node Lit(){ //‹lit-int› | ‹lit-char› | ‹lit-str›
            switch(Current){
                case TokenCategory.INT:
                    var n = new Lit(){
                        Expect(TokenCategory.INT);            
                    }
                    return n;
                case TokenCategory.CHAR:
                    var n = new Lit(){
                        Expect(TokenCategory.CHAR);            
                    }
                    return n;
                case TokenCategory.STR:
                    var n = new Lit(){
                        Expect(TokenCategory.STR);            
                    }
                    return n;
                default:
                    throw new SyntaxError(category, tokenStream.Current);                
            }
        }
    }
    
    public class Token {
        public TokenCategory Category { get; }
        public String Lexeme { get; }
        public Token(TokenCategory category, String lexeme) {
            Category = category;
            Lexeme = lexeme;
        }
        public override String ToString() {
            return String.Format("[{0}, \"{1}\"]", Category, Lexeme);
        }
    }
    
    public class Program: Node{}
    public class Def_list: Node{}
    public class Def: Node{}
    public class VarDef: Node{}
    public class VarList: Node{}
    public class IdList: Node{}
    public class IdListCont: Node{}
    public class FunDef: Node{}
    public class ParamList: Node{}
    public class VarDefList: Node{}
    public class StmtList: Node{}
    public class Stmt: Node{}
    public class FunCall: Node{}
    public class Expr: Node{}
    public class ExprListCont: Node{}
    public class ExprList: Node{}
    public class Else: Node{}
    public class ElseIfList: Node{}
    public class Expr: Node{}
    public class ExprOr: Node{}
    public class ExprAnd: Node{}
    public class ExprComp: Node{}
    public class OpComp: Node{}
    public class ExprRel: Node{}
    public class OpRel: Node{}
    public class ExprAdd: Node{}
    public class OpAdd: Node{}
    public class ExprMul: Node{}
    public class OpMul: Node{}
    public class ExprUnary: Node{}
    public class OpUnary: Node{}
    public class ExprPrimary: Node{}
    public class Array: Node{}
    public class Lit: Node{}
}

public class Node: IEnumerable<Node> {
    IList<Node> children = new List<Node>();

    public Node this[int index] {
        get {
            return children[index];
        }
    }

    public Token AnchorToken { get; set; }

    public void Add(Node node) {
        children.Add(node);
    }

    public IEnumerator<Node> GetEnumerator() {
        return children.GetEnumerator();
    }

    System.Collections.IEnumerator
    System.Collections.IEnumerable.GetEnumerator() {
        throw new NotImplementedException();
    }

    public override string ToString() {
        return String.Format("{0} {1}", GetType().Name, AnchorToken);
    }

    public string ToStringTree() {
        var sb = new StringBuilder();
        TreeTraversal(this, "", sb);
        return sb.ToString();
    }

    static void TreeTraversal(Node node, string indent, StringBuilder sb) {
        sb.Append(indent);
        sb.Append(node);
        sb.Append('\n');
        foreach (var child in node.children) {
            TreeTraversal(child, indent + "  ", sb);
        }
    }
}