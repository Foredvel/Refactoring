using PersonalFinanceCli.Application.Repositories;
using PersonalFinanceCli.Domain.Entities;
using PersonalFinanceCli.Domain.ValueObjects;
using static Validation.Utility.ValidationOperation;

namespace PersonalFinanceCli.Application.CommandHandlers;

public sealed class AddCardHandler
{
    private readonly ICardRepository _cardRepository;

    public AddCardHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public Card Handle(string name, string currencyRaw, decimal? initialBalance)
    {

        ErrorCatcher(string.IsNullOrWhiteSpace(name), Error.CardNameCannotBeEmpty);

        ErrorCatcher(!Enum.TryParse<Currency>(currencyRaw, true, out var currency), Error.UnknownCurrencyAllowedRUBEUR);

        var card = new Card
        {
            Name = name,
            Currency = currency,
            InitialBalance = initialBalance ?? 0m,
            IsDefault = _cardRepository.GetAll().Count == 0
        };

        return _cardRepository.Add(card);
    }
}
