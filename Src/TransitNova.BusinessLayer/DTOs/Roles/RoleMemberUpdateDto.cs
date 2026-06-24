namespace TransitNova.BusinessLayer.DTOs.Roles
{

    public sealed class RoleMemberUpdateDto
    {
        public Guid UserId { get; set; }

        public bool IsInRole { get; set; }
    }

}
