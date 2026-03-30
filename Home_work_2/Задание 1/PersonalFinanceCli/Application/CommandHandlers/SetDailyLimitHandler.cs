using PersonalFinanceCli.Application.Repositories;
using PersonalFinanceCli.Infrastructure.Time;
using static Validation.Utility.ValidationOperation;

namespace PersonalFinanceCli.Application.CommandHandlers;

public sealed class SetDailyLimitHandler
{
    private readonly ILimitRepository _limitRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IClock _clock;

    public SetDailyLimitHandler(ILimitRepository limitRepository, ICardRepository cardRepository, IClock clock)
    {
        _limitRepository = limitRepository;
        _cardRepository = cardRepository;
        _clock = clock;
    }

    public void Handle(decimal amount)
    {
        ErrorCatcher(amount <= 0, Error.LimitMustBePositive);

        var cards = _cardRepository.GetAll();
        ErrorCatcher(cards.Count == 0, Error.CannotSetLimitWithoutCards);

        var currency = _cardRepository.GetDefault()?.Currency
            ?? _cardRepository.GetFirst()?.Currency
            ?? cards[0].Currency;

        _limitRepository.Upsert(_clock.Today, amount, currency);
    }
}
