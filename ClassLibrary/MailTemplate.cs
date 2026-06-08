namespace ClassLibrary;

public class MailTemplate
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public class OrderConfirmationMail : MailTemplate
{
    public int OrderId { get; set; }
    public string ShippingNumber { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public List<OrderMailItem> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
}

public class OrderShippingMail : MailTemplate
{
    public int OrderId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string ShippingNumber { get; set; } = string.Empty;
}

public class OrderMailItem
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class RegisterConfirmationMail : MailTemplate
{
    public string RecipientName { get; set; } = string.Empty;
    public string ConfirmationLink { get; set; } = string.Empty;
}