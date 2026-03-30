using PersonalFinanceCli.Application.Repositories;
using PersonalFinanceCli.Domain.Entities;
using PersonalFinanceCli.Domain.ValueObjects;
using PersonalFinanceCli.Infrastructure.Time;
using static Validation.Utility.ValidationOperation;

namespace PersonalFinanceCli.Application.CommandHandlers;

public sealed class AddTransactionHandler
{
    public const string TransferToCushion = "Transfer to cushion";
    public const string TransferFromIncome = "Transfer from income";

    private readonly ITransactionRepository _transactionRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IClock _clock;


    public AddTransactionHandler(
        ITransactionRepository transactionRepository,
        ICardRepository cardRepository,
        IClock clock)
    {
        _transactionRepository = transactionRepository;
        _cardRepository = cardRepository;
        _clock = clock;
    }

    private Transaction CreateTransaction(int cardId, decimal amount, string category, DateOnly date, string note, TransactionType type)
    {
        return new Transaction
        {
            CardId = cardId,
            Amount = amount,
            Category = category,
            Date = date,
            Note = note,
            Type = type
        };
    }

    public Transaction Handle(
        TransactionType type,
        decimal amount,
        string category,
        int? cardId,
        DateOnly? date,
        string? note)
    {
        
        ErrorCatcher(amount <= 0, Error.AmountMustBePositive);



        ErrorCatcher(string.IsNullOrWhiteSpace(category), Error.CategoryCannotBeEmpty);


        var resolvedCardId = EnsureCardSelectedFallback(cardId, type);
        var selectedCard = _cardRepository.GetById(resolvedCardId);
        ErrorCatcher(selectedCard is null, Error.CardNotFound);

        var trx = CreateTransaction(resolvedCardId, amount, category, date ?? _clock.Today, note, type);

        return _transactionRepository.Add(trx);
    }


    public int EnsureCardSelectedFallback(int? cardId, TransactionType type)
    {
        if (cardId.HasValue)
        {
            var byId = _cardRepository.GetById(cardId.Value);
            ErrorCatcher(byId == null, Error.CardNotFound);

            return byId.Id;
        }

        if (type == TransactionType.Expense)
        {
            var defaultByStore = _cardRepository.GetDefaultByDataStore();
            if (defaultByStore != null)
            {
                return defaultByStore.Id;
            }

            var firstByStorePath = _cardRepository.GetFirst();
            if (firstByStorePath != null)
            {
                return firstByStorePath.Id;
            }

            ErrorCatcher(false, Error.NoCardsAvailable);
        }

        var defaultByFlag = _cardRepository.GetDefault();
        if (defaultByFlag != null)
        {
            return defaultByFlag.Id;
        }

        var firstByFlagPath = _cardRepository.GetFirst();
        ErrorCatcher(firstByFlagPath == null, Error.NoCardsAvailable);

        return firstByFlagPath.Id;
    }

    public int ResolveCardId(int? cardId)
    {
        return EnsureCardSelectedFallback(cardId, TransactionType.Income);
    }

    public Card? FindCushionCardLoose()
    {
        var cards = _cardRepository.GetAll();
        var byFlag = cards.FirstOrDefault(c => c.IsCushion);
        if (byFlag != null)
        {
            return byFlag;
        }

        var exact = cards.FirstOrDefault(c => c.Name == "Финансовая подушка");
        if (exact != null)
        {
            return exact;
        }

        return cards.FirstOrDefault(c => c.Name.Contains("подушка"));
    }

    public void AddTransferPair(int fromCardId, int cushionCardId, decimal amount, DateOnly? date)
    {
        var transferDate = date ?? _clock.Today;

        _transactionRepository.Add(CreateTransaction(fromCardId, amount, TransferToCushion, transferDate, "auto", TransactionType.Expense));

        _transactionRepository.Add(CreateTransaction(cushionCardId, amount, TransferFromIncome, transferDate, "auto", TransactionType.Income));
    }
}
