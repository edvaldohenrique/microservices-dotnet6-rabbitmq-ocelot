using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.RabbitMQSender;
using GeekShopping.OrderAPI.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {
        private readonly OrderRepository _orderRepository;
        private IConnection _connection;
        private IModel _channel;
        private IRabbitMQMessageSender _rabbitMQMessageSender;

        public RabbitMQCheckoutConsumer(OrderRepository orderRepository, IRabbitMQMessageSender rabbitMQMessageSender)
        {
            _orderRepository = orderRepository;
            _rabbitMQMessageSender = rabbitMQMessageSender;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "checkouQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (channel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                CheckoutHeaderVO vo = JsonSerializer.Deserialize<CheckoutHeaderVO>(content);
                ProcessOrder(vo).GetAwaiter().GetResult();
                _channel.BasicAck(evt.DeliveryTag, false);
            };

            _channel.BasicConsume("checkouQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessOrder(CheckoutHeaderVO vo)
        {
            OrderHeader order = new()
            {
                UserID = vo.UserID,
                FirstName = vo.FirstName,
                LastName = vo.LastName,
                OrderDetails = new List<OrderDetail>(),
                CardNumber = vo.CardNumber,
                CVV = vo.CVV,
                CouponCode = vo.CouponCode,
                DateTime = vo.DateTime,
                Email = vo.Email,
                ExpiryMonthYear = vo.ExpiryMountYear,
                PurchaseAmount = vo.PurchaseAmount,
                Phone = vo.Phone,
                DiscountTotal = vo.DiscountTotal,
                CartTotalItens = vo.CartTotalItens,
                OrderTime = DateTime.Now,
                PaymentStatus = false
            };

            foreach (var detail in vo.CartDetails)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = detail.ProductId,
                    ProductName = detail.Product.Name,
                    Price = detail.Product.Price,
                    Count = detail.Count
                };

                order.CartTotalItens += detail.Count;
                order.OrderDetails.Add(orderDetail);
            }

            await _orderRepository.AddOrder(order);

            PaymentVO payment = new PaymentVO()
            {
                Name = order.FirstName+" "+order.LastName,
                CardNumber = order.CardNumber,
                Email = order.Email,
                CVV = order.CVV,
                PurchaseAmount = order.PurchaseAmount,
                ExpiryMonthYear = order.ExpiryMonthYear,
                OrderId = order.Id
            };

            try
            {
                _rabbitMQMessageSender.SendMessage(payment, "orderPaymentProcessQueue");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
