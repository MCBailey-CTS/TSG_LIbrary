//using MoreLinq;

using System;

namespace TSG_Library.UFuncs.DrainHoleCreator
{
    public interface IDrainHoleView
    {
        event EventHandler MainButtonClicked;

        event EventHandler<bool> MidpointsCheckChanged;

        event EventHandler<bool> CornersCheckChanged;

        event EventHandler<bool> SubtractCheckChanged;

        event EventHandler<Units> UnitsChanged;

        event EventHandler<double> DiameterChanged;

        event EventHandler<double> StartLimitChanged;

        event EventHandler<double> EndLimitChanged;
    }
}