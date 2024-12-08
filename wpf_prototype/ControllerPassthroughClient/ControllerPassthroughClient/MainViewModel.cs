﻿using CommonWpf.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows.Input;

namespace ControllerPassthroughClient;

public partial class MainViewModel : ViewModel
{
    [ObservableProperty]
    private string _serverConnectionButtonText = "Connect to Server";

    [ObservableProperty]
    private bool _debugLight;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ToggleServerConnectionCommand))]
    private ConnectionStatus _serverConnectionStatus;

    [RelayCommand]
    private void OnPreviewKeyDown(KeyEventArgs? keyEventArgs)
    {
        if (keyEventArgs is null)
        {
            return;
        }

        Debug.WriteLine($"Down: {keyEventArgs.Key}");
        keyEventArgs.Handled = true;
    }

    [RelayCommand]
    private void OnPreviewKeyUp(KeyEventArgs? keyEventArgs)
    {
        if (keyEventArgs is null)
        {
            return;
        }

        Debug.WriteLine($"Up: {keyEventArgs.Key}");
    }

    [RelayCommand(CanExecute = nameof(CanToggleServerConnection))]
    private void ToggleServerConnection()
    {

    }

    private bool CanToggleServerConnection()
    {
        return true;
    }

    [ObservableProperty]
    private InputMode _inputMode = InputMode.Zero;

    [ObservableProperty]
    private string _inputStatus = "Zeroed";

    partial void OnInputModeChanged(InputMode value)
    {
        InputStatus = value switch
        {
            InputMode.Zero => "Zeroed",
            InputMode.Keyboard => "Keyboard",
            _ => "",
        };

        UpdateKeyboardPressLabels(value);
    }

    [ObservableProperty]
    private ControllerViewModel _controllerViewModel = new();

    private void UpdateKeyboardPressLabels(InputMode inputMode)
    {
        if (inputMode is InputMode.Keyboard)
        {
            ControllerViewModel.StartTitle = $"{ControllerViewModel.DefaultStartTitle} (B)";
            ControllerViewModel.SelectTitle = $"{ControllerViewModel.DefaultSelectTitle} (N)";
            ControllerViewModel.HomeTitle = $"{ControllerViewModel.DefaultHomeTitle} (M)";
            ControllerViewModel.BigHomeTitle = $"{ControllerViewModel.DefaultBigHomeTitle}\n(SPACE)";
            ControllerViewModel.XTitle = $"{ControllerViewModel.DefaultXTitle} (T)";
            ControllerViewModel.YTitle = $"{ControllerViewModel.DefaultYTitle} (H)";
            ControllerViewModel.ATitle = $"{ControllerViewModel.DefaultATitle} (G)";
            ControllerViewModel.BTitle = $"{ControllerViewModel.DefaultBTitle} (F)";
            ControllerViewModel.UpTitle = $"{ControllerViewModel.DefaultUpTitle} (I)";
            ControllerViewModel.RightTitle = $"{ControllerViewModel.DefaultRightTitle} (L)";
            ControllerViewModel.DownTitle = $"{ControllerViewModel.DefaultDownTitle} (K)";
            ControllerViewModel.LeftTitle = $"{ControllerViewModel.DefaultLeftTitle} (J)";
            ControllerViewModel.LeftStickTitle = $"{ControllerViewModel.DefaultLeftStickTitle} (WASD)";
            ControllerViewModel.LeftStickInTitle = $"{ControllerViewModel.DefaultLeftStickInTitle} (Q)";
            ControllerViewModel.RightStickTitle = $"{ControllerViewModel.DefaultRightStickTitle} (\u2191\u2192\u2193\u2190)";
            ControllerViewModel.RightStickInTitle = $"{ControllerViewModel.DefaultRightStickInTitle} (E)";
            ControllerViewModel.LeftBumperTitle = $"{ControllerViewModel.DefaultLeftBumperTitle} (Z)";
            ControllerViewModel.LeftTriggerTitle = $"{ControllerViewModel.DefaultLeftTriggerTitle} (X)";
            ControllerViewModel.RightBumperTitle = $"{ControllerViewModel.DefaultRightBumperTitle} (C)";
            ControllerViewModel.RightTriggerTitle = $"{ControllerViewModel.DefaultRightTriggerTitle} (V)";
        }
        else
        {
            ControllerViewModel.StartTitle = ControllerViewModel.DefaultStartTitle;
            ControllerViewModel.SelectTitle = ControllerViewModel.DefaultSelectTitle;
            ControllerViewModel.HomeTitle = ControllerViewModel.DefaultHomeTitle;

            ControllerViewModel.BigHomeTitle = inputMode is InputMode.XboxController ?
                $"{ControllerViewModel.DefaultBigHomeTitle}\n(SPACE)" :
                ControllerViewModel.DefaultBigHomeTitle;

            ControllerViewModel.XTitle = ControllerViewModel.DefaultXTitle;
            ControllerViewModel.YTitle = ControllerViewModel.DefaultYTitle;
            ControllerViewModel.ATitle = ControllerViewModel.DefaultATitle;
            ControllerViewModel.BTitle = ControllerViewModel.DefaultBTitle;
            ControllerViewModel.UpTitle = ControllerViewModel.DefaultUpTitle;
            ControllerViewModel.RightTitle = ControllerViewModel.DefaultRightTitle;
            ControllerViewModel.DownTitle = ControllerViewModel.DefaultDownTitle;
            ControllerViewModel.LeftTitle = ControllerViewModel.DefaultLeftTitle;
            ControllerViewModel.LeftStickTitle = ControllerViewModel.DefaultLeftStickTitle;
            ControllerViewModel.LeftStickInTitle = ControllerViewModel.DefaultLeftStickInTitle;
            ControllerViewModel.RightStickTitle = ControllerViewModel.DefaultRightStickTitle;
            ControllerViewModel.RightStickInTitle = ControllerViewModel.DefaultRightStickInTitle;
            ControllerViewModel.LeftBumperTitle = ControllerViewModel.DefaultLeftBumperTitle;
            ControllerViewModel.LeftTriggerTitle = ControllerViewModel.DefaultLeftTriggerTitle;
            ControllerViewModel.RightBumperTitle = ControllerViewModel.DefaultRightBumperTitle;
            ControllerViewModel.RightTriggerTitle = ControllerViewModel.DefaultRightTriggerTitle;
        }
    }
}
