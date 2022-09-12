namespace Bakery.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

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
