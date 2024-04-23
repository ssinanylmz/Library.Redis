namespace Library.Redis
{
    public static class RedisKeyGenerator
    {
        public static string GetKey(Type type)
        {

            if (type == null)
                throw new ArgumentNullException("RedisKeyGenerator.GetListType.value");


            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>))
            {
                return type.Name;
            }
            else
            {
                return type.GetGenericArguments().Single().Name;
            }
        }
    }
}
