using System;


namespace Validation.Utility
{
    public static class ValidationOperation
    {
        public enum Error
        {
            /*Card name cannot be empty,
            Unknown currency. Allowed: RUB, EUR,
            Amount must be > 0,
            Category cannot be empty,
            Card not found,
            No cards available,
            Limit must be > 0.,
            Cannot set limit without cards.,
            Command is empty.,
            Unknown command.,
            Card command is incomplete.,
            card add requires: card add \"name\" <currency> [initialBalance].,
            Invalid initialBalance.,
            card set-default requires cardId.,
            Unknown card command.,
            Transaction command is incomplete.,
            Only add is supported for transactions.,
            Invalid amount.,
            Invalid --card value.,
            Invalid --date value. Use YYYY-MM-DD.,
            Invalid --note value.,
            Limit command is incomplete.,
            Unknown limit command.
            */

            UnknownCurrencyAllowedRUBEUR,
            AmountMustBePositive,
            CategoryCannotBeEmpty,
            CardNotFound,
            NoCardsAvailable,
            LimitMustBePositive,
            CannotSetLimitWithoutCards,
            CardNameCannotBeEmpty,
            CommandIsEmpty,
            UnknownCommand,
            CardCommandIsIncomplete,
            CardAddRequires,
            InvalidInitialBalance,
            CardSetDefault,
            UnknownCardCommand,
            TransactionCommandIsIncomplete,
            OnlyAddIsSupportedForTransactions,
            InvalidAmount,
            InvalidÑardValue,
            InvalidDateValue,
            InvalidNoteValue,
            UnknownOption,
            LimitCommandIsIncomplete,
            limitSetRequiresAmount,
            UnknownLimitCommand,
            ReportDayIsTheOnlySupportedReportCommand,

        }

        public static void ErrorCatcher(bool condition, Error error)
        {
            if (!condition)
            {
                throw new InvalidOperationException(error switch
                {
                    Error.UnknownCurrencyAllowedRUBEUR => "Unknown currency. Allowed: RUB, EUR",
                    Error.AmountMustBePositive => "Amount must be > 0",
                    Error.CategoryCannotBeEmpty => "Category cannot be empty",
                    Error.CardNotFound => "Card not found",
                    Error.NoCardsAvailable => "No cards available",
                    Error.LimitMustBePositive => "Limit must be > 0.",
                    Error.CannotSetLimitWithoutCards => "Cannot set limit without cards.",
                    Error.CommandIsEmpty => "Command is empty.",
                    Error.UnknownCommand => "Unknown command.",
                    Error.CardCommandIsIncomplete => "Card command is incomplete.",
                    Error.CardAddRequires => "card add requires: card add \"name\" <currency> [initialBalance].",
                    Error.InvalidInitialBalance => "Invalid initialBalance.",
                    Error.CardSetDefault => "card set-default requires cardId.",
                    Error.UnknownCardCommand => "Unknown card command.",
                    Error.TransactionCommandIsIncomplete => "Transaction command is incomplete.",
                    Error.OnlyAddIsSupportedForTransactions => "Only add is supported for transactions.",
                    Error.InvalidAmount => "Invalid amount.",
                    Error.InvalidÑardValue => "Invalid --card value.",
                    Error.InvalidDateValue => "Invalid --date value. Use YYYY-MM-DD.",
                    Error.InvalidNoteValue => "Invalid --note value.",
                    Error.LimitCommandIsIncomplete => "Limit command is incomplete.",
                    Error.limitSetRequiresAmount => "limit set requires amount.",
                    Error.UnknownLimitCommand => "Unknown limit command.",
                    Error.ReportDayIsTheOnlySupportedReportCommand => "report day is the only supported report command.",
                    _ => $"Unknown option {error}."
                });
            }
        }
    }

}

