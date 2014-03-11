namespace Warranty.Core.Entities
{
    public class HomeOwner
    {
        public virtual long Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string HomePhone { get; set; }
        public virtual string WorkPhone1 { get; set; }
        public virtual string WorkPhone2 { get; set; }
        public virtual string OtherPhone { get; set; }

    }
}