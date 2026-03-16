using PersonalFinanceCli.Application.Repositories;
using static Validation.Utility.ValidationOperation;

namespace PersonalFinanceCli.Application.CommandHandlers;

public sealed class SetDefaultCardHandler
{
    private readonly ICardRepository _cardRepository;

    public SetDefaultCardHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public void Handle(int cardId)
    {
        var card = _cardRepository.GetById(cardId);
        ErrorCatcher(card is null, Error.CardNotFound);

        _cardRepository.SetDefault(cardId);
    }
}
