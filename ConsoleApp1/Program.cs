using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    DispatchConsumersAsync = true,
};

var _connection = factory.CreateConnection();

var _channel = _connection.CreateModel();




//_channel.ExchangeDeclare("test1","fanout",false,false);
//_channel.QueueDeclare("test", false,false,false);

//_channel.BasicPublish("test1", "", null, Encoding.UTF8.GetBytes("kerim"));


//_channel.QueueBind("test", "test1", "");

var consumer = new AsyncEventingBasicConsumer(_channel);
_channel.BasicConsume("test", autoAck: false, consumer: consumer);

consumer.Received += Consumer_Received;

 
async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
{
    throw new NotImplementedException();
}



Console.Read();