namespace HamrahFelez.Utilities
{
    public class Enums
    {
        public enum CrudMod
        {
            Insert = 1,
            Update = 2,
            Delete = 3,
        }
        public enum ApiRoles
        {
            ParentID = 3300,
            NoAccess = 3301,    // عدم دسترسی
            CEO = 3302,         // مدیرعامل
            CFO = 3303,         // مدیر مالی
            CCO = 3304,         // مدیر بازرگانی
            CIO = 3305,         // مدیر فناوری اطلاعات
            CMO = 3306,         // مدیر بازاریابی
            CPO = 3307,         // مدیر تولید
            CWO = 3308,         // مدیر انبار
            CHO = 3309,         // مدیر اداری
            ACS = 3310,         // کارشناس حسابداری
            CMS = 3311,         // کارشناس بازرگانی
            ITS = 3312,         // کارشناس فناوری اطلاعات
            MKS = 3313,         // کارشناس بازاریابی
            PDS = 3314,         // کارشناس تولید
            WHS = 3315,         // کارشناس انبار
            ADS = 3316,         // کارشناس اداری
            CUS = 3317,         // مشتری
        }
    }
}
