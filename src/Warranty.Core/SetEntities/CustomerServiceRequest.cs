using System;
using System.Collections.Generic;
using Warranty.Core.Enumerations;

namespace Warranty.Core.Entities
{
    public class CustomerServiceRequest
    {
        private readonly IList<CustomerServiceLineItem> _items;

        public CustomerServiceRequest()
        {
            _items = new List<CustomerServiceLineItem>();
        }

        public virtual long Id { get; set; }
        public virtual DateTime DateOpened { get; set; }
        public virtual RequestType RequestType { get; set; }
        public virtual TeamMember AssignedBy { get; set; }
        public virtual bool AllItemsComplete { get; set; }
        public virtual DateTime? DateCompleted { get; set; }
        public virtual string Summary { get; set; }

        //Home Information
        public virtual Home Home { get; set; }
        public virtual Community Community { get; set; }

        //Organizational Information
        public virtual TeamMember WarrantyServiceRepresentative { get; set; }
        public virtual TeamMember Builder { get; set; }
        public virtual TeamMember SalesConsultant { get; set; }

        public virtual IList<CustomerServiceLineItem> Items
        {
            get { return _items; }
        }

        public virtual void AddItemToRequest(CustomerServiceLineItem item)
        {
            if (_items.Contains(item))
                throw new ArgumentException("This item already exists on this request.");

            _items.Add(item);
        }

        public virtual void RemoveItemFromRequest(CustomerServiceLineItem item)
        {
            if (!_items.Contains(item))
                throw new ArgumentException("This item does not exist on this reqeust");

            _items.Remove(item);
        }
    }
}
