using Aliyun.OSS;
using Aliyun.OSS.Common;
using System.Net;

namespace Service
{
    internal class OssService
    {
        static string accessKeyId = "accessKeyId";
        static string accessKeySecret = "accessKeySecret";
        static string endpoint = "oss-cn-shanghai.aliyuncs.com";
        static string bucketName = "snapdish";    //OSS图片存储空间

        public static bool PutObjectFromLocalFile(string objectName, string localFilePath, out string url)
        {
            OssClient client = new(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var obj = client.PutObject(bucketName, objectName, localFilePath);
                if (obj != null && obj.HttpStatusCode == HttpStatusCode.OK)
                {
                    url = "https://" + bucketName + "." + endpoint + "/" + objectName;
                    return true;
                }
                else
                {
                    url = null;
                    return false;
                }
            }
            catch (OssException ex)
            {
                Console.WriteLine(string.Format("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3},BucketName:{4},fileName:{5}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId, bucketName, objectName));
                url = null;
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed with error info: {0},BucketName:{1},fileName:{2}", ex.Message, bucketName, objectName));
                url = null;
                return false;
            }
        }
    }
}
