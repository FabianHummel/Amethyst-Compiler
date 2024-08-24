using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Exceptions;

namespace Tests;

public class AmethystDispatcher : CommandDispatcher<object?>
{
    public readonly Dictionary<string, Dictionary<string, int>> Scoreboard = new();
    
    public AmethystDispatcher()
    {
        Register(context => context.Literal("scoreboard")
            .Then(context => context.Literal("objectives")
                .Then(context => context.Literal("add")
                    .Then(context => context.Argument("objective", Arguments.Word())
                        .Then(context => context.Literal("dummy")
                            .Executes(context =>
                            {
                                Scoreboard.Add(context.GetArgument<string>("objective"), new Dictionary<string, int>());
                                return 1;
                            }))))
                
                .Then(context => context.Literal("remove")
                    .Then(context => context.Argument("objective", Arguments.Word())
                        .Executes(context =>
                        {
                            Scoreboard.Remove(context.GetArgument<string>("objective"));
                            return 1;
                        }))))
            
            .Then(context => context.Literal("players")
                .Then(context => context.Literal("set")
                    .Then(context => context.Argument("player", Arguments.Word())
                        .Then(context => context.Argument("objective", new ScoreboardObjectiveArgumentType(Scoreboard))
                            .Then(context => context.Argument("value", Arguments.Integer())
                                .Executes(context =>
                                {
                                    var objective = context.GetArgument<Dictionary<string, int>>("objective");
                                    objective[context.GetArgument<string>("player")] = context.GetArgument<int>("value");
                                    return 1;
                                })))))
                
                .Then(context => context.Literal("get")
                    .Then(context => context.Argument("player", Arguments.Word())
                        .Then(context => context.Argument("objective", new ScoreboardObjectiveArgumentType(Scoreboard))
                            .Executes(context =>
                            {
                                var objective = context.GetArgument<Dictionary<string, int>>("objective");
                                
                                if (!objective.TryGetValue(context.GetArgument<string>("player"), out var value))
                                {
                                    throw new SimpleCommandExceptionType(new LiteralMessage("Player not found")).Create();
                                }

                                return value;
                            }))))
                
                .Then(context => context.Literal("add")
                    .Then(context => context.Argument("player", Arguments.Word())
                        .Then(context => context.Argument("objective", new ScoreboardObjectiveArgumentType(Scoreboard))
                            .Then(context => context.Argument("value", Arguments.Integer())
                                .Executes(context =>
                                {
                                    var objective = context.GetArgument<Dictionary<string, int>>("objective");
                                    objective[context.GetArgument<string>("player")] = objective.TryGetValue(context.GetArgument<string>("player"), out var value)
                                        ? value + context.GetArgument<int>("value")
                                        : context.GetArgument<int>("value");
                                    return 1;
                                })))))
                
                .Then(context => context.Literal("remove")
                    .Then(context => context.Argument("player", Arguments.Word())
                        .Then(context => context.Argument("objective", new ScoreboardObjectiveArgumentType(Scoreboard))
                            .Then(context => context.Argument("value", Arguments.Integer())
                                .Executes(context =>
                                {
                                    var objective = context.GetArgument<Dictionary<string, int>>("objective");
                                    objective[context.GetArgument<string>("player")] = objective.TryGetValue(context.GetArgument<string>("player"), out var value)
                                        ? value - context.GetArgument<int>("value")
                                        : -context.GetArgument<int>("value");
                                    return 1;
                                })))))
                
                .Then(context => context.Literal("reset")
                    .Then(context => context.Argument("player", Arguments.Word())
                        .Then(context => context.Argument("objective", new ScoreboardObjectiveArgumentType(Scoreboard))
                            .Executes(context =>
                            {
                                var objective = context.GetArgument<Dictionary<string, int>>("objective");
                                objective.Remove(context.GetArgument<string>("player"));
                                return 1;
                            }))))
                
                .Then(context => context.Literal("operation")
                    .Then(context => context.Argument("targetPlayer", Arguments.Word())
                        .Then(context => context.Argument("targetObjective", new ScoreboardObjectiveArgumentType(Scoreboard))
                            .Then(context => context.Argument("operation", Arguments.String())
                                .Then(context => context.Argument("sourcePlayer", Arguments.Word())
                                    .Then(context => context.Argument("sourceObjective", new ScoreboardObjectiveArgumentType(Scoreboard))
                                        .Executes(context =>
                                        {
                                            var targetObjective = context.GetArgument<Dictionary<string, int>>("targetObjective");
                                            var sourceObjective = context.GetArgument<Dictionary<string, int>>("sourceObjective");
                                            var operation = context.GetArgument<string>("operation");
                                            
                                            if (!sourceObjective.TryGetValue(context.GetArgument<string>("sourcePlayer"), out var sourceValue))
                                            {
                                                throw new SimpleCommandExceptionType(new LiteralMessage("Source player not found")).Create();
                                            }

                                            if (!targetObjective.TryGetValue(context.GetArgument<string>("targetPlayer"), out var targetValue))
                                            {
                                                throw new SimpleCommandExceptionType(new LiteralMessage("Target player not found")).Create();
                                            }

                                            targetObjective[context.GetArgument<string>("targetPlayer")] = operation switch
                                            {
                                                "+=" => targetValue + sourceValue,
                                                "-=" => targetValue - sourceValue,
                                                "*=" => targetValue * sourceValue,
                                                "/=" => targetValue / sourceValue,
                                                "%=" => targetValue % sourceValue,
                                                "=" => sourceValue,
                                                _ => throw new SimpleCommandExceptionType(new LiteralMessage("Invalid operation")).Create()
                                            };

                                            return 1;
                                        })))))))));
    }
}