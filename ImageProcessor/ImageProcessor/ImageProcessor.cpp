// ImageProcessor.cpp : 定义 DLL 的导出函数。
//

#include "pch.h"
#include "framework.h"
#include "ImageProcessor.h"

#include <iostream>
#include <vector>
#include <cstring>
#include <cmath>

#include "opencv2\opencv.hpp"
#include "opencv2\core\core.hpp"
#include "opencv2\highgui\highgui.hpp"
#include "opencv2\imgproc\imgproc.hpp"

#define CV_SORT_EVERY_ROW    0
#define CV_SORT_EVERY_COLUMN 1
#define CV_SORT_ASCENDING    0
#define CV_SORT_DESCENDING   16

using namespace std;
using namespace cv;


// 这是导出变量的一个示例
IMAGEPROCESSOR_API int nImageProcessor=0;

// 这是导出函数的一个示例。
IMAGEPROCESSOR_API int fnImageProcessor(void)
{
    return 0;
}

// 这是已导出类的构造函数。
CImageProcessor::CImageProcessor()
{
    return;
}

extern "C" __declspec(dllexport) int EnhanceImage(const char* input, const char* output)
{
    CImageProcessor imageProcessor;
    return imageProcessor.EnhanceImage(input, output);
}

// 直方图均衡化
Mat histogramEqualization(const Mat& input) {
    Mat output;
    if (input.channels() == 1) {
        equalizeHist(input, output);
    }
    else {
        vector<Mat> channels;
        split(input, channels);
        for (int i = 0; i < channels.size(); i++) {
            equalizeHist(channels[i], channels[i]);
        }
        merge(channels, output);
    }
    return output;
}

// 拉普拉斯滤波
Mat laplacianEnhancement(const Mat& input) {
    Mat output, laplacian;
    laplacian = (Mat_<float>(3, 3) << 0, -1, 0, 0, 5, 0, 0, -1, 0);
    filter2D(input, output, CV_8UC3, laplacian);
    return output;
}

// 对数变换
Mat logTransform(const Mat& input, float alpha) {
    Mat output(input.size(), CV_32FC3);
    for (int i = 0; i < input.rows; i++) {
        for (int j = 0; j < input.cols; j++) {
            output.at<Vec3f>(i, j)[0] = log(1 + alpha * input.at<Vec3b>(i, j)[0]);
            output.at<Vec3f>(i, j)[1] = log(1 + alpha * input.at<Vec3b>(i, j)[1]);
            output.at<Vec3f>(i, j)[2] = log(1 + alpha * input.at<Vec3b>(i, j)[2]);
        }
    }
    normalize(output, output, 0, 255, NORM_MINMAX);
    convertScaleAbs(output, output);
    return output;
}

// 伽马校正
Mat gammaCorrection(const Mat& input) {
    Mat output(input.size(), CV_32FC3);
    for (int i = 0; i < input.rows; i++)
    {
        for (int j = 0; j < input.cols; j++)
        {
            output.at<Vec3f>(i, j)[0] = (input.at<Vec3b>(i, j)[0]) * (input.at<Vec3b>(i, j)[0]) * (input.at<Vec3b>(i, j)[0]);
            output.at<Vec3f>(i, j)[1] = (input.at<Vec3b>(i, j)[1]) * (input.at<Vec3b>(i, j)[1]) * (input.at<Vec3b>(i, j)[1]);
            output.at<Vec3f>(i, j)[2] = (input.at<Vec3b>(i, j)[2]) * (input.at<Vec3b>(i, j)[2]) * (input.at<Vec3b>(i, j)[2]);
        }
    }
    normalize(output, output, 0, 255, NORM_MINMAX);
    convertScaleAbs(output, output);
    return output;
}

// 选择增强方法
Mat chooseEnhancementMethod(const Mat& input) {
    // 计算图像的均值和标准差
    Scalar mean, stddev;
    meanStdDev(input, mean, stddev);

    double contrast = stddev[0];
    double brightness = mean[0];

    // 定义阈值
    double contrastThresholdLow = 50;
    double brightnessThresholdLow = 100;
    double brightnessThresholdHigh = 200;

    // 根据对比度和亮度选择增强方法
    if (contrast < contrastThresholdLow) {
        cout << "Using Histogram Equalization" << endl;
        return histogramEqualization(input);
    }
    else if (brightness < brightnessThresholdLow) {
        cout << "Using Log Transform" << endl;
        return logTransform(input, 0.2);
    }
    else if (brightness > brightnessThresholdHigh) {
        cout << "Using Gamma Correction" << endl;
        return gammaCorrection(input);
    }
    else {
        cout << "Using Laplacian Enhancement" << endl;
        return laplacianEnhancement(input);
    }
}

// 判断图像是否需要增强
bool needsEnhancement(const Mat& input) {
    Scalar mean, stddev;
    meanStdDev(input, mean, stddev);

    double contrast = stddev[0];
    double brightness = mean[0];

    // 判断对比度和亮度是否在适当范围内
    if (contrast >= 50 && brightness >= 100 && brightness <= 200) {
        return false; // 图像无需增强
    }
    return true; // 图像需要增强
}

// 去噪处理
Mat denoiseImage(const Mat& input) {
    Mat output;
    // fastNlMeansDenoisingColored(input, output, 10, 10, 7, 21); // 调整这些参数以获得更好的结果
    // 使用中值滤波进行温和去噪
    medianBlur(input, output, 3);
    return output;
}

int CImageProcessor::EnhanceImage(const char* input, const char* output) {
    // 读取图像
    Mat image = imread(input);
    if (image.empty()) {
        cout << "Error: Image cannot be loaded." << endl;
        return -1;
    }

    // 预处理去噪
    Mat denoisedImage = denoiseImage(image);

    // 判断图像是否需要增强
    if (!needsEnhancement(denoisedImage)) {
        cout << "Image does not need enhancement." << endl;
        // 直接保存原图像
        if (!imwrite(output, denoisedImage)) {
            cout << "Error: Cannot save the output image." << endl;
            return -1;
        }
        cout << "Image saved successfully." << endl;
        return 0;
    }

    // 选择并应用增强方法
    Mat enhancedImage = chooseEnhancementMethod(denoisedImage);

    // 后处理去噪
    Mat finalImage = denoiseImage(enhancedImage);

    // 保存增强后的图像
    if (!imwrite(output, finalImage)) {
        cout << "Error: Cannot save the output image." << endl;
        return -1;
    }

    cout << "Image enhanced and saved successfully." << endl;
    return 0;
}
