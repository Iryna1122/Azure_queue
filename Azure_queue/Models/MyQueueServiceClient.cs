using Azure.Storage.Queues;
using Azure;
using Azure.Storage.Queues.Models;
using System.Text.Json;

namespace Azure_queue.Models
{
    public class MyQueueServiceClient
    {
       public static string connectionString = "DefaultEndpointsProtocol=https;AccountName=storagenewtoday;AccountKey=EsIIDEWtFs1cof9N51eavuB8bbGMx/7I6cbt+110RCJsoKHhuLwlSlP6DokDvuJ33nG2KOEPesnh+ASt/kFwBA==;EndpointSuffix=core.windows.net";
        public static string queuename = "myhomework";

        QueueServiceClient queueService;

        QueueClient queueClient;

        List<QueueMessage> messages = new List<QueueMessage>();

        public MyQueueServiceClient()
        {
            queueService = new QueueServiceClient(connectionString);
            try
            {
                queueClient = queueService.CreateQueue(queuename);
            }
            catch (Exception)
            {

                queueClient = queueService.GetQueueClient(queuename);
            }
            

           

        }

       
        public async Task AddMessageAsync(string message) //Add queue
        {

            queueClient = queueService.GetQueueClient(queuename);
            await queueClient.SendMessageAsync(message, timeToLive: TimeSpan.FromDays(5));
        }
        
        
       
        public async Task<List<object>> ShowAllAsync() //Show All Messages
        {
            List<object> mess = new List<object>();
            queueClient = queueService.GetQueueClient(queuename);

            foreach (PeekedMessage item in (await queueClient.PeekMessagesAsync(maxMessages: 15)).Value)
            //foreach (QueueMessage item in (await queueClient.ReceiveMessagesAsync(maxMessages: 10)).Value)
                {
                mess.Add(new { lot = JsonSerializer.Deserialize<Lot>(item.Body.ToString()), id = item.MessageId });
            }
            return mess;
        }
       
        public async Task<List<object>> ShowTarget(string id) //Show specific currency
        {
            List<object> messages = new List<object>();
            queueClient = queueService.GetQueueClient(queuename);


             foreach (PeekedMessage item in (await queueClient.PeekMessagesAsync(maxMessages: 10)).Value)
           // foreach (QueueMessage item in (await queueClient.ReceiveMessagesAsync(maxMessages: 10)).Value)
            {
                var tempLot = new { lot = JsonSerializer.Deserialize<Lot>(item.Body.ToString()), id = item.MessageId };
                if (tempLot.id == id)
                {
                    messages.Add(tempLot);
                }
            }
            return messages;
        }

        
        public async Task DeleteMessage(string? id)//Delete Lot
        {
            // await queueClient.DeleteMessageAsync(item.MessageId, item.PopReceipt);
            if (id != null)
            {
                var arr = (await queueClient.ReceiveMessagesAsync(maxMessages: 15, visibilityTimeout: TimeSpan.FromSeconds(2))).Value;
                var message = arr.Where(a => a.MessageId == id).First();
                await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            }
        }





        ////QueueServiceClient queueService { get; set; }
        //// QueueClient queueClient { get; set; }

        //public static async Task CreateQueueAsync()
        //{
        //    queueService = new QueueServiceClient(connectionString);

        //    QueueClient queue = await queueService.CreateQueueAsync(queueName);
        //}


        //static async Task DeleteQueueAsync(string connectionString, string queueName)
        //{
        //    MyQueueServiceClient serviceClient = new QueueServiceClient(connString);
        //    QueueClient queue = serviceClient.GetQueueClient(queueName);
        //    await queue.DeleteAsync();
        //}

        //static async Task SendMessageAsync(string connString, string queueName, string message,int TTL)
        //{
        //    MyQueueServiceClient serviceClient = new MyQueueServiceClient(connString);
        //    QueueClient queue = serviceClient.GetQueueClient(queueName);
        //    await queue.SendMessageAsync(message, timeToLive: TimeSpan.FromSeconds(TTL))
        //    Console.WriteLine($"Сообщение успешно отправлено в очередь {queueName}");
        //}

        //static async Task PeekMessagesAsync(string connString, string queueName)  //Данная функция асинхронно извлекает maxMessages сообщений из  очереди и выводит информацию о Id, тексте, количеству выведений из очереди для всех извлеченных сообщений:

        //{
        //    MyQueueServiceClient serviceClient = new MyQueueServiceClient(connString);
        //    QueueClient queue = serviceClient.GetQueueClient(queueName);
        //    foreach (PeekedMessage message in (await queue.PeekMessagesAsync(maxMessages: 10)).Value)
        //    {
        //        Console.WriteLine($"Id: {message.MessageId}");
        //        Console.WriteLine($"Text: {message.MessageText}");
        //        Console.WriteLine($"{message.DequeueCount}");
        //        Console.WriteLine("---------------");
        //    }
        //}

        //static async Task ReceiveMessageAsync(string connString, string queueName) //Чтобы асинхронно скачать ранее добавленные сообщения,необходимо вызвать метод ReceiveMessagesAsync
        //{
        //    MyQueueServiceClient serviceClient = new MyQueueServiceClient(connString);
        //    QueueClient queue = serviceClient.GetQueueClient(queueName);
        //    foreach (QueueMessage message in (await queue.ReceiveMessagesAsync(maxMessages: 10)).Value)
        //    {
        //        Console.WriteLine($"Id: {message.MessageId}");
        //        Console.WriteLine($"Text: {message.MessageText}");
        //        Console.WriteLine($"{message.DequeueCount}");
        //        Console.WriteLine("---------------");
        //    }
        //}

        //static async Task DeleteMessageAsync(string connString, string queueName)
        //{
        //    MyQueueServiceClient serviceClient = new MyQueueServiceClient(connString);
        //    QueueClient queue = serviceClient.GetQueueClient(queueName);
        //    foreach (QueueMessage message in (await queue.ReceiveMessagesAsync(maxMessages: 10)).Value)
        //    {
        //        Console.WriteLine($"Id: {message.MessageId}");
        //        Console.WriteLine($"Text: {message.MessageText}");
        //        Console.WriteLine($"{message.DequeueCount}");
        //        Console.WriteLine("---------------");
        //        //моделируем обработку сообщения
        //        await Task.Delay(2000);
        //        await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt);
        //    }
        //}

        //static async Task ReceiveAndUpdateAsync( conne, string queueName)
        //{
        //    QueueClient queue = new QueueClient(connectionString, queueName);
        //    await queue.SendMessageAsync("first");
        //    await queue.SendMessageAsync("second");
        //    await queue.SendMessageAsync("third");
        //    // получаем сообщения из очереди с коротким таймаутом видимости в 1 с
        //    List<QueueMessage> messages = new List<QueueMessage>();
        //    Response<QueueMessage[]> received = await queue.ReceiveMessagesAsync(10, visibilityTimeout: TimeSpan.FromSeconds(1));
        //    foreach (QueueMessage message in received.Value)
        //    {
        //        // Сообщаем сервису, что нам нужно немного больше времени на обработку сообщения,
        //        // указывая для него окно видимости в 5 с
        //        UpdateReceipt receipt = await queue.UpdateMessageAsync(
        //        message.MessageId,
        //        message.PopReceipt,
        //        message.MessageText,
        //        TimeSpan.FromSeconds(5));
        //        // Сохраняем сообщения для дальнейшего отслеживания
        //        messages.Add(message.Update(receipt));
        //    }
        //    // Ждем, пока исходное окно видимости в 1 с истечет и проверяем, что сообщения еще не видимы
        //    //для этого повторно инициируем получение сообщений
        //    await Task.Delay(TimeSpan.FromSeconds(1.5));
        //   // Assert.AreEqual(0, (await queue.ReceiveMessagesAsync(10)).Value.Length);
        //    // Заканчиваем обработку сообщений
        //    foreach (QueueMessage message in messages)
        //    {
        //        // "Обрабатываем" сообщения
        //        Console.WriteLine($"Message: {message.MessageText}");
        //        // Заканчиваем обработку сообщений, удаляя их и очереди
        //        await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt);
        //    }
        //}

    }
}
