using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

namespace AnimationControl.OAL
{
    public class OALToPythonVisitor : OALBaseVisitor<String>
    {
        private String tab;
        private List<String> attributesList;

        public OALToPythonVisitor(List<String> attributesList)
        {
            this.tab = "\t\t"; //mozno sa bude menit
            this.attributesList = attributesList; //mozno budeme potrebovat neskor
        }

        public override String VisitLines([NotNull] OALParser.LinesContext context)
        {
            int count = context.ChildCount - 1;
            String lines = "";

            for (int i = 0; i < count; i++)
            {
                lines += Visit(context.GetChild(i));
            }           
            
            return lines; 
        }

        public override String VisitLine([NotNull] OALParser.LineContext context)
        {
            String line = this.tab + VisitChildren(context) + Environment.NewLine;
            return line;
        }

        public override String VisitExeCommandQueryCreate([NotNull] OALParser.ExeCommandQueryCreateContext context)
        {//mozno by sme chceli doplnit do gramatiky aj parametre pre inicializaciu atributov

            if (context.GetChild(1).GetType().Name.Equals("InstanceHandleContext"))
            {
                String instanceName = Visit(context.GetChild(1));

                return instanceName + " = " + context.GetChild(3).GetText() + "()";
            }
            else if (context.GetChild(1).GetType().Name.Equals("KeyLetterContext"))
            {
                return context.GetChild(1).GetText() + "()";
            }

            return null;
        }

        public override String VisitExeCommandQueryRelate([NotNull] OALParser.ExeCommandQueryRelateContext context)
        {
            return base.VisitExeCommandQueryRelate(context);
        }

        public override String VisitExeCommandQuerySelect([NotNull] OALParser.ExeCommandQuerySelectContext context)
        {        
            String instanceName = Visit(context.GetChild(1));
            String keyLetter = context.GetChild(3).GetText();

            if (context.GetChild(0).GetText().Contains("many"))
            {
                if (context.GetChild(4).GetText().Contains("where"))
                {
                    String expr = Visit(context.GetChild(5));
                    return instanceName + " = [selected for selected in " + keyLetter + ".instances if " + expr + "]";
                }
                else
                {
                    return instanceName + " = " + keyLetter + ".instances";
                }
            }
            else if (context.GetChild(0).GetText().Contains("any"))
            {
                if (context.GetChild(4).GetText().Contains("where"))
                {
                    String expr = Visit(context.GetChild(5));
                    return instanceName + " = next((selected for selected in " + keyLetter + ".instances if " + expr + "), None)";  //berieme prvy nalez
                }
                else
                {
                    return instanceName + " = " + keyLetter + ".instances[0] if " + keyLetter + ".instances else None"; //berieme prvy prvok 
                }
            }
            
            return null;
        }

        public override String VisitExeCommandQuerySelectRelatedBy([NotNull] OALParser.ExeCommandQuerySelectRelatedByContext context)
        {
            return base.VisitExeCommandQuerySelectRelatedBy(context);
        }

        public override String VisitExeCommandQueryDelete([NotNull] OALParser.ExeCommandQueryDeleteContext context)
        {   //problem je s delete alebo "select any" pretoze nevieme pozistovat vsetky referencie na odstranenie
            String instanceName = Visit(context.GetChild(1));

            return "del " + instanceName;
        }

        public override String VisitExeCommandQueryUnrelate([NotNull] OALParser.ExeCommandQueryUnrelateContext context)
        {
            return base.VisitExeCommandQueryUnrelate(context);
        }

        public override String VisitExeCommandAssignment([NotNull] OALParser.ExeCommandAssignmentContext context)
        {   // nemusime pozerat ci je attribute lebo to robime v expression commande
            String expr;
            String instanceName;

            if (context.GetChild(0).GetText().Equals("assign "))
            {
                expr = Visit(context.GetChild(3));
                instanceName = Visit(context.GetChild(1));
            }
            else
            {
                expr = Visit(context.GetChild(2));
                instanceName = Visit(context.GetChild(0));
            }

            return instanceName + " = " + expr;
        }

        public override String VisitExeCommandCall([NotNull] OALParser.ExeCommandCallContext context)
        {
            string instanceName = Visit(context.GetChild(0));

            if (context.GetChild(3).GetText().Equals("(") && context.GetChild(4).GetText().Equals(")"))
            {
                return instanceName + "." + context.GetChild(2).GetText() + "()";
            }
            else
            {
                String parameters = "";

                for (int i = 4; i < context.ChildCount; i++)
                {
                    if (context.GetChild(i).GetType().Name.Equals("ExprContext"))
                    {
                        parameters += Visit(context.GetChild(i));
                    }
                    else if (context.GetChild(i).GetText().Equals(","))
                    {
                        parameters += ", ";
                    }
                    else if (context.GetChild(i).GetText().Equals(")"))
                    {
                        break;
                    }
                }

                return instanceName + "." + context.GetChild(2).GetText() + "(" + parameters + ")";
            }
        }

        public override String VisitExeCommandCreateList([NotNull] OALParser.ExeCommandCreateListContext context)
        {
            String listName = Visit(context.GetChild(1));
            
            if (context.GetChild(4).GetText().Equals("{"))
            {
                String items = "";
                for (int i = 5; i < context.ChildCount - 2; i++)
                {
                    if (context.GetChild(i).GetText().Equals(","))
                    {
                        items += ", ";
                    }
                    else if (context.GetChild(i).GetType().Name.Equals("InstanceHandleContext"))
                    {
                        items += Visit(context.GetChild(i));
                    }
                }

                return listName + " = [" + items + "]";
            }

            return listName + " = []";
        }

        public override String VisitExeCommandAddingToList([NotNull] OALParser.ExeCommandAddingToListContext context) 
        {
            String instanceName = Visit(context.GetChild(1));
            String listName = Visit(context.GetChild(3));

            return listName + ".append(" + instanceName + ")";
        }

        public override String VisitExeCommandRemovingFromList([NotNull] OALParser.ExeCommandRemovingFromListContext context)
        {
            String instanceName = Visit(context.GetChild(1));
            String listName = Visit(context.GetChild(3));

            return listName + " = [instance for instance in " + listName + " if instance != " + instanceName + "]";
        }

        public override String VisitExeCommandWrite([NotNull] OALParser.ExeCommandWriteContext context)
        {
            if (!context.GetChild(2).GetText().Equals(")"))
            {
                String arguments = "";

                for (int i = 2; i < context.ChildCount - 2; i++)
                {
                    if (context.GetChild(i).GetText().Equals(","))
                    {
                        arguments += ", ";
                    }
                    else
                    {
                        arguments += Visit(context.GetChild(i));
                    }
                }

                return "print(" + arguments + ", sep='')";
            }

            return "print()";
        }

        public override String VisitExeCommandRead([NotNull] OALParser.ExeCommandReadContext context)
        {
            String prompt = "";
            String variableName;

            if (context.GetChild(0).GetText().Equals("assign "))
            {
                variableName = Visit(context.GetChild(1));

                if (context.GetChild(4).GetType().Name.Equals("ExprContext"))
                {
                    prompt = Visit(context.GetChild(4));
                }

                if (context.GetChild(3).GetText().Contains("int"))
                {
                    return variableName + " = int(input(" + prompt + "))";
                }
                else if (context.GetChild(3).GetText().Contains("real"))
                {
                    return variableName + " = float(input(" + prompt + "))";
                }
                else if (context.GetChild(3).GetText().Contains("bool"))
                {
                    return variableName + " = boolean(input(" + prompt + "))";
                }
                else
                {
                    return variableName + " = input(" + prompt + ")";
                }
            }
            else
            {
                variableName = Visit(context.GetChild(0));

                if (context.GetChild(3).GetType().Name.Equals("ExprContext"))
                {
                    prompt = Visit(context.GetChild(3));
                }

                if (context.GetChild(2).GetText().Contains("int"))
                {
                    return variableName + " = int(input(" + prompt + "))";
                }
                else if (context.GetChild(2).GetText().Contains("real"))
                {
                    return variableName + " = float(input(" + prompt + "))";
                }
                else if (context.GetChild(2).GetText().Contains("bool"))
                {
                    return variableName + " = boolean(input(" + prompt + "))";
                }
                else
                {
                    return variableName + " = input(" + prompt + ")";
                }
            }
        }

        public override String VisitIfCommand([NotNull] OALParser.IfCommandContext context)
        {
            String expr = Visit(context.GetChild(1));
            String temp_tab = this.tab;
            this.tab += "\t";
            String lines = "";      //"if " + expr + ":" + Environment.NewLine;

            for (int i = 2; i < context.ChildCount; i++)
            {
                if (context.GetChild(i).GetType().Name.Equals("LineContext"))
                {
                    lines += Visit(context.GetChild(i));
                }
                else if (context.GetChild(i).GetText().Equals("elif"))
                {
                    //this.tab = temp_tab;
                    //temp_tab = this.tab;
                    lines += temp_tab + "elif (" + Visit(context.GetChild(i + 2)) + "):" + Environment.NewLine;
                   // this.tab += "\t";
                }
                else if (context.GetChild(i).GetText().Equals("else"))
                {
                    lines += temp_tab + "else:" + Environment.NewLine;
                }
                else if (context.GetChild(i).GetText().Equals("end if"))
                {
                    break;
                }
            }

            this.tab = temp_tab;
            //return lines;
            return "if " + expr + ":" + Environment.NewLine + lines;
        }

        public override String VisitWhileCommand([NotNull] OALParser.WhileCommandContext context)
        {
            String expr = Visit(context.GetChild(2));
            String temp_tab = this.tab;
            this.tab += "\t";
            String lines = "";

            for (int i = 4; i < context.ChildCount; i++)
            {
                if (context.GetChild(i).GetType().Name.Equals("LineContext"))
                {
                    lines += Visit(context.GetChild(i));
                }
                else if (context.GetChild(i).GetText().Equals("end while"))
                {
                    break;
                }
            }

            this.tab = temp_tab;
            return "while (" + expr + "):" + Environment.NewLine + lines;   
        }

        public override String VisitForeachCommand([NotNull] OALParser.ForeachCommandContext context)
        {
            String temp_tab = this.tab;
            this.tab += "\t";
            String lines = "";

            for (int i = 4; i < context.ChildCount; i++)
            {
                if (context.GetChild(i).GetType().Name.Equals("LineContext"))
                {
                    lines += Visit(context.GetChild(i));
                }
                else if (context.GetChild(i).GetText().Equals("end for"))
                {
                    break;
                }
            }

            this.tab = temp_tab;

            return "for " + context.GetChild(1).GetText() + " in " + Visit(context.GetChild(3)) + ":" + Environment.NewLine + lines;
        }

        public override String VisitReturnCommand([NotNull] OALParser.ReturnCommandContext context)
        {
            if (context.GetChild(1).GetType().Name.Equals("ExprContext"))
            {
                String expr = Visit(context.GetChild(1));

                return "return " + expr;
            }

            return "return";
        }

        public override String VisitContinueCommand([NotNull] OALParser.ContinueCommandContext context)
        {
            return "continue";
        }

        public override String VisitBreakCommand([NotNull] OALParser.BreakCommandContext context)
        {
            return "break";
        }

        public override String VisitParCommand([NotNull] OALParser.ParCommandContext context)
        {
            return base.VisitParCommand(context);
        }

        public override String VisitCommentCommand([NotNull] OALParser.CommentCommandContext context)
        {
            return context.GetText();
        }

        public override String VisitInstanceHandle([NotNull] OALParser.InstanceHandleContext context)
        {
            String instanceName = context.GetChild(0).GetText();

            /*if (this.attributesList.Contains(instanceName)) //mozno budeme potrebovat neskor
            {
                instanceName = "self." + instanceName;
            }*/

            if (context.ChildCount == 1)
            {
                return instanceName;
            }
            else
            {
                return instanceName + "." + context.GetChild(2).GetText();
            }
        }

        public override String VisitExpr([NotNull] OALParser.ExprContext context)
        {
            if (context.ChildCount == 1)
            {
                if (context.BOOL() != null)
                {
                    if (context.GetChild(0).GetText().Equals("TRUE"))
                    {
                        return "True";
                    }
                    else
                    {
                        return "False";
                    }
                }
                else if (context.GetChild(0).GetText().Equals("UNDEFINED"))
                {
                    return "None";
                }
                else
                {
                    /*if (this.attributesList.Contains(context.GetChild(0).GetText()))  //mozno budeme potrebovat neskor
                    {
                        return "self." + context.GetChild(0).GetText();
                    }*/

                    return context.GetChild(0).GetText();
                }
            }
            else if (context.ChildCount == 2)
            {
                if (context.GetChild(0).GetText().Equals("-"))
                {
                    String expr = Visit(context.GetChild(1));
                    return "-" + expr;
                }
                else if (context.GetChild(0).GetText().ToLower().Equals("not "))
                {
                    String expr = Visit(context.GetChild(1));
                    return "not " + expr;
                }
                else if (context.GetChild(0).GetText().Equals("cardinality "))
                {
                    return "cardinality(" + Visit(context.GetChild(1)) + ")";
                }
                else if (context.GetChild(0).GetText().Equals("empty "))
                {
                    return "not " + Visit(context.GetChild(1));
                }
                else if (context.GetChild(0).GetText().Equals("not_empty "))
                {
                    return Visit(context.GetChild(1));
                }
            }
            else if (context.ChildCount == 3)
            {
                if (context.GetChild(0).GetText().Equals("("))
                {
                    String expr = Visit(context.GetChild(1));
                    return "(" + expr + ")";
                }
                else if (!context.GetChild(1).GetText().Equals("."))
                {
                    String leftExpr = Visit(context.GetChild(0));
                    String rightExpr = Visit(context.GetChild(2));
                    return leftExpr + " " + context.GetChild(1).GetText().ToLower() + " " + rightExpr;
                }
                else
                {
                    /*if (this.attributesList.Contains(context.GetChild(0).GetText()))  //mozno budeme potrebovat neskor
                    {
                        return "self." + context.GetChild(0).GetText() + "." + context.GetChild(2).GetText();
                    }*/

                    return context.GetChild(0).GetText() + "." + context.GetChild(2).GetText();
                }
            }

            return null;
        }
    }
}
