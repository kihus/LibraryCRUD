namespace Library.Models;
internal class MenuLibrary 
{
    public string Option { get; set; }
    public Action ExecuteCommandMenu { get; set; }

    public override string ToString()
    {
        return Option;
    }
}
