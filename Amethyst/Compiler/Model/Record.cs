
namespace Amethyst.Model;

/// <summary>Represents a record definition within the Amethyst source code. Contextually very similar
/// to a <see cref="Variable" />.</summary>
public class Record : Symbol
{
    /// <summary>The record's name.</summary>
    public required string Name { get; init; }

    /// <summary>The record's datatype</summary>
    public required AbstractDatatype Datatype { get; init; }

    /// <summary>The initial value that the record should use for new entries. This value is only evaluated
    /// once at the time of declaring the record, not every time a new entry is created.</summary>
    /// <example><p><c>[assign_on_join]</c><br /><c>record coins = 10;</c></p><br />
    ///     <p>A player's coins is set to <c>10</c> every time they join the server. If the number changes
    ///     (when using a variable instead), it'll be used for new entries as well.</p>
    /// </example>
    public required IRuntimeValue? InitialValue { get; init; }

    /// <summary>A set of attributes linked to the record. For example, records with an
    /// <see cref="Constants.CustomRecordName" /> can be given custom styles using text components.</summary>
    public required HashSet<string> Attributes { get; init; }

    /// <summary>summary>The MCFunction path that is used to reference this record in the datapack code.</summary>
    public required string McFunctionPath { get; init; }
}