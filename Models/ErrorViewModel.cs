namespace gfps.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public int? StatusCode { get; set; }

    public bool Is404 => StatusCode == 404;

    public bool Is403 => StatusCode == 403;
}
