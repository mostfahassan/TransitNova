namespace TransitNova.UI.ViewModels;

public sealed record ErrorPageViewModel(
    string Code,
    string Eyebrow,
    string Title,
    string Description,
    string PrimaryLabel,
    string PrimaryArea,
    string PrimaryController,
    string PrimaryAction,
    string SecondaryLabel,
    string SecondaryArea,
    string SecondaryController,
    string SecondaryAction,
    string Tone = "neutral");