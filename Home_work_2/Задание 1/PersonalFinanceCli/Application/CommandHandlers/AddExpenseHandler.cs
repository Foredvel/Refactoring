using PersonalFinanceCli.Application.Repositories;
using PersonalFinanceCli.Domain.Entities;
using PersonalFinanceCli.Domain.ValueObjects;
using PersonalFinanceCli.Infrastructure.Time;
using static Validation.Utility.ValidationOperation;

namespace PersonalFinanceCli.Application.CommandHandlers;

public sealed class AddExpenseHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IClock _clock;

    public AddExpenseHandler(
        ITransactionRepository transactionRepository,
        ICardRepository cardRepository,
        IClock clock)
    {
        _transactionRepository = transactionRepository;
        _cardRepository = cardRepository;
        _clock = clock;
    }

    public Transaction Handle(decimal amount, string category, int? cardId, DateOnly? date, string? note)
    {
        ErrorCatcher(amount <= 0, Error.AmountMustBePositive);

        ErrorCatcher(string.IsNullOrWhiteSpace(category), Error.CategoryCannotBeEmpty);

        int resolvedCardId;
        if (cardId.HasValue)
        {
            var byId = _cardRepository.GetById(cardId.Value);
            ErrorCatcher(byId == null, Error.CardNotFound);

            resolvedCardId = byId.Id;
        }
        else
        {
            var defaultByStore = _cardRepository.GetDefaultByDataStore();
            if (defaultByStore != null)
            {
                resolvedCardId = defaultByStore.Id;
            }
            else
            {
                var first = _cardRepository.GetFirst();
                ErrorCatcher(first == null, Error.NoCardsAvailable);

                resolvedCardId = first.Id;
            }
        }

        var trx = new Transaction
        {
            CardId = resolvedCardId,
            Amount = amount,
            Category = category,
            Date = date ?? _clock.Today,
            Note = note,
            Type = TransactionType.Expense
        };

        return _transactionRepository.Add(trx);
    }
}
