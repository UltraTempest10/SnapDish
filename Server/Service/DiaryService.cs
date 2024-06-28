using System.Runtime.InteropServices;
using Service.Interface;
using SqlSugar;
using Model.Entity;
using Model.DTO;
using Aliyun.OSS;

namespace Service
{
    public partial class DiaryService : IDiaryService
    {
        private readonly ISqlSugarClient _db;
        public DiaryService(ISqlSugarClient db)
        {
            _db = db;
        }

        [LibraryImport("ImageProcessor.dll", StringMarshalling = StringMarshalling.Utf8)]
        public static partial int EnhanceImage(string inputPath, string outputPath);

        public List<DiaryBriefDTO> GetDiaryList(int userId)
        {
            return _db.Queryable<Diary>()
                .Where(d => d.UserId == userId)
                .OrderBy(d => d.CreateTime, OrderByType.Desc)
                .Select(d => new DiaryBriefDTO
                {
                    Id = d.DiaryId,
                    Title = d.Title,
                    Location = d.Location,
                    Image = d.Image,
                    CreateTime = d.CreateTime
                })
                .ToList();
        }

        public Diary GetDiary(int diaryId)
        {
            return _db.Queryable<Diary>()
                .Where(d => d.DiaryId == diaryId)
                .First();
        }

        public int AddDiary(Diary diary, string imagePath)
        {
            // 上传图片到OSS
            string imageName = "diary/" + diary.UserId + "/" + DateTime.Now.ToFileTime().ToString().Replace("/", "") + Path.GetRandomFileName().Replace(".", "") + Path.GetExtension(imagePath);
            OssService.PutObjectFromLocalFile(imageName, imagePath, out string imageUrl);
            diary.Image = imageUrl;
            return _db.Insertable(diary).ExecuteCommand();
        }

        public int Enhance(string inputPath, string outputPath)
        {
            return EnhanceImage(inputPath, outputPath);
        }

        public int UpdateDiary(Diary diary, string imagePath)
        {
            // 上传图片到OSS
            string imageName = "diary/" + diary.UserId + "/" + DateTime.Now.ToFileTime().ToString().Replace("/", "") + Path.GetRandomFileName().Replace(".", "") + Path.GetExtension(imagePath);
            OssService.PutObjectFromLocalFile(imageName, imagePath, out string imageUrl);
            diary.Image = imageUrl;

            // 不修改CreateTime
            return _db.Updateable(diary)
                .SetColumns(d => new Diary
                {
                    Title = diary.Title,
                    Location = diary.Location,
                    Content = diary.Content,
                    Rating = diary.Rating,
                    Image = diary.Image
                })
                .Where(d => d.DiaryId == diary.DiaryId)
                .ExecuteCommand();
        }

        public int DeleteDiary(int diaryId)
        {
            return _db.Deleteable<Diary>()
                .Where(d => d.DiaryId == diaryId)
                .ExecuteCommand();
        }
    }
}
