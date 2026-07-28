using System;
using System.Collections.Generic;

namespace URLShortner.API.Models;

public partial class Url
{
    public int Id { get; set; }

    public string OriginalUrl { get; set; } = null!;

    public string ShortCode { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int ClickCount { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
