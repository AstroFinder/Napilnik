using System;
using System.Linq;
using System.Security.Cryptography;

namespace Napilnik
{
    class Program
    {
        static void Main(string[] args)
        {
            //Выведите платёжные ссылки для трёх разных систем платежа: 
            //pay.system1.ru/order?amount=12000RUB&hash={MD5 хеш ID заказа}
            //order.system2.ru/pay?hash={MD5 хеш ID заказа + сумма заказа}
            //system3.com/pay?amount=12000&curency=RUB&hash={SHA-1 хеш сумма заказа + ID заказа + секретный ключ от системы}

            var order = new Order(15, 12000);

            var system1 = new System1(new MD5Hash());
            var system2 = new System2(new MD5Hash());
            var system3 = new System3(new SHA1Hash());

            Console.WriteLine(system1.GetPayingLink(order));
            Console.WriteLine(system2.GetPayingLink(order));
            Console.WriteLine(system3.GetPayingLink(order));
        }
    }

    public interface IHash
    {
        public string CreateHash(int input);
    }

    public interface IPaymentSystem
    {
        public string GetPayingLink(Order order);
    }

    public abstract class PaymentSystem : IPaymentSystem
    {
        protected readonly IHash _hash;

        protected PaymentSystem(IHash hash)
        {
            _hash = hash ?? throw new ArgumentNullException(nameof(hash));
        }

        public string GetPayingLink(Order order)
        {
            order = order ?? throw new ArgumentNullException(nameof(order));

            return CreateLink(order);
        }

        protected abstract string CreateLink(Order order);
    }

    public class System1 : PaymentSystem
    {
        private readonly string _host = "pay.system1.ru";

        public System1(IHash hash) : base(hash) { }

        protected override string CreateLink(Order order)
        {
            order = order ?? throw new ArgumentNullException(nameof(order));

            return _host + "/order?amount=" + order.Amount + "RUB&hash=" + _hash.CreateHash(order.Id);
        }
    }

    public class System2 : PaymentSystem
    {
        private readonly string _host = "order.system2.ru";

        public System2(IHash hash) : base(hash) { }

        protected override string CreateLink(Order order)
        {
            order = order ?? throw new ArgumentNullException(nameof(order));

            return _host + "/pay?hash=" + _hash.CreateHash(order.Id) + order.Amount;
        }
    }

    public class System3 : PaymentSystem
    {
        private readonly string _host = "system3.com";
        private readonly string _secretKey = "MFkwEwYHKoZIzj0C";
        public System3(IHash hash) : base(hash) { }

        protected override string CreateLink(Order order)
        {
            order = order ?? throw new ArgumentNullException(nameof(order));

            return _host + "/pay?amount=" + order.Amount + "&curency=" + "RUB&hash=" + _hash.CreateHash(order.Amount) + order.Id + _secretKey;
        }
    }

    public class MD5Hash : IHash
    {
        public string CreateHash(int input)
        {
            var md5 = MD5.Create();
            string hash = string.Concat(md5.ComputeHash(BitConverter
              .GetBytes(input))
              .Select(x => x.ToString("x2")));

            return hash;
        }
    }

    public class SHA1Hash : IHash
    {
        public string CreateHash(int input)
        {
            var sha1 = new SHA1Managed();
            string hash = string.Concat(sha1.ComputeHash(BitConverter
              .GetBytes(input))
              .Select(x => x.ToString("x2")));

            return hash;
        }
    }

    public class Order
    {
        public readonly int Id;
        public readonly int Amount;

        public Order(int id, int amount) => (Id, Amount) = (id, amount);
    }
}