﻿namespace Warranty.Core.Calculator
{
    using System;

    public class SurveyDataResult
    {
        public string DefinitelyWillRecommend { get; set; }
        public string ExcellentWarrantyService { get; set; }
        public string OutstandingWarrantyService
        {
            get { return ExcellentWarrantyService; }
            set { ExcellentWarrantyService = value; }
        }
        public string RightFirstTime { get; set; }
        public string Division { get; set; }
        public string Project { get; set; }
        public DateTime SurveyDate { get; set; }
    }
}