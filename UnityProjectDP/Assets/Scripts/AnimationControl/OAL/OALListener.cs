//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.9.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from OAL.g4 by ANTLR 4.9.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="OALParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.2")]
[System.CLSCompliant(false)]
public interface IOALListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.lines"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLines([NotNull] OALParser.LinesContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.lines"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLines([NotNull] OALParser.LinesContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLine([NotNull] OALParser.LineContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLine([NotNull] OALParser.LineContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.parCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParCommand([NotNull] OALParser.ParCommandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.parCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParCommand([NotNull] OALParser.ParCommandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.ifCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfCommand([NotNull] OALParser.IfCommandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.ifCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfCommand([NotNull] OALParser.IfCommandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.whileCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileCommand([NotNull] OALParser.WhileCommandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.whileCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileCommand([NotNull] OALParser.WhileCommandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.foreachCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForeachCommand([NotNull] OALParser.ForeachCommandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.foreachCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForeachCommand([NotNull] OALParser.ForeachCommandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.continueCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterContinueCommand([NotNull] OALParser.ContinueCommandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.continueCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitContinueCommand([NotNull] OALParser.ContinueCommandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.breakCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBreakCommand([NotNull] OALParser.BreakCommandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.breakCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBreakCommand([NotNull] OALParser.BreakCommandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.commentCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCommentCommand([NotNull] OALParser.CommentCommandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.commentCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCommentCommand([NotNull] OALParser.CommentCommandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandQueryCreate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandQueryCreate([NotNull] OALParser.ExeCommandQueryCreateContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandQueryCreate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandQueryCreate([NotNull] OALParser.ExeCommandQueryCreateContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandQueryRelate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandQueryRelate([NotNull] OALParser.ExeCommandQueryRelateContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandQueryRelate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandQueryRelate([NotNull] OALParser.ExeCommandQueryRelateContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandQuerySelect"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandQuerySelect([NotNull] OALParser.ExeCommandQuerySelectContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandQuerySelect"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandQuerySelect([NotNull] OALParser.ExeCommandQuerySelectContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandQuerySelectRelatedBy"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandQuerySelectRelatedBy([NotNull] OALParser.ExeCommandQuerySelectRelatedByContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandQuerySelectRelatedBy"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandQuerySelectRelatedBy([NotNull] OALParser.ExeCommandQuerySelectRelatedByContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandQueryDelete"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandQueryDelete([NotNull] OALParser.ExeCommandQueryDeleteContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandQueryDelete"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandQueryDelete([NotNull] OALParser.ExeCommandQueryDeleteContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandQueryUnrelate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandQueryUnrelate([NotNull] OALParser.ExeCommandQueryUnrelateContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandQueryUnrelate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandQueryUnrelate([NotNull] OALParser.ExeCommandQueryUnrelateContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandAssignment([NotNull] OALParser.ExeCommandAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandAssignment([NotNull] OALParser.ExeCommandAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandCall([NotNull] OALParser.ExeCommandCallContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandCall([NotNull] OALParser.ExeCommandCallContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandCreateList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandCreateList([NotNull] OALParser.ExeCommandCreateListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandCreateList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandCreateList([NotNull] OALParser.ExeCommandCreateListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandAddingToList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandAddingToList([NotNull] OALParser.ExeCommandAddingToListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandAddingToList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandAddingToList([NotNull] OALParser.ExeCommandAddingToListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandRemovingFromList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandRemovingFromList([NotNull] OALParser.ExeCommandRemovingFromListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandRemovingFromList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandRemovingFromList([NotNull] OALParser.ExeCommandRemovingFromListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandWrite"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandWrite([NotNull] OALParser.ExeCommandWriteContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandWrite"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandWrite([NotNull] OALParser.ExeCommandWriteContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.exeCommandRead"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExeCommandRead([NotNull] OALParser.ExeCommandReadContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.exeCommandRead"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExeCommandRead([NotNull] OALParser.ExeCommandReadContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.returnCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturnCommand([NotNull] OALParser.ReturnCommandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.returnCommand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturnCommand([NotNull] OALParser.ReturnCommandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr([NotNull] OALParser.ExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr([NotNull] OALParser.ExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.instanceHandle"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInstanceHandle([NotNull] OALParser.InstanceHandleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.instanceHandle"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInstanceHandle([NotNull] OALParser.InstanceHandleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.instanceName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInstanceName([NotNull] OALParser.InstanceNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.instanceName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInstanceName([NotNull] OALParser.InstanceNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.keyLetter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterKeyLetter([NotNull] OALParser.KeyLetterContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.keyLetter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitKeyLetter([NotNull] OALParser.KeyLetterContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.whereExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhereExpression([NotNull] OALParser.WhereExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.whereExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhereExpression([NotNull] OALParser.WhereExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.className"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassName([NotNull] OALParser.ClassNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.className"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassName([NotNull] OALParser.ClassNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.variableName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariableName([NotNull] OALParser.VariableNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.variableName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariableName([NotNull] OALParser.VariableNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.methodName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMethodName([NotNull] OALParser.MethodNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.methodName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMethodName([NotNull] OALParser.MethodNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAttribute([NotNull] OALParser.AttributeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAttribute([NotNull] OALParser.AttributeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.relationshipLink"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRelationshipLink([NotNull] OALParser.RelationshipLinkContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.relationshipLink"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRelationshipLink([NotNull] OALParser.RelationshipLinkContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="OALParser.relationshipSpecification"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRelationshipSpecification([NotNull] OALParser.RelationshipSpecificationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="OALParser.relationshipSpecification"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRelationshipSpecification([NotNull] OALParser.RelationshipSpecificationContext context);
}
