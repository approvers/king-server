namespace Approvers.King.Common;

public class AppCache : Singleton<AppCache>
{
    public DateTime? LoxyLastTranslateTime { get; set; }
    public DateTime? LoxyLastJpToEnTranslateTime { get; set; }
}
