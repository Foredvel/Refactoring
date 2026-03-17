using PersonalFinanceCli.Domain.ValueObjects;
using System.Text.RegularExpressions;
using static Validation.Utility.ValidationOperation;

namespace PersonalFinanceCli.Presentation.Parsing;

public sealed class CommandParser
{
    private const string Card = "card";
    private const string Expense = "expense";
    private const string Income = "income";
    private const string Limit = "limit";
    private const string Report = "report";

    public ParsedCommand Parse(string[] args)
    {
        return Parse(args.ToList());
    }

    public ParsedCommand Parse(string line)
    {
        return Parse(Tokenizer.Tokenize(line));
    }

    private ParsedCommand Parse(IReadOnlyList<string> tokens)
    {

        ErrorCatcher(tokens.Count == 0, Error.CommandIsEmpty);

        var root = tokens[0].ToLowerInvariant();
        return root switch
        {
            Card => ParseCard(tokens),
            Expense => ParseTransaction(tokens, TransactionType.Expense),
            Income => ParseTransaction(tokens, TransactionType.Income),
            Limit => ParseLimit(tokens),
            Report => ParseReport(tokens),
            _ => throw new InvalidOperationException("Unknown command.")
        };
    }

    private static ParsedCommand ParseCard(IReadOnlyList<string> tokens)
    {
       ErrorCatcher(tokens.Count < 2, Error.CardCommandIsIncomplete);

        var action = tokens[1].ToLowerInvariant();
        switch (action)
        {
            case "add":
                {
                    ErrorCatcher(tokens.Count < 4, Error.CardAddRequires);

                    decimal? initial = null;
                    if (tokens.Count >= 5)
                    {
                        ErrorCatcher(!decimal.TryParse(tokens[4], out var value), Error.InvalidInitialBalance);


                        initial = value;
                    }

                    return new CardAddCommand(tokens[2], tokens[3], initial);
                }
            case "list":
                {
                    return new CardListCommand();
                }
            case "set-default":
                {
                    ErrorCatcher(tokens.Count < 3, Error.CardSetDefault);
                    ErrorCatcher(!int.TryParse(tokens[2], out var cardId), Error.CardSetDefault);
                    return new CardSetDefaultCommand(cardId);
                }
            default:
                {
                    throw new InvalidOperationException("Unknown command.");
                }
        }
    }

    private static ParsedCommand ParseTransaction(IReadOnlyList<string> tokens, TransactionType type)
    {
        ErrorCatcher(tokens.Count < 4, Error.TransactionCommandIsIncomplete);

        var action = tokens[1].ToLowerInvariant();
        ErrorCatcher(action != "add", Error.OnlyAddIsSupportedForTransactions);

        ErrorCatcher(!decimal.TryParse(tokens[2], out var amount), Error.InvalidAmount);

        var category = type == TransactionType.Expense ? tokens[3].Trim() : tokens[3];
        ErrorCatcher(type == TransactionType.Expense && category.Length == 0, Error.CategoryCannotBeEmpty);

        var options = ParseTransactionOptions(tokens, 4);
        return new TransactionAddCommand(
            type,
            amount,
            category,
            options.CardId,
            options.Date,
            options.Note);
    }

    private static (int? CardId, DateOnly? Date, string? Note) ParseTransactionOptions(IReadOnlyList<string> tokens, int startIndex)
    {
        int? cardId = null;
        DateOnly? date = null;
        string? note = null;

        var i = startIndex;
        while (i < tokens.Count)
        {

            var option = tokens[i];
            switch (option)
            {
                case "--card":
                    {
                        i++;

                        ErrorCatcher(i >= tokens.Count, Error.InvalidÑardValue);

                        var parsedCardId = ResolveCardFromArgs(tokens[i]);

                        ErrorCatcher(!parsedCardId.HasValue, Error.InvalidÑardValue);
                        cardId = parsedCardId;
                        break;
                    }
                case "--date":
                    {
                        i++;
                        ErrorCatcher(i >= tokens.Count, Error.InvalidDateValue);
                        ErrorCatcher(!DateOnly.TryParse(tokens[i], out var parsedDate), Error.InvalidDateValue);
                        date = parsedDate;
                        break;
                    }
                case "--note":
                    {
                        i++;
                        ErrorCatcher(i >= tokens.Count, Error.InvalidNoteValue);
                        note = tokens[i];
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException($"Unknown option {option}");
                    }
            }
        
            i++;
        }

        return (cardId, date, note);
    }

    public static int? ResolveCardFromArgs(string raw)
    {
        if (int.TryParse(raw, out var numericId))
        {
            return numericId;
        }

        if (Regex.IsMatch(raw, "^[0-9a-fA-F-]{36}$") && Guid.TryParse(raw, out var parsedGuid))
        {
            var tail = parsedGuid.ToString("N")[20..];
            if (int.TryParse(tail, out var fromGuid))
            {
                return fromGuid;
            }
        }

        return null;
    }

    private static ParsedCommand ParseLimit(IReadOnlyList<string> tokens)
    {
        ErrorCatcher(tokens.Count < 2, Error.LimitCommandIsIncomplete);

        var action = tokens[1].ToLowerInvariant();
        if (action == "set")
        {
            ErrorCatcher(tokens.Count < 3 , Error.limitSetRequiresAmount);
            ErrorCatcher(!decimal.TryParse(tokens[2], out var amount), Error.limitSetRequiresAmount);

            return new LimitSetCommand(amount);
        }

        if (action == "show")
        {
            return new LimitShowCommand();
        }

        throw new InvalidOperationException("Unknown limit command.");
    }

    private static ParsedCommand ParseReport(IReadOnlyList<string> tokens)
    {
        ErrorCatcher(OutOfMassive(tokens), Error.ReportDayIsTheOnlySupportedReportCommand);

        if (tokens.Count == 2)
        {
            return new ReportDayCommand(null);
        }

        DateOnly? date = null;
        var i = 2;
        while (i < tokens.Count)
        {
            var option = tokens[i];
            if (option == "--date")
            {
                i++;
                ErrorCatcher(i >= tokens.Count, Error.InvalidDateValue);
                ErrorCatcher(!DateOnly.TryParse(tokens[i], out var parsedDate), Error.InvalidDateValue);

                date = parsedDate;
            }
            else
            {
                throw new InvalidOperationException($"Unknown option {option}");
            }

            i++;
        }

        return new ReportDayCommand(date);
    }

    private static bool OutOfMassive(IReadOnlyList<string> tokens)
    {
        return tokens.Count < 2 || tokens[1].ToLowerInvariant() != "day";
    }
}

public abstract record ParsedCommand;

public sealed record CardAddCommand(string Name, string Currency, decimal? InitialBalance) : ParsedCommand;

public sealed record CardListCommand : ParsedCommand;

public sealed record CardSetDefaultCommand(int CardId) : ParsedCommand;

public sealed record TransactionAddCommand(
    TransactionType Type,
    decimal Amount,
    string Category,
    int? CardId,
    DateOnly? Date,
    string? Note) : ParsedCommand;

public sealed record LimitSetCommand(decimal Amount) : ParsedCommand;

public sealed record LimitShowCommand : ParsedCommand;

public sealed record ReportDayCommand(DateOnly? Date) : ParsedCommand;





