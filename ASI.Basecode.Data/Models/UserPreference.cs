namespace ASI.Basecode.Data.Models
{
    public partial class UserPreference
    {
        public int PreferenceId { get; set; }
        public int UserId { get; set; }
        
        // the number of tickets shown in the view tickets table
        public int DefaultTicketView { get; set; }
        public bool EmailNotifications { get; set; }
        public bool InAppNotifications { get; set; }

        public User User { get; set; }
    }
}