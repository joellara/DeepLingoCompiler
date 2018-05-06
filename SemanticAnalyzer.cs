/*
    DeepLingo compiler - Semantic Analyzer class.
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

namespace DeepLingo {
    class SemanticAnalyzer {
        public SymbolTable Global_Symbol_Table;
        public FunctionTable Global_Function_Table;
        public SymbolTable currentNamespaceTable;
        public IDictionary<string, SymbolTable> localSymbolTables = new SortedDictionary<string, SymbolTable>();
        public bool firstPass;
        public bool insideFunction;
        public int insideLoop;
        
        //-----------------------------------------------------------
        public SemanticAnalyzer() {
            Global_Symbol_Table      = new SymbolTable();
            Global_Function_Table    = new FunctionTable();
            currentNamespaceTable    = new SymbolTable();
            firstPass                = true;
            insideFunction           = false;
            insideLoop               = 0;
            
            /* System predefined functions*/
            Global_Function_Table["add"] = 2;
            Global_Function_Table["get"] = 2;
            Global_Function_Table["new"] = 1;
            Global_Function_Table["printc"] = 1;
            Global_Function_Table["printi"] = 1;
            Global_Function_Table["println"] = 0;
            Global_Function_Table["prints"] = 1;
            Global_Function_Table["readi"] = 0;
            Global_Function_Table["reads"] = 0;
            Global_Function_Table["set"] = 3;
            Global_Function_Table["size"] = 1;
        }
        
        public void Visit(Program node){
            VisitChildren(node);
            if(!Global_Function_Table.Contains("main")){
                throw new SemanticError("No main function declared");
            }
            firstPass = false;
            VisitChildren(node);
        }
        public void Visit(IntLiteral node){
            var intStr = node.AnchorToken.Lexeme;
            try {
                Convert.ToInt32(intStr);
            } 
            catch (OverflowException) {
                throw new SemanticError("Integer literal too large: " + intStr, node.AnchorToken);
            }
        }
        public void Visit(Identifier node){
            var variableName = node.AnchorToken.Lexeme;
            if (!currentNamespaceTable.Contains(variableName) && !Global_Symbol_Table.Contains(variableName)) {
                throw new SemanticError("No defined variable: " + variableName,node.AnchorToken);
            }
        }
        public void Visit(Arr node){
            VisitChildren(node);
        }
        public void Visit(Char node){
        }
        public void Visit(VarDefList node){
            VisitChildren(node);
        }
        public void Visit(VarDef node){
            foreach (var n in node[0]) {
                var variableName = n.AnchorToken.Lexeme;
                if(insideFunction){
                    if(currentNamespaceTable.Contains(variableName)){
                        throw new SemanticError("Duplicated variable: " + variableName,n.AnchorToken);
                    }
                    currentNamespaceTable.Add(variableName);
                }
                else{
                    if (firstPass){
                        if(Global_Symbol_Table.Contains(variableName)){
                            throw new SemanticError("Duplicated variable: " + variableName,n.AnchorToken);    
                        } 
                        else{
                            Global_Symbol_Table.Add(variableName);
                        }
                    }
                }
                
                VisitChildren(n);
            }
            
        }
        public void Visit(IdList node){
            if(!firstPass){
                foreach(var n in node){
                    var variableName = n.AnchorToken.Lexeme;
                    if(currentNamespaceTable.Contains(variableName)){
                        throw new SemanticError("Duplicated variable: " + variableName,node[0].AnchorToken);
                    }
                    else{
                        currentNamespaceTable.Add(variableName);
                    }
                    VisitChildren(n);
                }
            }
        }
        public void Visit(FunDef node){
            var functionName = node.AnchorToken.Lexeme;
            if(firstPass){
                if(Global_Function_Table.Contains(functionName)){
                    throw new SemanticError("Duplicated function: " + functionName,node.AnchorToken);    
                }
                else{
                    if(node[0] is IdList){
                        var t = 0; 
                        foreach (var n in node[0]){t++;}
                        Global_Function_Table[functionName] = t;              
                    }
                    else{
                        Global_Function_Table[functionName] = 0;                  
                    }
                }
            }
            else{
                currentNamespaceTable = new SymbolTable();
                insideFunction = true;
                VisitChildren(node);
                localSymbolTables[functionName] = currentNamespaceTable;
                insideFunction = false;
                currentNamespaceTable = new SymbolTable();
            }
        }
        public void Visit(Assignment node){
            var varName = node.AnchorToken.Lexeme;
            if(!currentNamespaceTable.Contains(varName) && !Global_Symbol_Table.Contains(varName)){
                throw new SemanticError("Undeclared variable: " + varName,node[0].AnchorToken);    
            }
            VisitChildren(node);
        }
        public void Visit(FunCall node){
            var varName = node.AnchorToken.Lexeme;
            if(!Global_Function_Table.Contains(varName)){
                throw new SemanticError("Undeclared function: " + varName,node.AnchorToken);    
            }
            else{
                var t = 0;
                foreach (var n in node[0]){t++;}
                if(t != Global_Function_Table[varName]){
                    throw new SemanticError("Function called with different parameters as defined ",node.AnchorToken);    
                }
                VisitChildren(node);
            }
        }
        public void Visit(StmtList node){
            VisitChildren(node);
        }
        public void Visit(Def node){
            VisitChildren(node);
        }
        public void Visit(If node){
            VisitChildren(node);
        }
        public void Visit(ElseIfList node){
            VisitChildren(node);
        }
        public void Visit(ElseIf node){
            VisitChildren(node);
        }
        public void Visit(Else node){
            VisitChildren(node);
        }
        public void Visit(Loop node){
            insideLoop++;
            VisitChildren(node);
            insideLoop--;
        }
        public void Visit(Return node){
            VisitChildren(node);
        }
        public void Visit(Increment node){
            VisitChildren(node);
        }
        public void Visit(Decrement node){
            VisitChildren(node);
        }
        public void Visit(Stmt node){
            if(node.AnchorToken.Category == TokenCategory.BREAK && insideLoop == 0){
                throw new SemanticError("Found Break outside loop declaration",node.AnchorToken);
            }
            VisitChildren(node);
        }
        public void Visit(ExprList node){
            VisitChildren(node);
        }
        public void Visit(ExprOr node){
            VisitChildren(node);
        }
        public void Visit(ExprAnd node){
            VisitChildren(node);
        }
        public void Visit(ExprAdd node){
            VisitChildren(node);
        }
        public void Visit(ExprComp node){
            VisitChildren(node);
        }
        public void Visit(ExprRel node){
            VisitChildren(node);
        }
        public void Visit(ExprMul node){
            VisitChildren(node);
        }
        public void Visit(ExprUnary node){
            VisitChildren(node);
        }
        public void Visit(ExprPrimary node){
            VisitChildren(node);
        }
        public void Visit(Str node) {
            VisitChildren(node);
        }
    
        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }
        
    }
}
