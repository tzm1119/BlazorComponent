﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorComponent
{
    public partial class BCalendarCategoryDayIntervals<TCalendarCategory> where TCalendarCategory : ICalendarCategory
    {
        [Parameter]
        public CalendarTimestamp Day { get; set; }

        [Parameter]
        public int Index { get; set; }

        public List<List<CalendarTimestamp>> Intervals => Component.Intervals();
    }
}
