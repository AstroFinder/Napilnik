using System;
using System.Collections.Generic;
using System.Linq;

namespace Napilnik
{
    public class Program
    {
        static void Main(string[] args)
        {
            Warehouse warehouse = new Warehouse(100);

            Good iPhone12 = new Good("IPhone 12");
            Good iPhone11 = new Good("IPhone 11");

            warehouse.Ship(iPhone12, 10);
            warehouse.Ship(iPhone11, 1);

            Shop shop = new Shop(warehouse);

            Cart cart = shop.Cart();

            cart.Add(iPhone12, 4);
            cart.Add(iPhone11, 3);

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

    public class Cart
    {
        private readonly List<Cell> _cells;
        private readonly Warehouse _warehouse;

        public Cart(Warehouse warehouse)
        {
            _warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));
            _cells = new List<Cell>();
        }

        public IReadOnlyList<IReadOnlyCell> Cells => _cells;

        public void Add(Good good, int count)
        {
            bool result = _warehouse.TryGetGood(good, count);

            if (result == false)
                throw new InvalidOperationException();

            var newCell = new Cell(good, count);

            int cellIndex = _cells.FindIndex(cell => cell.Good == good);

            if (cellIndex == -1)
                _cells.Add(newCell);
            else
                _cells[cellIndex].Merge(newCell);
        }

        public Order Order()
        {
            if (Cells.Count <= 0)
                throw new ArgumentOutOfRangeException();

            _warehouse.Send(this);

            return new Order();
        }
    }

    public class Order
    {
        public string Paylink { get; private set; }

        public Order()
        {
            Paylink = GetHashCode().ToString();
        }
    }

    public class Warehouse
    {
        private readonly List<Cell> _cells;

        public Warehouse(int maxCapacity)
        {
            _cells = new List<Cell>();
            MaxСapacity = maxCapacity;
        }

        public IReadOnlyList<IReadOnlyCell> Places => _cells;

        public int MaxСapacity { get; private set; }

        public int CurrentCapacity => _cells.Sum(cell => cell.Count);

        public void Ship(Good good, int count)
        {
            var newCell = new Cell(good, count);

            if (CurrentCapacity + newCell.Count > MaxСapacity)
                throw new InvalidOperationException();

            int cellIndex = _cells.FindIndex(cell => cell.Good == good);

            if (cellIndex == -1)
                _cells.Add(newCell);
            else
                _cells[cellIndex].Merge(newCell);
        }

        public void Send(Cart cart)
        {
            for (int i = 0; i < cart.Cells.Count; i++)
            {
                IReadOnlyCell cartCell = cart.Cells[i];

                if (cartCell.Count < 0)
                    throw new InvalidOperationException();

                int cellIndex = _cells.FindIndex(cell => cell.Good == cartCell.Good);

                if (cellIndex == -1)
                    throw new InvalidOperationException();
                else if (_cells[cellIndex].CanRemove)
                    _cells[cellIndex].Remove(cartCell);
            }
        }

        public bool TryGetGood(Good good, int count)
        {
            return _cells.Any(cell => cell.Good == good && cell.Count >= count);
        }
    }

    public class Cell : IReadOnlyCell
    {
        public Good Good { get; private set; }
        public int Count { get; private set; }

        public Cell(Good good, int count)
        {

            Good = good ?? throw new ArgumentNullException(nameof(good));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Count = count;
        }

        public bool CanRemove => Count > 0;

        public void Merge(IReadOnlyCell newCell)
        {
            if (newCell.Good != Good)
                throw new ArgumentOutOfRangeException();

            Count += newCell.Count;
        }

        public void Remove(IReadOnlyCell newCell)
        {
            if (newCell.Good != Good)
                throw new ArgumentOutOfRangeException();

            if (CanRemove == false)
                throw new InvalidOperationException();

            Count -= newCell.Count;
        }
    }

    public interface IReadOnlyCell
    {
        public Good Good { get; }
        public int Count { get; }
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
}
