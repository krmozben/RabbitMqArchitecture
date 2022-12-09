using MessageBroker.Configuration;
using MessageBroker.Enums;
using MessageBroker.Message;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MessageBroker.Bus;

public class BusSubcribe : IBusSubcribe
{
    private readonly BusConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;

    public BusSubcribe(IOptions<BusConfiguration> configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration.Value;
        _serviceProvider = serviceProvider;

        var factory = new ConnectionFactory()
        {
            HostName = _configuration.HostName,
            VirtualHost = _configuration.VHost,
            UserName = _configuration.UserName,
            Password = _configuration.Password,
            Port = _configuration.Port,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
    }

    public Task SubcribeAsync<TMessage>(
        string exchangeName,
        string queueName,
        string routingKey,
        ExchangeTypes exchangeType = ExchangeTypes.Unknown,
        ushort prefetchCount = 1,
        CancellationToken ct = default) where TMessage : IMessage
    {
        if (exchangeType == ExchangeTypes.Unknown)
        {
            throw new ArgumentNullException(nameof(exchangeName), $"'{nameof(exchangeName)}' cannot be null or empty.");
        }

        /// Her bir işlem için aynı connection üzerinden yeni bir kanal oluşturulur.
        var _channel = _connection.CreateModel();

        /// Exchange oluşturur
        /// durable: true ayarlanırsa exchange artık ram e değil disk e yazılır. Bu durumda rabbitmq restart vs.. olduğunda ilgili exchange silinmemiş olur.
        /// autoDelete : true ayarlanırsa bağlı olan son consumer da bağlantıyı keserse ilgili exchange silinir, false olursa exhange kalıcı hale gelmiş olur.
        _channel.ExchangeDeclare(exchangeName, exchangeType.ToString().ToLower(), durable: true, autoDelete: false);

        /// Queue oluşturur
        /// durable: true ayarlanırsa queue artık ram e değil disk e yazılır. Bu durumda rabbitmq restart vs.. olduğunda ilgili queue silinmemiş olur.
        /// autoDelete : true ayarlanırsa bağlı olan son consumer da bağlantıyı keserse ilgili queue silinir. false olursa queue kalıcı hale gelmiş olur.
        /// exclusive : true ayarlanırsa bu queue ya sadece bu kanaldan bağlantı açılabilir demek istemiş oluruz, false seçerek herhangi bir kanaldan bağlantıya izin vermiş oluyoruz.
        _channel.QueueDeclare(queueName, durable: true, autoDelete: false, exclusive: false);

        /// Bir queue yu exchange e bağlanmaya yarar
        _channel.QueueBind(queueName, exchangeName, routingKey);

        /// prefetchSize : mesaj boyutunu belirler, 0 olarak şeçerek herhangi bir boyutta mesaj kabul edebileceğini söylüyoruz.
        /// prefetchCount : her bir consumer a kaç adet mesaj bırakacağını belirler
        /// global : prefetchCount değerinin global olup olmamasını belirler. Yani; false durumunda prefetchCount 5 ise her dinleyen consumer aynı anda kuyruktan 5 er adet mesaj alır, true durumunda ise dinleyen her consumer toplamda 5 adet mesaj alabilir (örn: 2 consumer var ise 3 mesaj birine 2 mesaj birine gönderilir)
        _channel.BasicQos(0, prefetchCount, true);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (sender, ea) =>
        {
            var messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<TMessage>(messageBody);

            if (message == null)
                return;

            if (Guid.TryParse(ea.BasicProperties.MessageId, out Guid messageId))
            {
                message.MessageId = messageId;
            }

            if (await TryHandleAsync(message))
            {
                /// mesajı başarılı olarak işlediğimiz taktirde ilgili queue dan mesaj silinmesi için bildirimde bulunuyoruz
                /// multiple : true olması durumunda ram de işlenmiş ama message brokera bildirimi gitmemiş mesajlar varsa onlarıda gönderir, false diyerek sadece ilgili mesajın bildirimini yapmış oluyoruz.
                /// deliveryTag : mesaj tag ını belirler
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            else
            {
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        /// queue yu dinlemesi için consumer ı queue ya bağlar
        /// autoAck: mesaj queue dan alındıktan sonra otomatik olarak silinmemesi için false olarak işaretledik. Başarılı bir şekilde mesaj işlenirse BasicAck metodu ile manuel olarak queue dan  mesajı silmesi için bildirimde bulunuyoruz
        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task<bool> TryHandleAsync<TMessage>(TMessage message) where TMessage : IMessage
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<TMessage>>();
            await handler.HandleAsync(message);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
