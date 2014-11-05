namespace SbManager.Models.ViewModels
{
    public interface IHaveSettings
    {
        Time AutoDeleteOnIdle { get; set; }
        Time DefaultMessageTTL { get; set; }
        Time DuplicateDetectionWindow { get; set; }
        Time LockDuration { get; set; }
    }
}
