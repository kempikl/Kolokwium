using Kolokwium.Dtos;

namespace Kolokwium.Services;

public interface ISubscriptionService
{
    Task<ClientDto> GetClientWithSubscriptions(int clientId);
    Task<int> AddSubscriptionPayment(PaymentDto paymentDto);
}
