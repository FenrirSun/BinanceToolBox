public static class ProjectConfig
{
    public const string PACKAGE_ID_ANDROID  = "com.rainyday.home.binancetoolbox";
    public const string PACKAGE_ID_IOS      = "com.rainyday.home.binancetoolbox";
    public const string Z_PROJECT_ID = "binance_tool_box";
    public static string APP_VERSION = "0.1";
    public const int VERSION_CODE = 1000;
    public const string BUILD_ENV = "alpha";
    
    public static string GetChannelName()
    {
#if UNITY_ANDROID
        return "googleplay";
#else
        return "appstore";
#endif
    }
    
    static ProjectInfo projectInfo = new ProjectInfo() {
#if UNITY_IOS
        PackageName = PACKAGE_ID_IOS,
#else
        PackageName = PACKAGE_ID_ANDROID,
#endif
        ProjectId = Z_PROJECT_ID,
        Version = APP_VERSION,
        VersionCode = VERSION_CODE,
        Env = BUILD_ENV,
        Channel = GetChannelName(),
    };
    
    public static ProjectInfo GetProjectInfo() {
        return projectInfo;
    }
}