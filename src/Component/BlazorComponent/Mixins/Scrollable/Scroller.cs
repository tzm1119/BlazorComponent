﻿using BlazorComponent.Web;
using Microsoft.JSInterop;

namespace BlazorComponent;

public class Scroller : IScrollable
{
    private bool _isActive;
    private bool _isScrollingUp;

    public Scroller(IScrollable scrollable)
    {
        ScrollTarget = scrollable.ScrollTarget;
        ScrollThreshold = scrollable.ScrollThreshold;

        Js = scrollable.Js;
    }

    #region Parameters, should reset in OnParametersSet()

    // typeof window !== 'undefined' in vuetify, but is always true in Blazor
    public bool CanScroll => true;

    public string ScrollTarget { get; set; }

    public double ScrollThreshold { get; set; }

    #endregion

    public double CurrentScroll { get; set; }

    public double CurrentThreshold { get; set; }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                SavedScroll = 0;
            }

            _isActive = value;
        }
    }

    public bool IsScrollingUp
    {
        get => _isScrollingUp;
        set
        {
            if (_isScrollingUp != value)
            {
                if (SavedScroll <= 0)
                {
                    SavedScroll = CurrentScroll;
                }
            }

            _isScrollingUp = value;
        }
    }

    public IJSRuntime Js { get; init; }

    public double PreviousScroll { get; set; }

    public double SavedScroll { get; set; }

    public double ComputedScrollThreshold => ScrollThreshold != 0 ? ScrollThreshold : 300;

    public async Task OnScroll(Action<Scroller> thresholdMet)
    {
        if (!CanScroll) return;

        PreviousScroll = CurrentScroll;

        // TODO: Merge the following two js interops
        var dom = await Js.InvokeAsync<Web.Element>(JsInteropConstants.GetDomInfo, ScrollTarget);
        if (dom != null)
        {
            CurrentScroll = dom.ScrollTop;
        }
        else
        {
            var window = await Js.InvokeAsync<Window>(JsInteropConstants.GetWindow);
            CurrentScroll = window.PageYOffset;
        }

        IsScrollingUp = CurrentScroll < PreviousScroll;
        CurrentThreshold = Math.Abs(CurrentScroll - ComputedScrollThreshold);

        if (Math.Abs(CurrentScroll - SavedScroll) > ComputedScrollThreshold)
        {
            thresholdMet.Invoke(this);
        }
    }
}
