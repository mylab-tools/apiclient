namespace TestServer
{
    public record TestDto(
        string? StringVal,
        int IntVal
    )
    {
        public TestDto(): this(null, 0)
        {
            
        }
    }
}
