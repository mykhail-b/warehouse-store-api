namespace ClassLibrary.Settings;

public class CookieSettings
{
    public int DefaultExpirationDays { get; set; }
    public bool HttpOnly { get; set; }
    public bool Secure { get; set; }
    public string SameSite { get; set; } = "Strict";
}
