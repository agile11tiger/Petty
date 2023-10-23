namespace Petty.Views.Controls.Magic.RadialGradientFolder;

[Flags]
public enum RadialGradientFlags
{
    None = 0x0,
    XProportional = 0x1,
    YProportional = 0x2,
    WidthProportional = 0x4,
    HeightProportional = 0x8,
    PositionProportional = 0x3,
    SizeProportional = 0xC,
    All = -1
}