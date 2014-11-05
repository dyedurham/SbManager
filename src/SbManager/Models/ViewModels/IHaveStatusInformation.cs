using System;

namespace SbManager.Models.ViewModels
{
    public interface IHaveStatusInformation
    {
        string Status { get; set; }
        bool IsReadOnly { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        DateTime AccessedAt { get; set; }
    }
}
