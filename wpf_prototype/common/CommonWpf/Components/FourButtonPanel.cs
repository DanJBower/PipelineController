using SimpleSourceGenerators;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommonWpf.Components;

[AutoDependencyProperty<string>(Name = "GroupTitle", DefaultValue = "")]
public partial class FourButtonPanel : UserControl
{
    static FourButtonPanel()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(FourButtonPanel), new FrameworkPropertyMetadata(typeof(FourButtonPanel)));
    }

    public string GroupTitle
    {
        get => (string)GetValue(GroupTitleProperty);
        set => SetValue(GroupTitleProperty, value);
    }

    public static readonly DependencyProperty GroupTitleProperty
        = DependencyProperty.Register(
            nameof(GroupTitle),
            typeof(string),
            typeof(FourButtonPanel),
            new PropertyMetadata("")
        );


    public string ButtonOneTitle
    {
        get => (string)GetValue(ButtonOneTitleProperty);
        set => SetValue(ButtonOneTitleProperty, value);
    }

    public static readonly DependencyProperty ButtonOneTitleProperty
        = DependencyProperty.Register(
            nameof(ButtonOneTitle),
            typeof(string),
            typeof(FourButtonPanel),
            new PropertyMetadata("")
        );

    public bool ButtonOnePressed
    {
        get => (bool)GetValue(ButtonOnePressedProperty);
        set => SetValue(ButtonOnePressedProperty, value);
    }

    public static readonly DependencyProperty ButtonOnePressedProperty
        = DependencyProperty.Register(
            nameof(ButtonOnePressed),
            typeof(bool),
            typeof(FourButtonPanel),
            new PropertyMetadata(false)
        );

    public SolidColorBrush ButtonOnePressedColor
    {
        get => (SolidColorBrush)GetValue(ButtonOnePressedColorProperty);
        set => SetValue(ButtonOnePressedColorProperty, value);
    }

    public static readonly DependencyProperty ButtonOnePressedColorProperty
        = DependencyProperty.Register(
            nameof(ButtonOnePressedColor),
            typeof(SolidColorBrush),
            typeof(FourButtonPanel),
            new PropertyMetadata(Brushes.DarkMagenta)
        );

    public string ButtonTwoTitle
    {
        get => (string)GetValue(ButtonTwoTitleProperty);
        set => SetValue(ButtonTwoTitleProperty, value);
    }

    public static readonly DependencyProperty ButtonTwoTitleProperty
        = DependencyProperty.Register(
            nameof(ButtonTwoTitle),
            typeof(string),
            typeof(FourButtonPanel),
            new PropertyMetadata("")
        );

    public bool ButtonTwoPressed
    {
        get => (bool)GetValue(ButtonTwoPressedProperty);
        set => SetValue(ButtonTwoPressedProperty, value);
    }

    public static readonly DependencyProperty ButtonTwoPressedProperty
        = DependencyProperty.Register(
            nameof(ButtonTwoPressed),
            typeof(bool),
            typeof(FourButtonPanel),
            new PropertyMetadata(false)
        );

    public SolidColorBrush ButtonTwoPressedColor
    {
        get => (SolidColorBrush)GetValue(ButtonTwoPressedColorProperty);
        set => SetValue(ButtonTwoPressedColorProperty, value);
    }

    public static readonly DependencyProperty ButtonTwoPressedColorProperty
        = DependencyProperty.Register(
            nameof(ButtonTwoPressedColor),
            typeof(SolidColorBrush),
            typeof(FourButtonPanel),
            new PropertyMetadata(Brushes.DarkMagenta)
        );

    public string ButtonThreeTitle
    {
        get => (string)GetValue(ButtonThreeTitleProperty);
        set => SetValue(ButtonThreeTitleProperty, value);
    }

    public static readonly DependencyProperty ButtonThreeTitleProperty
        = DependencyProperty.Register(
            nameof(ButtonThreeTitle),
            typeof(string),
            typeof(FourButtonPanel),
            new PropertyMetadata("")
        );

    public bool ButtonThreePressed
    {
        get => (bool)GetValue(ButtonThreePressedProperty);
        set => SetValue(ButtonThreePressedProperty, value);
    }

    public static readonly DependencyProperty ButtonThreePressedProperty
        = DependencyProperty.Register(
            nameof(ButtonThreePressed),
            typeof(bool),
            typeof(FourButtonPanel),
            new PropertyMetadata(false)
        );

    public SolidColorBrush ButtonThreePressedColor
    {
        get => (SolidColorBrush)GetValue(ButtonThreePressedColorProperty);
        set => SetValue(ButtonThreePressedColorProperty, value);
    }

    public static readonly DependencyProperty ButtonThreePressedColorProperty
        = DependencyProperty.Register(
            nameof(ButtonThreePressedColor),
            typeof(SolidColorBrush),
            typeof(FourButtonPanel),
            new PropertyMetadata(Brushes.DarkMagenta)
        );

    public string ButtonFourTitle
    {
        get => (string)GetValue(ButtonFourTitleProperty);
        set => SetValue(ButtonFourTitleProperty, value);
    }

    public static readonly DependencyProperty ButtonFourTitleProperty
        = DependencyProperty.Register(
            nameof(ButtonFourTitle),
            typeof(string),
            typeof(FourButtonPanel),
            new PropertyMetadata("")
        );

    public bool ButtonFourPressed
    {
        get => (bool)GetValue(ButtonFourPressedProperty);
        set => SetValue(ButtonFourPressedProperty, value);
    }

    public static readonly DependencyProperty ButtonFourPressedProperty
        = DependencyProperty.Register(
            nameof(ButtonFourPressed),
            typeof(bool),
            typeof(FourButtonPanel),
            new PropertyMetadata(false)
        );

    public SolidColorBrush ButtonFourPressedColor
    {
        get => (SolidColorBrush)GetValue(ButtonFourPressedColorProperty);
        set => SetValue(ButtonFourPressedColorProperty, value);
    }

    public static readonly DependencyProperty ButtonFourPressedColorProperty
        = DependencyProperty.Register(
            nameof(ButtonFourPressedColor),
            typeof(SolidColorBrush),
            typeof(FourButtonPanel),
            new PropertyMetadata(Brushes.DarkMagenta)
        );
}
