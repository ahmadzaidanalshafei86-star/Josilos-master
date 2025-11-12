namespace ES.Web.Seeds
{
    public class DefaultSocialMediaLinks
    {
        public static async Task SeedSocialMediaLinks(ApplicationDbContext context)
        {
            if (!context.SocialMediaLinks.Any())
            {
                await context.SocialMediaLinks.AddRangeAsync(
                    new SocialMediaLink { Name = "Facebook", Url = "https://www.facebook.com", IconClass = "fab fa-facebook-f", IconColor = "#000000" },
                    new SocialMediaLink { Name = "Twitter", Url = "https://www.x.com", IconClass = "fab fa-twitter", IconColor = "#000000" },
                    new SocialMediaLink { Name = "Instagram", Url = "https://www.instagram.com", IconClass = "fab fa-instagram", IconColor = "#000000" },
                    new SocialMediaLink { Name = "LinkedIn", Url = "https://www.linkedin.com", IconClass = "fab fa-linkedin-in", IconColor = "#000000" },
                    new SocialMediaLink { Name = "YouTube", Url = "https://www.youtube.com", IconClass = "fab fa-youtube", IconColor = "#000000" },
                    new SocialMediaLink { Name = "Telegram", Url = "https://web.telegram.org/k/", IconClass = "fab fa-telegram", IconColor = "#000000" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
