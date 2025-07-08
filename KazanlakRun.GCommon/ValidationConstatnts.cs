namespace KazanlakRun.GCommon
{
    public static class ValidationConstatnts
    {
        // Names: точно две думи на латиница, разделени с един интервал
        public const string NamesRegex = @"^[A-Za-z]+ [A-Za-z]+$";
        public const int NamesMinLen = 5;    // примерно "Al Li" = 5 char
        public const int NamesMaxLen = 40;

        // Email: локално име (без спейсове), една @, домейн с точка и TLD ≥2
        public const string EmailRegex = @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$";
        public const int EmailMaxLen = 60;

        // Phone: цифри, може +, интервали и тирета
        public const string PhoneRegex = @"^\+?[0-9\s\-]+$";
        public const int PhoneMinLen = 7;
        public const int PhoneMaxLen = 20;
    }
}