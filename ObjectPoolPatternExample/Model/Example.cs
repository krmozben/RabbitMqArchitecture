namespace ObjectPoolPatternExample.Model
{
    public class Example
    {
        public Example(Guid key)
        {
            Key = key;
        }
        public Guid? Key { get; set; }
    }
}
