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
            var n1 = DefList();
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
            
            if(){
                
            }
        }
        public Node VarDefList(){
            //‹var-list›
            return new VarList()
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
            Expect(TokenCategory.ID);
            Expect(TokenCategory.OPENEDPAR);
            var n = ExprList();
            Expect(TokenCategory.CLOSEDPAR);
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
        
        public Node ExprList(){ //Borre ExprListCont //(‹expr› (,‹expr›)*)?
            var n = new ExprListCont()
            while(Current == TokenCategory.COMA){
                Expect(TokenCategory.COMA);
                n.Add(Expr());
            }
            return n;
        }
        
        public Node Expr(){ //ExprOr  --> ‹expr-and› < || ‹expr-and› >*
            var n1 = ExprAnd();
            while(Current = TokenCategory.OR){
                n2 = new Expr();
                n2.AnchorToken = Expect(TokenCategory.OR)
                n2.Add(n1);
                n2.Add(ExprAnd());
                n1 = n2;
            }
            return n1;
        }
        
        public Node ExprAnd(){//‹expr-comp› < &&‹expr-comp› >*
            var n1 = ExprComp();
            while(Current = TokenCategory.AND){
                n2 = new ExprAnd();
                n2.AnchorToken = Expect(TokenCategory.AND)
                n2.Add(n1);
                n2.Add(ExprComp());
                n1 = n2;
            }
            return n1;
        }
        
        public Node ExprComp(){
            //‹expr-rel›  <‹op-comp› ‹expr-rel›>*
            var n1 = ExprRel();
            while(Current = TokenCategory.NOTEQUALS || Current = TokenCategory.EQUALS){
                n2 = new ExprComp();
                switch(Current){
                    case TokenCategory.EQUALS:
                        n2.AnchorToken = Expect(TokenCategory.EQUALS);
                        break;
                    case TokenCategory.LESSEQUAL:
                        n2.AnchorToken = Expect(TokenCategory.LESSEQUAL);
                        break;
                    default:
                        throw new SyntaxError(category, tokenStream.Current);
                }
                n2.Add(n1);
                n2.Add(ExprRel());
                n1 = n2
            }
            return n1;
        }

        public Node ExprRel(){
            //‹expr-add› <‹op-rel› ‹expr-add›>*
            var n1 = ExprAdd();
            while(Current = TokenCategory.LESS || Current = TokenCategory.LESSEQUAL || Current = TokenCategory.GREATER || Current = TokenCategory.GREATEREQUAL){
                n2 = new ExprRel();
                switch(Current){
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
                        throw new SyntaxError(category, tokenStream.Current);
                }
                n2.Add(n1);
                n2.Add(ExprAdd());
                n1 = n2
            }
            return n1;
            
        }

        public Node ExprAdd(){
            //‹expr-mul› < ‹op-add› ‹expr-mul› >*
            var n1 = ExprMul();
            while(Current == TokenCategory.MINUS || Current == TokenCategory.PLUS){
                n2 = new ExprMul();
                switch(Current){
                    case TokenCategory.MINUS:
                        n2.AnchorToken = Expect(TokenCategory.MINUS);
                        break;
                    case TokenCategory.PLUS:
                        n2.AnchorToken = Expect(TokenCategory.PLUS);
                        break;
                    default:
                        throw new SyntaxError(category, tokenStream.Current);
                }
                n2.Add(n1);
                n2.Add(ExprMul());
                n1 = n2
            }
            return n1;
        }

        public Node ExprMul(){ //‹expr-unary› < ‹op-mul› ‹expr-unary› >*
            var n1 = ExprUnary();
            while(Current == TokenCategory.MULTIPLICATION || Current == TokenCategory.DIVIDE || Current == TokenCategory.MODULO){
                n2 = new ExprMul();
                switch(Current){
                    case TokenCategory.MULTIPLICATION:
                        n2.AnchorToken = Expect(TokenCategory.MULTIPLICATION);
                        break;
                    case TokenCategory.DIVIDE:
                        n2.AnchorToken = Expect(TokenCategory.DIVIDE);
                        break;
                    case TokenCategory.MODULO:
                        n2.AnchorToken = Expect(TokenCategory.MODULO);
                        break;
                    default:
                        throw new SyntaxError(category, tokenStream.Current);
                }
                n2.Add(n1);
                n2.Add(ExprUnary());
                n1 = n2
            }
            return n1;
        }

        //TODO Check-Unary
        public Node ExprUnary(){
            //< ‹op-unary› >* ‹expr-primary›
            var top = new ExprUnary();
            var temp = top; //reference _magic_ 
            while(Current = TokenCategory.MINUS || Current = TokenCategory.PLUS || Current = TokenCategory.NOT){ 
                switch(Current){
                    case TokenCategory.MINUS:
                        temp.AnchorToken = Expect(TokenCategory.MINUS);
                        break;
                    case TokenCategory.PLUS:
                        temp.AnchorToken = Expect(TokenCategory.PLUS);
                        break;
                    case TokenCategory.NOT:
                        temp.AnchorToken = Expect(TokenCategory.NOT);
                        break;
                    default:
                        throw new SyntaxError(category, tokenStream.Current);
                }
                var newNode = new ExprUnary(); //ToBeReplacedPrimary or next Unary
                temp.Add(newNode);
                temp = newNode;
            }
            temp = ExprPrimary(); //replace the invisible current node for the real Primary
            return top; //Always return the head
        }

        public Node ExprPrimary(){
            //‹id› | ‹fun-call› | ‹array› | ‹lit› | (‹expr›)
            switch(Current){
                case TokenCategory.IDENTIFIER:
                    var n = new ExprPrimary(){
                        AnchorToken = Expect(TokenCategory.IDENTIFIER)
                    }
                    //TODO Check fun-call
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
                ExprList(); //TODO: Check this
            }
            Expect(TokenCategory.CLOSEDBRACKET);
            return n;
        }
        
        public Node Lit(){ //‹lit-int› | ‹lit-char› | ‹lit-str›
            switch(Current){
                case TokenCategory.INT:
                    var n = new Lit(){
                        AnchorToken = Expect(TokenCategory.INT);            
                    }
                    return n;
                case TokenCategory.CHAR:
                    var n = new Lit(){
                        AnchorToken =  Expect(TokenCategory.CHAR);            
                    }
                    return n;
                case TokenCategory.STR:
                    var n = new Lit(){
                        AnchorToken =  Expect(TokenCategory.STR);            
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
    public class ExprList: Node{}
    public class Else: Node{}
    public class ElseIfList: Node{}
    public class Expr: Node{}
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
        }
        sb.Append('\n');
        foreach (var child in node.children) {
            TreeTraversal(child, indent + "  ", sb);
    }
}