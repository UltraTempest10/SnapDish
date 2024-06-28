#include "pch.h"

#include "DataAnalyzer.h"

#include <algorithm>
#include <numeric>
#include <unordered_map>

using namespace DataAnalyzerCLI;

double Analyzer::CalculateOverallAvg(List<double>^ data) {
    if (data->Count == 0) return 0.0;
    double sum = 0;
    for each (double value in data) {
        sum += value;
    }
    return sum / data->Count;
}

Dictionary<String^, double>^ Analyzer::CalculateCategoryAvg(List<String^>^ categories, List<double>^ values) {
    if (categories->Count != values->Count) throw gcnew ArgumentException("Categories and values lists must have the same length.");

    auto categoryValues = gcnew Dictionary<String^, List<double>^>();
    for (int i = 0; i < categories->Count; i++) {
        if (!categoryValues->ContainsKey(categories[i])) {
            categoryValues[categories[i]] = gcnew List<double>();
        }
        categoryValues[categories[i]]->Add(values[i]);
    }

    auto categoryAvg = gcnew Dictionary<String^, double>();
    for each (auto kvp in categoryValues) {
        double sum = 0;
        for each (double value in kvp.Value) {
            sum += value;
        }
        categoryAvg[kvp.Key] = sum / kvp.Value->Count;
    }

    return categoryAvg;
}

Dictionary<String^, int>^ Analyzer::CalculateCategoryFrequency(List<String^>^ categories) {
    auto categoryFrequency = gcnew Dictionary<String^, int>();
    for each (String ^ category in categories) {
        if (!categoryFrequency->ContainsKey(category)) {
            categoryFrequency[category] = 0;
        }
        categoryFrequency[category]++;
    }
    return categoryFrequency;
}

List<int>^ Analyzer::CalculateValueDistribution(List<double>^ values, int numBins) {
    if (numBins <= 0) throw gcnew ArgumentException("Number of bins must be greater than 0.");

    auto distribution = gcnew List<int>(numBins);
    for (int i = 0; i < numBins; i++) {
        distribution->Add(0);
    }

    if (values->Count == 0) return distribution;

    double minVal = values[0];
    double maxVal = values[0];
    for each (double value in values) {
        if (value < minVal) minVal = value;
        if (value > maxVal) maxVal = value;
    }

    double binWidth = (maxVal - minVal) / numBins;
    for each (double value in values) {
        int bin = static_cast<int>((value - minVal) / binWidth);
        if (bin == numBins) bin--; // Ensure the max value falls into the last bin
        distribution[bin]++;
    }

    return distribution;
}