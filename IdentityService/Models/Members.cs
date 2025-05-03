namespace IdentityService.Models
{
    public class Members
    {
        public int MemberId { get; set; }

        public string? OwnerId { get; set; }

        public string? MemberName { get; set; }
        public string? MemberEmail { get; set; }
        public string? MemberPhone { get; set; }
        public Contactstatus? ContactStatus { get; set; }
    }
    public enum Contactstatus
    {
        Submitted,
        Approved,
        Rejected
    }
}
