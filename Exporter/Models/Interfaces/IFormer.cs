namespace Exporter.Models.Interfaces
{
    public interface IFormer
    {
        string FileName { get; }
        string FilePath { get; }
        void FormDocument();
    }
}
