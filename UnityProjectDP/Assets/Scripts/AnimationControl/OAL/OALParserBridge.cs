using AnimationControl.OAL;
using Antlr4.Runtime;
using OALProgramControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.AnimationControl.OAL
{
    class OALParserBridge
    {
        public static EXEScope Parse(String Code)
        {
            OALParser parser = null;
            try
            {
                ICharStream target = new AntlrInputStream(Code);
                ITokenSource lexer = new OALLexer(target);
                ITokenStream tokens = new CommonTokenStream(lexer);
                parser = new OALParser(tokens);
                parser.BuildParseTree = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }

            //ExprParser.LiteralContext result = parser.literal();
            OALParser.LinesContext result = parser.lines();
            Console.Write(result.ToStringTree());
            Console.WriteLine();

            OALVisitor2 test = new OALVisitor2();

            try
            {
                test.VisitLines(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }

            return test.globalExeScope;
        }
    }
}
