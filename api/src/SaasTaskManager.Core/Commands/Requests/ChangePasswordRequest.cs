using System.ComponentModel.DataAnnotations;

namespace SaasTaskManager.Core.Commands.Requests;

public record ChangePasswordRequest
{
    [Required(ErrorMessage = "Current password is required.")]
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Current password must be between 1 and 128 characters.")]
    public string CurrentPassword { get; init; } = string.Empty;

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "New password must be between 8 and 128 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+=\[{\]};:<>|./?,-]).{8,}$", 
        ErrorMessage = "New password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    public string NewPassword { get; init; } = string.Empty;
} 