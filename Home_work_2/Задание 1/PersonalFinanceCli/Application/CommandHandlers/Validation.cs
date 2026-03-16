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
            Cannot set limit without cards.,*/

            UnknownCurrencyAllowedRUBEUR,
            AmountMustBePositive,
            CategoryCannotBeEmpty,
            CardNotFound,
            NoCardsAvailable,
            LimitMustBePositive,
            CannotSetLimitWithoutCards,
            CardNameCannotBeEmpty
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
                    Error.CardNameCannotBeEmpty => "Card name cannot be empty",
                    _ => "Unknown error code"

                });
            }
        }
    }

}

