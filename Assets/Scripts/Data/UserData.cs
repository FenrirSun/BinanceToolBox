
public class UserData : UserDataBase
{
    public int level;
    
    public UserData() : base()
    {
        Clear();
    }

    public void Clear()
    {
        level = 0;
    }

    public void SetLevel(int _level)
    {
        level = _level;
        MarkDirty();
    }
}
