namespace TSG_Library.UFuncs
{
    public interface _IUFunc
    {
        string ufunc_name { get; }

        void execute();
    }
}