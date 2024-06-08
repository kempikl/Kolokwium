using Microsoft.EntityFrameworkCore;
using Kolokwium.Data;
using Kolokwium.Dtos;
using Kolokwium.Models;

namespace Kolokwium.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClientDto> GetClientWithSubscriptions(int clientId)
        {
            var client = await _context.Clients
                .Include(c => c.Sales)
                .ThenInclude(s => s.Subscription)
                .ThenInclude(sub => sub.Payments)
                .FirstOrDefaultAsync(c => c.IdClient == clientId);

            if (client == null)
                throw new Exception("Client not found");

            var clientDto = new ClientDto
            {
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                Phone = client.Phone,
                Subscriptions = client.Sales.Select(s => new SubscriptionDto
                {
                    IdSubscription = s.Subscription.IdSubscription,
                    Name = s.Subscription.Name,
                    TotalPaidAmount = s.Subscription.Payments.Sum(p => p.Amount)
                }).ToList()
            };

            return clientDto;
        }

        public async Task<int> AddSubscriptionPayment(PaymentDto paymentDto)
        {
            var client = await _context.Clients.FindAsync(paymentDto.IdClient);
            if (client == null)
                throw new Exception("Client not found");

            var subscription = await _context.Subscriptions.FindAsync(paymentDto.IdSubscription);
            if (subscription == null)
                throw new Exception("Subscription not found");

            if (subscription.EndTime < DateTime.Now)
                throw new Exception("Subscription is not active");

            var existingPayment = await _context.Payments
                .Where(p => p.IdClient == paymentDto.IdClient && p.IdSubscription == paymentDto.IdSubscription && p.Date >= DateTime.Now.AddMonths(-subscription.RenewalPeriod))
                .FirstOrDefaultAsync();

            if (existingPayment != null)
                throw new Exception("Payment for this period already exists");

            var highestDiscount = await _context.Discounts
                .Where(d => d.IdSubscription == paymentDto.IdSubscription && d.DateFrom <= DateTime.Now && d.DateTo >= DateTime.Now)
                .OrderByDescending(d => d.Value)
                .FirstOrDefaultAsync();

            var finalAmount = subscription.Price;
            if (highestDiscount != null)
            {
                finalAmount -= finalAmount * highestDiscount.Value / 100;
            }

            if (finalAmount != paymentDto.Amount)
                throw new Exception("Incorrect payment amount");

            var payment = new Payment
            {
                Date = DateTime.Now,
                IdClient = paymentDto.IdClient,
                IdSubscription = paymentDto.IdSubscription,
                Amount = paymentDto.Amount
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return payment.IdPayment;
        }
    }
}
