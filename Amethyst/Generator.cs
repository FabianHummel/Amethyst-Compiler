// using Amethyst.utility;
//
// namespace Amethyst;
//
// public class CodeStringAttribute : Attribute
// {
//     public string Code { get; }
//
//     public CodeStringAttribute(string code)
//     {
//         Code = code;
//     }
//
//     public override string ToString()
//     {
//         return Code;
//     }
// }
//
// public static class Generator
// {
//     public static void Generate(IEnumerable<AstNode> nodes, string rootNamespace, string outDir)
//     {
//         var context = new GenerationContext
//         {
//             RootNamespace = rootNamespace,
//             CurrentNamespace = "",
//             OutDir = outDir + "/data/" + rootNamespace + "/functions"
//         };
//         Project.CreateInitializationFunction(outDir, context);
//         
//         foreach (var node in nodes)
//         {
//             node.GenerateCode(context, out _, "_out");
//         }
//
//         Project.CreateFunctionTags(outDir, context);
//     }
//     
//     public static void AddCommand(this GenerationContext context, string command)
//     {
//         if (context.CurrentFunction == null)
//         {
//             AddInitCommand(context, command);
//         }
//         else
//         {
//             var path = Path.Combine(context.OutDir, context.CurrentNamespace, context.CurrentFunction + ".mcfunction");
//             File.AppendAllText(path, command + "\n");
//         }
//     }
//     
//     public static void AddInitCommand(this GenerationContext context, string command)
//     {
//         var path = Path.Combine(context.OutDir, context.CurrentNamespace, "_amethyst_init.mcfunction");
//         if (!File.Exists(path))
//         {
//             context.LoadFunctions.Add(context.RootNamespace + ":" + Path.Combine(context.CurrentNamespace, "_amethyst_init"));
//         }
//         File.AppendAllText(path, command + "\n");
//     }
// }
//
// public partial interface AstNode
// {
//     void GenerateCode(GenerationContext context, out Subject? subject, string target);
// }
//
// public partial class Variable
// {
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//         
//         if (!context.Variables.Add(new VariableDefinition
//         {
//             Name = Name,
//             Type = Type
//         }))
//         {
//             throw new Exception($"Variable {Name} is already defined in the current scope.");
//         }
//         
//         var name = Path.Combine(context.CurrentNamespace, context.CurrentFunction ?? "", Name).Replace("/", ".");
//         
//         Value.GenerateCode(context, out var s, target);
//         switch (s)
//         { 
//             case Subject.Scoreboard:
//                 context.AddCommand($"scoreboard players operation {name} amethyst = {target} amethyst");
//                 break;
//             case Subject.Storage:
//                 context.AddCommand($"data modify storage amethyst:internal {name} set from storage amethyst:internal {target} value");
//                 break;
//         }
//     }
// }
//
// public partial class Assignment
// {
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//     }
// }
//
// public partial class Function
// {
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//         
//         context.Functions.Add(new FunctionDefinition
//         {
//             Name = context.RootNamespace + ":" + Path.Combine(context.CurrentNamespace, Name),
//             ReturnType = "void", // TODO: infer return type
//             Parameters = Arguments
//         });
//         
//         var path = Path.Combine(context.OutDir, context.CurrentNamespace, Name + ".mcfunction");
//         File.Create(path).Close();
//         
//         var currentFunction = context.CurrentFunction;
//         context.CurrentFunction = Name;
//         
//         Decorators.GenerateCode(context, out subject, target);
//         
//         foreach (var node in Body)
//         {
//             node.GenerateCode(context, out subject, target);
//         }
//         
//         context.CurrentFunction = currentFunction;
//     }
// }
//
// public partial class FunctionDecorators
// {
//    
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//         
//         var path = context.RootNamespace + ":" + Path.Combine(context.CurrentNamespace, context.CurrentFunction!);
//         
//         if (IsTicking)
//         {
//             context.TickFunctions.Add(path);
//         }
//
//         if (IsInitializing)
//         {
//             context.LoadFunctions.Add(path);
//         }
//     }
// }
//
// public abstract partial class Identifier
// {
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//     }
// }
//
// public abstract partial class Arguments
// {
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//     }
// }
//
// public abstract partial class Parameters
// {
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//     }
// }
//
// public abstract partial class Body
// {
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//     }
// }
//
// public partial class Namespace
// {
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//         
//         var dir = Path.Combine(context.OutDir, context.CurrentNamespace, Name);
//         Directory.CreateDirectory(dir);
//         
//         var @namespace = context.CurrentNamespace;
//         context.CurrentNamespace = Path.Combine(context.CurrentNamespace, Name);
//         
//         foreach (var node in Body)
//         {
//             node.GenerateCode(context, out subject, target);
//         }
//         
//         context.CurrentNamespace = @namespace;
//     }
// }
//
// public partial class FunctionCall
// {
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//         
//         var path = context.RootNamespace + ":" + Path.Combine(context.CurrentNamespace, Name);
//         context.AddCommand($"function {path}\n");
//     }
// }
//
// public partial class VariableReference
// {
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//     }
// }
//
//
// public partial class Constant
// {
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//         
//         var definition = context.GetConstant(this);
//         
//         if (definition == null)
//         {
//             context.AddConstant(this, out definition);
//             switch (Type)
//             {
//                 case DataType.Number:
//                 {
//                     context.AddInitCommand($"scoreboard players set {definition.Name} amethyst_const {Value}");
//                     subject = Subject.Scoreboard;
//                     break;
//                 }
//                 case DataType.String:
//                 {
//                     context.AddInitCommand($"data modify storage amethyst:constants {definition.Name} set value '{Value}'");
//                     subject = Subject.Storage;
//                     break;
//                 }
//             }
//         }
//         
//         switch (Type)
//         {
//             case DataType.Number:
//             {
//                 context.AddCommand($"scoreboard players operation {target} amethyst = {definition.Name} amethyst_const");
//                 subject = Subject.Scoreboard;
//                 break;
//             }
//             case DataType.String:
//             {
//                 context.AddCommand($"data modify storage amethyst:internal {target} set from storage amethyst:constants {definition.Name}");
//                 subject = Subject.Storage;
//                 break;
//             }
//         }
//     }
// }
//
// public partial class Operation
// {
//     public AstNode Calculate(GenerationContext context)
//     {
//         if (Left is Operation leftOp)
//         {
//             Left = leftOp.Calculate(context);
//         }
//         
//         if (Right is Operation rightOp)
//         {
//             Right = rightOp.Calculate(context);
//         }
//         
//         // Optimization: if both sides are constants, calculate the result and return a new constant
//         if (Left is Constant { Type: DataType.Number } leftNum && Right is Constant { Type: DataType.Number } rightNum)
//         {
//             return new Constant
//             {
//                 Type = DataType.Number,
//                 Value = Convert.ToString(Op switch
//                 {
//                     ArithmeticOperator.OP_ADD => int.Parse(leftNum.Value) + int.Parse(rightNum.Value),
//                     ArithmeticOperator.OP_SUB => int.Parse(leftNum.Value) - int.Parse(rightNum.Value),
//                     ArithmeticOperator.OP_MUL => int.Parse(leftNum.Value) * int.Parse(rightNum.Value),
//                     ArithmeticOperator.OP_DIV => int.Parse(leftNum.Value) / int.Parse(rightNum.Value),
//                     ArithmeticOperator.OP_MOD => int.Parse(leftNum.Value) % int.Parse(rightNum.Value),
//                     _ => throw new Exception($"Unsupported arithmetic operation '{Op}' between constants '{leftNum.Value}' and '{rightNum.Value}'")
//                 })
//             };
//         }
//         
//         if (Left is Constant leftConst && Right is Constant rightConst)
//         {
//             return Op switch
//             {
//                 ArithmeticOperator.OP_ADD => new Constant
//                 {
//                     Type = DataType.String,
//                     Value = leftConst.Value + rightConst.Value
//                 },
//                 _ => throw new Exception($"Unsupported arithmetic operation '{Op}' between constants '{leftConst.Value}' and '{rightConst.Value}'")
//             };
//         }
//
//         return this;
//     }
//     
//     public void GenerateCode(GenerationContext context, out Subject? subject, string target)
//     {
//         subject = null;
//         
//         var calculated = Calculate(context);
//
//         if (calculated is not Operation)
//         {
//             calculated.GenerateCode(context, out subject, target);
//             return;
//         }
//         
//         Left.GenerateCode(context, out var leftSubject, target);
//         Right.GenerateCode(context, out var rightSubject, "_tmp");
//         
//         if (leftSubject == Subject.Storage && rightSubject == Subject.Storage)
//         {
//             
//         }
//         else if (leftSubject == Subject.Scoreboard && rightSubject == Subject.Storage)
//         {
//             throw new Exception("Unsupported operation modifying scoreboard with a storage value");
//         }
//         else if (leftSubject == Subject.Storage && rightSubject == Subject.Scoreboard)
//         {
//             
//         }
//         else if (leftSubject == Subject.Scoreboard && rightSubject == Subject.Scoreboard)
//         {
//             var opString = Op.GetAttributeOfType<CodeStringAttribute>();
//             context.AddCommand($"scoreboard players operation {target} amethyst {opString} _tmp amethyst");
//             subject = Subject.Scoreboard;
//         }
//     }
// }