/*
  DeepLingo compiler - Specific node subclasses for the AST (Abstract 
  Syntax Tree).
    DeepLingo compiler - Parent Node class for the AST (Abstract Syntax Tree).
    Esteban Gil Martinez        A01375048
    Javier Esponda Hernández    A01374645
    Joel Lara Quintana          A01374649

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
    
    class VarDefList: Node{}
    
    class Arr: Node{}
    class Program: Node {}
    class Def: Node {}
    class Char: Node {}
    class VarDef: Node {}
    class IdList: Node {}
    class Identifier: Node {}
    class If:Node{}
    class FunDef: Node {}
    class ElseIfList: Node {}
    class ElseIf: Node {}
    class Else: Node {}
    class StmtList: Node {}
    class Loop: Node {}
    class Return: Node {}
    class Assignment: Node {}
    class Increment: Node {}
    class IntLiteral: Node{}
    class Decrement: Node {}
    class FunCall: Node {}
    class Stmt: Node{}
    class ExprList: Node {}
    class ExprOr: Node {}
    class ExprAnd: Node {}
    class ExprAdd: Node {}
    class ExprComp: Node {}
    class ExprRel: Node {}
    class ExprMul: Node {}
    class ExprUnary: Node {}
    class ExprPrimary: Node {}
    class Str: Node  {}
}