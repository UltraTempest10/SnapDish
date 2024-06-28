using Model.Entity;
using Model.DTO;

namespace Service.Interface
{
    public interface IDiaryService
    {
        public List<DiaryBriefDTO> GetDiaryList(int userId);
        public Diary GetDiary(int diaryId);
        public int AddDiary(Diary diary, string imagePath);
        public int Enhance(string inputPath, string outputPath);
        public int UpdateDiary(Diary diary, string imagePath);
        public int DeleteDiary(int diaryId);
    }
}
