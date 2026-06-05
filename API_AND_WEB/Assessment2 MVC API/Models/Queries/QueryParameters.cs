namespace Assessment2_MVC_API.Models.Queries
{
    public class QueryParameters
    {
        const int MaxSize = 100;

        private int _pageSize = 50;

        public string SortBy { get; set; } = "Id"; // This needs to meet the requirements of matching the attribute name - in this case to sort by id you need to use "Id" or price as "Price"

        public string sortOrder = "asc";

        public string SortOrder
        {
            get
            {
                return sortOrder;
            }
            set
            {
                if (value == "asc" || value == "desc")
                {
                    sortOrder = value;
                }
            }
        }

        public int Page { get; set; } = 1;
        public int Size
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = Math.Min(_pageSize, value);
            }
        }



    }
}
