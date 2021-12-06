using System;
using System.Collections.Generic;
using System.Linq;

namespace Napilnik
{
    public class Program
    {
        static void Main(string[] args)
        {
            ConsoleWritter writter = new ConsoleWritter();

            Good iPhone12 = new Good("IPhone 12");
            Good iPhone11 = new Good("IPhone 11");

            Warehouse warehouse = new Warehouse();

            Shop shop = new Shop(warehouse);

            warehouse.Ship(iPhone12, 10);
            warehouse.Ship(iPhone11, 1);

            writter.DisplayGoods(warehouse.Goods);

            Cart cart = shop.Cart();

            cart.Add(iPhone12, 4);
            cart.Add(iPhone11, 3);

            writter.DisplayGoods(cart.Goods);

            Console.WriteLine(cart.Order().Paylink);

            cart.Add(iPhone12, 9);

            Console.ReadLine();
        }
    }

    public class Shop
    {
        private readonly Warehouse _warehouse;

        public Shop(Warehouse warehouse)
        {
            _warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));
        }

        public Cart Cart() => new Cart(_warehouse);
    }

    public class Order
    {
        public string Paylink { get; private set; }

        public Order()
        {
            Paylink = GetHashCode().ToString();
        }
    }

    public abstract class Storage
    {
        private readonly Dictionary<string, int> _goods;

        public Storage()
        {
            _goods = new Dictionary<string, int>();
        }

        public IReadOnlyDictionary<string, int> Goods => _goods;

        public virtual void Ship(Good good, int count)
        {
            good = good ?? throw new ArgumentNullException(nameof(good));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (_goods.TryGetValue(good.Name, out int value))
                _goods[good.Name] = value + count;
            else
                _goods.Add(good.Name, count);
        }

        public virtual void Send(IReadOnlyDictionary<string, int> cart)
        {
            cart = cart ?? throw new ArgumentNullException(nameof(cart));

            foreach (var good in cart.Where(good => _goods.ContainsKey(good.Key)))
            {
                if (_goods[good.Key] < good.Value)
                    throw new InvalidOperationException();

                _goods[good.Key] -= good.Value;
            }
        }

        public virtual bool TryGetGood(Good good, int count)
        {
            return _goods.Any(goods => goods.Key == good.Name && goods.Value >= count);
        }
    }

    public class Cart : Storage
    {
        private readonly Warehouse _warehouse;

        public Cart(Warehouse warehouse)
        {
            _warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));
        }
        public void Add(Good good, int count)
        {
            bool result = _warehouse.TryGetGood(good, count);

            if (result == false)
                throw new InvalidOperationException();

            Ship(good, count);
        }

        public Order Order()
        {
            if (Goods.Count <= 0)
                throw new ArgumentOutOfRangeException();

            _warehouse.Send(Goods);

            return new Order();
        }
    }

    public class Warehouse : Storage
    {
    }

    public class Good
    {
        public string Name { get; private set; }
        public Good(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }

    public class ConsoleWritter
    {
        public void DisplayGoods(IReadOnlyDictionary<string, int> goods)
        {
            goods = goods ?? throw new ArgumentNullException(nameof(goods));

            foreach (var item in goods)
            {
                Console.Write($"Товар: {item.Key} - Остаток: {item.Value} \n");
            }

            Console.WriteLine();
        }
    }
}
