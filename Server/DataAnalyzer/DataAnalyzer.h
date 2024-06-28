#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace DataAnalyzerCLI {
    public ref class Analyzer {
    public:
        static double CalculateOverallAvg(List<double>^ data);
        static Dictionary<String^, double>^ CalculateCategoryAvg(List<String^>^ categories, List<double>^ values);
        static Dictionary<String^, int>^ CalculateCategoryFrequency(List<String^>^ categories);
        static List<int>^ CalculateValueDistribution(List<double>^ values, int numBins);
    };
}
