namespace SiteManagement.Domain.Utulity
{
    public static class Ensure
    {
        public static void NotEmpty(string value, Exception exception)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw exception;
            }
        }
        public static void NotEmpty(Guid value, Exception exception) 
        {
           if(value == Guid.Empty)
                throw exception;
        }
        public static void NotEmpty(DateTime value, Exception exception)
        {
            if (value == default)
                throw exception;

        }
        public static void NotNull<TValue>(TValue value, Exception exception)
            where TValue : class
        {
            if(value == null) 
                throw exception;
        }
      
    }
}
