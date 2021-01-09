using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp.Site.Models
{
    public class PageViewModel<T>
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }

        public IEnumerable<T> list { get; private set; }

        public PageViewModel(IEnumerable<T> list, int pageNumber, int pageSize)
        {
            if (list == null)
                list = new List<T>();
            if (list.Count() > pageSize)
                this.list = list.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            else
                this.list = list;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(list.Count() / (double)pageSize);
        }

        public PageViewModel(IEnumerable<T> list, int count, int pageNumber, int pageSize)
        {
            if (list == null)
                list = new List<T>();
            if (list.Count() > pageSize)
                this.list = list.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            else
                this.list = list;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }
    }
}
