using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;
using System.Collections;
using OALProgramControl;

namespace AnimationControl.OAL
{
    public class OALVisitor2:OALBaseVisitor<object>
    {
        public EXEScopeMethod globalExeScope;
        private Stack<EXEScope> stackEXEScope;
        private Stack<EXEASTNode> stackEXEASTNode;
		private String instanceName;
		private String attributeName;

        public OALVisitor2()
        {
            this.globalExeScope = new EXEScopeMethod();
            this.stackEXEASTNode = new Stack<EXEASTNode>();
            this.stackEXEScope = new Stack<EXEScope>();
            this.stackEXEScope.Push(this.globalExeScope);
			
			this.instanceName = null;
			this.attributeName = null;
        }


        public override object VisitExeCommandQueryCreate([NotNull] OALParser.ExeCommandQueryCreateContext context)
        {
            String ClassName = null;
            String InstanceName = null;

            foreach (IParseTree child in context.children)
            {
                //Console.WriteLine(context.ChildCount);
                if (child.GetType().ToString().Contains("KeyLetter"))
                {
                    ClassName = ParseUtil.StripWhiteSpace((child.GetText()));
                    Console.WriteLine("Class name---" + ClassName + "---");
                }

                if (child.GetType().ToString().Contains("InstanceHandle"))
                {
					Visit(child);
                    InstanceName = ParseUtil.StripWhiteSpace(this.instanceName);
                    Console.WriteLine("Instance name---" + InstanceName + "---");
                }
                
            }
            
            if (InstanceName != null)
            {
                stackEXEScope.Peek().AddCommand(new EXECommandQueryCreate(ClassName, InstanceName));
            }
            else
            {
                stackEXEScope.Peek().AddCommand(new EXECommandQueryCreate(ClassName));
            }
            
            return null;
        }


        public override object VisitExeCommandQueryRelate([NotNull] OALParser.ExeCommandQueryRelateContext context)
        {
			Visit(context.GetChild(1));
            String VariableName1 = this.instanceName;
			Visit(context.GetChild(3));
            String VariableName2 = this.instanceName;
            String RelationshipName = context.GetChild(5).GetText();

            //Console.WriteLine(VariableName1);
            //Console.WriteLine(VariableName2);
            //Console.WriteLine(RelationshipName);

            stackEXEScope.Peek().AddCommand(new EXECommandQueryRelate(VariableName1, VariableName2, RelationshipName));

            return null;
            //return base.VisitExeCommandQueryRelate(context);
        }


        public override object VisitExeCommandQuerySelect([NotNull] OALParser.ExeCommandQuerySelectContext context)
        {
            String Cardinality = context.GetChild(0).GetText().Contains("many") ? "many" : "any";
			Visit(context.GetChild(1));
            String VariableName = this.instanceName;
            String ClassName = context.GetChild(3).GetText();
            EXEASTNode WhereExpression;

            if(context.GetChild(4).GetText().Contains("where"))
            {
                //Console.WriteLine("where");
                Visit(context.GetChild(5));
                //Console.WriteLine("pocet v zasobniku: " + stackEXEASTNode.Count);

                WhereExpression = stackEXEASTNode.Peek();
                stackEXEScope.Peek().AddCommand(new EXECommandQuerySelect(Cardinality, ClassName, VariableName, WhereExpression));
            }
            else
            {
                stackEXEScope.Peek().AddCommand(new EXECommandQuerySelect(Cardinality, ClassName, VariableName));
            }

            //Console.WriteLine("ending execommandqueryselect");
            stackEXEASTNode.Clear();

            return null;
            //return base.VisitExeCommandQuerySelect(context);
        }


        public override object VisitExeCommandQueryUnrelate([NotNull] OALParser.ExeCommandQueryUnrelateContext context)
        {
			Visit(context.GetChild(1));
            String VariableName1 = this.instanceName;
			Visit(context.GetChild(3));
            String VariableName2 = this.instanceName;
            String RelationshipName = context.GetChild(5).GetText();

            stackEXEScope.Peek().AddCommand(new EXECommandQueryUnrelate(VariableName1, VariableName2, RelationshipName));

            return null;
            //return base.VisitExeCommandQueryUnrelate(context);
        }


        public override object VisitExeCommandQueryDelete([NotNull] OALParser.ExeCommandQueryDeleteContext context)
        {
			Visit(context.GetChild(1));
            String VariableName = this.instanceName;

            stackEXEScope.Peek().AddCommand(new EXECommandQueryDelete(VariableName));

            return null;
            //return base.VisitExeCommandQueryDelete(context);
        }


        public override object VisitExeCommandAssignment([NotNull] OALParser.ExeCommandAssignmentContext context)
        {//TODO priradenie atributu
            /*String VariableName =  context.GetChild(0).GetText().Equals("assign ") ? context.GetChild(1).GetText() : context.GetChild(0).GetText();
            String AttributeName = null;

            if((context.GetChild(0).GetText().Equals("assign ") ? context.GetChild(2).GetText() : context.GetChild(1).GetText()).Equals("."))
            {
                AttributeName = context.GetChild(0).GetText().Equals("assign ") ? context.GetChild(3).GetText() : context.GetChild(2).GetText();
                _ = context.GetChild(0).GetText().Equals("assign ") ? Visit(context.GetChild(5)) : Visit(context.GetChild(4));
            }
            else
            {
                _ = context.GetChild(0).GetText().Equals("assign ") ? Visit(context.GetChild(3)) : Visit(context.GetChild(2));
            }

            EXEASTNode expression = stackEXEASTNode.Peek();

            stackEXEScope.Peek().AddCommand(new EXECommandAssignment(VariableName, AttributeName, expression));

            stackEXEASTNode.Clear();

            return null;
            //return base.VisitExeCommandAssignment(context);*/
			
			_ = context.GetChild(0).GetText().Equals("assign ") ? Visit(context.GetChild(1)) : Visit(context.GetChild(0));
			String VariableName =  this.instanceName;
            String AttributeName = this.attributeName;
			
			_ = context.GetChild(0).GetText().Equals("assign ") ? Visit(context.GetChild(3)) : Visit(context.GetChild(2));
			EXEASTNode expression = stackEXEASTNode.Peek();
			
			stackEXEScope.Peek().AddCommand(new EXECommandAssignment(VariableName, AttributeName, expression));

            stackEXEASTNode.Clear();

            return null;
			//return base.VisitExeCommandAssignment(context);
        }


        public override object VisitExpr([NotNull] OALParser.ExprContext context)
        {

            //Console.WriteLine("Expr: " + context.ChildCount);
            //Console.WriteLine(context.GetChild(0).GetType().Name);
            
            if (context.ChildCount == 1)
            {
                if (context.GetChild(0).GetType().Name.Contains("TerminalNode") && stackEXEASTNode.Count() == 0)
                {
                    EXEASTNodeLeaf ast = new EXEASTNodeLeaf(ParseUtil.StripWhiteSpace(context.GetChild(0).GetText()));
                    stackEXEASTNode.Push(ast);
                    //stackEXEASTNode.Peek().AddOperand(new EXEASTNodeComposite(ParseUtil.StripWhiteSpace(context.GetChild(0).GetText())));
                }
                else
                {
                    ((EXEASTNodeComposite) stackEXEASTNode.Peek()).AddOperand(new EXEASTNodeLeaf(ParseUtil.StripWhiteSpace(context.GetChild(0).GetText())));
                }
            }
            else if(context.ChildCount == 2)
            {               
                EXEASTNodeComposite ast = new EXEASTNodeComposite(ParseUtil.StripWhiteSpace(context.GetChild(0).GetText()));
                stackEXEASTNode.Push(ast);
                
                base.VisitExpr(context);

                if (context.GetChild(0).GetType().Name.Contains("TerminalNode") && !context.GetChild(0).GetText().ToLower().Equals("not ") && !context.GetChild(0).GetText().Equals("-"))
                { 
					Visit(context.GetChild(1));
					String InstanceName =  this.instanceName;
					String AttributeName = this.attributeName;
                    EXEASTNode newOperand = null;

                    if (AttributeName == null)
					{
						newOperand = new EXEASTNodeLeaf(ParseUtil.StripWhiteSpace(InstanceName));
					}
					else
					{
						newOperand = new EXEASTNodeComposite(".");
                        ((EXEASTNodeComposite)newOperand).AddOperand(new EXEASTNodeLeaf(ParseUtil.StripWhiteSpace(InstanceName)));
                        ((EXEASTNodeComposite)newOperand).AddOperand(new EXEASTNodeLeaf(ParseUtil.StripWhiteSpace(AttributeName)));
					}

                    ((EXEASTNodeComposite)stackEXEASTNode.Peek()).AddOperand(newOperand);
                    EXEASTNode temp = stackEXEASTNode.Pop();
                    ((EXEASTNodeComposite)stackEXEASTNode.Peek()).AddOperand(temp);
                }
                else if(context.GetChild(0).GetText().ToLower().Equals("not ") || context.GetChild(0).GetText().Equals("-"))
                {
                    if (stackEXEASTNode.Count() > 1)
                    {
                        EXEASTNode temp = stackEXEASTNode.Pop();
                        ((EXEASTNodeComposite)stackEXEASTNode.Peek()).AddOperand(temp);
                    }
                }
            }
            else if(context.ChildCount == 3)
            {
                if (!context.GetChild(0).GetText().Equals("("))
                {
                    EXEASTNodeComposite ast = new EXEASTNodeComposite(ParseUtil.StripWhiteSpace(context.GetChild(1).GetText()));
                    stackEXEASTNode.Push(ast);
                }

                base.VisitExpr(context);
                
                if (context.GetChild(0).GetType().Name.Contains("TerminalNode") && !context.GetChild(0).GetText().Equals("("))
                {
                    ((EXEASTNodeComposite)stackEXEASTNode.Peek()).AddOperand(new EXEASTNodeLeaf(ParseUtil.StripWhiteSpace(context.GetChild(0).GetText())));
                    ((EXEASTNodeComposite)stackEXEASTNode.Peek()).AddOperand(new EXEASTNodeLeaf(ParseUtil.StripWhiteSpace(context.GetChild(2).GetText())));
                    EXEASTNode temp = stackEXEASTNode.Pop();
                    ((EXEASTNodeComposite)stackEXEASTNode.Peek()).AddOperand(temp);
                }
                else if (stackEXEASTNode.Count > 1 && !context.GetChild(0).GetText().Equals("("))
                {
                    EXEASTNode temp = stackEXEASTNode.Pop();
                    ((EXEASTNodeComposite)stackEXEASTNode.Peek()).AddOperand(temp);
                }

            }

            return null;
            //return base.VisitExpr(context);
        }


        public override object VisitExeCommandQuerySelectRelatedBy([NotNull] OALParser.ExeCommandQuerySelectRelatedByContext context)
        {
            String Cardinality = context.GetChild(0).GetText().Contains("many") ? "many" : "any";
            Console.WriteLine("Cardinality = " + Cardinality);

			Visit(context.GetChild(1));
            String VariableName = this.instanceName;
            Console.WriteLine("VariableName = " + VariableName);

            String StartingVariable = context.GetChild(3).GetText();
            Console.WriteLine("StartingVariable = " + StartingVariable);

            String ClassName = context.GetChild(5).GetText();
            Console.WriteLine("ClassName = " + ClassName);

            String RelationshipName = context.GetChild(6).GetText().Replace('[',' ').Replace(']',' ').Trim();
            Console.WriteLine("RelationshipName = " + RelationshipName);

            List<EXERelationshipLink> list = new List<EXERelationshipLink>();
            EXERelationshipLink eXERelationshipLink = new EXERelationshipLink(RelationshipName, ClassName);
            EXERelationshipSelection eXERelationshipSelection = new EXERelationshipSelection(StartingVariable);
            eXERelationshipSelection.AddRelationshipLink(eXERelationshipLink);
            EXEASTNode WhereExpression = null;

            int i = 7;
            while (context.GetChild(i).GetText().Equals("->"))
            {
                String ClassName2 = context.GetChild(i + 1).GetText();
                Console.WriteLine("ClassName = " + ClassName2);

                String RelationshipName2 = context.GetChild(i + 2).GetText().Replace('[', ' ').Replace(']', ' ').Trim();
                Console.WriteLine("RelationshipName = " + RelationshipName2);

                EXERelationshipLink eXERelationshipLink2 = new EXERelationshipLink(RelationshipName2, ClassName2);
                eXERelationshipSelection.AddRelationshipLink(eXERelationshipLink2);
                i += 3;
            }

            Console.WriteLine("Where 3= " + context.GetChild(context.ChildCount - 3).GetText());
            if (context.GetChild(context.ChildCount - 3).GetText().Contains("where"))
            {
                Visit(context.GetChild(context.ChildCount - 2));
                WhereExpression = stackEXEASTNode.Peek();
            }
            stackEXEScope.Peek().AddCommand(new EXECommandQuerySelectRelatedBy(Cardinality, VariableName, WhereExpression, eXERelationshipSelection));

            stackEXEASTNode.Clear();

            return null;
            //return base.VisitExeCommandQuerySelectRelatedBy(context);
        }

        public override object VisitExeCommandCall([NotNull] OALParser.ExeCommandCallContext context)
        {

            /*String CallerClass = ParseUtil.StripWhiteSpace(context.GetChild(1).GetText());
            String CallerMethod = ParseUtil.StripWhiteSpace(context.GetChild(3).GetText());

            String RelationshipName = context.GetChild(9).GetText().Contains("across") ? ParseUtil.StripWhiteSpace(context.GetChild(10).GetText()) : null;

            String CalledClass = ParseUtil.StripWhiteSpace(context.GetChild(5).GetText());
            String CalledMethod =  ParseUtil.StripWhiteSpace(context.GetChild(7).GetText());

            stackEXEScope.Peek().AddCommand(new EXECommandCall(CallerClass, CallerMethod, RelationshipName, CalledClass, CalledMethod));

            return null;
            //return base.VisitExeCommandCall(context);*/
			
			Visit(context.GetChild(0));
            String InstanceName = this.instanceName;
			String AttributeName = this.attributeName;
			String MethodName = ParseUtil.StripWhiteSpace(context.GetChild(2).GetText());
			
			List<EXEASTNode> Parameters = new List<EXEASTNode>();
			
			if (context.GetChild(3).GetText().Equals("("))  
			{
				for (int i = 4; i < context.ChildCount - 2; i++)
				{
					if (context.GetChild(i).GetType().Name.Contains("ExprContext"))
					{
						Visit(context.GetChild(i));
						EXEASTNode parameter = stackEXEASTNode.Peek();
						Parameters.Add(parameter);
						
						stackEXEASTNode.Clear();
					}
				}
			}

            stackEXEScope.Peek().AddCommand(new EXECommandCall(InstanceName, AttributeName, MethodName, Parameters));
			
			return null;
            //return base.VisitExeCommandCall(context);
        }

        public override object VisitBreakCommand([NotNull] OALParser.BreakCommandContext context)
        {
            stackEXEScope.Peek().AddCommand(new EXECommandBreak());
            
            return null;
            //return base.VisitBreakCommand(context);
        }

        public override object VisitContinueCommand([NotNull] OALParser.ContinueCommandContext context)
        {
            stackEXEScope.Peek().AddCommand(new EXECommandContinue());

            return null;
            //return base.VisitContinueCommand(context);
        }

        public override object VisitWhileCommand([NotNull] OALParser.WhileCommandContext context)
        {
            Visit(context.GetChild(2));

            EXEScopeLoopWhile EXEScopeLoopWhile = new EXEScopeLoopWhile(stackEXEASTNode.Peek());
            stackEXEASTNode.Clear();
            stackEXEScope.Push(EXEScopeLoopWhile);

            for (int i = 4; i < context.ChildCount; i++)
            {
                if (context.GetChild(i).GetType().Name.Contains("LineContext"))
                {
                    Visit(context.GetChild(i));
                }
                else if (context.GetChild(i).GetText().Contains("end while"))
                {
                    EXEScope temp = stackEXEScope.Pop();
                    stackEXEScope.Peek().AddCommand(temp);
                }
                
            }
            return null;
            //return base.VisitWhileCommand(context);
        }

        public override object VisitForeachCommand([NotNull] OALParser.ForeachCommandContext context)
        {
            String Iterator = context.GetChild(1).GetText();
			Visit(context.GetChild(3));
            String Iterable = this.instanceName;

            stackEXEScope.Push(new EXEScopeForEach(Iterator, Iterable));

            for (int i = 4; i < context.ChildCount; i++)
            {
                if (context.GetChild(i).GetType().Name.Contains("LineContext"))
                {
                    Visit(context.GetChild(i));
                }
                else if (context.GetChild(i).GetText().Contains("end for"))
                {
                    EXEScope temp = stackEXEScope.Pop();
                    stackEXEScope.Peek().AddCommand(temp);
                }

            }
            return null;
            //return base.VisitForeachCommand(context);
        }

        public override object VisitParCommand([NotNull] OALParser.ParCommandContext context)
        {
            stackEXEScope.Push(new EXEScopeParallel());

            for (int i = 1; i < context.ChildCount; i++)
            {
                Console.WriteLine(i + " -> " + context.GetChild(i).GetText() + " --- " + context.GetChild(i).GetType().Name);
                if (context.GetChild(i).GetText().Equals("thread"))
                {
                    stackEXEScope.Push(new EXEScope());
                }
                else if (context.GetChild(i).GetType().Name.Contains("LineContext"))
                {
                    Visit(context.GetChild(i));
                }
                else if (context.GetChild(i).GetText().Equals("end thread"))
                {
                    EXEScope temp = stackEXEScope.Pop();
                    ((EXEScopeParallel)stackEXEScope.Peek()).AddThread(temp);
                }
                else if (context.GetChild(i).GetText().Equals("end par"))
                {
                    EXEScope temp = stackEXEScope.Pop();
                    stackEXEScope.Peek().AddCommand(temp);
                }
            }

            return null;
            //return base.VisitParCommand(context);
        }

        public override object VisitIfCommand([NotNull] OALParser.IfCommandContext context)
        {
            Visit(context.GetChild(1));

            EXEScopeCondition EXEScopeCondition = new EXEScopeCondition(stackEXEASTNode.Peek());
            stackEXEASTNode.Clear();
            stackEXEScope.Push(EXEScopeCondition);

            Boolean els = false;
            Boolean elif = false;

            for (int i = 2; i < context.ChildCount; i++)
            {
                if (context.GetChild(i).GetType().Name.Contains("LineContext"))
                {
                    Console.WriteLine(i + " -> " + context.GetChild(i).GetText());
                    Visit(context.GetChild(i));
                }
                else if (context.GetChild(i).GetText().Equals("else"))
                {
                    if (elif)
                    {
                        EXEScopeCondition temp3 = ((EXEScopeCondition)stackEXEScope.Pop());
                        ((EXEScopeCondition)stackEXEScope.Peek()).AddElifScope(temp3);
                    }

                    els = true;
                    EXEScope temp2 = new EXEScope();
                    ((EXEScopeCondition)stackEXEScope.Peek()).ElseScope = temp2;

                    EXEScope temp = stackEXEScope.Pop();
                    stackEXEScope.Peek().AddCommand(temp);
                    stackEXEScope.Push(temp2);

                }
                else if (context.GetChild(i).GetText().Contains("elif"))
                {
                    if (elif)
                    {
                        EXEScopeCondition temp = ((EXEScopeCondition)stackEXEScope.Pop());
                        ((EXEScopeCondition)stackEXEScope.Peek()).AddElifScope(temp);
                    }

                    elif = true;
                    Console.WriteLine(i + " -> " + context.GetChild(i).GetText());
                    Console.WriteLine(i + "+ 2 -> " + context.GetChild(i + 2).GetText());
                    Visit(context.GetChild(i + 2));

                    EXEScopeCondition EXEScopeConditionELIF = new EXEScopeCondition(stackEXEASTNode.Peek());
                    stackEXEASTNode.Clear();
                    stackEXEScope.Push(EXEScopeConditionELIF);
                }
                else if (context.GetChild(i).GetText().Contains("end if"))
                {
                    Console.WriteLine(i + "-> " + context.GetChild(i).GetText());
                    EXEScope temp = stackEXEScope.Pop();

                    if (!els && !elif)//TODO ako toto funguje? - napisal tvorca
                    {
                        stackEXEScope.Peek().AddCommand(temp);
                    }

                    if (elif && !els)
                    {
                        ((EXEScopeCondition) stackEXEScope.Peek()).AddElifScope((EXEScopeCondition) temp);
                        EXEScopeCondition temp2 = ((EXEScopeCondition) stackEXEScope.Pop());
                        stackEXEScope.Peek().AddCommand(temp2);
                    }
                }
            }
            return null;
            //return base.VisitIfCommnad(context);
        }
		
		public override object VisitExeCommandCreateList([NotNull] OALParser.ExeCommandCreateListContext context)
        {
			return null;
		}
		
		public override object VisitExeCommandAddingToList([NotNull] OALParser.ExeCommandAddingToListContext context) 
        {
			return null;
		}
		
		public override object VisitExeCommandWrite([NotNull] OALParser.ExeCommandWriteContext context)
        {
			return null;
		}
		
		public override object VisitExeCommandRead([NotNull] OALParser.ExeCommandReadContext context)
        {
			return null;
		}
		
		public override object VisitInstanceHandle([NotNull] OALParser.InstanceHandleContext context)
		{
			this.instanceName = context.GetChild(0).GetText();
			this.attributeName = null;
			
			if (context.ChildCount == 3)
            {
                this.attributeName = context.GetChild(2).GetText();
            }
            return null;
            //return base.VisitInstanceHandle(context);
        }
		
		
    }
}