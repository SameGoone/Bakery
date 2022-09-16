namespace Bakery.Models
{
    public class ProductInOrder
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        private int _count;
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Count must be greater or equal 0");
                }

                _count = value;
            }
        }
    }
}
