namespace TransitNova.BusinessLayer.DTOs.UserProfile.Auth
{
    public record ChangePasswordDto
    (
        string CurrentPassword,
        string NewPassword,
        string ConfirmNewPassword
    );
}
