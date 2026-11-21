namespace Arma3TacMapWebApp
{
    public static class ApplicationInfos
    {
#if STABLE_BUILD
        public const bool IsBetaBuild = false;
#else
        public const bool IsBetaBuild = true;
#endif
    }
}
